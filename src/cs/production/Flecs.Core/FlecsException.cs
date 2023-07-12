// Copyright (c) Flecs Hub (https://github.com/flecs-hub). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System;

namespace Flecs;

public class FlecsException : Exception
{
    public FlecsException()
    {
    }

    public FlecsException(string message)
        : base(message)
    {
    }

    public FlecsException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
