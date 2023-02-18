//
//  BoolStringConverterTests.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using NosSmooth.PacketSerializer.Abstractions;
using NosSmooth.PacketSerializer.Converters.Basic;
using NosSmooth.PacketSerializer.Extensions;
using Xunit;

namespace NosSmooth.Packets.Tests.Converters.Basic;

/// <summary>
/// Tests for <see cref="BoolStringConverter"/>.
/// </summary>
public class BoolStringConverterTests
{
    private readonly IStringSerializer _stringSerializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="BoolStringConverterTests"/> class.
    /// </summary>
    public BoolStringConverterTests()
    {
        var provider = new ServiceCollection()
            .AddPacketSerialization()
            .BuildServiceProvider();

        _stringSerializer = provider.GetRequiredService<IStringSerializer>();
    }

    /// <summary>
    /// Tests that the serializer serializes null as -1.
    /// </summary>
    [Fact]
    public void TestsTreatsNullAsMinusOne()
    {
        bool? test = null;
        var stringBuilder = new PacketStringBuilder(stackalloc char[500]);
        var serializeResult = _stringSerializer.Serialize(test, ref stringBuilder);
        Assert.True(serializeResult.IsSuccess, !serializeResult.IsSuccess ? serializeResult.Error.Message : string.Empty);
        Assert.Equal("-1", stringBuilder.ToString());
    }

    /// <summary>
    /// Tests that the serializer deserializes null as -1.
    /// </summary>
    [Fact]
    public void TestsSerializesMinusOneAsNull()
    {
        var deserialize = "-1";
        var stringEnumerator = new PacketStringEnumerator(deserialize);
        var deserializeResult = _stringSerializer.Deserialize<bool?>(ref stringEnumerator, DeserializeOptions.Nullable);
        Assert.True(deserializeResult.IsSuccess, !deserializeResult.IsSuccess ? deserializeResult.Error.Message : string.Empty);
        Assert.Null(deserializeResult.Entity);
    }

    /// <summary>
    /// Tests that the serializer deserializes 1 as true.
    /// </summary>
    [Fact]
    public void TestsSerializesOneAsTrue()
    {
        var deserialize = "1";
        var stringEnumerator = new PacketStringEnumerator(deserialize);
        var deserializeResult = _stringSerializer.Deserialize<bool?>(ref stringEnumerator, default);
        Assert.True(deserializeResult.IsSuccess, !deserializeResult.IsSuccess ? deserializeResult.Error.Message : string.Empty);
        Assert.True(deserializeResult.Entity);
    }

    /// <summary>
    /// Tests that the serializer deserializes 0 as false.
    /// </summary>
    [Fact]
    public void TestsSerializesOneAsFalse()
    {
        var deserialize = "0";
        var stringEnumerator = new PacketStringEnumerator(deserialize);
        var deserializeResult = _stringSerializer.Deserialize<bool?>(ref stringEnumerator, default);
        Assert.True(deserializeResult.IsSuccess, !deserializeResult.IsSuccess ? deserializeResult.Error.Message : string.Empty);
        Assert.False(deserializeResult.Entity);
    }
}