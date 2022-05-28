internal static unsafe class Program
{
    private static int Main(string[] args)
    {
        /* Create the world, pass arguments for overriding the number of threads,fps
         * or for starting the admin dashboard (see flecs.h for details). */
        var world = ecs_init_w_args(args);

        /* Register a component with the world. */
        var component = ecs_component_init<Components.Position>(world);

        /* Create a new empty entity  */
        Span<ecs_id_t> entityComponentIds = stackalloc ecs_id_t[] { component };
        var entity = ecs_entity_init(world, Entities.MyEntity, entityComponentIds);

        /* Set the Position component on the entity */
        var position = new Components.Position
        {
            X = 10,
            Y = 20
        };
        ecs_set_id(world, entity, component, ref position);

        /* Get the Position component */
        var p = ecs_get_id<Components.Position>(world, entity, component);

        var name = ecs_get_name(world, entity);
        Console.WriteLine($"Position of {name} is {p.X}, {p.Y}");

        /* Cleanup */
        return ecs_fini(world);
    }

    private static class Components
    {
        [StructLayout(LayoutKind.Sequential)] // Necessary so the C# compiler is not allowed to reorganize the struct
        public struct Position
        {
            public double X;
            public double Y;
        }
    }

    private static class Entities
    {
        public static readonly flecs.Runtime.CString MyEntity = "MyEntity";
    }
}
