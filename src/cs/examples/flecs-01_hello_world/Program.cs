using flecs;

internal static class Program
{
    struct Position
    {
        public double X;
        public double Y;
    }

    struct Velocity
    {
        public double X;
        public double Y;
    }
    
    // Move system implementation. System callbacks may be called multiple times, as entities are grouped by which
    // components they have, and each group has its own set of component arrays.
    public static void Move(Iterator iterator)
    {
        var p = iterator.Term<Position>(1);
        var v = iterator.Term<Velocity>(2);

        // // Print the set of components for the iterated over entities
        // var typeString = ecs_table_str(it->world, it->table).ToString();
        // Console.WriteLine("Move entities with " + typeString);
        // // ecs_os_free(type_str);

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

        // Register components
        var componentPosition = world.InitializeComponent<Position>();
        var componentVelocity = world.InitializeComponent<Velocity>();
        
        // Register system
        world.InitializeSystem(Move, "Position, Velocity");

        // Register tags (components without a size)
        var eats = world.InitializeTag("eats");
        var apples = world.InitializeTag("apples");
        var pears = world.InitializeTag("pears");

        // Create an entity with name Bob, add Position and food preference
        var bob = world.InitializeEntity("Bob");
        world.SetComponent(bob, componentPosition, new Position { X = 0, Y = 0 });
        world.SetComponent(bob, componentVelocity, new Velocity { X = 2, Y = 2 });
        world.AddPair(bob, eats, apples);
        
        // Run systems twice. Usually this function is called once per frame
        world.Progress(0);
        world.Progress(0);
        
        // See if Bob has moved (he has)
        var p = world.GetComponent<Position>(bob, componentPosition);
        Console.WriteLine("Bob's position is {" + p.X + ", " + p.Y + "}");

        return world.Fini();
    }
}
