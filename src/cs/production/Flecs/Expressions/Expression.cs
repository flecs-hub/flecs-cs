// Copyright (c) Flecs Hub (https://github.com/flecs-hub). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flecs;

public class Expression
{
    private World _world;

    public Expression(World world)
    {
        _world = world;
    }

    public TermBuilder Term<T>()
        where T : unmanaged, IEcsComponent
    {
        var termBuilder = new TermBuilder(_world);
        termBuilder.First<T>();
        return termBuilder;
    }

    public TermBuilder Term<T1, T2>()
      where T1 : unmanaged, IEcsComponent
      where T2 : unmanaged, IEcsComponent
    {
        var termBuilder = new TermBuilder(_world);
        termBuilder.First<T1>();
        termBuilder.Second<T2>();
        return termBuilder;
    }

    public TermBuilder Term()
    {
        return new TermBuilder(_world);
    }
}
