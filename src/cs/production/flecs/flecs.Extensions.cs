// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

#nullable enable

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static flecs_hub.flecs.Runtime;

namespace flecs_hub
{
    public static unsafe partial class flecs
    {
        public static ecs_world_t* ecs_init_w_args(string[] args)
        {
            var argv = args.Length == 0 ? default : CStrings.CStringArray(args);
            var world = ecs_init_w_args(args.Length, argv);
            CStrings.FreeCStrings(argv, args.Length);
            return world;
        }

        public static ecs_entity_t ecs_entity_init(
            ecs_world_t* world, CString name, Span<ecs_id_t> componentIds)
        {
            var entityDescriptor = new ecs_entity_desc_t
            {
                name = name
            };

            for (var index = 0; index < componentIds.Length; index++)
            {
                var componentId = componentIds[index];
                entityDescriptor.add[index] = componentId;
            }

            return ecs_entity_init(world, &entityDescriptor);
        }

        private static void CheckStructLayout(StructLayoutAttribute? structLayoutAttribute)
        {
            if (structLayoutAttribute == null || structLayoutAttribute.Value == LayoutKind.Auto)
            {
                throw new FlecsException(
                    "Component must have a StructLayout attribute with LayoutKind sequential or explicit. This is to ensure that the struct fields are not reorganized by the C# compiler.");
            }
        }
    }
}
