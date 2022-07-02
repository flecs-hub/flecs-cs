using System.Runtime.InteropServices;
using Flecs;

internal static class Program
{
    [StructLayout(LayoutKind.Sequential)]
    struct Position : IComponent
    {
        public double X;
        public double Y;
    }

    struct Walking : ITag
    {
    }

    static int Main(string[] args)
    {
        var world = new World(args);

        var bob = world.InitializeEntity("Bob");
        bob.SetComponent(new Position { X = 10, Y = 20 });
        bob.AddTag<Walking>();

        var position = bob.GetComponent<Position>();
        Console.WriteLine("Bob's initial position: {" + position.X + ", " + position.Y + "}");
        
        // Print all the components the entity has. This will output:
        //    Position, Walking, (Identifier,Name)
        Console.WriteLine("Bob's type: " + bob.Type().String());

        bob.SetComponent(new Position { X = 20, Y = 30 });

        var alice = world.InitializeEntity("Alice");
        alice.SetComponent(new Position { X = 10, Y = 20 });
        alice.AddTag<Walking>();

        // Print all the components the entity has. This will output:
        //    Position, Walking, (Identifier,Name)
        Console.WriteLine("Alice's type: " + alice.Type().String());

        // Remove tag
        alice.RemoveTag<Walking>();

        // Iterate all entities with Position
        var it = world.EntityIterator<Position>();
        while (it.HasNext()) 
        {
            var p = it.Term<Position>(1);
            for (var i = 0; i < it.Count; i ++)
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
