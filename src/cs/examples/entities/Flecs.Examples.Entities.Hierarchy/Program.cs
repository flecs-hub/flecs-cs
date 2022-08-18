using System.Runtime.InteropServices;

namespace Flecs.Examples.Entities.Hierarchy;

[StructLayout(LayoutKind.Sequential)]
public struct Position : IComponent
{
    public double X;
    public double Y;
}

public struct Star : ITag
{
}
    
public struct Planet : ITag
{
}
    
public struct Moon : ITag
{
}

internal static class Program
{
    private static int Main(string[] args)
    {
        var world = new World(args);
        
        world.RegisterComponent<Position>();
        world.RegisterTag<Star>();
        world.RegisterTag<Planet>();
        world.RegisterTag<Moon>();

        // Create a simple hierarchy.
        // Hierarchies use ECS relations and the builtin flecs::ChildOf relation to
        // create entities as children of other entities.

        var sun = world.CreateEntity("Sun");
        sun.Add<Star>();
        sun.SetComponent(new Position { X = 1, Y = 1 });
        
        var mercury = world.CreateEntity("Mercury");
        mercury.AddParent(sun);
        mercury.Add<Planet>();
        mercury.SetComponent(new Position { X = 1, Y = 1 });
        
        var venus = world.CreateEntity("Venus");
        venus.AddParent(sun);
        venus.Add<Planet>();
        venus.SetComponent(new Position { X = 2, Y = 2 });
        
        var earth = world.CreateEntity("Earth");
        earth.AddParent(sun);
        earth.Add<Planet>();
        earth.SetComponent(new Position { X = 3, Y = 3 });
        
        var moon = world.CreateEntity("Moon");
        moon.AddParent(earth);
        moon.Add<Moon>();
        moon.SetComponent(new Position { X = 0.1f, Y = 0.1f });
        
        // Is the Moon a child of Earth?
        Console.WriteLine("Is Moon a child entity of Earth?: " + moon.IsChildOf(earth));
        Console.WriteLine("Is Earth a child entity of Moon?: " + earth.IsChildOf(moon));

        // Do a depth-first walk of the tree
        IterateTree(world, sun, default);

        return world.Fini();
    }

    private static void IterateTree(World world, Entity entity, Position parentPosition)
    {
        // Print hierarchical name of entity & the entity type
        var pathString = entity.FullPathString();
        var typeString = entity.Type().String();
        Console.WriteLine(pathString + " " + typeString);

        // Get entity position
        var position = entity.GetComponent<Position>();

        // Calculate actual position
        var actualPosition = new Position { X = position.X + parentPosition.X, Y = position.Y + parentPosition.Y };
        Console.WriteLine("{" + actualPosition.X + ", " + actualPosition.Y + "}");

        // Iterate children recursively
        var it = entity.Children();
        while (it.HasNext())
        {
            for (var i = 0; i < it.Count; i++)
            {
                IterateTree(world, it.Entity(i), actualPosition);
            }
        }
    }
}