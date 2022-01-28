//
//  LstDecryptor.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Buffers.Binary;
using Remora.Results;

namespace NosSmooth.Data.NOSFiles.Decryptors;

/// <inheritdoc />
public class LstDecryptor : IDecryptor
{
    /// <inheritdoc />
    public Result<byte[]> Decrypt(ReadOnlySpan<byte> data)
    {
        var output = new MemoryStream();
        int linesCount = BinaryPrimitives.ReadInt32LittleEndian(data);
        data = data.Slice(4);
        for (var i = 0; i < linesCount; i++)
        {
            int lineLength = BinaryPrimitives.ReadInt32LittleEndian(data);
            data = data.Slice(4);
            ReadOnlySpan<byte> line = data.Slice(0, lineLength);
            data = data.Slice(lineLength);
            foreach (var c in line)
            {
                output.WriteByte((byte)(c ^ 0x1));
            }
            output.WriteByte((byte)'\n');
        }

        return output.ToArray();
    }
}