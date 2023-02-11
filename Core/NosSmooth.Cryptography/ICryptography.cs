//
//  ICryptography.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;

namespace NosSmooth.Cryptography;

/// <summary>
/// An intefrace for NosTale cryptography, encryption, decryption of packets.
/// </summary>
public interface ICryptography
{
    /// <summary>
    /// Decrypt the raw packet (byte array) to a readable list string.
    /// </summary>
    /// <param name="str">Bytes to decrypt.</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns>Decrypted packet to string list.</returns>
    string Decrypt(in ReadOnlySpan<byte> str, Encoding encoding);

    /// <summary>
    /// Encrypt the string packet to byte array.
    /// </summary>
    /// <param name="packet">String to encrypt.</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns>Encrypted packet as byte array.</returns>
    byte[] Encrypt(string packet, Encoding encoding);
}