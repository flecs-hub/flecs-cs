using System.ComponentModel;
using System.Runtime.InteropServices;
using static flecs_hub.flecs;

namespace Flecs.Examples.Entities.RelationBasic;

public struct Eats : ITag
{
}

public struct People : ITag
{
}

internal static class Program
{
    private static int Main(string[] args)
    {
        var world = new World(args);
        world.RegisterTag<Eats>();
        world.RegisterTag<People>();
        var grows = world.CreateEntity("Grows");
        var apples = world.CreateEntity("Apples");
        var pears = world.CreateEntity("Pears");

        var bob = world.CreateEntity("Bob");
        bob.AddSecond<Eats>(apples);
        bob.AddSecond<Eats>(pears);
        bob.Add<Eats, People>();

        // Pairs can also be constructed from two entity ids
        bob.Add(grows, pears);

        Console.WriteLine("bob has (Grows, Pears) (dynamic): " + bob.Has(grows, pears));
        Console.WriteLine("bob has (Pears, Grows) (dynamic): " + bob.Has(pears, grows));

        Console.WriteLine("bob has (Eats, Apples) (dynamic): " + bob.Has<Eats>(apples));
        Console.WriteLine("bob has (Apples, Eats) (dynamic): " + bob.HasSecond<Eats>(apples));

        Console.WriteLine("bob has (Eats, People): " + bob.Has<Eats, People>());
        Console.WriteLine("bob has (People, Eats): " + bob.Has<People, Eats>());

        Console.WriteLine("\n");
        IterateComponents(bob);

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
