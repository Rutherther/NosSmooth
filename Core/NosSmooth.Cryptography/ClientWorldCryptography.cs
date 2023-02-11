//
//  ClientWorldCryptography.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;

namespace NosSmooth.Cryptography;

/// <summary>
/// A cryptography used on world server, has to have a session id (encryption key) set from the client.
/// </summary>
public class ClientWorldCryptography : ICryptography
{
    private static readonly char[] Keys = { ' ', '-', '.', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'n' };

    /// <summary>
    /// Gets or sets the encryption key.
    /// </summary>
    public int EncryptionKey { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientWorldCryptography"/> class.
    /// </summary>
    /// <param name="encryptionKey">Encryption key received by LoginServer.</param>
    public ClientWorldCryptography(int encryptionKey = 0)
    {
        EncryptionKey = encryptionKey;
    }

    /// <inheritdoc />
    public string Decrypt(in ReadOnlySpan<byte> bytes, Encoding encoding)
    {
        int index = 0;
        var currentPacket = new List<byte>();

        while (index < bytes.Length)
        {
            byte currentByte = bytes[index++];

            if (currentByte == 0xFF)
            {
                currentPacket.Add((byte)'\n');
                continue;
            }

            int length = currentByte & 0x7F;

            if ((currentByte & 0x80) != 0)
            {
                while (length != 0)
                {
                    if (index < bytes.Length)
                    {
                        currentByte = bytes[index++];
                        int firstIndex = (currentByte & 0xF0) >> 4;
                        char first = '?';
                        if (firstIndex != 0)
                        {
                            firstIndex--;
                            first = firstIndex != 14 ? Keys[firstIndex] : '\u0000';
                        }

                        if (first != 0x6E)
                        {
                            currentPacket.Add((byte)first);
                        }

                        if (length <= 1)
                        {
                            break;
                        }

                        int secondIndex = currentByte & 0xF;
                        char second = '?';
                        if (secondIndex != 0)
                        {
                            secondIndex--;
                            second = secondIndex != 14 ? Keys[secondIndex] : '\u0000';
                        }

                        if (second != 0x6E)
                        {
                            currentPacket.Add((byte)second);
                        }

                        length -= 2;
                    }
                    else
                    {
                        length--;
                    }
                }
            }
            else
            {
                while (length != 0)
                {
                    if (index < bytes.Length)
                    {
                        currentPacket.Add((byte)(bytes[index] ^ 0xFF));
                        index++;
                    }
                    else if (index == bytes.Length)
                    {
                        currentPacket.Add((byte)'\n');
                        index++;
                    }

                    length--;
                }
            }
        }

        return string.Concat(currentPacket.Select(x => (char)x));

        // byte[] tmp = Encoding.Convert(encoding, Encoding.UTF8, currentPacket.ToArray());
        // return Encoding.UTF8.GetString(tmp);
    }

    /// <inheritdoc />
    public byte[] Encrypt(string value, Encoding encoding)
    {
        var output = new List<byte>();

        string mask = new string
        (
            value.Select
            (
                c =>
                {
                    sbyte b = (sbyte)c;
                    if (c == '#' || c == '/' || c == '%')
                    {
                        return '0';
                    }

                    if ((b -= 0x20) == 0 || (b += unchecked((sbyte)0xF1)) < 0 || (b -= 0xB) < 0 ||
                        b - unchecked((sbyte)0xC5) == 0)
                    {
                        return '1';
                    }

                    return '0';
                }
            ).ToArray()
        );

        int packetLength = value.Length;

        int sequenceCounter = 0;
        int currentPosition = 0;

        while (currentPosition <= packetLength)
        {
            int lastPosition = currentPosition;
            while (currentPosition < packetLength && mask[currentPosition] == '0')
            {
                currentPosition++;
            }

            int sequences;
            int length;

            if (currentPosition != 0)
            {
                length = currentPosition - lastPosition;
                sequences = length / 0x7E;
                for (int i = 0; i < length; i++, lastPosition++)
                {
                    if (i == sequenceCounter * 0x7E)
                    {
                        if (sequences == 0)
                        {
                            output.Add((byte)(length - i));
                        }
                        else
                        {
                            output.Add(0x7E);
                            sequences--;
                            sequenceCounter++;
                        }
                    }

                    output.Add((byte)((byte)value[lastPosition] ^ 0xFF));
                }
            }

            if (currentPosition >= packetLength)
            {
                break;
            }

            lastPosition = currentPosition;
            while (currentPosition < packetLength && mask[currentPosition] == '1')
            {
                currentPosition++;
            }

            if (currentPosition == 0)
            {
                continue;
            }

            length = currentPosition - lastPosition;
            sequences = length / 0x7E;
            for (int i = 0; i < length; i++, lastPosition++)
            {
                if (i == sequenceCounter * 0x7E)
                {
                    if (sequences == 0)
                    {
                        output.Add((byte)((length - i) | 0x80));
                    }
                    else
                    {
                        output.Add(0x7E | 0x80);
                        sequences--;
                        sequenceCounter++;
                    }
                }

                byte currentByte = (byte)value[lastPosition];
                switch (currentByte)
                {
                    case 0x20:
                        currentByte = 1;
                        break;
                    case 0x2D:
                        currentByte = 2;
                        break;
                    case 0xFF:
                        currentByte = 0xE;
                        break;
                    default:
                        currentByte -= 0x2C;
                        break;
                }

                if (currentByte == 0x00)
                {
                    continue;
                }

                if (i % 2 == 0)
                {
                    output.Add((byte)(currentByte << 4));
                }
                else
                {
                    output[output.Count - 1] = (byte)(output.Last() | currentByte);
                }
            }
        }

        output.Add(0xFF);

        sbyte sessionNumber = (sbyte)((EncryptionKey >> 6) & 0xFF & 0x80000003);

        if (sessionNumber < 0)
        {
            sessionNumber = (sbyte)(((sessionNumber - 1) | 0xFFFFFFFC) + 1);
        }

        byte sessionKey = (byte)(EncryptionKey & 0xFF);

        if (EncryptionKey != 0)
        {
            sessionNumber = -1;
        }

        switch (sessionNumber)
        {
            case 0:
                for (int i = 0; i < output.Count; i++)
                {
                    output[i] = (byte)(output[i] + sessionKey + 0x40);
                }

                break;
            case 1:
                for (int i = 0; i < output.Count; i++)
                {
                    output[i] = (byte)(output[i] - (sessionKey + 0x40));
                }

                break;
            case 2:
                for (int i = 0; i < output.Count; i++)
                {
                    output[i] = (byte)((output[i] ^ 0xC3) + sessionKey + 0x40);
                }

                break;
            case 3:
                for (int i = 0; i < output.Count; i++)
                {
                    output[i] = (byte)((output[i] ^ 0xC3) - (sessionKey + 0x40));
                }

                break;
            default:
                for (int i = 0; i < output.Count; i++)
                {
                    output[i] = (byte)(output[i] + 0x0F);
                }

                break;
        }

        return output.ToArray();
    }
}