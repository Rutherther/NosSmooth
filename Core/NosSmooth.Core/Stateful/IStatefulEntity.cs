//
//  IStatefulEntity.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Client;

namespace NosSmooth.Core.Stateful;

/// <summary>
/// An entity that has a state that depends on the NosTale state.
/// </summary>
/// <remarks>
/// Stateful entities can be replaced with scoped entities that will be
/// injected depending on the current <see cref="INostaleClient"/>.
/// That can be useful in an application that has multiple nostale clients.
/// </remarks>
public interface IStatefulEntity
{
}