//
//  NosTextFileTypeReader.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using NosSmooth.Data.NOSFiles.Decryptors;
using NosSmooth.Data.NOSFiles.Files;
using Remora.Results;

namespace NosSmooth.Data.NOSFiles.Readers.Types;

/// <summary>
/// Reader of .NOS files that contain text files.
/// </summary>
public class NosTextFileTypeReader : BaseFileTypeReader<FileArchive>
{
    private readonly IDecryptor _datDecryptor;
    private readonly IDecryptor _lstDecryptor;

    /// <summary>
    /// Initializes a new instance of the <see cref="NosTextFileTypeReader"/> class.
    /// </summary>
    public NosTextFileTypeReader()
    {
        _datDecryptor = new DatDecryptor();
        _lstDecryptor = new LstDecryptor();
    }

    /// <inheritdoc />
    public override bool SupportsFile(RawFile file)
    {
        // Just checks that the number of the first file is zero.
        //  ?Should? be enough for excluding all other types.
        // This is questionable. The thing is
        // that there is not really an header for these files.
        // TODO: maybe try to read till the first file name, size etc.
        // and verify the name is a string, the number or the file is zero etc?
        if (file.Length < 10)
        {
            return false;
        }

        return file.Path.EndsWith
            ("NOS") && file.Content[4] == 1 && file.Content[5] == 0 && file.Content[6] == 0 && file.Content[7] == 0;
    }

    /// <inheritdoc />
    public override Result<ReadFile<FileArchive>> ReadExact(RawFile file)
    {
        List<RawFile> files = new List<RawFile>();
        ReadOnlySpan<byte> content = file.Content;
        var filesCount = BinaryPrimitives.ReadInt32LittleEndian(content);
        content = content.Slice(4); // skip file count

        for (var i = 0; i < filesCount; i++)
        {
            var fileResult = ReadFile(ref content);
            if (!fileResult.IsSuccess)
            {
                return Result<ReadFile<FileArchive>>.FromError(fileResult);
            }

            files.Add(fileResult.Entity);
        }

        // TODO: read time information?
        return new ReadFile<FileArchive>(file.Path, new FileArchive(files));
    }

    private Result<RawFile> ReadFile(ref ReadOnlySpan<byte> content)
    {
        // skip file number, it is of no interest, I think.
        content = content.Slice(4);
        var stringNameLength = BinaryPrimitives.ReadInt32LittleEndian(content);
        content = content.Slice(4);
        var fileName = Encoding.ASCII.GetString(content.Slice(0, stringNameLength).ToArray());
        content = content.Slice(stringNameLength);
        var isDat = BinaryPrimitives.ReadInt32LittleEndian(content);
        content = content.Slice(4);
        var fileSize = BinaryPrimitives.ReadInt32LittleEndian(content);
        content = content.Slice(4);
        var fileContent = content.Slice(0, fileSize);
        content = content.Slice(fileSize);

        var datDecryptResult = isDat != 0 ? _datDecryptor.Decrypt(fileContent) : _lstDecryptor.Decrypt(fileContent);
        if (!datDecryptResult.IsSuccess)
        {
            return Result<RawFile>.FromError(datDecryptResult);
        }

        return new RawFile(FileType.Text, fileName, fileSize, datDecryptResult.Entity);
    }
}