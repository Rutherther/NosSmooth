//
//  StringExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Cryptography.Extensions;

/// <summary>
/// Extension methods for string.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Split a string into lines without allocations.
    /// </summary>
    /// <param name="str">The string to split.</param>
    /// <returns>An enumerator with lines.</returns>
    public static LineSplitEnumerator SplitLines(this string str)
    {
        // LineSplitEnumerator is a struct so there is no allocation here
        return new LineSplitEnumerator(str.AsSpan());
    }

    /// <summary>
    /// Split a string without any allocation.
    /// </summary>
    /// <param name="str">The string to split.</param>
    /// <param name="split">The char to split with.</param>
    /// <returns>The split enumerator.</returns>
    public static SplitEnumerator SplitAllocationless(this string str, char split)
    {
        return new SplitEnumerator(str.AsSpan(), split);
    }

    /// <summary>
    /// An enumerator of a string split.
    /// </summary>
    public ref struct SplitEnumerator
    {
        private readonly char _split;
        private ReadOnlySpan<char> _str;

        /// <summary>
        /// Initializes a new instance of the <see cref="SplitEnumerator"/> struct.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="split">The split char.</param>
        public SplitEnumerator(ReadOnlySpan<char> str, char split)
        {
            _str = str;
            _split = split;
            Current = default;
        }

        /// <summary>
        /// Gets this enumerator.
        /// </summary>
        /// <returns>This.</returns>
        public SplitEnumerator GetEnumerator()
            => this;

        /// <summary>
        /// Move to next line.
        /// </summary>
        /// <returns>Whether move was successful.</returns>
        public bool MoveNext()
        {
            var span = _str;
            if (span.Length == 0)
            {
                return false;
            }

            var index = span.IndexOf(_split);
            if (index == -1)
            {
                _str = ReadOnlySpan<char>.Empty;
                Current = span;
                return true;
            }

            Current = span.Slice(0, index);
            _str = span.Slice(index + 1);
            return true;
        }

        /// <summary>
        /// Current line.
        /// </summary>
        public ReadOnlySpan<char> Current { get; private set; }
    }

    /// <summary>
    /// An enumerator of a string lines.
    /// </summary>
    public ref struct LineSplitEnumerator
    {
        private ReadOnlySpan<char> _str;

        /// <summary>
        /// Initializes a new instance of the <see cref="LineSplitEnumerator"/> struct.
        /// </summary>
        /// <param name="str">The string.</param>
        public LineSplitEnumerator(ReadOnlySpan<char> str)
        {
            _str = str;
            Current = default;
        }

        /// <summary>
        /// Gets this enumerator.
        /// </summary>
        /// <returns>This.</returns>
        public LineSplitEnumerator GetEnumerator()
            => this;

        /// <summary>
        /// Move to next line.
        /// </summary>
        /// <returns>Whether move was successful.</returns>
        public bool MoveNext()
        {
            var span = _str;
            if (span.Length == 0)
            {
                return false;
            }

            var index = span.IndexOfAny('\r', '\n');
            if (index == -1)
            {
                _str = ReadOnlySpan<char>.Empty;
                Current = new LineSplitEntry(span, ReadOnlySpan<char>.Empty);
                return true;
            }

            if (index < span.Length - 1 && span[index] == '\r')
            {
                var next = span[index + 1];
                if (next == '\n')
                {
                    Current = new LineSplitEntry(span.Slice(0, index), span.Slice(index, 2));
                    _str = span.Slice(index + 2);
                    return true;
                }
            }

            Current = new LineSplitEntry(span.Slice(0, index), span.Slice(index, 1));
            _str = span.Slice(index + 1);
            return true;
        }

        /// <summary>
        /// Current line.
        /// </summary>
        public LineSplitEntry Current { get; private set; }
    }

    /// <summary>
    /// A line.
    /// </summary>
    public readonly ref struct LineSplitEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LineSplitEntry"/> struct.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="separator">The line separator.</param>
        public LineSplitEntry(ReadOnlySpan<char> line, ReadOnlySpan<char> separator)
        {
            Line = line;
            Separator = separator;
        }

        /// <summary>
        /// Gets the line.
        /// </summary>
        public ReadOnlySpan<char> Line { get; }

        /// <summary>
        /// Gets the separator of the line.
        /// </summary>
        public ReadOnlySpan<char> Separator { get; }

        /// <summary>
        /// This method allow to deconstruct the type, so you can write any of the following code
        /// foreach (var entry in str.SplitLines()) { _ = entry.Line; }
        /// foreach (var (line, endOfLine) in str.SplitLines()) { _ = line; }
        /// https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/functional/deconstruct?WT.mc_id=DT-MVP-5003978#deconstructing-user-defined-types.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="separator">The line separator.</param>
        public void Deconstruct(out ReadOnlySpan<char> line, out ReadOnlySpan<char> separator)
        {
            line = Line;
            separator = Separator;
        }

        /// <summary>
        /// An implicit cast to ReadOnySpan.
        /// </summary>
        /// <param name="entry">The entry to cast.</param>
        /// <returns>The read only span of the entry.</returns>
        public static implicit operator ReadOnlySpan<char>(LineSplitEntry entry)
            => entry.Line;
    }
}