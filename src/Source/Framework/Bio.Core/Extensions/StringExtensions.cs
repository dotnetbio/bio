using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Bio.Util;

namespace Bio.Extensions
{
    /// <summary>
    /// StringExtensions
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// ToMixedInvariant
        /// </summary>
        /// <param name="text">text</param>
        /// <returns>string</returns>
        public static string ToMixedInvariant(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            return char.ToUpperInvariant(text[0]).ToString() + text.Substring(1).ToLowerInvariant();

        }

        /// <summary>
        /// Converts an inbound string to a byte array
        /// </summary>
        /// <returns>Byte array</returns>
        public static byte[] ToByteArray(this string data)
        {
            if (string.IsNullOrEmpty(data))
                return null;

            byte[] rc = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                rc[i] = (byte)data[i];
            }

            return rc;
        }

        /// <summary>
        /// Implement the Contains(ch) method for PCL.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool Contains(this string text, char ch)
        {
            return text.IndexOf(ch) >= 0;
        }

        /// <summary>
        /// Reverse
        /// </summary>
        /// <param name="text">text</param>
        /// <returns>string</returns>
        public static string Reverse(this string text)
        {
            if (String.IsNullOrEmpty(text))
                return text;

            char[] charArray = text.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        /// <summary>
        /// Splits a string, but allows you to protect using, for example, balanced parentheses.
        /// </summary>
        /// <param name="text">String to split</param>
        /// <param name="openParenCharacter">The open paren character</param>
        /// <param name="closeParenCharacter">The close paren character</param>
        /// <param name="removeEmptyItems">If true, the empty string will never by emitted.</param>
        /// <param name="splitCharacters">List of characters on which to split</param>
        /// <returns>Strings between split characters that are not wrapped in protecting parens.</returns>
        public static IEnumerable<string> ProtectedSplit(this string text, char openParenCharacter, char closeParenCharacter, bool removeEmptyItems, params char[] splitCharacters)
        {
            int depth = 0;
            StringBuilder sb = new StringBuilder();
            foreach (char c in text)
            {
                if (c == openParenCharacter)
                {
                    depth++;
                    sb.Append(c);
                }
                else if (c == closeParenCharacter)
                {
                    depth--;
                    sb.Append(c);
                    Helper.CheckCondition(depth >= 0, string.Format(CultureInfo.CurrentCulture, Properties.Resource.UnbalancedParanthesis));
                }
                else if (splitCharacters.Contains(c))
                {
                    if (depth > 0)
                        sb.Append(c);
                    else
                    {
                        if (!removeEmptyItems || sb.Length > 0)
                            yield return sb.ToString();
                        sb = new StringBuilder();
                    }
                }
                else
                    sb.Append(c);
            }

            if (!removeEmptyItems || sb.Length > 0)
                yield return sb.ToString();
        }
    }
}
