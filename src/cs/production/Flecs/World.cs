// Copyright (c) Flecs Hub (https://github.com/flecs-hub). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using static flecs_hub.flecs;

namespace Flecs;

[PublicAPI]
public unsafe class World
{
    internal static Dictionary<IntPtr, World> Pointers = new();

    internal readonly ecs_world_t* Handle;
    private Dictionary<Type, ulong> _componentIdentifiersByType = new();
    private Dictionary<Type, ulong> _tagIdentifiersByType = new();

    public int ExitCode { get; private set; }

    public World(string[] args)
    {
        var argv = args.Length == 0 ? default : Runtime.CStrings.CStringArray(args);
        Handle = ecs_init_w_args(args.Length, argv);
        Pointers.Add((IntPtr)Handle, this);
        Runtime.CStrings.FreeCStrings(argv, args.Length);
    }

    public int Fini()
    {
        Pointers.Remove((IntPtr)Handle);
        var exitCode = ecs_fini(Handle);
        return exitCode;
    }

    public void RegisterComponent<TComponent>(ComponentHooks? hooks = null)
        where TComponent : unmanaged
    {
        var type = typeof(TComponent);
        var componentName = GetFlecsTypeName(type);
        var componentNameC = Runtime.CStrings.CString(componentName);
        var structLayoutAttribute = type.StructLayoutAttribute;
        CheckStructLayout(structLayoutAttribute);
        var structSize = Unsafe.SizeOf<TComponent>();
        var structAlignment = structLayoutAttribute!.Pack;

        ecs_entity_desc_t entityDesc = default;
        entityDesc.name = componentNameC;
        entityDesc.symbol = componentNameC;
        ecs_component_desc_t componentDesc = default;
        componentDesc.entity = ecs_entity_init(Handle, &entityDesc);
        componentDesc.type.size = structSize;
        componentDesc.type.alignment = structAlignment;
        var id = ecs_component_init(Handle, &componentDesc);
        _componentIdentifiersByType[typeof(TComponent)] = id.Data.Data;
        SetHooks(hooks, id);
    }

    public void RegisterTag<TTag>()
        where TTag : unmanaged, ITag
    {
        ecs_entity_desc_t desc = default;
        var type = typeof(TTag);
        var typeName = GetFlecsTypeName<TTag>();
        desc.name = typeName;
        var id = ecs_entity_init(Handle, &desc);
        Debug.Assert(id.Data != 0, "ECS_INVALID_PARAMETER");
        _tagIdentifiersByType[type] = id.Data.Data;
    }

    public void RegisterSystem(
        CallbackIterator callback, ecs_entity_t phase, string filterExpression, string? name = null)
    {
        ecs_system_desc_t desc = default;
        FillSystemDescriptorCommon(ref desc, callback, phase, name);

        desc.query.filter.expr = filterExpression;
        ecs_system_init(Handle, &desc);
    }

    public void RegisterSystem<TComponent1>(
        CallbackIterator callback, ecs_entity_t phase, string? name = null)
    {
        ecs_system_desc_t desc = default;
        FillSystemDescriptorCommon(ref desc, callback, phase, name);

        desc.query.filter.expr = GetFlecsTypeName<TComponent1>();
        ecs_system_init(Handle, &desc);
    }

    public void RegisterSystem<TComponent1, TComponent2>(
        CallbackIterator callback, string? name = null)
    {
        ecs_system_desc_t desc = default;
        desc.query.filter.name = name ?? callback.Method.Name;
        var phase = EcsOnUpdate;
        FillSystemDescriptorCommon(ref desc, callback, phase, name);

        var componentName1 = GetFlecsTypeName<TComponent1>();
        var componentName2 = GetFlecsTypeName<TComponent2>();
        desc.query.filter.expr = componentName1 + ", " + componentName2;
        ecs_system_init(Handle, &desc);
    }

