using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using static flecs_hub.flecs;

namespace flecs;

[PublicAPI]
public readonly unsafe struct Iterator
{
    public readonly ecs_iter_t* Handle;

    public int Count => Handle->count;

    internal Iterator(ecs_iter_t* it)
    {
        Handle = it;
    }

    public Span<T> Term<T>(int index)
    {
        var structSize = Marshal.SizeOf<T>();
        var pointer = ecs_term_w_size(Handle, (ulong) structSize, index);
        return new Span<T>(pointer, Handle->count);
    }

    public Table Table()
    {
        return new Table(Handle->world, Handle->table);
    }
}
