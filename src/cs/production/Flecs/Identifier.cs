// Copyright (c) Flecs Hub (https://github.com/flecs-hub). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Runtime.InteropServices;
using static flecs_hub.flecs;

namespace Flecs;

public readonly unsafe struct Identifier
{
    internal readonly World World;
    internal readonly ecs_id_t Handle;

    internal Identifier(World world, ecs_id_t handle)
    {
        World = world;
        Handle = handle;
    }

    public bool IsPair
    {
        get
        {
            var value = Handle.Data & ECS_ROLE_MASK;
            return value == pinvoke_ECS_PAIR().Data;
        }
    }

    public string String()
    {
        var cString = ecs_id_str(World.Handle, Handle);
        var result = Marshal.PtrToStringAnsi(cString._pointer)!;
        Marshal.FreeHGlobal(cString._pointer);
        return result;
    }

    public string RoleString()
    {
        var id = default(ecs_id_t);
        id.Data = Handle.Data & ECS_ROLE_MASK;
        var cString = ecs_role_str(id);
        var result = Marshal.PtrToStringAnsi(cString._pointer)!;
        return result;
    }

    public Pair AsPair()
    {
        var first = ecs_pair_first(World.Handle, Handle);
        var firstEntity = new Entity(World, first);
        var second = ecs_pair_second(World.Handle, Handle);
        var secondEntity = new Entity(World, second);
        return new Pair(firstEntity, secondEntity);
    }

    public Entity AsComponent()
    {
        var value = Handle.Data & ECS_COMPONENT_MASK;
        return new Entity(World, *(ecs_entity_t*)&value);
    }
}
