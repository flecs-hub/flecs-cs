using System.Runtime.InteropServices;
using static flecs_hub.flecs;

internal static unsafe class Program
{ 
    /* Move system implementation. System callbacks may be called multiple times,
     * as entities are grouped by which components they have, and each group has
     * its own set of component arrays. */
    [UnmanagedCallersOnly]
    private static void Move(ecs_iter_t *it) 
    {
        var p = ecs_term<Position>(it, 1);
        var v = ecs_term<Velocity>(it, 2);

        // /* Print the set of components for the iterated over entities */
        // var typeString = (string)ecs_table_str(it->world, it->table);
        // Console.WriteLine("Move entities with [{0}]", typeString);
        // flecs.ecs_os_free(typeString);

        /* Iterate entities for the current group */
        for (var i = 0; i < it->count; i ++)
        {
            p[i].X += v[i].X;
            p[i].Y += v[i].Y;
        }
    }
    
    private static int Main(string[] args)
    {
        /* Create the world, pass arguments for overriding the number of threads,fps
         * or for starting the admin dashboard (see flecs.h for details). */
        var world = ecs_init_w_args(args);
        
        /* Register a component with the world. */
        var component = ecs_component_init<Position>(world);
        
        /* Create a new empty entity  */
        Span<ecs_id_t> entityComponentIds = stackalloc ecs_id_t[] { component };
        var entity = ecs_entity_init(world, Entities.MyEntity, entityComponentIds);
        
        /* Set the Position component on the entity */
        var position = new Position
        {
            X = 10,
            Y = 20
        };
        ecs_set_id(world, entity, component, ref position);
        
        /* Get the Position component */
        var p = ecs_get_id<Position>(world, entity, component);
        
        var name = ecs_get_name(world, entity);
        Console.WriteLine($"Position of {name} is {p.X}, {p.Y}");
        
        /* Cleanup */
        return ecs_fini(world);
    }

    [StructLayout(LayoutKind.Sequential)] // Necessary so the C# compiler is not allowed to reorganize the struct
    public struct Position
    {
        public double X;
        public double Y;
    }

    [StructLayout(LayoutKind.Sequential)] // Necessary so the C# compiler is not allowed to reorganize the struct
    public struct Velocity
    {
        public double X;
        public double Y;
    }

    private static class Entities
    {
        public static readonly Runtime.CString MyEntity = "MyEntity";
    }
}
