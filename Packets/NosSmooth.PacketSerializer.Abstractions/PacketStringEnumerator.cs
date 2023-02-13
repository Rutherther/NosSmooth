//
//  PacketStringEnumerator.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Errors;
using Remora.Results;

namespace NosSmooth.PacketSerializer.Abstractions;

/// <summary>
/// Enumerator for packet strings.
/// </summary>
public ref struct PacketStringEnumerator
{
    private readonly ReadOnlySpan<char> _data;
    private readonly Dictionary<char, ushort> _numberOfSeparators;
    private EnumeratorLevel _currentLevel;
    private bool _currentTokenRead;
    private PacketToken _currentToken;
    private bool _readToLast;

    private int _cursor;

    /// <summary>
    /// Initializes a new instance of the <see cref="PacketStringEnumerator"/> struct.
    /// </summary>
    /// <param name="data">The packet string data.</param>
    /// <param name="separator">The separator to use on the highest level.</param>
    public PacketStringEnumerator(ReadOnlySpan<char> data, char separator = ' ')
    {
        _currentLevel = new EnumeratorLevel(null, separator);
        _data = data;
        _cursor = 0;
        _numberOfSeparators = new Dictionary<char, ushort>();
        _numberOfSeparators.Add(separator, 1);
        _currentToken = new PacketToken(default, default, default, default);
        _readToLast = false;
        _currentTokenRead = false;
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
        _currentTokenRead = false;
        _currentLevel.SeparatorOnce = separator;
    }

    /// <summary>
    /// Capture current read tokens number.
    /// </summary>
    /// <remarks>
    /// Usable for lists that may have indeterminate separators (same multiple separators).
    /// The list converter may capture read tokens before reading an element and increment after token is read.
    /// </remarks>
    public void CaptureReadTokens()
    {
        _currentLevel.CapturedTokensRead = _currentLevel.TokensRead;
    }

    /// <summary>
    /// Increment read tokens from the captured state taht was captured using <see cref="CaptureReadTokens"/>.
    /// </summary>
    /// <remarks>
    /// Usable for lists that may have indeterminate separators (same multiple separators).
    /// The list converter may capture read tokens before reading an element and increment after token is read.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown in case <see cref="CaptureReadTokens"/> was not called before.</exception>
    public void IncrementReadTokens()
    {
        if (_currentLevel.CapturedTokensRead is null)
        {
            throw new InvalidOperationException
                ("The read tokens cannot be incremented as CaptureReadTokens was not called.");
        }

        _currentLevel.TokensRead = _currentLevel.CapturedTokensRead.Value + 1;
        _currentLevel.CapturedTokensRead = null;

        if (_currentLevel.TokensRead >= _currentLevel.MaxTokens)
        {
            _currentLevel.ReachedEnd = true;
        }
    }

    /// <summary>
    /// Sets that the next token should be read to the last entry in the level.
    /// </summary>
    public void SetReadToLast()
    {
        _readToLast = true;
    }

    /// <summary>
    /// Prepare the given level that can be set when needed.
    /// </summary>
    /// <param name="separator">The separator of the prepared level.</param>
    /// <param name="maxTokens">The maximum number of tokens for the level.</param>
    public void PrepareLevel(char separator, uint? maxTokens = null)
    {
        _currentLevel.PreparedLevel = (separator, maxTokens);
    }

    /// <summary>
    /// Reset the prepared level, if there is any.
    /// </summary>
    public void RemovePreparedLevel()
    {
        _currentLevel.PreparedLevel = null;
    }

    /// <summary>
    /// Push next level with the separator given in the prepared level.
    /// </summary>
    /// <returns>Whether there is a prepared level present.</returns>
    public bool PushPreparedLevel()
    {
        var preparedLevel = _currentLevel.PreparedLevel;
        if (preparedLevel is null)
        {
            return false;
        }

        _currentTokenRead = false;
        _currentLevel = new EnumeratorLevel(_currentLevel, preparedLevel.Value.Separator, preparedLevel.Value.MaxTokens)
        {
            ReachedEnd = _currentLevel.ReachedEnd
        };

        if (!_numberOfSeparators.ContainsKey(preparedLevel.Value.Separator))
        {
            _numberOfSeparators.Add(preparedLevel.Value.Separator, 0);
        }

        _numberOfSeparators[preparedLevel.Value.Separator]++;
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
    /// <param name="maxTokens">The maximum number of tokens to read.</param>
    public void PushLevel(char separator, uint? maxTokens = default)
    {
        _currentTokenRead = false;
        _currentLevel = new EnumeratorLevel(_currentLevel, separator, maxTokens)
        {
            ReachedEnd = _currentLevel.ReachedEnd
        };

        if (!_numberOfSeparators.ContainsKey(separator))
        {
            _numberOfSeparators.Add(separator, 0);
        }

        _numberOfSeparators[separator]++;
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

        _numberOfSeparators[_currentLevel.Separator]--;
        _currentLevel = _currentLevel.Parent;
        return Result.FromSuccess();
    }

    /// <summary>
    /// Skip the given amount of characters.
    /// </summary>
    /// <param name="count">The count of characters to skip.</param>
    public void Skip(int count)
    {
        _cursor += count;
    }

    /// <summary>
    /// Get the next token.
    /// </summary>
    /// <param name="packetToken">The resulting token.</param>
    /// <param name="seek">Whether to seek the cursor to the end of the token.</param>
    /// <returns>The found token.</returns>
    public Result GetNextToken(out PacketToken packetToken, bool seek = true)
    {
        // The token is cached if seek was false to speed things up.
        if (_currentTokenRead)
        {
            var cachedToken = _currentToken;
            if (seek)
            {
                UpdateCurrentAndParentLevels(cachedToken);
                _currentLevel.TokensRead++;
                _currentTokenRead = false;
                _cursor += cachedToken.Token.Length + 1;
                _currentLevel.SeparatorOnce = null;
            }

            packetToken = new PacketToken(default, default, default, default);
            packetToken = cachedToken;
            return Result.FromSuccess();
        }

        if ((_cursor >= _data.Length) || (_currentLevel.ReachedEnd ?? false))
        {
            packetToken = new PacketToken(default, default, default, default);
            return new PacketEndReachedError(_data.ToString(), _currentLevel.ReachedEnd ?? false);
        }

        var currentIndex = _cursor;
        var length = 0;
        var startIndex = currentIndex;
        char currentCharacter = _data[currentIndex];

        bool? isLast, encounteredUpperLevel;

        // If the current character is a separator, stop, else, add it to the builder.
        // If should read to last, then read until isLast is null or true.
        while (!IsSeparator(currentCharacter, out isLast, out encounteredUpperLevel) || (_readToLast && !(isLast ?? true)))
        {
            length++;
            currentIndex++;

            if (currentIndex >= _data.Length)
            {
                isLast = true;
                encounteredUpperLevel = true;
                break;
            }

            currentCharacter = _data[currentIndex];
        }

        _readToLast = false;
        currentIndex++;

        var token = new PacketToken(_data.Slice(startIndex, length), isLast, encounteredUpperLevel, _cursor >= _data.Length);
        if (seek)
        {
            UpdateCurrentAndParentLevels(token);
            _cursor = currentIndex;
            _currentLevel.TokensRead++;
        }
        else
        {
            _currentToken = token;
        }

        packetToken = token;
        return Result.FromSuccess();
    }

    /// <summary>
    /// Update fields that are used in the process.
    /// </summary>
    /// <param name="token">The token.</param>
    private void UpdateCurrentAndParentLevels(PacketToken token)
    {
        // If the token is last in the current level, then set reached end of the current level.
        if (_currentLevel.ReachedEnd != true)
        {
            _currentLevel.ReachedEnd = token.IsLast;
        }

        // IsLast is set if parent separator was encountered. The parent needs to be updated.
        if (_currentLevel.Parent is not null && (token.IsLast ?? false))
        {
            var parent = _currentLevel.Parent;

            // Adding TokensRead is supported only one layer currently.
            parent.TokensRead++; // Add read tokens of the parent, because we encountered its separator.
            if (parent.TokensRead >= parent.MaxTokens)
            {
                parent.ReachedEnd = true;
                _currentLevel.ReachedEnd = true;
            }
            _currentLevel.Parent = parent;
        }

        // Encountered Upper Level is set if the reaached separator is not from neither the current level neither the parent
        if ((token.EncounteredUpperLevel ?? false) && _currentLevel.Parent is not null)
        {
            // Just treat it as last, even though that may be incorrect.
            var parent = _currentLevel.Parent;
            parent.ReachedEnd = true;
            _currentLevel.ReachedEnd = true;
            _currentLevel.Parent = parent;
        }

        // The once separator is always used just once, whatever happens.
        _currentLevel.SeparatorOnce = null;
    }

    /// <summary>
    /// Whether the last token of the current level was read.
    /// </summary>
    /// <returns>Whether the last token was read. Null if cannot determine (ie. there are multiple levels with the same separator.)</returns>
    public bool? IsOnLastToken()
    {
        if (_cursor >= _data.Length)
        {
            return true;
        }

        return _currentLevel.ReachedEnd;
    }

    /// <summary>
    /// Checks whether the current character is a separator.
    /// </summary>
    /// <returns>Whether the current character is a separator.</returns>
    public bool IsOnSeparator()
    {
        return IsSeparator(_data[_cursor], out _, out _);
    }

    /// <summary>
    /// Checks if the given character is a separator.
    /// </summary>
    /// <param name="c">The character to check.</param>
    /// <param name="isLast">Whether the separator indicates last separator in this level. True if numberOfSeparators is exactly one and this is the parent's separator.</param>
    /// <param name="encounteredUpperLevel">Whether higher level than the parent was encountered. That could indicate some kind of an error if this is not the last token.</param>
    /// <returns>Whether the character is a separator.</returns>
    private bool IsSeparator(char c, out bool? isLast, out bool? encounteredUpperLevel)
    {
        isLast = null;
        encounteredUpperLevel = null;

        // Separator once has the highest preference
        if (_currentLevel.SeparatorOnce == c)
        {
            isLast = false;
            return true;
        }

        // The separator is not in any level.
        if (!_numberOfSeparators.ContainsKey(c))
        {
            return false;
        }

        var number = _numberOfSeparators[c];
        if (number == 0)
        { // The separator is not in any level.
            return false;
        }

        // The separator is in one level, we can correctly determine which level it corresponds to.
        // If the number is higher, we cannot know which level the separator corresponds to,
        // thus we have to let encounteredUpperLevel and isLast be null.
        // Typical for arrays that are at the end of packets or of specified length.
        if (number == 1)
        {
            if (_currentLevel.Parent?.Separator == c)
            {
                isLast = true;
                encounteredUpperLevel = false;
            }
            else if (_currentLevel.Separator == c)
            {
                isLast = false;
                encounteredUpperLevel = false;
            }
            else
            {
                encounteredUpperLevel = isLast = true;
            }
        }

        return true;
    }

    private class EnumeratorLevel
    {
        public EnumeratorLevel(EnumeratorLevel? parent, char separator, uint? maxTokens = default)
        {
            Parent = parent;
            Separator = separator;
            SeparatorOnce = null;
            MaxTokens = maxTokens;
            TokensRead = 0;
            ReachedEnd = false;
        }

        public EnumeratorLevel? Parent { get; set; }

        public char Separator { get; }

        public char? SeparatorOnce { get; set; }

        public uint? MaxTokens { get; set; }

        public uint TokensRead { get; set; }

        public bool? ReachedEnd { get; set; }

        public (char Separator, uint? MaxTokens)? PreparedLevel { get; set; }

        public uint? CapturedTokensRead { get; set; }
    }
}