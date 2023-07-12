// Copyright (c) Flecs Hub (https://github.com/flecs-hub). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace Flecs.Examples.Entities.OptionalOr;

internal static class Program
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Position : IComponent
    {
        public float X;
        public float Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Velocity : IComponent
    {
        public float Value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Speed : IComponent
    {
        public float Value;
    }

    private static int Main(string[] args)
    {
        var world = new World(args);
        world.RegisterComponent<Position>();
        world.RegisterComponent<Velocity>();
        world.RegisterComponent<Speed>();

        var e1 = world.CreateEntity("e1");
        e1.Set(default(Position));
        e1.Set(default(Velocity));
        var e2 = world.CreateEntity("e2");
        e2.Set(default(Position));
        e2.Set(default(Speed));
        world.RegisterSystem(
            MoveOptionalVel,
            EcsOnUpdate,
            $"{world.GetFlecsTypeName(typeof(Position))}, ?{world.GetFlecsTypeName(typeof(Velocity))}");
        world.RegisterSystem(
            MoveOr,
            EcsOnUpdate,
            $"{world.GetFlecsTypeName(typeof(Position))}, " + $"{world.GetFlecsTypeName(typeof(Velocity))} || {world.GetFlecsTypeName(typeof(Speed))}");

        world.Progress(0);

        return world.Fini();
    }

    private static void MoveOptionalVel(Iterator iterator)
    {
        var p = iterator.Field<Position>(1);

        if (iterator.FieldIsSet(2))
        {
            var v = iterator.Field<Velocity>(2);
            for (var i = 0; i < iterator.Count; i++)
            {
                Console.WriteLine("entity has: pos, vel for");
                Console.WriteLine(iterator.Table().String());
            }
        }
        else
        {
            for (var i = 0; i < iterator.Count; i++)
            {
                Console.WriteLine("entity has: pos, no vel");
                Console.WriteLine(iterator.Table().String());
            }
        }
    }

    private static void MoveOr(Iterator iterator)
    {
        var p = iterator.Field<Position>(1);

        if (iterator.FieldIs<Velocity>(2))
        {
            var v = iterator.Field<Velocity>(2);
            for (var i = 0; i < iterator.Count; i++)
            {
                Console.WriteLine("entity has: pos, vel");
                Console.WriteLine(iterator.Table().String());
            }
        }
        else if (iterator.FieldIs<Speed>(2))
        {
            for (var i = 0; i < iterator.Count; i++)
            {
                Console.WriteLine("entity has: pos, speed");
                Console.WriteLine(iterator.Table().String());
            }
        }
        else
        {
            // top could be written as else
        }
    }
}
