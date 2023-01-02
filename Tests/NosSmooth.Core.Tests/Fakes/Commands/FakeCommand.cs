//
//  FakeCommand.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Commands;

namespace NosSmooth.Core.Tests.Fakes.Commands;

public record FakeCommand(string Input) : ICommand;