//
//  PacketStringEnumeratorTests.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using NosSmooth.Packets.Errors;
using Xunit;

namespace NosSmooth.Packets.Tests;

/// <summary>
/// Test for <see cref="PacketStringEnumerator"/>.
/// </summary>
public class PacketStringEnumeratorTests
{
    /// <summary>
    /// Test that array of complex types can be parsed.
    /// </summary>
    [Fact]
    public void EnumeratorListComplexStringGivesCorrectResult()
    {
        var stringEnumerator = new PacketStringEnumerator("in 1 11.12.13|14.15.16|17.18.19");
        var headerTokenResult = stringEnumerator.GetNextToken(out var packetToken);
        Assert.True(headerTokenResult.IsSuccess);
        Assert.False(packetToken.PacketEndReached);
        Assert.NotNull(packetToken.IsLast);
        Assert.NotNull(packetToken.EncounteredUpperLevel);
        Assert.False(packetToken.IsLast);
        Assert.False(packetToken.EncounteredUpperLevel);
        Assert.Matches("in", packetToken.Token.ToString());

        var firstToken = stringEnumerator.GetNextToken(out packetToken);
        Assert.True(firstToken.IsSuccess);
        Assert.False(packetToken.PacketEndReached);
        Assert.NotNull(packetToken.IsLast);
        Assert.NotNull(packetToken.EncounteredUpperLevel);
        Assert.False(packetToken.IsLast);
        Assert.False(packetToken.EncounteredUpperLevel);
        Assert.Matches("1", packetToken.Token.ToString());

        stringEnumerator.PushLevel('|');
        stringEnumerator.PrepareLevel('.');

        for (int i = 0; i < 3; i++)
        {
            stringEnumerator.PushPreparedLevel();

            for (int j = 0; j < 3; j++)
            {
                string currentNum = (j + (i * 3) + 1 + 10).ToString();
                var currentToken = stringEnumerator.GetNextToken(out packetToken);
                Assert.True(currentToken.IsSuccess);
                Assert.False(packetToken.PacketEndReached);
                Assert.NotNull(packetToken.IsLast);
                Assert.NotNull(packetToken.EncounteredUpperLevel);
                if (j == 2 && i == 2)
                {
                    Assert.True(packetToken.EncounteredUpperLevel);
                }
                else
                {
                    Assert.False(packetToken.EncounteredUpperLevel);
                }

                if (j != 2)
                {
                    Assert.False(packetToken.IsLast);
                }
                else
                {
                    Assert.True(packetToken.IsLast);
                }

                Assert.Matches(currentNum, packetToken.Token.ToString());
            }

            Assert.True(stringEnumerator.IsOnLastToken());
            stringEnumerator.PopLevel();
        }

        stringEnumerator.PopLevel();
        Assert.True(stringEnumerator.IsOnLastToken());
    }

    /// <summary>
    /// Test that over reaching the end is not allowed.
    /// </summary>
    [Fact]
    public void EnumeratorDoesNotAllowOvereachingPacketEnd()
    {
        var stringEnumerator = new PacketStringEnumerator("in 1 2 3 4");
        var tokenResult = stringEnumerator.GetNextToken(out _);
        Assert.True(tokenResult.IsSuccess); // in
        tokenResult = stringEnumerator.GetNextToken(out _);
        Assert.True(tokenResult.IsSuccess); // 1
        tokenResult = stringEnumerator.GetNextToken(out _);
        Assert.True(tokenResult.IsSuccess); // 2
        tokenResult = stringEnumerator.GetNextToken(out _);
        Assert.True(tokenResult.IsSuccess); // 3
        tokenResult = stringEnumerator.GetNextToken(out _);
        Assert.True(tokenResult.IsSuccess); // 4

        tokenResult = stringEnumerator.GetNextToken(out _);
        Assert.False(tokenResult.IsSuccess);
        Assert.IsType<PacketEndReachedError>(tokenResult.Error);
    }

    /// <summary>
    /// Test that over reaching the end of a list is not allowed.
    /// </summary>
    [Fact]
    public void EnumeratorDoesNotAllowOvereachingListComplexTypeEnd()
    {
        var stringEnumerator = new PacketStringEnumerator("in 1|2.2|3.3|4.4|5");
        var tokenResult = stringEnumerator.GetNextToken(out _);
        Assert.True(tokenResult.IsSuccess); // in

        stringEnumerator.PushLevel('.');
        stringEnumerator.PushLevel('|');

        tokenResult = stringEnumerator.GetNextToken(out _);
        Assert.True(tokenResult.IsSuccess); // 1

        tokenResult = stringEnumerator.GetNextToken(out var packetToken);
        Assert.True(tokenResult.IsSuccess); // 2
        Assert.True(packetToken.IsLast);

        tokenResult = stringEnumerator.GetNextToken(out _);
        Assert.False(tokenResult.IsSuccess);
        Assert.IsType<PacketEndReachedError>(tokenResult.Error);
        Assert.True(((PacketEndReachedError)tokenResult.Error!).LevelEnd);
    }

    /// <summary>
    /// Test that over reaching the length of a list is not allowed.
    /// </summary>
    [Fact]
    public void EnumeratorDoesNotAllowOvereachingListLength()
    {
        var stringEnumerator = new PacketStringEnumerator("in 1|2.2|3.3|4.4|5");
        var tokenResult = stringEnumerator.GetNextToken(out _);
        Assert.True(tokenResult.IsSuccess); // in

        stringEnumerator.PushLevel('.', 2);
        stringEnumerator.PushLevel('|');

        // first item
        tokenResult = stringEnumerator.GetNextToken(out _);
        Assert.True(tokenResult.IsSuccess);

        tokenResult = stringEnumerator.GetNextToken(out var packetToken);
        Assert.True(tokenResult.IsSuccess);
        Assert.True(packetToken.IsLast);

        stringEnumerator.PopLevel();

        // second item
        stringEnumerator.PushLevel('|');
        tokenResult = stringEnumerator.GetNextToken(out _);
        Assert.True(tokenResult.IsSuccess);

        stringEnumerator.GetNextToken(out packetToken);
        Assert.True(tokenResult.IsSuccess);
        Assert.True(packetToken.IsLast);

        stringEnumerator.PopLevel();

        // cannot reach third item
        Assert.True(stringEnumerator.IsOnLastToken());
        stringEnumerator.PushLevel('|');
        tokenResult = stringEnumerator.GetNextToken(out _);
        Assert.False(tokenResult.IsSuccess);
        Assert.IsType<PacketEndReachedError>(tokenResult.Error);
        Assert.True(((PacketEndReachedError)tokenResult.Error!).LevelEnd);
    }

    /// <summary>
    /// Test that EncounteredUpperLevel is returned if appropriate.
    /// </summary>
    [Fact]
    public void EnumeratorReturnsEncounteredUpperLevel()
    {
        var stringEnumerator = new PacketStringEnumerator("in 1|2 1");
        var tokenResult = stringEnumerator.GetNextToken(out _);
        Assert.True(tokenResult.IsSuccess); // in

        stringEnumerator.PushLevel('.');
        stringEnumerator.PushLevel('|');

        tokenResult = stringEnumerator.GetNextToken(out _);
        Assert.True(tokenResult.IsSuccess);

        tokenResult = stringEnumerator.GetNextToken(out var packetToken);
        Assert.True(tokenResult.IsSuccess);
        Assert.True(packetToken.IsLast);
        Assert.True(packetToken.EncounteredUpperLevel);
    }
}