    private void FillSystemDescriptorCommon(
        ref ecs_system_desc_t systemDesc, CallbackIterator callback, ecs_entity_t phase, string? name)
    {
        ecs_entity_desc_t entityDesc = default;
        entityDesc.name = name ?? callback.Method.Name;
        entityDesc.add[0] = phase.Data != 0 ? ecs_pair(EcsDependsOn, phase) : default;
        entityDesc.add[1] = phase;
        systemDesc.entity = ecs_entity_init(Handle, &entityDesc);
        systemDesc.callback.Data.Pointer = &SystemCallback;
        systemDesc.binding_ctx = (void*)CallbacksHelper.CreateSystemCallbackContext(this, callback);
    }

    [UnmanagedCallersOnly]
    private static void SystemCallback(ecs_iter_t* it)
    {
        CallbacksHelper.GetSystemCallbackContext((IntPtr)it->binding_ctx, out var data);

        var iterator = new Iterator(data.World, it);
        data.Callback(iterator);
    }

    public Entity CreateEntity(string name)
    {
        var desc = default(ecs_entity_desc_t);
        desc.name = name;
        var entity = ecs_entity_init(Handle, &desc);
        var result = new Entity(this, entity);
        return result;
    }

    public Entity CreatePrefab(string name)
    {
        var desc = default(ecs_entity_desc_t);
        desc.name = name;
        desc.add[0] = pinvoke_EcsPrefab();

        var entity = ecs_entity_init(Handle, &desc);
        var result = new Entity(this, entity);
        return result;
    }

    // public Query CreateQuery(ref QueryDescriptor descriptor)
    // {
    //     ecs_query_t* query;
    //     fixed (ecs_query_desc_t* desc = &descriptor)
    //     {
    //        query = ecs_query_init(Handle, desc);
    //     }
    //
    //     return new Query(this, query);
    // }

    public EntityIterator EntityIterator<TComponent>()
        where TComponent : unmanaged, IComponent
    {
        var term = default(ecs_term_t);
        term.id = _componentIdentifiersByType[typeof(TComponent)];
        var iterator = ecs_term_iter(Handle, &term);
        var result = new EntityIterator(this, iterator);
        return result;
    }

    public bool Progress(float deltaTime)
    {
        return ecs_progress(Handle, deltaTime);
    }

    private void SetHooks(ComponentHooks? hooksNullable, ecs_entity_t id)
    {
        if (hooksNullable == null)
        {
            return;
        }

        var hooksDesc = default(ecs_type_hooks_t);
        var hooks = hooksNullable.Value;
        ComponentHooks.Fill(this, ref hooks, &hooksDesc);
        ecs_set_hooks_id(Handle, id, &hooksDesc);
    }

    private static void CheckStructLayout(StructLayoutAttribute? structLayoutAttribute)
    {
        if (structLayoutAttribute == null || structLayoutAttribute.Value == LayoutKind.Auto)
        {
            throw new FlecsException(
                "Component must have a StructLayout attribute with LayoutKind sequential or explicit. This is to ensure that the struct fields are not reorganized by the C# compiler.");
        }
    }

    public string GetFlecsTypeName(Type type)
    {
        return type.FullName!.Replace("+", ".", StringComparison.InvariantCulture);
    }

    public string GetFlecsTypeName<T>()
    {
        return GetFlecsTypeName(typeof(T));
    }

    public Identifier GetComponentIdentifier<TComponent>()
        where TComponent : unmanaged, IComponent
    {
        var type = typeof(TComponent);
        var containsKey = _componentIdentifiersByType.TryGetValue(type, out var value);
        if (!containsKey)
        {
            RegisterComponent<TComponent>();
            value = _componentIdentifiersByType[type];
        }

        var id = default(ecs_id_t);
        id.Data = value;
        return new Identifier(this, id);
    }

    public Identifier GetTagIdentifier<TTag>()
        where TTag : unmanaged, ITag
    {
        var type = typeof(TTag);
        var containsKey = _tagIdentifiersByType.TryGetValue(type, out var value);
        if (!containsKey)
        {
            RegisterTag<TTag>();
            value = _tagIdentifiersByType[type];
        }

        var id = default(ecs_id_t);
        id.Data = value;
        return new Identifier(this, id);
    }
}
