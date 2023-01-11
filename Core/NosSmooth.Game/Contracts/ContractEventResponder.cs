//
//  ContractEventResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Contracts;
using NosSmooth.Game.Events.Core;
using Remora.Results;

namespace NosSmooth.Game.Contracts;

/// <summary>
/// A responder that calls Contractor update.
/// </summary>
public class ContractEventResponder : IEveryGameResponder
{
    private readonly Contractor _contractor;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContractEventResponder"/> class.
    /// </summary>
    /// <param name="contractor">The contractor.</param>
    public ContractEventResponder(Contractor contractor)
    {
        _contractor = contractor;

    }

    /// <inheritdoc />
    public Task<Result> Respond<TEvent>(TEvent gameEvent, CancellationToken ct = default)
        => _contractor.Update(gameEvent, ct);
}