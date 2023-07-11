using System.Runtime.InteropServices;
using bottlenoselabs.C2CS.Runtime;
using flecs_hub;

namespace Flecs.Examples.Entities.Hooks;

// Component hooks are callbacks that can be registered for a type that are
// invoked during different parts of the component lifecycle.

[StructLayout(LayoutKind.Sequential)]
public struct String : IComponent
{
    public CString Value;
}

public struct Tag : ITag
{
}

internal static class Program
{
    static int Main(string[] args)
    {
        var world = new World(args);

        var componentHooks = new ComponentHooks
        {
            // Resource management hooks. These hooks should primarily be used for managing memory used by the component.
            Constructor = ComponentHookConstructor,
            Deconstructor = ComponentHookDeconstructor,
            Copy = ComponentHookCopy,
            Move = ComponentHookMove,
            // Lifecycle hooks. These hooks should be used for application logic.
            OnAdd = HookCallback,
            OnSet = HookCallback,
            OnRemove = HookCallback
        };
        world.RegisterComponent<String>(componentHooks);
        world.RegisterTag<Tag>();

        var entity = world.CreateEntity("Entity");

        Console.WriteLine("entity.Add<String>()");
        entity.Add<String>();

        Console.WriteLine("entity.SetComponent<String>()(\"Hello World\")");
        entity.Set(new String { Value = (CString)"Hello World" });

        Console.WriteLine("entity.AddTag<Tag>()");
        // This operation changes the entity's archetype, which invokes a move
        entity.Add<Tag>();

        Console.WriteLine("entity.Delete()");
        entity.Delete();

        return world.Fini();
    }

    // The constructor should initialize the component value.
    private static void ComponentHookConstructor(ref ComponentConstructorContext context)
    {
        Console.WriteLine("\tConstructor");
        ref var component = ref context.Get<String>();
        component.Value = default;
    }

    // The destructor should free resources.
    private static void ComponentHookDeconstructor(ref ComponentDeconstructorContext context)
    {
        Console.WriteLine("\tDeconstructor");
        ref var component = ref context.Get<String>();
        Marshal.FreeHGlobal(component.Value);
    }

    // The copy hook should copy resources from one location to another.
    private static void ComponentHookCopy(ref ComponentCopyContext context)
    {
        Console.WriteLine("\tCopy");
        ref var source = ref context.GetSource<String>();
        ref var destination = ref context.GetDestination<String>();
        var value = source.Value.ToString();
        source.Value = default;
        destination.Value = (CString)value;
    }

    // The move hook should move resources from one location to another.
    private static void ComponentHookMove(ref ComponentMoveContext context)
    {
        Console.WriteLine("\tMove");
        ref var source = ref context.GetSource<String>();
        ref var destination = ref context.GetDestination<String>();
        destination.Value = source.Value;
        source.Value = default;
    }

    // This callback is used for the add, remove and set hooks. Note that the
    // signature is the same as systems, triggers, observers.
    private static void HookCallback(Iterator iterator)
    {
        for (var i = 0; i < iterator.Count; i++)
        {
            var eventName = iterator.Event().Name();
            var entityName = iterator.Entity(i).Name();
            Console.WriteLine("\t" + eventName + ": " + entityName);
        }
    }
}
