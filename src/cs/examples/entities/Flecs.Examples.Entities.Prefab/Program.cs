using System.Runtime.InteropServices;

namespace Flecs.Examples.Entities.Prefab;

[StructLayout(LayoutKind.Sequential)]
public struct Attack : IComponent
{
    public double Value;
}

[StructLayout(LayoutKind.Sequential)]
public struct Defense : IComponent
{
    public double Value;
}

[StructLayout(LayoutKind.Sequential)]
public struct FreightCapacity : IComponent
{
    public double Value;
}

[StructLayout(LayoutKind.Sequential)]
public struct ImpulseSpeed : IComponent
{   
    public double Value;
}

[StructLayout(LayoutKind.Sequential)]
public struct Position : IComponent
{   
    public double X;
    public double Y;
}

public struct HasFtl : ITag
{
}

public struct Amount : IComponent
{
    public int Max;
    public int Current;
}

internal static class Program
{
    private static int Main(string[] args)
    {
        var world = new World(args);

        world.RegisterComponent<Position>();
        world.RegisterComponent<Attack>();
        world.RegisterComponent<Defense>();
        world.RegisterComponent<FreightCapacity>();
        world.RegisterComponent<ImpulseSpeed>();
        world.RegisterComponent<Amount>();
        world.RegisterTag<HasFtl>();

        var hull = world.CreateEntity("Hull");
        var shield = world.CreateEntity("Shield");

        // Create a prefab hierarchy. Prefabs are entities that by default are
        // ignored by queries.
        var spaceShip = world.CreatePrefab("SpaceShip");
        spaceShip.SetComponent(new ImpulseSpeed { Value = 50 });
        spaceShip.SetComponent(new Defense { Value = 50 });
        //setting as overridable, every ship instance can be damaged individually
        spaceShip.SetPairSecondCompOverride(hull, new Amount() { Max = 200, Current = 200});
        spaceShip.SetPairSecondCompOverride(shield, new Amount() { Max = 200, Current = 200 });

        // By default components in an inheritance hierarchy are shared between
        // entities. The override function ensures that instances have a private
        // copy of the component.
        spaceShip.SetOverride(new Position { X = 0, Y = 0 });

        var freighter = world.CreatePrefab("Freighter");
        // This ensures the entity inherits all components from spaceship.
        freighter.IsA(spaceShip);
        freighter.SetComponent(new FreightCapacity { Value = 100 });
        freighter.SetComponent(new Defense { Value = 50 });

        var mammothFreighter = world.CreatePrefab("MammothFreighter");
        mammothFreighter.IsA(freighter);
        mammothFreighter.SetComponent(new FreightCapacity { Value = 500 });
        mammothFreighter.SetComponent(new Defense { Value = 300 });

        var frigate = world.CreatePrefab("Frigate");
        // This ensures the entity inherits all components from spaceship.
        frigate.IsA(spaceShip);
        frigate.SetComponent(new Attack { Value = 100 });
        frigate.SetComponent(new Defense { Value = 75 });
        frigate.SetComponent(new ImpulseSpeed { Value = 125 });

        // Create an entity from a prefab.
        // The instance will have a private copy of the Position component, because
        // of the override in the spaceship entity. All other components are shared.
        var freighter1 = world.CreateEntity("my_mammoth_freighter1");
        freighter1.IsA(mammothFreighter);

        var freighter2 = world.CreateEntity("my_mammoth_freighter2");
        freighter2.IsA(mammothFreighter);

        // Inspect the type of the entity. This outputs:
        //    Position,(Identifier,Name),(IsA,MammothFreighter)
        var instanceTypeString = freighter1.Type().String();
        Console.WriteLine("Instance type: " + instanceTypeString);
        
        // Even though the instance doesn't have a private copy of ImpulseSpeed, we
        // can still get it using the regular API (outputs 50)
        var impulseSpeed = freighter1.GetComponent<ImpulseSpeed>();
        Console.WriteLine("Impulse speed: " + impulseSpeed.Value);

        ref var freighter1Hull = ref freighter1.GetPairSecondComp<Amount>(hull);
        freighter1Hull.Current -= 100;

        Console.WriteLine($"Freigher 1 hull: {freighter1.GetPairSecondComp<Amount>(hull).Current}");
        Console.WriteLine($"Freigher 2 hull: {freighter2.GetPairSecondComp<Amount>(hull).Current}");

        return world.Fini();
    }
}