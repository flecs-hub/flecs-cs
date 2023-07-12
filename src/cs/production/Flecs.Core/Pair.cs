// Copyright (c) Flecs Hub (https://github.com/flecs-hub). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace Flecs;

public readonly struct Pair
{
    public readonly Entity First;
    public readonly Entity Second;

    internal Pair(Entity first, Entity second)
    {
        First = first;
        Second = second;
    }
}
