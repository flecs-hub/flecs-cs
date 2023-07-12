// Copyright (c) Flecs Hub (https://github.com/flecs-hub). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using JetBrains.Annotations;
using static flecs_hub.flecs;

namespace Flecs;

[PublicAPI]
public readonly unsafe struct IteratorEvent
{
    internal readonly World World;
    internal readonly ecs_entity_t Handle;

    internal IteratorEvent(World world, ecs_entity_t handle)
    {
        World = world;
        Handle = handle;
    }

    public string Name()
    {
        var result = ecs_get_name(World.Handle, Handle);
        return result.ToString();
    }
}
