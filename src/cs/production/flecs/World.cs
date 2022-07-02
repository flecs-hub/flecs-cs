using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using static flecs_hub.flecs;

namespace flecs;

[PublicAPI]
public unsafe class World
{
    internal static Dictionary<IntPtr, World> Pointers = new();

    internal readonly ecs_world_t* Handle;
    private Dictionary<Type, ecs_entity_t> _componentIdentifiersByType = new();
    private Dictionary<Type, ecs_entity_t> _tagIdentifiersByType = new();

    private readonly ecs_id_t ECS_PAIR;
    private readonly ecs_entity_t EcsOnUpdate;
    private readonly ecs_entity_t EcsDependsOn;

    public int ExitCode { get; private set; }

    public World(string[] args)
    {
        var argv = args.Length == 0 ? default : Runtime.CStrings.CStringArray(args);
        Handle = ecs_init_w_args(args.Length, argv);
        Pointers.Add((IntPtr)Handle, this);
        Runtime.CStrings.FreeCStrings(argv, args.Length);

        ECS_PAIR = pinvoke_ECS_PAIR();
        EcsOnUpdate = pinvoke_EcsOnUpdate();
        EcsDependsOn = pinvoke_EcsDependsOn();
    }

    public int Fini()
    {
        Pointers.Remove((IntPtr)Handle);
        var exitCode = ecs_fini(Handle);
        return exitCode;
    }

    public void InitializeComponent<TComponent>()
        where TComponent : unmanaged
    {
        var type = typeof(TComponent);
        var componentName = GetFlecsTypeName(type);
        var componentNameC = Runtime.CStrings.CString(componentName);
        var structLayoutAttribute = type.StructLayoutAttribute;
        CheckStructLayout(structLayoutAttribute);
        var structSize = Unsafe.SizeOf<TComponent>();
        var structAlignment = structLayoutAttribute!.Pack;

        var id = new ecs_entity_t();
        ecs_component_desc_t desc;
        desc.entity.entity = id;
        desc.entity.name = componentNameC;
        desc.entity.symbol = componentNameC;
        desc.type.size = structSize;
        desc.type.alignment = structAlignment;
        id = ecs_component_init(Handle, &desc);
        Debug.Assert(id.Data.Data != 0, "ECS_INVALID_PARAMETER");
        _componentIdentifiersByType[typeof(TComponent)] = id;
    }

    public void InitializeTag<TTag>()
        where TTag : unmanaged, ITag
    {
        ecs_entity_desc_t desc = default;
        var type = typeof(TTag);
        var typeName = GetFlecsTypeName<TTag>();
        desc.name = typeName;
        var id = ecs_entity_init(Handle, &desc);
        Debug.Assert(id.Data != 0, "ECS_INVALID_PARAMETER");
        _tagIdentifiersByType[type] = id;
    }

    public ecs_entity_t InitializeSystem(
        SystemCallback callback, ecs_entity_t phase, string filterExpression, string? name = null)
    {
        ecs_system_desc_t desc = default;
        FillSystemDescriptorCommon(ref desc, callback, phase, name);

        desc.query.filter.expr = filterExpression;

        var id = ecs_system_init(Handle, &desc);
        return id;
    }

    public ecs_entity_t InitializeSystem<TComponent1>(
        SystemCallback callback, ecs_entity_t phase, string? name = null)
    {
        ecs_system_desc_t desc = default;
        FillSystemDescriptorCommon(ref desc, callback, phase, name);

        desc.query.filter.expr = GetFlecsTypeName<TComponent1>();

        var id = ecs_system_init(Handle, &desc);
        return id;
    }

    public ecs_entity_t InitializeSystem<TComponent1, TComponent2>(
        SystemCallback callback, string? name = null)
    {
        var id = new ecs_entity_t();
        ecs_system_desc_t desc = default;
        desc.entity.name = name ?? callback.Method.Name;
        var phase = EcsOnUpdate;
        desc.entity.add[0] = phase.Data != 0 ? ecs_pair(EcsDependsOn, phase) : default;
        desc.entity.add[1] = phase;
        desc.callback.Data.Pointer = &SystemCallback;
        desc.binding_ctx = (void*)SystemBindingContextHelper.CreateSystemBindingContext(this, callback);

        var componentName1 = GetFlecsTypeName<TComponent1>();
        var componentName2 = GetFlecsTypeName<TComponent2>();
        desc.query.filter.expr = componentName1 + ", " + componentName2;

        id = ecs_system_init(Handle, &desc);
        return id;
    }

    private void FillSystemDescriptorCommon(
        ref ecs_system_desc_t desc, SystemCallback callback, ecs_entity_t phase, string? name)
    {
        desc.entity.name = name ?? callback.Method.Name;
        desc.entity.add[0] = phase.Data != 0 ? ecs_pair(EcsDependsOn, phase) : default;
        desc.entity.add[1] = phase;
        desc.callback.Data.Pointer = &SystemCallback;
        desc.binding_ctx = (void*)SystemBindingContextHelper.CreateSystemBindingContext(this, callback);
    }

    [UnmanagedCallersOnly]
    private static void SystemCallback(ecs_iter_t* it)
    {
        SystemBindingContextHelper.GetSystemBindingContext((IntPtr)it->binding_ctx, out var data);
        
        var iterator = new SystemIterator(data.World, it);
        data.Callback(iterator);
    }

    public Entity InitializeEntity(string name)
    {
        var desc = default(ecs_entity_desc_t);
        desc.name = name;
        var entity = ecs_entity_init(Handle, &desc);
        var result = new Entity(this, entity);
        return result;
    }

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

    private static void CheckStructLayout(StructLayoutAttribute? structLayoutAttribute)
    {
        if (structLayoutAttribute == null || structLayoutAttribute.Value == LayoutKind.Auto)
        {
            throw new FlecsException(
                "Component must have a StructLayout attribute with LayoutKind sequential or explicit. This is to ensure that the struct fields are not reorganized by the C# compiler.");
        }
    }

    private string GetFlecsTypeName(Type type)
    {
        return type.FullName!.Replace("+", ".", StringComparison.InvariantCulture);
    }
    
    private string GetFlecsTypeName<T>()
    {
        return GetFlecsTypeName(typeof(T));
    }

    internal ecs_id_t GetComponentIdentifierFrom(Type type)
    {
        return _componentIdentifiersByType[type];
    }

    internal ecs_id_t GetTagIdentifierFrom(Type type)
    {
        return _tagIdentifiersByType[type];
    }
}