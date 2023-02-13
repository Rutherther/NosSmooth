//
//  CryptographyManager.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Buffers;
using System.Text;

namespace NosSmooth.Cryptography;

/// <summary>
/// A storage of server and client cryptography.
/// </summary>
public class CryptographyManager
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CryptographyManager"/> class.
    /// </summary>
    public CryptographyManager()
    {
        ServerWorld = new ServerWorldCryptography(0);
        ServerLogin = new ServerLoginCryptography();
        ClientLogin = new ClientLoginCryptography();
        ClientWorld = new ClientWorldCryptography(0);
    }

    /// <summary>
    /// Gets the cryptography for server world.
    /// </summary>
    public ICryptography ServerWorld { get; }

    /// <summary>
    /// Gets the cryptography for server login.
    /// </summary>
    public ICryptography ServerLogin { get; }

    /// <summary>
    /// Gets the cryptography for client world.
    /// </summary>
    public ICryptography ClientWorld { get; }

    /// <summary>
    /// Gets the cryptography for client login.
    /// </summary>
    public ICryptography ClientLogin { get; }

    /// <summary>
    /// Gets or sets the encryption key of the connection.
    /// </summary>
    public int EncryptionKey
    {
        get => ((ServerWorldCryptography)ServerWorld).EncryptionKey;
        set
        {
            ((ServerWorldCryptography)ServerWorld).EncryptionKey = value;
            ((ClientWorldCryptography)ClientWorld).EncryptionKey = value;
        }
    }

    /// <summary>
    /// Attempts to decrypt a packet that was received by the client. May be login or world packet.
    /// Encryption key is not needed.
    /// </summary>
    /// <remarks>
    /// Expects only failc or NsTeST for login packets.
    /// </remarks>
    /// <param name="data">The received data.</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns>Decrypted packet or empty string in case of fail.</returns>
    public string DecryptUnknownClientPacket(in ReadOnlySpan<byte> data, Encoding encoding)
    {
        if (data.Length < 6)
        {
            return ClientWorld.Decrypt(data, encoding);
        }

        var firstCharDecrypted = ClientLogin.Decrypt(data.Slice(0, 1), encoding);

        if (firstCharDecrypted.StartsWith("f") ||
                firstCharDecrypted.StartsWith("N"))
        {
            var beginning = data.Slice(0, 6);
            var beginningLoginDecrypted = ClientLogin.Decrypt(beginning, encoding);

            if (beginningLoginDecrypted.StartsWith("failc") ||
                beginningLoginDecrypted.StartsWith("NsTeST"))
            {
                return ClientLogin.Decrypt(data, encoding);
            }
        }

        return ClientWorld.Decrypt(data, encoding);
    }

    /// <summary>
    /// Attempts to decrypt a packet that was received by the server. May be login or world packet.
    /// Encryption key is needed for world packets. If the packet contains session id,
    /// it will be saved to <see cref="EncryptionKey"/>.
    /// </summary>
    /// <remarks>
    /// Expects only failc or NsTeST for login packets.
    /// </remarks>
    /// <param name="data">The received data.</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns>Decrypted packet or empty string in case of fail.</returns>
    public string DecryptUnknownServerPacket(in ReadOnlySpan<byte> data, Encoding encoding)
    {
        var firstCharDecrypted = ClientLogin.Decrypt(data.Slice(0, 1), encoding);

        if (firstCharDecrypted.StartsWith("N"))
        {
            var beginning = data.Slice(0, 4);
            var beginningLoginDecrypted = ClientLogin.Decrypt(beginning, encoding);

            if (beginningLoginDecrypted.StartsWith("NoS "))
            {
                return ServerLogin.Decrypt(data, encoding);
            }
        }

        var encryptionKey = EncryptionKey;
        var decrypted = ServerWorld.Decrypt(data, encoding);

        if (EncryptionKey == 0)
        { // we are not in a session, so the packet may be the session id.
          // or we are in an initialized session and won't know the encryption key...
          if (int.TryParse(decrypted, out var obtainedEncryptionKey))
          {
              EncryptionKey = obtainedEncryptionKey;
          }
        }

        return decrypted;
    }
}