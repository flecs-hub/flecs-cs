#include "pinvoke.h"
#include "flecs.h"

// Roles
ecs_id_t pinvoke_ECS_PAIR()
{
    return ECS_PAIR;
}

ecs_id_t pinvoke_ECS_OVERRIDE()
{
    return ECS_OVERRIDE;
}

// Relationships
ecs_entity_t pinvoke_EcsIsA()
{
    return EcsIsA;
}

ecs_entity_t pinvoke_EcsDependsOn()
{
    return EcsDependsOn;
}

ecs_entity_t pinvoke_EcsChildOf()
{
    return EcsChildOf;
}

// Entity tags
ecs_entity_t pinvoke_EcsPrefab()
{
    return EcsPrefab;
}

// System tags
ecs_entity_t pinvoke_EcsOnUpdate()
{
    return EcsOnUpdate;
}