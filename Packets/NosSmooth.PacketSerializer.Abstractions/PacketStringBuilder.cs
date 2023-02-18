//
//  PacketStringBuilder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Buffers;
using System.Text;
using Remora.Results;

namespace NosSmooth.PacketSerializer.Abstractions;

/// <summary>
/// String builder for packets.
/// </summary>
public ref struct PacketStringBuilder
{
    private Span<char> _buffer;
    private char[]? _bufferArray;
    private int _position;
    private StringBuilderLevel _currentLevel;
    private char? _insertSeparator;

    /// <summary>
    /// Initializes a new instance of the <see cref="PacketStringBuilder"/> struct.
    /// </summary>
    /// <param name="initialBuffer">The initial buffer to store the packet to. Will grow in size if needed.</param>
    /// <param name="separator">The top level separator.</param>
    public PacketStringBuilder(Span<char> initialBuffer, char separator = ' ')
    {
        _currentLevel = new StringBuilderLevel(null, separator);
        _insertSeparator = null;
        _buffer = initialBuffer;
        _position = 0;
        _bufferArray = null;
    }

    /// <summary>
    /// Sets the separator to search for only once.
    /// </summary>
    /// <remarks>
    /// This separator will have the most priority.
    /// </remarks>
    /// <param name="separator">The separator to look for.</param>
    public void SetAfterSeparatorOnce(char separator)
    {
        _currentLevel.SeparatorOnce = separator;
    }

    /// <summary>
    /// Prepare the given level that can be set when needed.
    /// </summary>
    /// <param name="separator">The separator of the prepared level.</param>
    public void PrepareLevel(char separator)
    {
        _currentLevel.PreparedLevelSeparator = separator;
    }

    /// <summary>
    /// Reset the prepared level, if there is any.
    /// </summary>
    public void RemovePreparedLevel()
    {
        _currentLevel.PreparedLevelSeparator = null;
    }

    /// <summary>
    /// Create next level with the separator given in the prepared level.
    /// </summary>
    /// <remarks>
    /// Level of the current builder will stay the same.
    /// Will return null, if there is not a level prepared.
    /// </remarks>
    /// <returns>An enumerator with the new level pushed.</returns>
    public bool PushPreparedLevel()
    {
        if (_currentLevel.PreparedLevelSeparator is null)
        {
            return false;
        }

        _currentLevel = new StringBuilderLevel(_currentLevel, _currentLevel.PreparedLevelSeparator.Value);
        return true;
    }

    /// <summary>
    /// Push new separator level to the stack.
    /// </summary>
    /// <remarks>
    /// This will change the current enumerator.
    /// It has to be <see cref="PopLevel"/> after parent level should be used.
    /// </remarks>
    /// <param name="separator">The separator of the new level.</param>
    public void PushLevel(char separator)
    {
        _currentLevel = new StringBuilderLevel(_currentLevel, separator);
    }

    /// <summary>
    /// Pop the current level.
    /// </summary>
    /// <param name="replaceSeparator">Whether to replace the last separator with the parent one.</param>
    /// <returns>A result that may or may not have succeeded. There will be an error if the current level is the top one.</returns>
    public Result PopLevel(bool replaceSeparator = true)
    {
        if (_currentLevel.Parent is null)
        {
            return new InvalidOperationError("The level cannot be popped, the stack is already at the top level.");
        }

        if (replaceSeparator)
        {
            ReplaceWithParentSeparator();
        }

        _currentLevel = _currentLevel.Parent;

        return Result.FromSuccess();
    }

    /// <summary>
    /// Appends a value that is span formattable.
    /// </summary>
    /// <param name="value">The value to append.</param>
    /// <typeparam name="T">The span formattable type.</typeparam>
    public void Append<T>(T value)
        where T : ISpanFormattable
    {
        AppendSpanFormattable(value);
    }

    /// <summary>
    /// Appends the value to the string.
    /// </summary>
    /// <param name="value">The value to append.</param>
    public void Append(ReadOnlySpan<char> value)
    {
        BeforeAppend();
        if (!value.TryCopyTo(_buffer.Slice(_position)))
        {
            GrowBuffer(value.Length);
        }
        _position += value.Length;
        AfterAppend();
    }

    /// <summary>
    /// Appends the value to the string.
    /// </summary>
    /// <param name="value">The value to append.</param>
    public void Append(int value)
        => AppendSpanFormattable(value);

    /// <summary>
    /// Appends the value to the string.
    /// </summary>
    /// <param name="value">The value to append.</param>
    public void Append(uint value)
        => AppendSpanFormattable(value);

    /// <summary>
    /// Appends the value to the string.
    /// </summary>
    /// <param name="value">The value to append.</param>
    public void Append(short value)
        => AppendSpanFormattable(value);

    /// <summary>
    /// Appends the value to the string.
    /// </summary>
    /// <param name="value">The value to append.</param>
    public void Append(char value)
        => AppendSpanFormattable(value);

    /// <summary>
    /// Appends the value to the string.
    /// </summary>
    /// <param name="value">The value to append.</param>
    public void Append(ushort value)
        => AppendSpanFormattable(value);

    /// <summary>
    /// Appends the value to the string.
    /// </summary>
    /// <param name="value">The value to append.</param>
    public void Append(long value)
        => AppendSpanFormattable(value);

    /// <summary>
    /// Appends the value to the string.
    /// </summary>
    /// <param name="value">The value to append.</param>
    public void Append(ulong value)
        => AppendSpanFormattable(value);

    /// <summary>
    /// Appends the value to the string.
    /// </summary>
    /// <param name="value">The value to append.</param>
    public void Append(byte value)
        => AppendSpanFormattable(value);

    /// <summary>
    /// Appends the value to the string.
    /// </summary>
    /// <param name="value">The value to append.</param>
    public void Append(sbyte value)
        => AppendSpanFormattable(value);

    /// <summary>
    /// Appends the value to the string.
    /// </summary>
    /// <param name="value">The value to append.</param>
    public void Append(float value)
        => AppendSpanFormattable(value);

    /// <summary>
    /// Appends the value to the string.
    /// </summary>
    /// <param name="value">The value to append.</param>
    public void Append(double value)
        => AppendSpanFormattable(value);

    /// <summary>
    /// Appends the value to the string.
    /// </summary>
    /// <param name="value">The value to append.</param>
    public void Append(decimal value)
        => AppendSpanFormattable(value);

    /// <summary>
    /// Returns buffer to ArrayPool if it has been used.
    /// </summary>
    public void Dispose()
    {
        if (_bufferArray is not null)
        {
            ArrayPool<char>.Shared.Return(_bufferArray);
        }
    }

    private void AppendSpanFormattable<T>(T value)
        where T : ISpanFormattable
    {
        BeforeAppend();
        int charsWritten;
        while (!value.TryFormat(_buffer.Slice(_position), out charsWritten, default, null))
        {
            GrowBuffer();
        }
        _position += charsWritten;
        AfterAppend();
    }

    private void GrowBuffer(int needed = 0)
    {
        var sizeNeeded = _buffer.Length + needed;
        var doubleSize = _buffer.Length * 2;
        var newSize = Math.Max(doubleSize, sizeNeeded);
        var newBuffer = ArrayPool<char>.Shared.Rent(newSize);

        _buffer.CopyTo(newBuffer);
        _buffer = newBuffer;

        if (_bufferArray is not null)
        {
            ArrayPool<char>.Shared.Return(_bufferArray);
        }
        _bufferArray = newBuffer;
    }

    private void BeforeAppend()
    {
        if (_insertSeparator is not null)
        {
            if (_buffer.Length <= _position + 1)
            {
                GrowBuffer();
            }

            _buffer[_position] = _insertSeparator.Value;
            _position += 1;
            _insertSeparator = null;
        }
    }

    private void AfterAppend()
    {
        _insertSeparator = _currentLevel.SeparatorOnce ?? _currentLevel.Separator;
        _currentLevel.SeparatorOnce = null;
    }

    /// <summary>
    /// Replace the last separator with the parent separator.
    /// </summary>
    public void ReplaceWithParentSeparator()
    {
        var parent = _currentLevel.Parent;
        if (_insertSeparator is null || parent is null)
        {
            return;
        }

        _insertSeparator = parent.SeparatorOnce ?? parent.Separator;
        parent.SeparatorOnce = null;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return _buffer.Slice(0, _position).ToString();
    }

    private class StringBuilderLevel
    {
        public StringBuilderLevel(StringBuilderLevel? parent, char separator)
        {
            Parent = parent;
            Separator = separator;
        }

        public StringBuilderLevel? Parent { get; }

        public char? PreparedLevelSeparator { get; set; }

        public char Separator { get; }

        public char? SeparatorOnce { get; set; }
    }
}