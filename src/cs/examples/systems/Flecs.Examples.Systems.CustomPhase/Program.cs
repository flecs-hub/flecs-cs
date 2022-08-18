using System.ComponentModel;
using System.Runtime.InteropServices;
using static flecs_hub.flecs;

namespace Flecs.Examples.Systems.CustomPhase;

internal static class Program
{
    internal struct Position : IComponent
    {
        public float X;
        public float Y;
    }

    private static int Main(string[] args)
    {
        var world = new World(args);

        var physics = world.CreateEntity("physics");
        physics.Add(world.EcsPhase);
        physics.DependsOn(world.EcsDependsOn);

        var collision = world.CreateEntity("collision");
        collision.Add(world.EcsPhase);
        collision.DependsOn(physics);

        world.RegisterSystem(System1, physics, "");
        world.RegisterSystem(System2, collision, "");
        world.RegisterSystem(System3, EcsOnUpdate, "");

        world.Progress(0);

        return world.Fini();
    }

    private static void System1(Iterator iterator)
    {
        Console.WriteLine("Sys1: Physics < OnUpdate");
    }

    private static void System2(Iterator iterator)
    {
        Console.WriteLine("Sys2: Collision < Physics < OnUpdate");
    }

    private static void System3(Iterator iterator)
    {
        Console.WriteLine("Sys3 OnUpdate");
    }
}

