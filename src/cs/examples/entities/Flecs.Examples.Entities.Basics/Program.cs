// Copyright (c) Flecs Hub (https://github.com/flecs-hub). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Runtime.InteropServices;

namespace Flecs.Examples.Entities.Basics;

[StructLayout(LayoutKind.Sequential)]
public struct Position : IComponent
{
    public double X;
    public double Y;
}

public struct Walking : ITag
{
}

internal static class Program
{
    private static int Main(string[] args)
    {
        var world = new World(args);

        world.RegisterComponent<Position>();
        world.RegisterTag<Walking>();

        var bob = world.CreateEntity("Bob");
        bob.Set(new Position { X = 10, Y = 20 });
        bob.Add<Walking>();

        var position = bob.GetComponent<Position>();
        Console.WriteLine("Bob's initial position: {" + position.X + ", " + position.Y + "}");

        // Print all the components the entity has. This will output:
        //    Position, Walking, (Identifier,Name)
        Console.WriteLine("Bob's type: " + bob.Type().String());

        bob.Set(new Position { X = 20, Y = 30 });

        var alice = world.CreateEntity("Alice");
        alice.Set(new Position { X = 10, Y = 20 });
        alice.Add<Walking>();

        // Print all the components the entity has. This will output:
        //    Position, Walking, (Identifier,Name)
        Console.WriteLine("Alice's type: " + alice.Type().String());

        // Remove tag
        alice.Remove<Walking>();

        // Iterate all entities with Position
        var it = world.EntityIterator<Position>();
        while (it.HasNext())
        {
            var p = it.Field<Position>(1);
            for (var i = 0; i < it.Count; i++)
            {
                var entity = it.Entity(i);
                var entityName = entity.Name();
                var entityPosition = p[i];
                Console.WriteLine(entityName + ": {" + entityPosition.X + ", " + entityPosition.Y + "}");
            }
        }

        return world.Fini();
    }
}
