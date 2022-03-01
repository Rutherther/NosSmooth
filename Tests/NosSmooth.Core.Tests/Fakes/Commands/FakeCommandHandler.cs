//
//  FakeCommandHandler.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using NosSmooth.Core.Commands;
using Remora.Results;

namespace NosSmooth.Core.Tests.Fakes.Commands;

/// <inheritdoc />
public class FakeCommandHandler : ICommandHandler<FakeCommand>
{
    private readonly Func<FakeCommand, Result> _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeCommandHandler"/> class.
    /// </summary>
    /// <param name="handler">The handler.</param>
    public FakeCommandHandler(Func<FakeCommand, Result> handler)
    {
        _handler = handler;

    }

    /// <inheritdoc />
    public Task<Result> HandleCommand(FakeCommand command, CancellationToken ct = default)
        => Task.FromResult(_handler(command));
}