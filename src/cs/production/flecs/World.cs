using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace flecs_hub;
using static flecs;

public unsafe class World
{
    private readonly IntPtr _libraryHandle;
    private readonly ecs_world_t* _worldHandle;

    private readonly ecs_id_t ECS_PAIR;
    private readonly ecs_entity_t EcsOnUpdate;
    private readonly ecs_entity_t EcsDependsOn;

    public int ExitCode { get; private set; }

    public World(string[] args)
    {
        _worldHandle = ecs_init_w_args(args);
        _libraryHandle = CLibrary.Load("libflecs.dylib");

        var ecsPair = CLibrary.GetExport(_libraryHandle, "ECS_PAIR");
        ECS_PAIR.Data = (ulong)Marshal.ReadInt64(ecsPair);

        var ecsOnUpdate = CLibrary.GetExport(_libraryHandle, "EcsOnUpdate");
        EcsOnUpdate.Data.Data = (ulong)Marshal.ReadInt64(ecsOnUpdate);
        
        var ecsDependsOn = CLibrary.GetExport(_libraryHandle, "EcsDependsOn");
        EcsDependsOn.Data.Data = (ulong)Marshal.ReadInt64(ecsDependsOn);
    }

    public int Fini()
    {
        CLibrary.Free(_libraryHandle);
        var exitCode = ecs_fini(_worldHandle);
        return exitCode;
    }

    public ecs_entity_t InitializeComponent<TComponent>()
        where TComponent : unmanaged
    {
        var componentType = typeof(TComponent);
        var componentName = componentType.Name;
        var componentNameC = Runtime.CStrings.CString(componentName);
        var structLayoutAttribute = componentType.StructLayoutAttribute;
        var structSize = Unsafe.SizeOf<TComponent>();
        var structAlignment = structLayoutAttribute!.Pack;

        var id = new ecs_entity_t();
        ecs_component_desc_t desc;
        desc.entity.entity = id;
        desc.entity.name = componentNameC;
        desc.entity.symbol = componentNameC;
        desc.type.size = structSize;
        desc.type.alignment = structAlignment;
        id = ecs_component_init(_worldHandle, &desc);
        Debug.Assert(id.Data.Data != 0, "ECS_INVALID_PARAMETER");
        return id;
    }

    public ecs_entity_t InitializeSystem(SystemCallback callback, string? filterExpression = null)
    {
        var id = new ecs_entity_t();
        ecs_system_desc_t desc = default;
        desc.entity.name = "Test";
        var phase = EcsOnUpdate;
        desc.entity.add[0] = phase.Data != 0 ? ecs_pair(EcsDependsOn, phase) : default;
        desc.entity.add[1] = phase;
        desc.query.filter.expr = filterExpression ?? string.Empty;
        desc.callback.Data.Pointer = &SystemCallback;
        desc.binding_ctx = (void*)SystemBindingContextHelper.CreateSystemBindingContext(callback);

        id = ecs_system_init(_worldHandle, &desc);
        return id;
    }
    
    public ecs_entity_t InitializeTag(string name)
    {
        ecs_entity_desc_t desc = default;
        desc.name = name;
        var id = ecs_entity_init(_worldHandle, &desc);
        Debug.Assert(id.Data != 0, "ECS_INVALID_PARAMETER");
        return id;
    }

    public ref T GetComponent<T>(ecs_entity_t entity, ecs_id_t id)
        where T : unmanaged
    {
        var pointer = ecs_get_id(_worldHandle, entity, id);
        return ref Unsafe.AsRef<T>(pointer);
    }
    
    public ecs_entity_t SetComponent<TComponent>(ecs_entity_t entity, ecs_id_t componentId, ref TComponent component)
        where TComponent : unmanaged
    {
        var structSize = Unsafe.SizeOf<TComponent>();
        var pointer = Unsafe.AsPointer(ref component);
        var result = ecs_set_id(_worldHandle, entity, componentId, (ulong)structSize, pointer);
        return result;
    }
    
    public ecs_entity_t SetComponent<TComponent>(ecs_entity_t entity, ecs_id_t componentId, TComponent component)
        where TComponent : unmanaged
    {
        var result = SetComponent(entity, componentId, ref component);
        return result;
    }

    [UnmanagedCallersOnly]
    private static void SystemCallback(ecs_iter_t* it)
    {
        SystemBindingContextHelper.GetSystemBindingContext((IntPtr)it->binding_ctx, out var data);
        
        var context = new Iterator(it);
        data.Callback(context);
    }

    public ecs_entity_t CreateEntity(string name)
    {
        var desc = default(ecs_entity_desc_t);
        desc.name = name;
        var result = ecs_entity_init(_worldHandle, &desc);
        return result;
    }
    
    // #define ecs_pair(pred, obj) (ECS_PAIR | ecs_entity_t_comb(obj, pred))

    public void AddPair(ecs_entity_t subject, ecs_entity_t relation, ecs_entity_t @object)
    {
        var id = ecs_pair(relation, @object);
        ecs_add_id(_worldHandle, subject, id);
    }

    public bool Progress(float deltaTime)
    {
        return ecs_progress(_worldHandle, deltaTime);
    }

    private ulong ecs_pair(ecs_entity_t pred, ecs_entity_t obj)
    {
        return ECS_PAIR | ecs_entity_t_comb(obj.Data.Data, pred.Data.Data);
    }

    private ulong ecs_entity_t_comb(ulong lo, ulong hi)
    {
        return (hi << 32) + lo;
    }
}
