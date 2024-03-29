// Copyright (c) Flecs Hub (https://github.com/flecs-hub). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Flecs;

public readonly unsafe struct ComponentConstructorContext
{
    private readonly void* _pointer;
    private readonly int _count;

    public ComponentConstructorContext(void* pointer, int count)
    {
        _pointer = pointer;
        _count = count;
    }

    public ref TComponent Get<TComponent>()
        where TComponent : unmanaged, IComponent
    {
        return ref Unsafe.AsRef<TComponent>(_pointer);
    }
}
