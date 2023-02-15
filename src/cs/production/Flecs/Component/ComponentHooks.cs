// Copyright (c) Flecs Hub (https://github.com/flecs-hub). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Runtime.InteropServices;
using JetBrains.Annotations;
using static flecs_hub.flecs;

namespace Flecs;

[PublicAPI]
public unsafe struct ComponentHooks
{
    public CallbackComponentConstructor? Constructor;
    public CallbackComponentDeconstructor? Deconstructor;
    public CallbackComponentCopy? Copy;
    public CallbackComponentMove? Move;
    public CallbackIterator? OnAdd;
    public CallbackIterator? OnSet;
    public CallbackIterator? OnRemove;

    [UnmanagedCallersOnly]
    private static void CallbackConstructor(void* pointer, int count, ecs_type_info_t* typeInfo)
    {
        ref var data = ref CallbacksHelper.GetComponentHooksCallbackContext(typeInfo->hooks.binding_ctx);
        var context = new ComponentConstructorContext(pointer, count);
        data.Hooks.Constructor?.Invoke(ref context);
    }

    [UnmanagedCallersOnly]
    private static void CallbackDeconstructor(void* pointer, int count, ecs_type_info_t* typeInfo)
    {
        ref var data = ref CallbacksHelper.GetComponentHooksCallbackContext(typeInfo->hooks.binding_ctx);
        var context = new ComponentDeconstructorContext(pointer, count);
        data.Hooks.Deconstructor?.Invoke(ref context);
    }

    [UnmanagedCallersOnly]
    private static void CallbackCopy(void* destinationPointer, void* sourcePointer, int count, ecs_type_info_t* typeInfo)
    {
        ref var data = ref CallbacksHelper.GetComponentHooksCallbackContext(typeInfo->hooks.binding_ctx);
        var context = new ComponentCopyContext(destinationPointer, sourcePointer, count);
        data.Hooks.Copy?.Invoke(ref context);
    }

    [UnmanagedCallersOnly]
    private static void CallbackMove(void* destinationPointer, void* sourcePointer, int count, ecs_type_info_t* typeInfo)
    {
        ref var data = ref CallbacksHelper.GetComponentHooksCallbackContext(typeInfo->hooks.binding_ctx);
        var context = new ComponentMoveContext(destinationPointer, sourcePointer, count);
        data.Hooks.Move?.Invoke(ref context);
    }

    [UnmanagedCallersOnly]
    private static void CallbackOnAdd(ecs_iter_t* it)
    {
        ref var data = ref CallbacksHelper.GetComponentHooksCallbackContext(it->binding_ctx);
        var iterator = new Iterator(data.World, it);
        data.Hooks.OnAdd?.Invoke(iterator);
    }

    [UnmanagedCallersOnly]
    private static void CallbackOnSet(ecs_iter_t* it)
    {
        ref var data = ref CallbacksHelper.GetComponentHooksCallbackContext(it->binding_ctx);
        var iterator = new Iterator(data.World, it);
        data.Hooks.OnSet?.Invoke(iterator);
    }

    [UnmanagedCallersOnly]
    private static void CallbackOnRemove(ecs_iter_t* it)
    {
        ref var data = ref CallbacksHelper.GetComponentHooksCallbackContext(it->binding_ctx);
        var iterator = new Iterator(data.World, it);
        data.Hooks.OnRemove?.Invoke(iterator);
    }

    internal static void Fill(World world, ref ComponentHooks hooks, ecs_type_hooks_t* desc)
    {
        desc->ctor.Pointer = &CallbackConstructor;
        desc->dtor.Pointer = &CallbackDeconstructor;
        desc->copy.Pointer = &CallbackCopy;
        desc->move.Pointer = &CallbackMove;
        desc->on_add.Pointer = &CallbackOnAdd;
        desc->on_set.Pointer = &CallbackOnSet;
        desc->on_remove.Pointer = &CallbackOnRemove;
        desc->binding_ctx = (void*)CallbacksHelper.CreateComponentHooksCallbackContext(world, hooks);
    }
}
