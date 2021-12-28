//
//  EventDispatcher.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using Remora.Results;

namespace NosSmooth.Game.Events.Handlers;

/// <summary>
/// Dispatches <see cref="IGameResponder"/> with <see cref="IGameEvent"/>.
/// </summary>
public class EventDispatcher
{
    private readonly IServiceProvider _provider;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventDispatcher"/> class.
    /// </summary>
    /// <param name="provider">The services provider.</param>
    public EventDispatcher(IServiceProvider provider)
    {
        _provider = provider;
    }

    /// <summary>
    /// Dispatches game responders that are registered in the service collection.
    /// </summary>
    /// <param name="event">The event to dispatch.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <returns>A result that may or may not have succeeded.</returns>
    public async Task<Result> DispatchEvent<TEvent>(TEvent @event, CancellationToken ct = default)
        where TEvent : IGameEvent
    {
        var results = await Task.WhenAll(
            _provider
                .GetServices<IGameResponder<TEvent>>()
                .Select(responder => responder.Respond(@event, ct))
        );

        return results.Length switch
        {
            0 => Result.FromSuccess(),
            1 => results[0],
            _ => new AggregateError(results.Cast<IResult>().ToArray()),
        };
    }
}