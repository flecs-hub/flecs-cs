using System;
using System.Runtime.CompilerServices;
using static flecs_hub.flecs;

namespace flecs;

public readonly unsafe struct Iterator
{
    private readonly ecs_iter_t* _handle;

    public int Count => _handle->count;

    internal Iterator(ecs_iter_t* it)
    {
        _handle = it;
    }

    public Span<T> Term<T>(int index)
        where T : unmanaged
    {
        var structSize = Unsafe.SizeOf<T>();
        var pointer = ecs_term_w_size(_handle, (ulong) structSize, index);
        return new Span<T>(pointer, _handle->count);
    }
}
