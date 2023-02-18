//
//  PacketStringBuilderTests.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions;
using Xunit;

namespace NosSmooth.Packets.Tests;

/// <summary>
/// Tests for <see cref="PacketStringBuilder"/>.
/// </summary>
public class PacketStringBuilderTests
{
    /// <summary>
    /// Tests that the builder correctly builds array of complex types.
    /// </summary>
    [Fact]
    public void BuilderCorrectlyBuildsComplexArray()
    {
        // in 1 11.12.13|14.15.16|17.18.19
        var stringBuilder = new PacketStringBuilder(stackalloc char[500]);
        stringBuilder.Append("in");
        stringBuilder.Append("1");

        stringBuilder.PushLevel('|');
        for (int i = 0; i < 3; i++)
        {
            stringBuilder.PushLevel('.');
            for (int j = 0; j < 3; j++)
            {
                stringBuilder.Append((1 + (i * 3) + j + 10).ToString());
            }
            stringBuilder.ReplaceWithParentSeparator();
            stringBuilder.PopLevel();
        }

        stringBuilder.PopLevel();

        Assert.Equal("in 1 11.12.13|14.15.16|17.18.19", stringBuilder.ToString());
    }

    /// <summary>
    /// Tests that the builder correctly uses once separator.
    /// </summary>
    [Fact]
    public void BuilderCorrectlyUsesOnceSeparator()
    {
        var stringBuilder = new PacketStringBuilder(stackalloc char[500]);
        stringBuilder.Append("in");

        stringBuilder.SetAfterSeparatorOnce('.');
        stringBuilder.PushLevel('|');
        stringBuilder.Append("a");
        stringBuilder.Append("b");
        stringBuilder.Append("c");
        stringBuilder.PopLevel();
        stringBuilder.Append("d");

        Assert.Equal("in a|b|c.d", stringBuilder.ToString());
    }
}