// Copyright (c) Flecs Hub (https://github.com/flecs-hub). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Flecs;

internal static class CallbacksHelper
{
    private static SystemCallbackContext[] _systemCallbackContexts = new SystemCallbackContext[64];
    private static int _systemCallbackContextsCount;

    private static ComponentHooksCallbackContext[] _componentHooksCallbackContexts = new ComponentHooksCallbackContext[64];
    private static int _componentHooksCallbackContextsCount;

    public static IntPtr CreateSystemCallbackContext(World world, CallbackIterator callback)
    {
        var data = new SystemCallbackContext(world, callback);
        var count = Interlocked.Increment(ref _systemCallbackContextsCount);
        if (count > _systemCallbackContexts.Length)
        {
            Array.Resize(ref _systemCallbackContexts, count * 2);
        }

        _systemCallbackContexts[count - 1] = data;
        var result = (IntPtr)count;
        return result;
    }

    public static void GetSystemCallbackContext(IntPtr pointer, out SystemCallbackContext data)
    {
        var index = (int)pointer;
        data = _systemCallbackContexts[index - 1];
    }

    public static IntPtr CreateComponentHooksCallbackContext(World world, ComponentHooks hooks)
    {
        var data = new ComponentHooksCallbackContext(world, hooks);
        var count = Interlocked.Increment(ref _componentHooksCallbackContextsCount);
        if (count > _componentHooksCallbackContexts.Length)
        {
            Array.Resize(ref _componentHooksCallbackContexts, count * 2);
        }

        _componentHooksCallbackContexts[count - 1] = data;
        var result = (IntPtr)count;
        return result;
    }

    public static unsafe ref ComponentHooksCallbackContext GetComponentHooksCallbackContext(void* pointer)
    {
        var index = (int)pointer;
        return ref _componentHooksCallbackContexts[index - 1];
    }

    public readonly struct SystemCallbackContext
    {
        public readonly World World;
        public readonly CallbackIterator Callback;

        public SystemCallbackContext(World world, CallbackIterator callback)
        {
            World = world;
            Callback = callback;
        }
    }

    public readonly struct ComponentHooksCallbackContext
    {
        public readonly World World;
        public readonly ComponentHooks Hooks;

        public ComponentHooksCallbackContext(World world, ComponentHooks hooks)
        {
            World = world;
            Hooks = hooks;
        }
    }
}
