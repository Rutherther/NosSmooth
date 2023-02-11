//
//  CryptographyManager.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
}