// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

#nullable enable

using System;

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
