using System.Runtime.InteropServices;

namespace flecs;
using static flecs_hub.flecs;

public readonly unsafe struct EntityType
{
    private readonly World _world;
    private readonly ecs_type_t* _handle;

    internal EntityType(World world, ecs_type_t* handle)
    {
        _handle = handle;
        _world = world;
    }

    public string String()
    {
        var cString = ecs_type_str(_world.Handle, _handle);
        var result = Marshal.PtrToStringAnsi(cString._pointer)!;
        Marshal.FreeHGlobal(cString._pointer);
        return result;
    }
}
