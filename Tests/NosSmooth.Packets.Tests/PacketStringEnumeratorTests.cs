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
        var headerTokenResult = stringEnumerator.GetNextToken();
        Assert.True(headerTokenResult.IsSuccess);
        Assert.False(headerTokenResult.Entity.PacketEndReached);
        Assert.NotNull(headerTokenResult.Entity.IsLast);
        Assert.NotNull(headerTokenResult.Entity.EncounteredUpperLevel);
        Assert.False(headerTokenResult.Entity.IsLast);
        Assert.False(headerTokenResult.Entity.EncounteredUpperLevel);
        Assert.Matches("in", headerTokenResult.Entity.Token);

        var firstToken = stringEnumerator.GetNextToken();
        Assert.True(firstToken.IsSuccess);
        Assert.False(firstToken.Entity.PacketEndReached);
        Assert.NotNull(firstToken.Entity.IsLast);
        Assert.NotNull(firstToken.Entity.EncounteredUpperLevel);
        Assert.False(firstToken.Entity.IsLast);
        Assert.False(firstToken.Entity.EncounteredUpperLevel);
        Assert.Matches("1", firstToken.Entity.Token);

        var listEnumerator = stringEnumerator.CreateLevel('|');
        listEnumerator.PrepareLevel('.');

        for (int i = 0; i < 3; i++)
        {
            var preparedLevel = listEnumerator.CreatePreparedLevel();
            Assert.NotNull(preparedLevel);

            for (int j = 0; j < 3; j++)
            {
                string currentNum = (j + (i * 3) + 1 + 10).ToString();
                var currentToken = preparedLevel!.Value.GetNextToken();
                Assert.True(currentToken.IsSuccess);
                Assert.False(currentToken.Entity.PacketEndReached);
                Assert.NotNull(currentToken.Entity.IsLast);
                Assert.NotNull(currentToken.Entity.EncounteredUpperLevel);
                if (j == 2 && i == 2)
                {
                    Assert.True(currentToken.Entity.EncounteredUpperLevel);
                }
                else
                {
                    Assert.False(currentToken.Entity.EncounteredUpperLevel);
                }

                if (j != 2)
                {
                    Assert.False(currentToken.Entity.IsLast);
                }
                else
                {
                    Assert.True(currentToken.Entity.IsLast);
                }

                Assert.Matches(currentNum, currentToken.Entity.Token);
            }

            Assert.True(preparedLevel!.Value.IsOnLastToken());
        }

        Assert.True(stringEnumerator.IsOnLastToken());
    }

    /// <summary>
    /// Test that over reaching the end is not allowed.
    /// </summary>
    [Fact]
    public void EnumeratorDoesNotAllowOvereachingPacketEnd()
    {
        var stringEnumerator = new PacketStringEnumerator("in 1 2 3 4");
        var tokenResult = stringEnumerator.GetNextToken();
        Assert.True(tokenResult.IsSuccess); // in
        tokenResult = stringEnumerator.GetNextToken();
        Assert.True(tokenResult.IsSuccess); // 1
        tokenResult = stringEnumerator.GetNextToken();
        Assert.True(tokenResult.IsSuccess); // 2
        tokenResult = stringEnumerator.GetNextToken();
        Assert.True(tokenResult.IsSuccess); // 3
        tokenResult = stringEnumerator.GetNextToken();
        Assert.True(tokenResult.IsSuccess); // 4

        tokenResult = stringEnumerator.GetNextToken();
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
        var tokenResult = stringEnumerator.GetNextToken();
        Assert.True(tokenResult.IsSuccess); // in

        var listEnumerator = stringEnumerator.CreateLevel('.');
        var itemEnumerator = listEnumerator.CreateLevel('|');

        tokenResult = itemEnumerator.GetNextToken();
        Assert.True(tokenResult.IsSuccess); // 1

        tokenResult = itemEnumerator.GetNextToken();
        Assert.True(tokenResult.IsSuccess); // 2
        Assert.True(tokenResult.Entity.IsLast);

        tokenResult = itemEnumerator.GetNextToken();
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
        var tokenResult = stringEnumerator.GetNextToken();
        Assert.True(tokenResult.IsSuccess); // in

        var listEnumerator = stringEnumerator.CreateLevel('.', 2);
        var itemEnumerator = listEnumerator.CreateLevel('|');

        // first item
        tokenResult = itemEnumerator.GetNextToken();
        Assert.True(tokenResult.IsSuccess);

        tokenResult = itemEnumerator.GetNextToken();
        Assert.True(tokenResult.IsSuccess);
        Assert.True(tokenResult.Entity.IsLast);

        // second item
        itemEnumerator = listEnumerator.CreateLevel('|');
        tokenResult = itemEnumerator.GetNextToken();
        Assert.True(tokenResult.IsSuccess);

        tokenResult = itemEnumerator.GetNextToken();
        Assert.True(tokenResult.IsSuccess);
        Assert.True(tokenResult.Entity.IsLast);

        // cannot reach third item
        Assert.True(listEnumerator.IsOnLastToken());
        itemEnumerator = listEnumerator.CreateLevel('|');
        tokenResult = itemEnumerator.GetNextToken();
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
        var tokenResult = stringEnumerator.GetNextToken();
        Assert.True(tokenResult.IsSuccess); // in

        var listEnumerator = stringEnumerator.CreateLevel('.');
        var itemEnumerator = listEnumerator.CreateLevel('|');

        tokenResult = itemEnumerator.GetNextToken();
        Assert.True(tokenResult.IsSuccess);

        tokenResult = itemEnumerator.GetNextToken();
        Assert.True(tokenResult.IsSuccess);
        Assert.True(tokenResult.Entity.IsLast);
        Assert.True(tokenResult.Entity.EncounteredUpperLevel);
    }
}