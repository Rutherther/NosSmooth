//
//  FakeCommand.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Commands;

namespace NosSmooth.Core.Tests.Fakes.Commands;

/// <summary>
/// A command with arbitrary input.
/// </summary>
/// <param name="Input">The input of the command.</param>
public record FakeCommand(string Input) : ICommand;