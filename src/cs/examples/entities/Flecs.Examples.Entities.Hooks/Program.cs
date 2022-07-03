using System.Runtime.InteropServices;
using Flecs;
using flecs_hub;

internal static class Program
{
    // Component hooks are callbacks that can be registered for a type that are
    // invoked during different parts of the component lifecycle.
    
    [StructLayout(LayoutKind.Sequential)]
    struct String : IComponent
    {
        public flecs.Runtime.CString Value;
    }
    
    // Resource management hooks. The convenience macro's hide details of
    // the callback signature, while allowing hooks to be called on multiple 
    // entities.

    struct Tag : ITag
    {
    }

    static int Main(string[] args)
    {
        var world = new World(args);

        var componentHooks = new ComponentHooks
        {
            Constructor = ComponentConstructor,
            Deconstructor = ComponentDeconstructor,
            Copy = ComponentCopy,
            Move = ComponentMove,
            OnAdd = Callback,
            OnSet = Callback,
            OnRemove = Callback
        };
        world.RegisterComponent<String>(componentHooks);
        world.RegisterTag<Tag>();

        var entity = world.CreateEntity("Entity");

        Console.WriteLine("entity.AddComponent<String>()");
        entity.AddComponent<String>();
        
        Console.WriteLine("entity.SetComponent<String>()(\"Hello World\")");
        entity.SetComponent(new String { Value = "Hello World" });
        
        Console.WriteLine("entity.AddTag<Tag>()");
        // This operation changes the entity's archetype, which invokes a move
        entity.AddTag<Tag>();
        
        Console.WriteLine("entity.Delete()");
        entity.Delete();

        return world.Fini();
    }

    private static void ComponentConstructor(ref ComponentConstructorContext context)
    {
        Console.WriteLine("\tConstructor");
        ref var component = ref context.Get<String>();
        component.Value = default;
    }

    private static void ComponentDeconstructor(ref ComponentDeconstructorContext context)
    {
        Console.WriteLine("\tDeconstructor");
        ref var component = ref context.Get<String>();
        Marshal.FreeHGlobal(component.Value);
    }
    
    private static void ComponentCopy(ref ComponentCopyContext context)
    {
        Console.WriteLine("\tCopy");
        ref var source = ref context.GetSource<String>();
        ref var destination = ref context.GetDestination<String>();
        var value = source.Value.ToString();
        flecs.Runtime.CStrings.FreeCString(source.Value);
        source.Value = default;
        destination.Value = value;
    }

    private static void ComponentMove(ref ComponentMoveContext context)
    {
        Console.WriteLine("\tMove");
        ref var source = ref context.GetSource<String>();
        ref var destination = ref context.GetDestination<String>();
        destination.Value = source.Value;
        source.Value = default;
    }

    private static void Callback(Iterator iterator)
    {
        for (var i = 0; i < iterator.Count; i++)
        {
            var eventName = iterator.Event().Name();
            var entityName = iterator.Entity(i).Name();
            Console.WriteLine("\t" + eventName + ": " + entityName);
        }
    }
}
