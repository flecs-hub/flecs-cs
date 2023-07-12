// Copyright (c) Flecs Hub (https://github.com/flecs-hub). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System;
using System.Runtime.CompilerServices;

namespace Flecs;

public readonly unsafe struct ComponentCopyContext
{
    private readonly void* _destinationPointer;
    private readonly void* _sourcePointer;
    private readonly int _count;

    public IntPtr DestinationPointer => (IntPtr)_destinationPointer;

    public IntPtr SourcePointer => (IntPtr)_sourcePointer;

    public ComponentCopyContext(void* destinationPointer, void* sourcePointer, int count)
    {
        _destinationPointer = destinationPointer;
        _sourcePointer = sourcePointer;
        _count = count;
    }

    public ref TComponent GetDestination<TComponent>()
        where TComponent : unmanaged, IComponent
    {
        return ref Unsafe.AsRef<TComponent>(_destinationPointer);
    }

    public ref TComponent GetSource<TComponent>()
        where TComponent : unmanaged, IComponent
    {
        return ref Unsafe.AsRef<TComponent>(_sourcePointer);
    }
}
