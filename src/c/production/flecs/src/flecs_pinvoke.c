#include "pinvoke.h"
#include "flecs.h"

ecs_id_t pinvoke_ECS_PAIR()
{
    return ECS_PAIR;
}

ecs_entity_t pinvoke_EcsOnUpdate()
{
    return EcsOnUpdate;
}

ecs_entity_t pinvoke_EcsDependsOn()
{
    return EcsDependsOn;
}

ecs_entity_t pinvoke_EcsChildOf()
{
    return EcsChildOf;
}