// Copyright (c) Flecs Hub (https://github.com/flecs-hub). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using static flecs_hub.flecs;

namespace Flecs;

[PublicAPI]
public readonly unsafe struct Entity
{
    private readonly World _world;
    internal readonly ecs_entity_t _handle;

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

    public void RemoveTag<TTag>()
        where TTag : unmanaged, ITag
    {
        var tagId = _world.GetIdentifier<TTag>();
        ecs_remove_id(_world.Handle, _handle, tagId.Handle);
    }

    public void RemoveTag(Entity entity)
    {
        ecs_remove_id(_world.Handle, _handle, entity._handle);
    }

    public void AddPair<TTag1, TTag2>()
        where TTag1 : unmanaged, ITag
        where TTag2 : unmanaged, ITag
    {
        var tagId1 = _world.GetIdentifier<TTag1>();
        var tagId2 = _world.GetIdentifier<TTag2>();
        var id = ecs_pair(tagId1.Handle, tagId2.Handle);
        ecs_add_id(_world.Handle, _handle, id);
    }

    public void AddPairOverride<TTag1, TTag2>()
        where TTag1 : unmanaged, ITag
        where TTag2 : unmanaged, ITag
    {
        var tagId1 = _world.GetIdentifier<TTag1>();
        var tagId2 = _world.GetIdentifier<TTag2>();
        var id = ecs_pair(tagId1.Handle, tagId2.Handle);
        ecs_override_id(_world.Handle, _handle, id);
    }

    public void AddPair(Entity first, Entity second)
    {
        var id = ecs_pair(first._handle, second._handle);
        ecs_add_id(_world.Handle, _handle, id);
    }

    public void AddPairOverride(Entity first, Entity second)
    {
        var id = ecs_pair(first._handle, second._handle);
        ecs_override_id(_world.Handle, _handle, id);
    }

    public void AddPairFirst<TTag>(Entity first)
        where TTag : unmanaged, ITag
    {
        var tagId = _world.GetIdentifier<TTag>();
        var id = ecs_pair(first._handle, tagId.Handle);
        ecs_add_id(_world.Handle, _handle, id);
    }

    public void AddPairFirstOverride<TTag>(Entity first)
    where TTag : unmanaged, ITag
    {
        var tagId = _world.GetIdentifier<TTag>();
        var id = ecs_pair(first._handle, tagId.Handle);
        ecs_override_id(_world.Handle, _handle, id);
    }

    public void AddPairSecond<TTag>(Entity second)
        where TTag : unmanaged, ITag
    {
        var tagId = _world.GetIdentifier<TTag>();
        var id = ecs_pair(tagId.Handle, second._handle);
        ecs_add_id(_world.Handle, _handle, id);
    }

    public void AddPairSecondOverride<TTag>(Entity second)
        where TTag : unmanaged, ITag
    {
        var tagId = _world.GetIdentifier<TTag>();
        var id = ecs_pair(tagId.Handle, second._handle);
        ecs_override_id(_world.Handle, _handle, id);
    }

    public void SetPair<TTag, TComponent>(TComponent component) // right comp
        where TTag : unmanaged, ITag
        where TComponent : unmanaged, IComponent
    {
        var tagId = _world.GetIdentifier<TTag>();
        var compId = _world.GetIdentifier<TComponent>();
        SetPairData(component, tagId, compId);
    }

    public void SetPairOverride<TTag, TComponent>(TComponent component) // right comp
     where TTag : unmanaged, ITag
     where TComponent : unmanaged, IComponent
    {
        var tagId = _world.GetIdentifier<TTag>();
        var compId = _world.GetIdentifier<TComponent>();
        SetPairDataOverride(component, tagId, compId);
    }

    public void SetPair<TComponent, TTag>(TComponent component) // left comp
        where TComponent : unmanaged, IComponent
        where TTag : unmanaged, ITag
    {
        var componentId = _world.GetIdentifier<TComponent>();
        var tagId = _world.GetIdentifier<TTag>();
        SetPairData(component, componentId, tagId);
    }

    public void SetPairOverride<TComponent, TTag>(TComponent component) // left comp
      where TComponent : unmanaged, IComponent
      where TTag : unmanaged, ITag
    {
        var componentId = _world.GetIdentifier<TComponent>();
        var tagId = _world.GetIdentifier<TTag>();
        SetPairDataOverride(component, componentId, tagId);
    }

    public void SetPairFirstComp<TComponent1, TComponent2>(TComponent1 component) // assume left comp, right is used as tag
        where TComponent1 : unmanaged, IComponent
        where TComponent2 : unmanaged, IComponent
    {
        var componentId1 = _world.GetIdentifier<TComponent1>();
        var componentId2 = _world.GetIdentifier<TComponent2>();
        SetPairData(component, componentId1, componentId2);
    }

    public void SetPairFirstCompOverride<TComponent1, TComponent2>(TComponent1 component) // assume left comp, right is used as tag
       where TComponent1 : unmanaged, IComponent
       where TComponent2 : unmanaged, IComponent
    {
        var componentId1 = _world.GetIdentifier<TComponent1>();
        var componentId2 = _world.GetIdentifier<TComponent2>();
        SetPairDataOverride(component, componentId1, componentId2);
    }

    public void SetPairSecondComp<TComponent1, TComponent2>(TComponent2 component) // assume right comp, left is used as tag
        where TComponent1 : unmanaged, IComponent
        where TComponent2 : unmanaged, IComponent
    {
        var componentId1 = _world.GetIdentifier<TComponent1>();
        var componentId2 = _world.GetIdentifier<TComponent2>();
        SetPairData(component, componentId1, componentId2);
    }

    public void SetPairSecondCompOverride<TComponent1, TComponent2>(TComponent2 component) // assume right comp, left is used as tag
       where TComponent1 : unmanaged, IComponent
       where TComponent2 : unmanaged, IComponent
    {
        var componentId1 = _world.GetIdentifier<TComponent1>();
        var componentId2 = _world.GetIdentifier<TComponent2>();
        SetPairDataOverride(component, componentId1, componentId2);
    }

    public void SetPairFirstComp<TComponent>(TComponent first, Entity second) // assume left comp, right is used as tag
        where TComponent : unmanaged, IComponent
    {
        var firstId = _world.GetIdentifier<TComponent>();
        var secondId = new Identifier(_world, second._handle);
        SetPairData(first, firstId, secondId);
    }

    public void SetPairFirstCompOverride<TComponent>(TComponent first, Entity second) // assume left comp, right is used as tag
     where TComponent : unmanaged, IComponent
    {
        var firstId = _world.GetIdentifier<TComponent>();
        var secondId = new Identifier(_world, second._handle);
        SetPairDataOverride(first, firstId, secondId);
    }

    public void SetPairSecondComp<TComponent>(Entity first, TComponent second) // assume left comp, right is used as tag
        where TComponent : unmanaged, IComponent
    {
        var firstId = new Identifier(_world, first._handle);
        var secondId = _world.GetIdentifier<TComponent>();
        SetPairData(second, firstId, secondId);
    }

    public void SetPairSecondCompOverride<TComponent>(Entity first, TComponent second) // assume left comp, right is used as tag
        where TComponent : unmanaged, IComponent
    {
        var firstId = new Identifier(_world, first._handle);
        var secondId = _world.GetIdentifier<TComponent>();
        SetPairDataOverride(second, firstId, secondId);
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

    public ref TComponent GetPairFirst<TComponent, TTag>()
        where TTag : unmanaged, ITag
        where TComponent : unmanaged, IComponent
    {
        var compId = _world.GetIdentifier<TComponent>();
        var tagId = _world.GetIdentifier<TTag>();
        return ref GetPairData<TComponent>(compId, tagId);
    }

    public ref TComponent GetPairSecond<TTag, TComponent>()
        where TTag : unmanaged, ITag
        where TComponent : unmanaged, IComponent
    {
        var tagId = _world.GetIdentifier<TTag>();
        var compId = _world.GetIdentifier<TComponent>();
        return ref GetPairData<TComponent>(tagId, compId);
    }

    public ref TComponent1 GetPairFirstComp<TComponent1, TComponent2>()
        where TComponent1 : unmanaged, IComponent
        where TComponent2 : unmanaged, IComponent
    {
        var componentId1 = _world.GetIdentifier<TComponent1>();
        var componentId2 = _world.GetIdentifier<TComponent2>();
        return ref GetPairData<TComponent1>(componentId1, componentId2);
    }

    public ref TComponent GetPairFirstComp<TComponent>(Entity second)
        where TComponent : unmanaged, IComponent
    {
        var compId = _world.GetIdentifier<TComponent>();
        return ref GetPairData<TComponent>(compId, new Identifier(_world, second._handle));
    }

    public ref TComponent GetPairSecondComp<TComponent>(Entity first)
        where TComponent : unmanaged, IComponent
    {
        var compId = _world.GetIdentifier<TComponent>();
        return ref GetPairData<TComponent>(new Identifier(_world, first._handle), compId);
    }

    public ref TComponent2 GetPairSecondComp<TComponent1, TComponent2>()
        where TComponent1 : unmanaged, IComponent
        where TComponent2 : unmanaged, IComponent
    {
        var componentId1 = _world.GetIdentifier<TComponent1>();
        var componentId2 = _world.GetIdentifier<TComponent2>();
        return ref GetPairData<TComponent2>(componentId1, componentId2);
    }

    private ref TComp GetPairData<TComp>(Identifier first, Identifier second)
        where TComp : unmanaged, IComponent
    {
        var id = ecs_pair(first.Handle, second.Handle);
        var pointer = ecs_get_id(_world.Handle, _handle, id);
        return ref Unsafe.AsRef<TComp>(pointer);
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

    public void SetComponent<TComponent>(ref TComponent component)
        where TComponent : unmanaged, IComponent
    {
        var componentId = _world.GetIdentifier<TComponent>();
        var structSize = Unsafe.SizeOf<TComponent>();
        var pointer = Unsafe.AsPointer(ref component);
        ecs_set_id(_world.Handle, _handle, componentId.Handle, (ulong)structSize, pointer);
    }

    public void SetComponent<TComponent>(TComponent component)
        where TComponent : unmanaged, IComponent
    {
        SetComponent(ref component);
    }

    public void SetComponentOverride<TComponent>(ref TComponent component)
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

    public void SetComponentOverride<TComponent>(TComponent component)
        where TComponent : unmanaged, IComponent
    {
        SetComponentOverride(ref component);
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

    public bool HasPair<TTag1, TTag2>()
        where TTag1 : unmanaged, ITag
        where TTag2 : unmanaged, ITag
    {
        var tagId1 = _world.GetIdentifier<TTag1>();
        var tagId2 = _world.GetIdentifier<TTag2>();
        var id = ecs_pair(tagId1.Handle, tagId2.Handle);
        return ecs_has_id(_world.Handle, _handle, id);
    }

    public bool HasPair(Entity first, Entity second)
    {
        var id = ecs_pair(first._handle, second._handle);
        return ecs_has_id(_world.Handle, _handle, id);
    }

    public bool HasPairFirstComp<TComponent>(Entity second)
        where TComponent : unmanaged, IComponent
    {
        var compId = _world.GetIdentifier<TComponent>();
        var id = ecs_pair(compId.Handle, second._handle);
        return ecs_has_id(_world.Handle, _handle, id);
    }

    public bool HasPairComp<TComponent1, TComponent2>()
        where TComponent1 : unmanaged, IComponent
        where TComponent2 : unmanaged, IComponent
    {
        var compId1 = _world.GetIdentifier<TComponent1>();
        var compId2 = _world.GetIdentifier<TComponent2>();
        var id = ecs_pair(compId1.Handle, compId2.Handle);
        return ecs_has_id(_world.Handle, _handle, id);
    }

    public bool HasPairSecondComp<TComponent>(Entity first)
        where TComponent : unmanaged, IComponent
    {
        var compId = _world.GetIdentifier<TComponent>();
        var id = ecs_pair(first._handle, compId.Handle);
        return ecs_has_id(_world.Handle, _handle, id);
    }

    public bool HasPairSecond<TTag>(Entity first)
        where TTag : unmanaged, ITag
    {
        var tagId = _world.GetIdentifier<TTag>();
        var id = ecs_pair(first._handle, tagId.Handle);
        return ecs_has_id(_world.Handle, _handle, id);
    }

    public bool HasPairFirst<TTag>(Entity second)
        where TTag : unmanaged, ITag
    {
        var tagId = _world.GetIdentifier<TTag>();
        var id = ecs_pair(tagId.Handle, second._handle);
        return ecs_has_id(_world.Handle, _handle, id);
    }

    public void DependsOn(Entity entity)
    {
        AddPair(new Entity(_world, EcsDependsOn), entity);
    }

    public bool IsChildOf(Entity entity)
    {
        var pair = ecs_childof(entity._handle);
        var result = ecs_has_id(_world.Handle, _handle, pair);
        return result;
    }

    public string FullPathString()
    {
        var cString = ecs_get_path_w_sep(_world.Handle, default, _handle, ".", default);
        var result = Marshal.PtrToStringAnsi(cString._pointer)!;
        Marshal.FreeHGlobal(cString._pointer);
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
}
