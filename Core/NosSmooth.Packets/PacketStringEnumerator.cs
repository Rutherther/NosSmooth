//
//  PacketStringEnumerator.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Text;
using NosSmooth.Packets.Errors;
using Remora.Results;

namespace NosSmooth.Packets;

/// <summary>
/// Enumerator for packet strings.
/// </summary>
public struct PacketStringEnumerator
{
    private readonly EnumeratorData _data;
    private readonly Dictionary<char, ushort> _numberOfSeparators;
    private EnumeratorLevel _currentLevel;
    private (char Separator, uint? MaxTokens)? _preparedLevel;
    private PacketToken? _currentToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="PacketStringEnumerator"/> struct.
    /// </summary>
    /// <param name="data">The packet string data.</param>
    /// <param name="separator">The separator to use on the highest level.</param>
    public PacketStringEnumerator(string data, char separator = ' ')
    {
        _currentLevel = new EnumeratorLevel(null, separator);
        _data = new EnumeratorData(data);
        _numberOfSeparators = new Dictionary<char, ushort>();
        _numberOfSeparators.Add(separator, 1);
        _currentToken = null;
        _preparedLevel = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PacketStringEnumerator"/> struct.
    /// </summary>
    /// <param name="data">The data of the enumerator.</param>
    /// <param name="level">The current enumerator level.</param>
    /// <param name="numberOfSeparators">The number of separators.</param>
    private PacketStringEnumerator(EnumeratorData data, EnumeratorLevel level, Dictionary<char, ushort> numberOfSeparators)
    {
        _currentLevel = level;
        _data = data;

        // TODO: use something less heavy than copying everything from the dictionary.
        _numberOfSeparators = new Dictionary<char, ushort>(numberOfSeparators);
        _currentToken = null;
        _preparedLevel = null;
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
        _currentToken = null;
        _currentLevel.SeparatorOnce = separator;
    }

    /// <summary>
    /// Prepare the given level that can be set when needed.
    /// </summary>
    /// <param name="separator">The separator of the prepared level.</param>
    /// <param name="maxTokens">The maximum number of tokens for the level.</param>
    public void PrepareLevel(char separator, uint? maxTokens = null)
    {
        _preparedLevel = (separator, maxTokens);
    }

    /// <summary>
    /// Reset the prepared level, if there is any.
    /// </summary>
    public void RemovePreparedLevel()
    {
        _preparedLevel = null;
    }

    /// <summary>
    /// Create next level with the separator given in the prepared level.
    /// </summary>
    /// <remarks>
    /// Level of the current enumerator will stay the same.
    /// Will return null, if there is not a level prepared.
    /// </remarks>
    /// <returns>An enumerator with the new level pushed.</returns>
    public PacketStringEnumerator? CreatePreparedLevel()
    {
        return _preparedLevel is not null ? CreateLevel(_preparedLevel.Value.Separator, _preparedLevel.Value.MaxTokens) : null;
    }

    /// <summary>
    /// Push next level with the separator given in the prepared level.
    /// </summary>
    /// <returns>Whether there is a prepared level present.</returns>
    public bool PushPreparedLevel()
    {
        if (_preparedLevel is null)
        {
            return false;
        }

        _currentToken = null;
        _currentLevel = new EnumeratorLevel(_currentLevel, _preparedLevel.Value.Separator, _preparedLevel.Value.MaxTokens)
        {
            ReachedEnd = _currentLevel.ReachedEnd
        };

        if (!_numberOfSeparators.ContainsKey(_preparedLevel.Value.Separator))
        {
            _numberOfSeparators.Add(_preparedLevel.Value.Separator, 0);
        }

        _numberOfSeparators[_preparedLevel.Value.Separator]++;
        return true;
    }

    /// <summary>
    /// Create next level with the given separator and maximum number of tokens.
    /// </summary>
    /// <remarks>
    /// Level of the current enumerator will stay the same.
    /// The maximum number of tokens indicates how many tokens can be read ie. in lists,
    /// the enumerator won't allow reading more than that many tokens, error will be thrown if the user tries to read more.
    /// </remarks>
    /// <param name="separator">The separator of the new level.</param>
    /// <param name="maxTokens">The maximum number of tokens to read.</param>
    /// <returns>An enumerator with the new level pushed.</returns>
    public PacketStringEnumerator CreateLevel(char separator, uint? maxTokens = default)
    {
        _currentToken = null;
        var stringEnumerator = new PacketStringEnumerator(_data, _currentLevel, _numberOfSeparators);
        stringEnumerator.PushLevel(separator, maxTokens);
        return stringEnumerator;
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
        _preparedLevel = null;
        _currentToken = null;
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

        _preparedLevel = null;
        _numberOfSeparators[_currentLevel.Separator]--;
        _currentLevel = _currentLevel.Parent;
        return Result.FromSuccess();
    }

    /// <summary>
    /// Get the next token.
    /// </summary>
    /// <param name="seek">Whether to seek the cursor to the end of the token.</param>
    /// <returns>The found token.</returns>
    public Result<PacketToken> GetNextToken(bool seek = true)
    {
        // The token is cached if seek was false to speed things up.
        if (_currentToken != null)
        {
            var cachedToken = _currentToken.Value;
            if (seek)
            {
                UpdateCurrentAndParentLevels(cachedToken);
                _currentLevel.TokensRead++;
                _currentToken = null;
                _data.Cursor += cachedToken.Token.Length + 1;
                _currentLevel.SeparatorOnce = null;
            }

            return cachedToken;
        }

        if (_data.ReachedEnd || _currentLevel.ReachedEnd)
        {
            return new PacketEndReachedError(_data.Data, _currentLevel.ReachedEnd);
        }

        var currentIndex = _data.Cursor;
        char currentCharacter = _data.Data[currentIndex];
        StringBuilder tokenString = new StringBuilder();

        bool? isLast, encounteredUpperLevel;

        // If the current character is a separator, stop, else, add it to the builder.
        while (!IsSeparator(currentCharacter, out isLast, out encounteredUpperLevel))
        {
            tokenString.Append(currentCharacter);
            currentIndex++;

            if (currentIndex == _data.Data.Length)
            {
                isLast = true;
                encounteredUpperLevel = true;
                break;
            }

            currentCharacter = _data.Data[currentIndex];
        }

        currentIndex++;

        var token = new PacketToken(tokenString.ToString(), isLast, encounteredUpperLevel, _data.ReachedEnd);
        if (seek)
        {
            UpdateCurrentAndParentLevels(token);
            _data.Cursor = currentIndex;
            _currentLevel.TokensRead++;
        }
        else
        {
            _currentToken = token;
        }

        return token;
    }

    /// <summary>
    /// Update fields that are used in the process.
    /// </summary>
    /// <param name="token">The token.</param>
    private void UpdateCurrentAndParentLevels(PacketToken token)
    {
        // If the token is last in the current level, then set reached end of the current level.
        if (token.IsLast ?? false)
        {
            _currentLevel.ReachedEnd = true;
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
        => _data.ReachedEnd || _currentLevel.ReachedEnd;

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

    private class EnumeratorData
    {
        public EnumeratorData(string data)
        {
            Data = data;
            Cursor = 0;
        }

        public string Data { get; }

        public int Cursor { get; set; }

        public bool ReachedEnd => Cursor >= Data.Length;
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

        public bool ReachedEnd { get; set; }
    }
}