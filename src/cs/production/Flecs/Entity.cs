// Copyright (c) Flecs Hub (https://github.com/flecs-hub). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using static flecs_hub.flecs;

namespace Flecs;
[PublicAPI]
public readonly unsafe struct Entity
{
    private readonly World _world;
    private readonly ecs_entity_t _handle;

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

    public void AddTag<TTag>()
        where TTag : unmanaged, ITag
    {
        var tagId = _world.GetTagIdentifier<TTag>();
        ecs_add_id(_world.Handle, _handle, tagId.Handle);
    }

    public void RemoveTag<TTag>()
        where TTag : unmanaged, ITag
    {
        var tagId = _world.GetTagIdentifier<TTag>();
        ecs_remove_id(_world.Handle, _handle, tagId.Handle);
    }

    public void AddPair<TTag1, TTag2>()
        where TTag1 : unmanaged, ITag
        where TTag2 : unmanaged, ITag
    {
        var tagId1 = _world.GetTagIdentifier<TTag1>();
        var tagId2 = _world.GetTagIdentifier<TTag2>();
        var id = ecs_pair(tagId1.Handle, tagId2.Handle);
        ecs_add_id(_world.Handle, _handle, id);
    }

    public void AddPair(Entity first, Entity second)
    {
        var id = ecs_pair(first._handle, second._handle);
        ecs_add_id(_world.Handle, _handle, id);
    }

    public void AddPairFirst<TTag>(Entity first)
      where TTag : unmanaged, ITag
    {
        var tagId = _world.GetTagIdentifier<TTag>();
        var id = ecs_pair(first._handle, tagId.Handle);
        ecs_add_id(_world.Handle, _handle, id);
    }

    public void AddPairSecond<TTag>(Entity second)
       where TTag : unmanaged, ITag
    {
        var tagId = _world.GetTagIdentifier<TTag>();
        var id = ecs_pair(tagId.Handle, second._handle);
        ecs_add_id(_world.Handle, _handle, id);
    }

    public void SetPair<TTag, TComp>(TComp component) // right comp
   where TTag : unmanaged, ITag
   where TComp : unmanaged, IComponent
    {
        var tagId = _world.GetTagIdentifier<TTag>();
        var compId = _world.GetComponentIdentifier<TComp>();
        SetPairData(component, tagId, compId);
    }

    public void SetPair<TComp, TTag>(TComp component) // left comp
      where TComp : unmanaged, IComponent
      where TTag : unmanaged, ITag
    {
        var compId = _world.GetComponentIdentifier<TComp>();
        var tagId = _world.GetTagIdentifier<TTag>();
        SetPairData(component, compId, tagId);
    }

    public void SetPairFirstComp<TComp1, TComp2>(TComp1 component) // assume left comp, right is used as tag
     where TComp1 : unmanaged, IComponent
     where TComp2 : unmanaged, IComponent
    {
        var comp1id = _world.GetComponentIdentifier<TComp1>();
        var comp2id = _world.GetComponentIdentifier<TComp2>();
        SetPairData(component, comp1id, comp2id);
    }

    public void SetPairSecondComp<TComp1, TComp2>(TComp2 component) // assume right comp, left is used as tag
    where TComp1 : unmanaged, IComponent
    where TComp2 : unmanaged, IComponent
    {
        var comp1id = _world.GetComponentIdentifier<TComp1>();
        var comp2id = _world.GetComponentIdentifier<TComp2>();
        SetPairData(component, comp1id, comp2id);
    }

    public void SetPairFirstComp<TComp>(TComp first, Entity second) // assume left comp, right is used as tag
    where TComp : unmanaged, IComponent
    {
        var firstId = _world.GetComponentIdentifier<TComp>();
        var secondId = new Identifier(_world, second._handle);
        SetPairData(first, firstId, secondId);
    }

    public void SetPairSecondComp<TComp>(Entity first, TComp second) // assume left comp, right is used as tag
   where TComp : unmanaged, IComponent
    {
        var firstId = new Identifier(_world, first._handle);
        var secondId = _world.GetComponentIdentifier<TComp>();
        SetPairData(second, firstId, secondId);
    }

    private void SetPairData<TComp>(TComp component, Identifier left, Identifier right)
         where TComp : unmanaged, IComponent
    {
        var id = ecs_pair(left.Handle, right.Handle);
        var structSize = Unsafe.SizeOf<TComp>();
        var pointer = Unsafe.AsPointer(ref component);
        ecs_set_id(_world.Handle, _handle, id, (ulong)structSize, pointer);
    }

    public ref TComp GetPairFirst<TComp, TTag>()
        where TTag : unmanaged, ITag
        where TComp : unmanaged, IComponent
    {
        var compId = _world.GetComponentIdentifier<TComp>();
        var tagId = _world.GetTagIdentifier<TTag>();
        return ref GetPairData<TComp>(compId, tagId);
    }

    public ref TComp GetPairSecond<TTag, TComp>()
         where TTag : unmanaged, ITag
         where TComp : unmanaged, IComponent
    {
        var tagId = _world.GetTagIdentifier<TTag>();
        var compId = _world.GetComponentIdentifier<TComp>();
        return ref GetPairData<TComp>(tagId, compId);
    }

    public ref TComp1 GetPairFirstComp<TComp1, TComp2>()
     where TComp1 : unmanaged, IComponent
     where TComp2 : unmanaged, IComponent
    {
        var comp1id = _world.GetComponentIdentifier<TComp1>();
        var comp2id = _world.GetComponentIdentifier<TComp2>();
        return ref GetPairData<TComp1>(comp1id, comp2id);
    }

    public ref TComp GetPairFirstComp<TComp>(Entity second)
        where TComp : unmanaged, IComponent
    {
        var compId = _world.GetComponentIdentifier<TComp>();
        return ref GetPairData<TComp>(compId, new Identifier(_world, second._handle));
    }

    public ref TComp GetPairSecondComp<TComp>(Entity first)
       where TComp : unmanaged, IComponent
    {
        var compId = _world.GetComponentIdentifier<TComp>();
        return ref GetPairData<TComp>(new Identifier(_world, first._handle), compId);
    }

    public ref TComp2 GetPairSecondComp<TComp1, TComp2>()
    where TComp1 : unmanaged, IComponent
    where TComp2 : unmanaged, IComponent
    {
        var comp1id = _world.GetComponentIdentifier<TComp1>();
        var comp2id = _world.GetComponentIdentifier<TComp2>();
        return ref GetPairData<TComp2>(comp1id, comp2id);
    }

    private ref TComp GetPairData<TComp>(Identifier first, Identifier second)
         where TComp : unmanaged, IComponent
    {
        var id = ecs_pair(first.Handle, second.Handle);
        var pointer = ecs_get_id(_world.Handle, _handle, id);
        return ref Unsafe.AsRef<TComp>(pointer);
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

    public void AddComponent<TComponent>()
        where TComponent : unmanaged, IComponent
    {
        var componentId = _world.GetComponentIdentifier<TComponent>();
        ecs_add_id(_world.Handle, _handle, componentId.Handle);
    }

    public ref TComponent GetComponent<TComponent>()
        where TComponent : unmanaged, IComponent
    {
        var componentId = _world.GetComponentIdentifier<TComponent>();
        var pointer = ecs_get_id(_world.Handle, _handle, componentId.Handle);
        return ref Unsafe.AsRef<TComponent>(pointer);
    }

    public void SetComponent<TComponent>(ref TComponent component)
        where TComponent : unmanaged, IComponent
    {
        var componentId = _world.GetComponentIdentifier<TComponent>();
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
        var componentId = _world.GetComponentIdentifier<TComponent>();
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

    public bool HasPair<TTag1, TTag2>()
        where TTag1 : unmanaged, ITag
        where TTag2 : unmanaged, ITag
    {
        var tagId1 = _world.GetTagIdentifier<TTag1>();
        var tagId2 = _world.GetTagIdentifier<TTag2>();
        var id = ecs_pair(tagId1.Handle, tagId2.Handle);
        return ecs_has_id(_world.Handle, _handle, id);
    }

    public bool HasPair(Entity first, Entity second)
    {
        var id = ecs_pair(first._handle, second._handle);
        return ecs_has_id(_world.Handle, _handle, id);
    }

    public bool HasPairFirstComp<TComp>(Entity second)
        where TComp : unmanaged, IComponent
    {
        var compId = _world.GetComponentIdentifier<TComp>();
        var id = ecs_pair(compId.Handle, second._handle);
        return ecs_has_id(_world.Handle, _handle, id);
    }

    public bool HasPairComp<TComp1, TComp2>()
    where TComp1 : unmanaged, IComponent
    where TComp2 : unmanaged, IComponent
    {
        var compId1 = _world.GetComponentIdentifier<TComp1>();
        var compId2 = _world.GetComponentIdentifier<TComp2>();
        var id = ecs_pair(compId1.Handle, compId2.Handle);
        return ecs_has_id(_world.Handle, _handle, id);
    }

    public bool HasPairSecondComp<TComp>(Entity first)
        where TComp : unmanaged, IComponent
    {
        var compId = _world.GetComponentIdentifier<TComp>();
        var id = ecs_pair(first._handle, compId.Handle);
        return ecs_has_id(_world.Handle, _handle, id);
    }

    public bool HasPairSecond<TTag>(Entity first)
        where TTag : unmanaged, ITag
    {
        var tagId = _world.GetTagIdentifier<TTag>();
        var id = ecs_pair(first._handle, tagId.Handle);
        return ecs_has_id(_world.Handle, _handle, id);
    }

    public bool HasPairFirst<TTag>(Entity second)
        where TTag : unmanaged, ITag
    {
        var tagId = _world.GetTagIdentifier<TTag>();
        var id = ecs_pair(tagId.Handle, second._handle);
        return ecs_has_id(_world.Handle, _handle, id);
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
