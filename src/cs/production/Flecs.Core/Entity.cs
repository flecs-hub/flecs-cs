// Copyright (c) Flecs Hub (https://github.com/flecs-hub). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using bottlenoselabs.C2CS.Runtime;
using JetBrains.Annotations;
using static flecs_hub.flecs;

namespace Flecs;

[PublicAPI]
public readonly unsafe struct Entity
{
    internal readonly ecs_entity_t _handle;
    private readonly World _world;

    internal Entity(World world, ecs_entity_t handle)
    {
        _world = world;
        _handle = handle;
    }

    public static Entity FromIdentifier(Identifier identifier)
    {
        return new Entity(identifier.World, identifier.Handle);
    }

    public EntityType Type()
    {
        var type = ecs_get_type(_world.Handle, _handle);
        var result = new EntityType(_world, type);
        return result;
    }

    public void Add<T>()
        where T : unmanaged, IEcsComponent
    {
        var tagId = _world.GetIdentifier<T>();
        ecs_add_id(_world.Handle, _handle, tagId.Handle);
    }

    public void Add(Entity entity)
    {
        ecs_add_id(_world.Handle, _handle, entity._handle);
    }

    public void Remove<T>()
        where T : unmanaged, IEcsComponent
    {
        var tagId = _world.GetIdentifier<T>();
        ecs_remove_id(_world.Handle, _handle, tagId.Handle);
    }

    public void Remove(Entity entity)
    {
        ecs_remove_id(_world.Handle, _handle, entity._handle);
    }

    public void Add<TTag1, TTag2>()
        where TTag1 : unmanaged, ITag
        where TTag2 : unmanaged, ITag
    {
        var tagId1 = _world.GetIdentifier<TTag1>();
        var tagId2 = _world.GetIdentifier<TTag2>();
        var id = ecs_pair(tagId1.Handle, tagId2.Handle);
        ecs_add_id(_world.Handle, _handle, id);
    }

    public void AddOverride<TTag1, TTag2>()
        where TTag1 : unmanaged, ITag
        where TTag2 : unmanaged, ITag
    {
        var tagId1 = _world.GetIdentifier<TTag1>();
        var tagId2 = _world.GetIdentifier<TTag2>();
        var id = ecs_pair(tagId1.Handle, tagId2.Handle);
        ecs_override_id(_world.Handle, _handle, id);
    }

    public void Add(Entity first, Entity second)
    {
        var id = ecs_pair(first._handle, second._handle);
        ecs_add_id(_world.Handle, _handle, id);
    }

    public void AddOverride(Entity first, Entity second)
    {
        var id = ecs_pair(first._handle, second._handle);
        ecs_override_id(_world.Handle, _handle, id);
    }

    public void Add<TTag>(Entity first)
        where TTag : unmanaged, ITag
    {
        var tagId = _world.GetIdentifier<TTag>();
        var id = ecs_pair(first._handle, tagId.Handle);
        ecs_add_id(_world.Handle, _handle, id);
    }

    public void AddOverride<TTag>(Entity first)
    where TTag : unmanaged, ITag
    {
        var tagId = _world.GetIdentifier<TTag>();
        var id = ecs_pair(first._handle, tagId.Handle);
        ecs_override_id(_world.Handle, _handle, id);
    }

    public void AddSecond<TTag>(Entity second)
        where TTag : unmanaged, ITag
    {
        var tagId = _world.GetIdentifier<TTag>();
        var id = ecs_pair(tagId.Handle, second._handle);
        ecs_add_id(_world.Handle, _handle, id);
    }

    public void AddSecondOverride<TTag>(Entity second)
        where TTag : unmanaged, ITag
    {
        var tagId = _world.GetIdentifier<TTag>();
        var id = ecs_pair(tagId.Handle, second._handle);
        ecs_override_id(_world.Handle, _handle, id);
    }

    public void Set<TEcsComponent, TComponent>(TComponent component) // right comp
        where TEcsComponent : unmanaged, IEcsComponent
        where TComponent : unmanaged, IComponent
    {
        var ecsCompId = _world.GetIdentifier<TEcsComponent>();
        var compId = _world.GetIdentifier<TComponent>();
        SetPairData(component, ecsCompId, compId);
    }

    public void SetOverride<TEcsComponent, TComponent>(TComponent component) // right comp
     where TEcsComponent : unmanaged, IEcsComponent
     where TComponent : unmanaged, IComponent
    {
        var ecsCompId = _world.GetIdentifier<TEcsComponent>();
        var compId = _world.GetIdentifier<TComponent>();
        SetPairDataOverride(component, ecsCompId, compId);
    }

    public void Set<TComponent, TEcsComponent>(TComponent component) // left comp
        where TComponent : unmanaged, IComponent
        where TEcsComponent : unmanaged, IEcsComponent
    {
        var compId = _world.GetIdentifier<TComponent>();
        var ecsCompId = _world.GetIdentifier<TEcsComponent>();
        SetPairData(component, compId, ecsCompId);
    }

    public void SetOverride<TComponent, TEcsComponent>(TComponent component) // left comp
        where TComponent : unmanaged, IComponent
        where TEcsComponent : unmanaged, IEcsComponent
    {
        var compId = _world.GetIdentifier<TComponent>();
        var ecsCompId = _world.GetIdentifier<TEcsComponent>();
        SetPairData(component, compId, ecsCompId);
    }

    public void Set<TComponent>(TComponent first, Entity second) // assume left comp, right is used as tag
        where TComponent : unmanaged, IComponent
    {
        var firstId = _world.GetIdentifier<TComponent>();
        var secondId = new Identifier(_world, second._handle);
        SetPairData(first, firstId, secondId);
    }

    public void SetOverride<TComponent>(TComponent first, Entity second) // assume left comp, right is used as tag
     where TComponent : unmanaged, IComponent
    {
        var firstId = _world.GetIdentifier<TComponent>();
        var secondId = new Identifier(_world, second._handle);
        SetPairDataOverride(first, firstId, secondId);
    }

    public void Set<TComponent>(Entity first, TComponent second) // assume left comp, right is used as tag
        where TComponent : unmanaged, IComponent
    {
        var firstId = new Identifier(_world, first._handle);
        var secondId = _world.GetIdentifier<TComponent>();
        SetPairData(second, firstId, secondId);
    }

    public void SetOverride<TComponent>(Entity first, TComponent second) // assume left comp, right is used as tag
        where TComponent : unmanaged, IComponent
    {
        var firstId = new Identifier(_world, first._handle);
        var secondId = _world.GetIdentifier<TComponent>();
        SetPairDataOverride(second, firstId, secondId);
    }

    public ref TComponent Get<TComponent, TEscComponent>()
        where TComponent : unmanaged, IComponent
        where TEscComponent : unmanaged, IEcsComponent
    {
        var compId = _world.GetIdentifier<TComponent>();
        var tagId = _world.GetIdentifier<TEscComponent>();
        return ref GetPairData<TComponent>(compId, tagId);
    }

    public ref TComponent GetSecond<TEscComponent, TComponent>()
        where TEscComponent : unmanaged, IEcsComponent
        where TComponent : unmanaged, IComponent
    {
        var tagId = _world.GetIdentifier<TEscComponent>();
        var compId = _world.GetIdentifier<TComponent>();
        return ref GetPairData<TComponent>(tagId, compId);
    }

    public ref TComponent Get<TComponent>(Entity second)
        where TComponent : unmanaged, IComponent
    {
        var compId = _world.GetIdentifier<TComponent>();
        return ref GetPairData<TComponent>(compId, new Identifier(_world, second._handle));
    }

    public ref TComponent GetSecond<TComponent>(Entity first)
        where TComponent : unmanaged, IComponent
    {
        var compId = _world.GetIdentifier<TComponent>();
        return ref GetPairData<TComponent>(new Identifier(_world, first._handle), compId);
    }

    public Entity GetTarget(Entity relation)
    {
        var target = ecs_get_target(_world.Handle, _handle, relation._handle, 0);
        return new Entity(_world, target);
    }

    public void AddParent(Entity entity)
    {
        var id = ecs_pair(EcsChildOf, entity._handle);
        ecs_add_id(_world.Handle, _handle, id);
    }

    public void IsA(Entity entity)
    {
        var id = ecs_pair(EcsIsA, entity._handle);
        ecs_add_id(_world.Handle, _handle, id);
    }

    public void AddSlotOf(Entity entity)
    {
        var id = ecs_pair(EcsSlotOf, entity._handle);
        ecs_add_id(_world.Handle, _handle, id);
    }

    public ref TComponent GetComponent<TComponent>()
        where TComponent : unmanaged, IComponent
    {
        var componentId = _world.GetIdentifier<TComponent>();
        var pointer = ecs_get_id(_world.Handle, _handle, componentId.Handle);
        return ref Unsafe.AsRef<TComponent>(pointer);
    }

    public void Set<TComponent>(ref TComponent component)
        where TComponent : unmanaged, IComponent
    {
        var componentId = _world.GetIdentifier<TComponent>();
        var structSize = Unsafe.SizeOf<TComponent>();
        var pointer = Unsafe.AsPointer(ref component);
        ecs_set_id(_world.Handle, _handle, componentId.Handle, (ulong)structSize, pointer);
    }

    public void Set<TComponent>(TComponent component)
        where TComponent : unmanaged, IComponent
    {
        Set(ref component);
    }

    public void SetOverride<TComponent>(ref TComponent component)
        where TComponent : unmanaged, IComponent
    {
        var componentId = _world.GetIdentifier<TComponent>();
        var structSize = Unsafe.SizeOf<TComponent>();
        var pointer = Unsafe.AsPointer(ref component);

        var overrideValue = ECS_OVERRIDE.Data;
        var identifierValue = overrideValue | componentId.Handle.Data;
        ecs_add_id(_world.Handle, _handle, *(ecs_id_t*)&identifierValue);
        ecs_set_id(_world.Handle, _handle, componentId.Handle, (ulong)structSize, pointer);
    }

    public void SetOverride<TComponent>(TComponent component)
        where TComponent : unmanaged, IComponent
    {
        SetOverride(ref component);
    }

    public void Delete()
    {
        ecs_delete(_world.Handle, _handle);
    }

    public string Name()
    {
        var result = ecs_get_name(_world.Handle, _handle);
        return result.ToString();
    }

    public bool Has<T>()
        where T : unmanaged, IEcsComponent
    {
        var compId = _world.GetIdentifier<T>();
        return ecs_has_id(_world.Handle, _handle, compId.Handle);
    }

    public bool Has<T1, T2>()
        where T1 : unmanaged, IEcsComponent
        where T2 : unmanaged, IEcsComponent
    {
        var id1 = _world.GetIdentifier<T1>();
        var id2 = _world.GetIdentifier<T2>();
        var id = ecs_pair(id1.Handle, id2.Handle);
        return ecs_has_id(_world.Handle, _handle, id);
    }

    public bool Has(Entity first, Entity second)
    {
        var id = ecs_pair(first._handle, second._handle);
        return ecs_has_id(_world.Handle, _handle, id);
    }

    public bool Has<T>(Entity second)
        where T : unmanaged, IEcsComponent
    {
        var compId = _world.GetIdentifier<T>();
        var id = ecs_pair(compId.Handle, second._handle);
        return ecs_has_id(_world.Handle, _handle, id);
    }

    public bool HasSecond<T>(Entity first)
        where T : unmanaged, IEcsComponent
    {
        var compId = _world.GetIdentifier<T>();
        var id = ecs_pair(first._handle, compId.Handle);
        return ecs_has_id(_world.Handle, _handle, id);
    }

    public void DependsOn(Entity entity)
    {
        Add(new Entity(_world, EcsDependsOn), entity);
    }

    public bool IsChildOf(Entity entity)
    {
        var pair = ecs_childof(entity._handle);
        var result = ecs_has_id(_world.Handle, _handle, pair);
        return result;
    }

    public string FullPathString()
    {
        var cString = ecs_get_path_w_sep(_world.Handle, default, _handle, (CString)".", default);
        var result = Marshal.PtrToStringAnsi(cString.Pointer)!;
        Marshal.FreeHGlobal(cString.Pointer);
        return result;
    }

    public EntityIterator Children()
    {
        var term = default(ecs_term_t);
        term.id = ecs_childof(_handle);
        var iterator = ecs_term_iter(_world.Handle, &term);
        var result = new EntityIterator(_world, iterator);
        return result;
    }

    private ref TComp GetPairData<TComp>(Identifier first, Identifier second)
        where TComp : unmanaged, IComponent
    {
        var id = ecs_pair(first.Handle, second.Handle);
        var pointer = ecs_get_id(_world.Handle, _handle, id);
        return ref Unsafe.AsRef<TComp>(pointer);
    }

    private void SetPairData<TComponent>(TComponent component, Identifier left, Identifier right)
        where TComponent : unmanaged, IComponent
    {
        var id = ecs_pair(left.Handle, right.Handle);
        var structSize = Unsafe.SizeOf<TComponent>();
        var pointer = Unsafe.AsPointer(ref component);
        ecs_set_id(_world.Handle, _handle, id, (ulong)structSize, pointer);
    }

    private void SetPairDataOverride<TComponent>(TComponent component, Identifier left, Identifier right)
        where TComponent : unmanaged, IComponent
    {
        var id = ecs_pair(left.Handle, right.Handle);
        var structSize = Unsafe.SizeOf<TComponent>();
        var pointer = Unsafe.AsPointer(ref component);
        ecs_override_id(_world.Handle, _handle, id);
        ecs_set_id(_world.Handle, _handle, id, (ulong)structSize, pointer);
    }
}
