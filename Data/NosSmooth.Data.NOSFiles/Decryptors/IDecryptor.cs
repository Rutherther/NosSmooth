//
//  IDecryptor.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.Data.NOSFiles.Decryptors;

/// <summary>
/// A decryptor interface.
/// </summary>
public interface IDecryptor
{
    /// <summary>
    /// Decrypts the given data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>An array with data or an error.</returns>
    public Result<byte[]> Decrypt(ReadOnlySpan<byte> data);
}