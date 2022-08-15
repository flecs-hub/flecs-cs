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
PINVOKE_API ecs_entity_t pinvoke_EcsPreFrame()
{
    return EcsPreFrame;
}

PINVOKE_API ecs_entity_t pinvoke_EcsOnLoad()
{
    return EcsOnLoad;
}

PINVOKE_API ecs_entity_t pinvoke_EcsPostLoad()
{
    return EcsPostLoad;
}

PINVOKE_API ecs_entity_t pinvoke_EcsPreUpdate()
{
    return EcsPreUpdate;
}

PINVOKE_API ecs_entity_t pinvoke_EcsOnUpdate()
{
    return EcsOnUpdate;
}

PINVOKE_API ecs_entity_t pinvoke_EcsOnValidate()
{
    return EcsOnValidate;
}

PINVOKE_API ecs_entity_t pinvoke_EcsPostUpdate()
{
    return EcsPostUpdate;
}

PINVOKE_API ecs_entity_t pinvoke_EcsPreStore()
{
    return EcsPreStore;
}

PINVOKE_API ecs_entity_t pinvoke_EcsOnStore()
{
    return EcsOnStore;
}

PINVOKE_API ecs_entity_t pinvoke_EcsPostFrame()
{
    return EcsPostFrame;
}

PINVOKE_API ecs_entity_t pinvoke_EcsPhase()
{
    return EcsPhase;
}