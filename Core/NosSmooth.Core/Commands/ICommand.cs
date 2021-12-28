//
//  ICommand.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Client;

namespace NosSmooth.Core.Commands;

/// <summary>
/// Represents command for <see cref="INostaleClient"/>.
/// </summary>
/// <remarks>
/// Commands do complex operations that may take more time.
/// For handling commands, <see cref="ICommandHandler"/> is used.
/// </remarks>
public interface ICommand
{
}