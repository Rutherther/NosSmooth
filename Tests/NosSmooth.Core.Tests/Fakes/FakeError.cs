//
//  FakeError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;
using Remora.Results;

namespace NosSmooth.Core.Tests.Fakes;

/// <summary>
/// A fake error.
/// </summary>
/// <param name="Text">The text.</param>
public record FakeError(string Text = "Fake") : ResultError($"Fake error: {Text}");