// Copyright (c) Flecs Hub (https://github.com/flecs-hub). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Runtime.InteropServices;

namespace Flecs.Examples.Entities.RelationComponents;

[StructLayout(LayoutKind.Sequential)]
public struct Requires : IComponent
{
    public float Amount;
}

[StructLayout(LayoutKind.Sequential)]
public struct Expires : IComponent
{
    public float Timeout;
}

[StructLayout(LayoutKind.Sequential)]
public struct Position : IComponent
{
    public float X;
    public float Y;
}

public struct Gigawatts : ITag
{
}

public struct MustHave : ITag
{
}

internal static class Program
{
    private static int Main(string[] args)
    {
        var world = new World(args);

        world.RegisterComponent<Requires>();
        world.RegisterComponent<Expires>();
        world.RegisterComponent<Position>();
        world.RegisterTag<Gigawatts>();
        world.RegisterTag<MustHave>();

        // When one element of a pair is a component and the other element is a tag,
        // the pair assumes the type of the component.
        var e1 = world.CreateEntity("e1");
        e1.Set<Requires, Gigawatts>(new Requires() { Amount = 1.21f });
        ref var requires1 = ref e1.Get<Requires, Gigawatts>();
        Console.WriteLine($"<{nameof(Requires)}, {nameof(Gigawatts)}> (first is value) {nameof(Requires)}: {requires1.Amount}");

        // The component can be either the first or second part of a pair:
        var e2 = world.CreateEntity("e2");
        e2.Set<Gigawatts, Requires>(new Requires() { Amount = 2.5f });
        ref var requires2 = ref e2.GetSecond<Gigawatts, Requires>();
        Console.WriteLine($"<{nameof(Gigawatts)}, {nameof(Requires)}> (second is value) {nameof(Requires)}: {requires2.Amount}");

        // Note that <Requires, Gigawatts> and <Gigawatts, Requires> are two
        // different pairs, and can be added to an entity at the same time.

        // If both parts of a pair are components, the pair assumes the type of
        // the first element:#
        // cs binding: GetPairFirstComp <- component value of first
        var e3 = world.CreateEntity("e3");
        e3.Set<Expires, Position>(new Expires() { Timeout = 2.5f });
        ref var expires = ref e3.Get<Expires, Position>();
        Console.WriteLine($"<{nameof(Expires)}, {nameof(Position)}> (2 comps, first is value) {nameof(Expires)}: {expires.Timeout}");

        // cs binding: GetPairSecondComp <- component value of second
        var e4 = world.CreateEntity("e4");
        e4.Set<Expires, Position>(new Position() { X = 0.5f, Y = 1f });
        ref var pos = ref e4.GetSecond<Expires, Position>();
        Console.WriteLine($"<{nameof(Expires)}, {nameof(Position)}> (2 comps, second is value) {nameof(Position)}: {pos.X}/{pos.Y}");
        Console.WriteLine($"has <{nameof(Expires)}, {nameof(Position)}>: {e4.Has<Expires, Position>()}");
        Console.WriteLine($"has <{nameof(Position)}, {nameof(Expires)}>: {e4.Has<Position, Expires>()}");
        Console.WriteLine();

        var eRuntimeTag = world.CreateEntity("RuntimeTag");
        var e5 = world.CreateEntity("e5");
        e5.Set(new Position() { X = 0.5f, Y = 1f }, eRuntimeTag);
        e5.Set(eRuntimeTag, new Expires() { Timeout = 0.5f });
        Console.WriteLine($"has ({nameof(Position)}, {nameof(eRuntimeTag)}): {e5.Has<Position>(eRuntimeTag)}");
        Console.WriteLine($"has ({nameof(eRuntimeTag)}, {nameof(Position)}): {e5.HasSecond<Position>(eRuntimeTag)}");

        Console.WriteLine($"has ({nameof(Expires)}, {nameof(eRuntimeTag)}): {e5.Has<Expires>(eRuntimeTag)}");
        Console.WriteLine($"has ({nameof(eRuntimeTag)}, {nameof(Expires)}): {e5.HasSecond<Expires>(eRuntimeTag)}");

        ref var e4RuntimePos = ref e5.Get<Position>(eRuntimeTag);
        ref var e4RuntimeExpires = ref e5.GetSecond<Expires>(eRuntimeTag);
        Console.WriteLine($"({nameof(Position)}, {nameof(eRuntimeTag)}) (2 comps, second is runtime tag) {nameof(Position)}: {e4RuntimePos.X}/{e4RuntimePos.Y}");
        Console.WriteLine($"({nameof(eRuntimeTag)}, {nameof(Expires)}) (2 comps, first is runtime tag) {nameof(Expires)}: {e4RuntimeExpires.Timeout}");

        Console.WriteLine("\n\nComponents e1");
        IterateComponents(e1);
        Console.WriteLine("Components e2");
        IterateComponents(e2);
        Console.WriteLine("Components e3");
        IterateComponents(e3);
        Console.WriteLine("Components e4");
        IterateComponents(e4);

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
