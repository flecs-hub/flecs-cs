using System.Runtime.InteropServices;
using JetBrains.Annotations;
using static flecs_hub.flecs;
namespace flecs;

[PublicAPI]
public readonly unsafe struct Table
{
    private readonly ecs_world_t* _worldHandle;
    public readonly ecs_table_t* Handle;

    internal Table(ecs_world_t* world, ecs_table_t* handle)
    {
        _worldHandle = world;
        Handle = handle;
    }

    public string String()
    {
        var cString = ecs_table_str(_worldHandle, Handle);
        var result = Marshal.PtrToStringAnsi(cString._pointer)!;
        Marshal.FreeHGlobal(cString._pointer);
        return result;
    }
}
