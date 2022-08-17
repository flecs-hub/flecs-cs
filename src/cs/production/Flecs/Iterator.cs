// Copyright (c) Flecs Hub (https://github.com/flecs-hub). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using static flecs_hub.flecs;

namespace Flecs;

[PublicAPI]
public readonly unsafe struct Iterator
{
    private readonly World _world;
    internal readonly ecs_iter_t* Handle;

    public int Count => Handle->count;

    internal Iterator(World world, ecs_iter_t* it)
    {
        _world = world;
        Handle = it;
    }

    public float DeltaTime() => Handle->delta_time;

    public float DeltaSystemTime() => Handle->delta_system_time;

    public Span<T> Field<T>(int index)
    {
        var structSize = Marshal.SizeOf<T>();
        var pointer = ecs_field_w_size(Handle, (ulong) structSize, index);
        return new Span<T>(pointer, Handle->count);
    }

    public bool FieldIsSet(int index)
    {
        return ecs_field_is_set(Handle, index);
    }

    public bool FieldIs<T>(int index)
        where T : unmanaged, IComponent
    {
        var id = ecs_field_id(Handle, index);
        var compId = _world.GetComponentIdentifier<T>();
        return id == compId.Handle;
    }

    public Table Table()
    {
        return new Table(Handle->world, Handle->table);
    }

    public Entity Entity(int index)
    {
        var result = new Entity(_world, Handle->entities[index]);
        return result;
    }

    public bool HasQueryChanged()
    {
        return ecs_query_changed(null, Handle);
    }

    public void QuerySkip()
    {
        ecs_query_skip(Handle);
    }

    public bool IsSelf(int index)
    {
        return ecs_field_is_self(Handle, index);
    }

    public IteratorEvent Event()
    {
        var result = new IteratorEvent(_world, Handle->@event);
        return result;
    }
}
