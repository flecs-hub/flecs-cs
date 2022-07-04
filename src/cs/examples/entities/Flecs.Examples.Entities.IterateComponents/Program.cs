using System.Runtime.InteropServices;

namespace Flecs.Examples.Entities.IterateComponents;

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

public struct Human : ITag
{
}
    
public struct Eats : ITag
{
}
    
public struct Apples : ITag
{
}

internal static class Program
{
    private static int Main(string[] args)
    {
        var world = new World(args);

        // Ordinary components
        world.RegisterComponent<Position>();
        world.RegisterComponent<Velocity>();

        // A tag
        world.RegisterTag<Human>();

        // Two tags used to create a pair
        world.RegisterTag<Eats>();
        world.RegisterTag<Apples>();

        // Create an entity which all of the above
        var bob = world.CreateEntity("bob");
        
        bob.SetComponent(new Position { X = 10, Y = 20 });
        bob.SetComponent(new Velocity { X = 1, Y = 1 });
        bob.AddTag<Human>();
        bob.AddPair<Eats, Apples>();

        // Iterate & components of Bob
        Console.WriteLine("Bob's components:");
        IterateComponents(bob);

        // We can use the same function to iterate the components of a component
        Console.WriteLine("Position's components:");
        IterateComponents(Entity.FromIdentifier(world.GetComponentIdentifier<Position>()));

        return world.Fini();
    }

    private static void IterateComponents(Entity entity)
    {
        // First get the entity's type, which is a vector of (component) ids.
        var type = entity.Type();

        // 1. The easiest way to print the components is the type's string
        var typeString = type.String();
        Console.WriteLine("Type: " + typeString);

        // 2. To print individual ids, iterate the type array with ecs_id_str
        var i = 0;
        foreach (var identifier in type.Identifiers())
        {
            var identifierString = identifier.String();
            Console.WriteLine(i + ": " + identifierString);
            i++;
        }

        Console.WriteLine();

        // 3. we can also inspect and print the ids in our own way. This is a
        // bit more complicated as we need to handle the edge cases of what can be
        // encoded in an id, but provides the most flexibility.
        i = 0;
        foreach (var identifier in type.Identifiers())
        {
            Console.Write(i + ": ");

            var roleString = identifier.RoleString();
            if (roleString != "UNKNOWN")
            {
                Console.Write("Role: " + roleString + ", ");
            }

            if (identifier.IsPair)
            {
                var pair = identifier.AsPair();
                var relationName = pair.First.Name();
                var objectName = pair.Second.Name();
                Console.Write("Relation: " + relationName + ", Object: " + objectName);
            }
            else
            {
                var component = identifier.AsComponent();
                var componentName = component.Name();
                Console.Write("Name: " + componentName);
            }

            Console.WriteLine();
            i++;
        }
        
        Console.WriteLine();
    }
}