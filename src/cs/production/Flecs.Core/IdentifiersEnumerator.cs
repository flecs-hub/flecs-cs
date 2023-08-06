// Copyright (c) Flecs Hub (https://github.com/flecs-hub). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using static flecs_hub.Interop.Flecs.PInvoke;

namespace Flecs;

public unsafe struct IdentifiersEnumerator : IEnumerator<Identifier>
{
    private readonly World _world;
    private int _index;
    private readonly ecs_type_t* _type;

    public Identifier Current { get; private set; }

    public IdentifiersEnumerator(World world, ecs_type_t* type)
    {
        _world = world;
        _type = type;
        _index = 0;
        Current = default;
    }

    public bool MoveNext()
    {
        if (_index >= _type->count)
        {
            return false;
        }

        var current = _type->array[_index++];
        Current = new Identifier(_world, current);
        return true;
    }

    public void Reset()
    {
        _index = 0;
        Current = default;
    }

    public void Dispose()
    {
    }

    object IEnumerator.Current => throw new InvalidOperationException();
}
