using System.Runtime.InteropServices;

namespace Flecs.Examples.HelloWorld;

[StructLayout(LayoutKind.Sequential)]
public struct Position : IComponent
{
    public double X;
    public double Y;
}

[StructLayout(LayoutKind.Sequential)]
public struct Velocity : IComponent
{
    public double X;
    public double Y;
}

public struct Eats : ITag
{
}

public struct Apples : ITag
{
}

public struct Pears : ITag
{
}

internal static class Program
{
    // Move system implementation. System callbacks may be called multiple times, as entities are grouped by which
    // components they have, and each group has its own set of component arrays.
    private static void Move(Iterator iterator)
    {
        var p = iterator.Field<Position>(1);
        var v = iterator.Field<Velocity>(2);

        // Print the set of components for the iterated over entities
        var tableString = iterator.Table().String();
        Console.WriteLine("Move entities with table: " + tableString);

        // Iterate entities for the current group 
        for (var i = 0; i < iterator.Count; i++)
        {
            ref var position = ref p[i];
            var velocity = v[i];

            position.X += velocity.X;
            position.Y += velocity.Y;
        }
    }

    private static int Main(string[] args)
    {
        // Create the world
        var world = new World(args);
        
        world.RegisterComponent<Position>();
        world.RegisterComponent<Velocity>();
        world.RegisterTag<Eats>();
        world.RegisterTag<Apples>();
        world.RegisterTag<Pears>();

        // Register system
        world.RegisterSystem<Position, Velocity>(Move);

        // Create an entity with name Bob, add Position and food preference
        var bob = world.CreateEntity("Bob");
        bob.SetComponent(new Position { X = 0, Y = 0 });
        bob.SetComponent(new Velocity { X = 2, Y = 2 });
        bob.Add<Eats, Apples>();

        // Run systems twice. Usually this function is called once per frame
        world.Progress(0);
        world.Progress(0);
        
        // See if Bob has moved (he has)
        var p = bob.GetComponent<Position>();
        Console.WriteLine("Bob's position is {" + p.X + ", " + p.Y + "}");

        return world.Fini();
    }
}