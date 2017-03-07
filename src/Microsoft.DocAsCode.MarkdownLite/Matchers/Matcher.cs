﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.DocAsCode.MarkdownLite.Matchers
{
    using System;

    public abstract class Matcher
    {
        public const int NotMatch = -1;

        private static readonly AnyCharMatcher AnyCharMatcher = new AnyCharMatcher();
        private static readonly EndOfStringMatcher EndOfStringMatcher = new EndOfStringMatcher();

        /// <summary>
        /// Match string in content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>Char count of match, <c>-1</c> is not match.</returns>
        public abstract int Match(MatchContent content);

        public static Matcher Char(char ch)
        {
            return new CharMatcher(ch);
        }

        public static Matcher AnyChar() => AnyCharMatcher;

        public static Matcher AnyCharIn(params char[] ch)
        {
            if (ch == null)
            {
                throw new ArgumentNullException(nameof(ch));
            }
            var array = (char[])ch.Clone();
            Array.Sort(array);
            return new AnyCharInMatcher(ch);
        }

        public static Matcher AnyCharInRange(char start, char end)
        {
            if (start >= end)
            {
                throw new ArgumentException(nameof(end), $"Should be greater than {start.ToString()}.");
            }
            return new AnyCharInRangeMatcher(start, end);
        }

        public static Matcher AnyCharNotIn(params char[] ch)
        {
            if (ch == null)
            {
                throw new ArgumentNullException(nameof(ch));
            }
            var array = (char[])ch.Clone();
            Array.Sort(array);
            return new AnyCharNotInMatcher(ch);
        }

        public static Matcher String(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }
            if (text.Length == 0)
            {
                throw new ArgumentException("Cannot be empty.", nameof(text));
            }
            return new StringMatcher(text);
        }

        public static Matcher EndOfString() => EndOfStringMatcher;

        public static Matcher Maybe(Matcher matcher) =>
            Repeat(matcher, 0, 1);

        public static Matcher Repeat(Matcher matcher, int minOccur) =>
            Repeat(matcher, minOccur, int.MaxValue);

        public static Matcher Repeat(Matcher matcher, int minOccur, int maxOccur)
        {
            if (matcher == null)
            {
                throw new ArgumentNullException(nameof(matcher));
            }
            if (minOccur < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minOccur), "Should be greater than or equals 0.");
            }
            if (minOccur > maxOccur)
            {
                throw new ArgumentOutOfRangeException(nameof(maxOccur), "Should be greater than or equals minOccur.");
            }
            return new RepeatMatcher(matcher, minOccur, maxOccur);
        }

        public static Matcher Any(params Matcher[] matchers)
        {
            ValidateMatcherArray(matchers);
            return new AnyMatcher(matchers);
        }

        public static Matcher Sequence(params Matcher[] matchers)
        {
            ValidateMatcherArray(matchers);
            return new SequenceMatcher(matchers);
        }

        public static Matcher Test(params Matcher[] matchers)
        {
            ValidateMatcherArray(matchers);
            return new TestMatcher(matchers, false);
        }

        public static Matcher NegativeTest(params Matcher[] matchers)
        {
            ValidateMatcherArray(matchers);
            return new TestMatcher(matchers, true);
        }

        public static Matcher ReverseTest(params Matcher[] matchers)
        {
            ValidateMatcherArray(matchers);
            return new ReverseMatcher(new TestMatcher(matchers, false));
        }

        public static Matcher ReverseNegativeTest(params Matcher[] matchers)
        {
            ValidateMatcherArray(matchers);
            return new ReverseMatcher(new TestMatcher(matchers, true));
        }

        private static void ValidateMatcherArray(Matcher[] matchers)
        {
            if (matchers == null)
            {
                throw new ArgumentNullException(nameof(matchers));
            }
            if (matchers.Length == 0)
            {
                throw new ArgumentException("Cannot be zero length.", nameof(matchers));
            }
            foreach (var m in matchers)
            {
                if (m == null)
                {
                    throw new ArgumentException("Cannot contain null.", nameof(matchers));
                }
            }
        }
    }
}
