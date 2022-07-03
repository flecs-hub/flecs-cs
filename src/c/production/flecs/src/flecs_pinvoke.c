#include "pinvoke.h"
#include "flecs.h"

PINVOKE_API ecs_id_t pinvoke_ECS_PAIR()
{
    return ECS_PAIR;
}

PINVOKE_API ecs_entity_t pinvoke_EcsOnUpdate()
{
    return EcsOnUpdate;
}

PINVOKE_API ecs_entity_t pinvoke_EcsDependsOn()
{
    return EcsDependsOn;
}

PINVOKE_API ecs_entity_t pinvoke_EcsChildOf()
{
    return EcsChildOf;
}