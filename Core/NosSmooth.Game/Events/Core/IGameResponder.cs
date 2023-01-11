//
//  IGameResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.Game.Events.Core;

/// <summary>
/// Represents interface for classes that respond to <see cref="IGameEvent"/>.
/// </summary>
public interface IGameResponder
{
}

/// <summary>
/// Represents interface for classes that respond to game events.
/// Responds to <typeparamref name="TEvent"/>.
/// </summary>
/// <typeparam name="TEvent">The event type this responder responds to.</typeparam>
public interface IGameResponder<TEvent> : IGameResponder
    where TEvent : IGameEvent
{
    /// <summary>
    /// Respond to the given event.
    /// </summary>
    /// <param name="gameEvent">The event to respond to.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> Respond(TEvent gameEvent, CancellationToken ct = default);
}

/// <summary>
/// Represents interface for classes that respond to every game event.
/// Responds to any game event.
/// </summary>
public interface IEveryGameResponder
{
    /// <summary>
    /// Respond to any event.
    /// </summary>
    /// <param name="gameEvent">The event to respond to.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <typeparam name="TEvent">The current event type.</typeparam>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> Respond<TEvent>(TEvent gameEvent, CancellationToken ct = default);
}