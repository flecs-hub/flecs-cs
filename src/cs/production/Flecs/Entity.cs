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
    private readonly ecs_entity_t _handle;

    internal Entity(World world, ecs_entity_t handle)
    {
        _world = world;
        _handle = handle;
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
        var type = typeof(TTag);
        var tagId = _world.GetTagIdentifierFrom(type);
        ecs_add_id(_world.Handle, _handle, tagId);
    }

    public void RemoveTag<TTag>()
        where TTag : unmanaged, ITag
    {
        var type = typeof(TTag);
        var tagId = _world.GetTagIdentifierFrom(type);
        ecs_remove_id(_world.Handle, _handle, tagId);
    }

    public void AddPair<TTag1, TTag2>()
        where TTag1 : unmanaged, ITag
        where TTag2 : unmanaged, ITag
    {
        var type1 = typeof(TTag1);
        var tagId1 = _world.GetTagIdentifierFrom(type1);
        var type2 = typeof(TTag2);
        var tagId2 = _world.GetTagIdentifierFrom(type2);
        var id = ecs_pair(tagId1, tagId2);
        ecs_add_id(_world.Handle, _handle, id);
    }

    public void AddParent(Entity entity)
    {
        var ecsChildOf = pinvoke_EcsChildOf();
        var id = ecs_pair(ecsChildOf, entity._handle);
        ecs_add_id(_world.Handle, _handle, id);
    }

    public ref TComponent GetComponent<TComponent>()
        where TComponent : unmanaged, IComponent
    {
        var type = typeof(TComponent);
        var componentId = _world.GetComponentIdentifierFrom(type);
        var pointer = ecs_get_id(_world.Handle, _handle, componentId);
        return ref Unsafe.AsRef<TComponent>(pointer);
    }

    public void SetComponent<TComponent>(ref TComponent component)
        where TComponent : unmanaged, IComponent
    {
        var type = typeof(TComponent);
        var componentId = _world.GetComponentIdentifierFrom(type);
        var structSize = Unsafe.SizeOf<TComponent>();
        var pointer = Unsafe.AsPointer(ref component);
        ecs_set_id(_world.Handle, _handle, componentId, (ulong)structSize, pointer);
    }

    public void SetComponent<TComponent>(TComponent component)
        where TComponent : unmanaged, IComponent
    {
        SetComponent(ref component);
    }

    public string Name()
    {
        var result = ecs_get_name(_world.Handle, _handle);
        return result.ToString();
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
