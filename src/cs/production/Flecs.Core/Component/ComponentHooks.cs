// Copyright (c) Flecs Hub (https://github.com/flecs-hub). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Runtime.InteropServices;
using flecs_hub.Interop.Flecs;
using JetBrains.Annotations;
using static flecs_hub.Interop.Flecs.PInvoke;

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

    internal static void Fill(World world, ref ComponentHooks hooks, ecs_type_hooks_t* desc)
    {
#if UNITY_5_3_OR_NEWER
        desc->ctor.Data.Pointer = Marshal.GetFunctionPointerForDelegate<FnPtr_VoidPtr_Int_EcsTypeInfoTPtr_Void.@delegate>(CallbackConstructor);
        desc->dtor.Data.Pointer = Marshal.GetFunctionPointerForDelegate<FnPtr_VoidPtr_Int_EcsTypeInfoTPtr_Void.@delegate>(CallbackDeconstructor);
        desc->copy.Data.Pointer = Marshal.GetFunctionPointerForDelegate<FnPtr_VoidPtr_VoidPtr_Int_EcsTypeInfoTPtr_Void.@delegate>(CallbackCopy);
        desc->move.Data.Pointer = Marshal.GetFunctionPointerForDelegate<FnPtr_VoidPtr_VoidPtr_Int_EcsTypeInfoTPtr_Void.@delegate>(CallbackMove);
        desc->on_add.Data.Pointer = Marshal.GetFunctionPointerForDelegate<FnPtr_EcsIterTPtr_Void.@delegate>(CallbackOnAdd);
        desc->on_set.Data.Pointer = Marshal.GetFunctionPointerForDelegate<FnPtr_EcsIterTPtr_Void.@delegate>(CallbackOnSet);
        desc->on_remove.Data.Pointer = Marshal.GetFunctionPointerForDelegate<FnPtr_EcsIterTPtr_Void.@delegate>(CallbackOnRemove);
#else
        desc->ctor.Data.Pointer = &CallbackConstructor;
        desc->dtor.Data.Pointer = &CallbackDeconstructor;
        desc->copy.Data.Pointer = &CallbackCopy;
        desc->move.Data.Pointer = &CallbackMove;
        desc->on_add.Data.Pointer = &CallbackOnAdd;
        desc->on_set.Data.Pointer = &CallbackOnSet;
        desc->on_remove.Data.Pointer = &CallbackOnRemove;
#endif
        desc->binding_ctx = (void*)CallbacksHelper.CreateComponentHooksCallbackContext(world, hooks);
    }

#if !UNITY_5_3_OR_NEWER
    [UnmanagedCallersOnly]
#endif
    private static void CallbackConstructor(void* pointer, int count, ecs_type_info_t* typeInfo)
    {
        ref var data = ref CallbacksHelper.GetComponentHooksCallbackContext(typeInfo->hooks.binding_ctx);
        var context = new ComponentConstructorContext(pointer, count);
        data.Hooks.Constructor?.Invoke(ref context);
    }

#if !UNITY_5_3_OR_NEWER
    [UnmanagedCallersOnly]
#endif
    private static void CallbackDeconstructor(void* pointer, int count, ecs_type_info_t* typeInfo)
    {
        ref var data = ref CallbacksHelper.GetComponentHooksCallbackContext(typeInfo->hooks.binding_ctx);
        var context = new ComponentDeconstructorContext(pointer, count);
        data.Hooks.Deconstructor?.Invoke(ref context);
    }

#if !UNITY_5_3_OR_NEWER
    [UnmanagedCallersOnly]
#endif
    private static void CallbackCopy(void* destinationPointer, void* sourcePointer, int count, ecs_type_info_t* typeInfo)
    {
        ref var data = ref CallbacksHelper.GetComponentHooksCallbackContext(typeInfo->hooks.binding_ctx);
        var context = new ComponentCopyContext(destinationPointer, sourcePointer, count);
        data.Hooks.Copy?.Invoke(ref context);
    }

#if !UNITY_5_3_OR_NEWER
    [UnmanagedCallersOnly]
#endif
    private static void CallbackMove(void* destinationPointer, void* sourcePointer, int count, PInvoke.ecs_type_info_t* typeInfo)
    {
        ref var data = ref CallbacksHelper.GetComponentHooksCallbackContext(typeInfo->hooks.binding_ctx);
        var context = new ComponentMoveContext(destinationPointer, sourcePointer, count);
        data.Hooks.Move?.Invoke(ref context);
    }

#if !UNITY_5_3_OR_NEWER
    [UnmanagedCallersOnly]
#endif
    private static void CallbackOnAdd(ecs_iter_t* it)
    {
        ref var data = ref CallbacksHelper.GetComponentHooksCallbackContext(it->binding_ctx);
        var iterator = new Iterator(data.World, it);
        data.Hooks.OnAdd?.Invoke(iterator);
    }

#if !UNITY_5_3_OR_NEWER
    [UnmanagedCallersOnly]
#endif
    private static void CallbackOnSet(ecs_iter_t* it)
    {
        ref var data = ref CallbacksHelper.GetComponentHooksCallbackContext(it->binding_ctx);
        var iterator = new Iterator(data.World, it);
        data.Hooks.OnSet?.Invoke(iterator);
    }

#if !UNITY_5_3_OR_NEWER
    [UnmanagedCallersOnly]
#endif
    private static void CallbackOnRemove(ecs_iter_t* it)
    {
        ref var data = ref CallbacksHelper.GetComponentHooksCallbackContext(it->binding_ctx);
        var iterator = new Iterator(data.World, it);
        data.Hooks.OnRemove?.Invoke(iterator);
    }
}
