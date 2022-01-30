//
//  DatDecryptor.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;
using Remora.Results;

namespace NosSmooth.Data.NOSFiles.Decryptors;

/// <inheritdoc />
public class DatDecryptor : IDecryptor
{
    private readonly byte[] _cryptoArray;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatDecryptor"/> class.
    /// </summary>
    public DatDecryptor()
    {
        _cryptoArray = new byte[] { 0x00, 0x20, 0x2D, 0x2E, 0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x0A, 0x00 };
    }

    /// <inheritdoc />
    public Result<byte[]> Decrypt(ReadOnlySpan<byte> data)
    {
        using var output = new MemoryStream();
        int i = 0;
        while (i < data.Length)
        {
            byte currentByte = data[i];
            i++;

            if (currentByte == 0xFF)
            {
                output.WriteByte(0xD);
                continue;
            }

            int validate = currentByte & 0x7F;
            if ((currentByte & 0x80) != 0)
            {
                for (; validate > 0 && i < data.Length; validate -= 2)
                {
                    currentByte = data[i];
                    i++;
                    byte firstByte = _cryptoArray[(currentByte & 0xF0) >> 4];
                    output.WriteByte(firstByte);

                    if (validate <= 1)
                    {
                        break;
                    }

                    byte secondByte = _cryptoArray[currentByte & 0x0F];
                    if (secondByte == 0)
                    {
                        break;
                    }
                    output.WriteByte(secondByte);
                }
            }
            else
            {
                for (; validate > 0 && i < data.Length; validate--)
                {
                    currentByte = data[i];
                    output.WriteByte((byte)(currentByte ^ 0x33));
                    i++;
                }
            }
        }

        return output.ToArray();
    }
}