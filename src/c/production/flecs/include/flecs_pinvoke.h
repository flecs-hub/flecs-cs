#include "pinvoke.h"
#include "flecs.h"

// Roles
PINVOKE_API ecs_id_t pinvoke_ECS_PAIR();
PINVOKE_API ecs_id_t pinvoke_ECS_OVERRIDE();

// Relationships
PINVOKE_API ecs_entity_t pinvoke_EcsIsA();
PINVOKE_API ecs_entity_t pinvoke_EcsDependsOn();
PINVOKE_API ecs_entity_t pinvoke_EcsChildOf();

// Entity tags
PINVOKE_API ecs_entity_t pinvoke_EcsPrefab();

// System tags
PINVOKE_API ecs_entity_t pinvoke_EcsOnUpdate();