#include "pinvoke.h"
#include "flecs.h"

// Roles
PINVOKE_API ecs_id_t pinvoke_ECS_PAIR()
{
    return ECS_PAIR;
}

PINVOKE_API ecs_id_t pinvoke_ECS_OVERRIDE()
{
    return ECS_OVERRIDE;
}

// Relationships
PINVOKE_API ecs_entity_t pinvoke_EcsIsA()
{
    return EcsIsA;
}

PINVOKE_API ecs_entity_t pinvoke_EcsDependsOn()
{
    return EcsDependsOn;
}

PINVOKE_API ecs_entity_t pinvoke_EcsChildOf()
{
    return EcsChildOf;
}

PINVOKE_API ecs_entity_t pinvoke_EcsSlotOf()
{
    return EcsSlotOf;
}


// Entity tags
PINVOKE_API ecs_entity_t pinvoke_EcsPrefab()
{
    return EcsPrefab;
}

// System tags
PINVOKE_API ecs_entity_t pinvoke_EcsOnUpdate()
{
    return EcsOnUpdate;
}