// Copyright (c) Flecs Hub (https://github.com/flecs-hub). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Runtime.InteropServices;
using static flecs_hub.flecs;

namespace Flecs;
public readonly unsafe struct EntityType
{
    private readonly World _world;
    private readonly ecs_type_t* _handle;

    internal EntityType(World world, ecs_type_t* handle)
    {
        _handle = handle;
        _world = world;
    }

    public IdentifiersEnumerable Identifiers()
    {
        return new IdentifiersEnumerable(_world, _handle);
    }

    public string String()
    {
        var cString = ecs_type_str(_world.Handle, _handle);
        var result = Marshal.PtrToStringAnsi(cString._pointer)!;
        Marshal.FreeHGlobal(cString._pointer);
        return result;
    }
}
