//
//  NosZlibFileTypeReader.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Buffers;
using System.Buffers.Binary;
using System.Data.Common;
using System.IO.Compression;
using System.Text;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using NosSmooth.Data.NOSFiles.Files;
using Remora.Results;

namespace NosSmooth.Data.NOSFiles.Readers.Types;

/// <inheritdoc />
public class NosZlibFileTypeReader : BaseFileTypeReader<FileArchive>
{
    /// <inheritdoc />
    public override Result<ReadFile<FileArchive>> ReadExact(RawFile file)
    {
        using var fileStream = new MemoryStream(file.Content, false);
        ReadOnlySpan<byte> contentFromStart = file.Content;
        ReadOnlySpan<byte> content = contentFromStart;
        content = content.Slice(16); // skip header
        List<RawFile> files = new List<RawFile>();
        var filesCount = BinaryPrimitives.ReadInt32LittleEndian(content);
        content = content.Slice(4);

        content = content.Slice(1); // separator
        for (var i = 0; i < filesCount; i++)
        {
            var id = BinaryPrimitives.ReadInt32LittleEndian(content);
            var offset = BinaryPrimitives.ReadInt32LittleEndian(content.Slice(4));
            content = content.Slice(8);

            fileStream.Seek(offset, SeekOrigin.Begin);
            var readFileResult = ReadFile(id, fileStream, contentFromStart.Slice(offset));
            if (!readFileResult.IsSuccess)
            {
                return Result<ReadFile<FileArchive>>.FromError(readFileResult);
            }

            files.Add(readFileResult.Entity);
        }

        return new ReadFile<FileArchive>(file.Path, new FileArchive(files));
    }

    /// <inheritdoc />
    public override bool SupportsFile(RawFile file)
    {
        if (file.Length < 16)
        {
            return false;
        }

        var header = GetHeader(file.Content);
        return header.StartsWith("NT Data 05");
    }

    private ReadOnlySpan<char> GetHeader(ReadOnlySpan<byte> content)
    {
        return Encoding.ASCII.GetString(content.Slice(0, 16));
    }

    private Result<RawFile> ReadFile(int id, MemoryStream stream, ReadOnlySpan<byte> content)
    {
        // int creationDate = BinaryPrimitives.ReadInt32LittleEndian(content);
        content = content.Slice(4);
        int dataSize = BinaryPrimitives.ReadInt32LittleEndian(content);
        content = content.Slice(4);
        int compressedDataSize = BinaryPrimitives.ReadInt32LittleEndian(content);
        content = content.Slice(4);
        bool isCompressed = content[0] != 0;
        content = content.Slice(1);
        ReadOnlySpan<byte> data = content.Slice(0, compressedDataSize);
        byte[]? dataArray = null;

        if (isCompressed)
        {
            stream.Seek(4 + 4 + 4 + 1, SeekOrigin.Current);
            using var decompressionStream = new InflaterInputStream(stream)
            {
                IsStreamOwner = false
            };
            dataArray = new byte[dataSize];
            using var outputStream = new MemoryStream(dataSize);
            decompressionStream.CopyTo(outputStream);
        }

        // TODO: check that data size matches data.Length?
        return new RawFile(FileType.Binary, id.ToString(), data.Length, dataArray ?? data.ToArray());
    }
}