namespace Flecs.Examples.Entities.PrefabSlot;

internal static class Program
{
    private static int Main(string[] args)
    {
        var world = new World(args);
        // ecs_world_t* ecs = ecs_init_w_args(argc, argv);

        // Create the same prefab hierarchy as from the hierarchy example, but now with the SlotOf relationship.

        var spaceShipPrefab = world.CreatePrefab("SpaceShip");
        var enginePrefab = world.CreatePrefab("Engine");
        enginePrefab.AddParent(spaceShipPrefab);
        enginePrefab.AddSlotOf(spaceShipPrefab);

        var cockpitPrefab = world.CreatePrefab("Cockpit");
        cockpitPrefab.AddParent(spaceShipPrefab);
        cockpitPrefab.AddSlotOf(spaceShipPrefab);

        // Add an additional child to the Cockpit prefab to demonstrate how
        // slots can be different from the parent. This slot could have been
        // added to the Cockpit prefab, but instead we register it on the top
        // level SpaceShip prefab.
        var pilotSeat = world.CreatePrefab("PilotSeat");
        pilotSeat.AddParent(cockpitPrefab);
        pilotSeat.AddSlotOf(spaceShipPrefab);

        // Create a prefab instance.
        var shipInstance = world.CreateEntity("SpaceShipInstance");
        shipInstance.IsA(spaceShipPrefab);

        // Get the instantiated entities for the prefab slots
        Entity engineInstance = shipInstance.GetTarget(enginePrefab);
        Entity cockpitInstance = shipInstance.GetTarget(cockpitPrefab);
        Entity pilotSeatInstance = shipInstance.GetTarget(pilotSeat);

        Console.WriteLine($"Instance engine: {engineInstance.FullPathString()}");
        Console.WriteLine($"Instance cockpit: {cockpitInstance.FullPathString()}");
        Console.WriteLine($"Instance pilot seat: {pilotSeatInstance.FullPathString()}");

        return world.Fini();
    }
}
