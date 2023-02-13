//
//  ClientLoginCryptography.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;

namespace NosSmooth.Cryptography;

/// <summary>
/// A cryptography used for logging to NosTale server from the client.
/// </summary>
public class ClientLoginCryptography : ICryptography
{
    private static readonly Random Random = new Random(DateTime.Now.Millisecond);

    /// <inheritdoc />
    public string Decrypt(in ReadOnlySpan<byte> bytes, Encoding encoding)
    {
        try
        {
            var output = new StringBuilder();
            foreach (var c in bytes)
            {
                output.Append(Convert.ToChar(c - 0xF));
            }

            return output.ToString();
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <inheritdoc />
    public byte[] Encrypt(string value, Encoding encoding)
    {
        try
        {
            var output = new byte[value.Length + 1];
            for (int i = 0; i < value.Length; i++)
            {
                output[i] = (byte)((value[i] ^ 0xC3) + 0xF);
            }
            output[output.Length - 1] = 0xD8;
            return output;
        }
        catch
        {
            return Array.Empty<byte>();
        }
    }
}