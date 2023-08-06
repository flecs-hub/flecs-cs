// Copyright (c) Flecs Hub (https://github.com/flecs-hub). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using static flecs_hub.Interop.Flecs.PInvoke;

namespace Flecs;

[PublicAPI]
public readonly unsafe struct IdentifiersEnumerable : IEnumerable<Identifier>
{
    private readonly IdentifiersEnumerator _enumerator;

    internal IdentifiersEnumerable(World world, ecs_type_t* type)
    {
        _enumerator = new IdentifiersEnumerator(world, type);
    }

    public IdentifiersEnumerator GetEnumerator()
    {
        // This method is duck-typed for usage in a `foreach`
        return _enumerator;
    }

    IEnumerator<Identifier> IEnumerable<Identifier>.GetEnumerator()
    {
        throw new InvalidOperationException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new InvalidOperationException();
    }
}
