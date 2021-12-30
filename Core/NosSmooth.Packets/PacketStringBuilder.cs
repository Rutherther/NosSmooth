//
//  PacketStringBuilder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Text;
using Remora.Results;

namespace NosSmooth.Packets;

/// <summary>
/// String builder for packets.
/// </summary>
public class PacketStringBuilder
{
    private readonly StringBuilder _builder;
    private StringBuilderLevel _currentLevel;
    private char? _preparedLevelSeparator;
    private char? _insertSeparator;

    /// <summary>
    /// Initializes a new instance of the <see cref="PacketStringBuilder"/> class.
    /// </summary>
    /// <param name="separator">The top level separator.</param>
    public PacketStringBuilder(char separator = ' ')
    {
        _currentLevel = new StringBuilderLevel(null, separator);
        _preparedLevelSeparator = _insertSeparator = null;
        _builder = new StringBuilder();
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
        _preparedLevelSeparator = separator;
    }

    /// <summary>
    /// Reset the prepared level, if there is any.
    /// </summary>
    public void RemovePreparedLevel()
    {
        _preparedLevelSeparator = null;
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
        if (_preparedLevelSeparator is null)
        {
            return false;
        }

        _currentLevel = new StringBuilderLevel(_currentLevel, _preparedLevelSeparator.Value);
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
        _preparedLevelSeparator = null;
        _currentLevel = new StringBuilderLevel(_currentLevel, separator);
    }

    /// <summary>
    /// Pop the current level.
    /// </summary>
    /// <returns>A result that may or may not have succeeded. There will be an error if the current level is the top one.</returns>
    public Result PopLevel()
    {
        if (_currentLevel.Parent is null)
        {
            return new InvalidOperationError("The level cannot be popped, the stack is already at the top level.");
        }

        _currentLevel = _currentLevel.Parent;

        return Result.FromSuccess();
    }

    /// <summary>
    /// Appends the value to the string.
    /// </summary>
    /// <param name="value">The value to append.</param>
    public void Append(string value)
    {
        if (_insertSeparator is not null)
        {
            _builder.Append(_insertSeparator);
            _insertSeparator = null;
        }

        _builder.Append(value);
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
        return _builder.ToString();
    }

    private class StringBuilderLevel
    {
        public StringBuilderLevel(StringBuilderLevel? parent, char separator)
        {
            Parent = parent;
            Separator = separator;
        }

        public StringBuilderLevel? Parent { get; }

        public char Separator { get; }

        public char? SeparatorOnce { get; set; }
    }
}