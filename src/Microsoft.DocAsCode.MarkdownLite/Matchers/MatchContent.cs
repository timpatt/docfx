﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.DocAsCode.MarkdownLite.Matchers
{
    using System;

    public struct MatchContent
    {
        public readonly string Text;
        public readonly int StartIndex;
        public readonly bool IsForward;

        public MatchContent(string text, int startIndex, bool isForward = true)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }
            if (startIndex < 0 || startIndex > text.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Out of range.");
            }
            Text = text;
            StartIndex = startIndex;
            IsForward = isForward;
        }

        public char this[int offset] => Text[GetIndex(offset)];

        public bool Bos() => IsForward ? StartIndex == 0 : StartIndex == Text.Length;

        public bool Eos() => IsForward ? StartIndex == Text.Length : StartIndex == 0;

        public bool TestLength(int length)
        {
            int result = GetIndexNoThrow(length);
            return result >= 0 && result <= Text.Length;
        }

        public MatchContent Offset(int offset) => new MatchContent(Text, GetIndex(offset), IsForward);

        public MatchContent Reverse() => new MatchContent(Text, StartIndex, !IsForward);

        private int GetIndex(int offset)
        {
            int result = GetIndexNoThrow(offset);
            if (!IsForward)
            {
                result--;
            }
            if (result < 0 || result > Text.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }
            return result;
        }

        private int GetIndexNoThrow(int offset) =>
            IsForward ? StartIndex + offset : StartIndex - offset;
    }
}
