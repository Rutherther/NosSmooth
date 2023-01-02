//
//  FakePacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets;

namespace NosSmooth.Core.Tests.Fakes.Packets;

/// <summary>
/// A fake packet.
/// </summary>
/// <param name="Input">The input.</param>
public record FakePacket(string Input) : IPacket;