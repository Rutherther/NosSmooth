//
//  StringConverterTests.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using NosSmooth.PacketSerializer.Abstractions;
using NosSmooth.PacketSerializer.Extensions;
using Xunit;

namespace NosSmooth.Packets.Tests.Converters.Basic;

/// <summary>
/// Tests for <see cref="StringConverter"/>.
/// </summary>
public class StringConverterTests
{
    private readonly IStringSerializer _stringSerializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringConverterTests"/> class.
    /// </summary>
    public StringConverterTests()
    {
        var provider = new ServiceCollection()
            .AddPacketSerialization()
            .BuildServiceProvider();

        _stringSerializer = provider.GetRequiredService<IStringSerializer>();
    }

    /// <summary>
    /// Tests that the serializer serializes null as -.
    /// </summary>
    [Fact]
    public void TestsTreatsNullAsMinus()
    {
        string? test = null;
        var stringBuilder = new PacketStringBuilder(stackalloc char[500]);
        var serializeResult = _stringSerializer.Serialize(test, in stringBuilder);
        Assert.True(serializeResult.IsSuccess, !serializeResult.IsSuccess ? serializeResult.Error.Message : string.Empty);
        Assert.Equal("-", stringBuilder.ToString());
    }

    /// <summary>
    /// Tests that the serializer serializes null as -.
    /// </summary>
    [Fact]
    public void TestsTreatsMinusAsNull_IfNullEnabled()
    {
        var deserialize = "-";
        var stringEnumerator = new PacketStringEnumerator(deserialize);
        var deserializeResult = _stringSerializer.Deserialize<string?>(in stringEnumerator, DeserializeOptions.Nullable);
        Assert.True(deserializeResult.IsSuccess, !deserializeResult.IsSuccess ? deserializeResult.Error.Message : string.Empty);
        Assert.Null(deserializeResult.Entity);
    }

    /// <summary>
    /// Tests that the serializer serializes - as -.
    /// </summary>
    [Fact]
    public void TestsTreatsMinusAsNull_IfNullDisabled()
    {
        var deserialize = "-";
        var stringEnumerator = new PacketStringEnumerator(deserialize);
        var deserializeResult = _stringSerializer.Deserialize<string?>(in stringEnumerator, default);
        Assert.True(deserializeResult.IsSuccess, !deserializeResult.IsSuccess ? deserializeResult.Error.Message : string.Empty);
        Assert.NotNull(deserializeResult.Entity);
        Assert.Equal("-", deserializeResult.Entity);
    }
}