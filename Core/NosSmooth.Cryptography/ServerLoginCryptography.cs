﻿//
//  ServerLoginCryptography.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;

namespace NosSmooth.Cryptography;

/// <summary>
/// A cryptography used for logging to NosTale server from the server.
/// </summary>
public class ServerLoginCryptography : ICryptography
{
    /// <inheritdoc />
    public string Decrypt(in ReadOnlySpan<byte> str, Encoding encoding)
    {
        try
        {
            StringBuilder decryptedPacket = new StringBuilder(str.Length);

            foreach (byte character in str)
            {
                if (character > 14)
                {
                    decryptedPacket.Append(Convert.ToChar((character - 15) ^ 195));
                }
                else
                {
                    decryptedPacket.Append(Convert.ToChar((256 - (15 - character)) ^ 195));
                }
            }

            return decryptedPacket.ToString();
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <inheritdoc />
    public byte[] Encrypt(string packet, Encoding encoding)
    {
        try
        {
            packet += " ";
            byte[] tmp = encoding.GetBytes(packet);
            for (int i = 0; i < packet.Length; i++)
            {
                tmp[i] = Convert.ToByte(tmp[i] + 15);
            }
            tmp[tmp.Length - 1] = 25;
            return tmp;
        }
        catch
        {
            return Array.Empty<byte>();
        }
    }
}