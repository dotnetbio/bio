// *********************************************************
//Copyright (c) 2004-2006 Jaroslaw Kowalski <jaak@jkowalski.net>
 
//All rights reserved.
 
//Redistribution and use in source and binary forms, with or without
//modification, are permitted provided that the following conditions
//are met:
 
//* Redistributions of source code must retain the above copyright notice,
//  this list of conditions and the following disclaimer.
 
//* Redistributions in binary form must reproduce the above copyright notice,
//  this list of conditions and the following disclaimer in the documentation
//  and/or other materials provided with the distribution.
 
//* Neither the name of Jaroslaw Kowalski nor the names of its
//  contributors may be used to endorse or promote products derived from this
//  software without specific prior written permission.
 
//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
//AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
//IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
//ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
//LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
//CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
//SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
//INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
//CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
//ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF
//THE POSSIBILITY OF SUCH DAMAGE.

// 
// *********************************************************
using System;
using System.IO;
using System.Linq;

namespace Bio.Util
{
    /// <summary>
    /// Provides methods for encoding URLs when processing Web requests.
    /// </summary>
    public static class HttpUtility
    {
        #region Private Constants
        /// <summary>
        /// Holds hexa characters.
        /// </summary>
        private static char[] hexChars = "0123456789abcdef".ToCharArray();

        /// <summary>
        /// Holds nonencoded characters.
        /// </summary>
        private const string notEncodedChars = "!'()*-._";
        #endregion

        #region Public Methods
        /// <summary>
        /// Encodes a URL string.
        /// </summary>
        /// <param name="str">The text to encode.</param>
        /// <returns>An encoded string.</returns>
        public static string UrlEncode(string str)
        {
            return UrlEncode(str, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// Encodes a URL string using the specified encoding object.
        /// </summary>
        /// <param name="str">The text to encode.</param>
        /// <param name="enc">The System.Text.Encoding object that specifies the encoding scheme.</param>
        /// <returns>An encoded string.</returns>
        public static string UrlEncode(string str, System.Text.Encoding enc)
        {
            if (enc == null)
            {
                throw new ArgumentNullException("enc");
            }

            if(string.IsNullOrEmpty(str))
            {
                return str;
            }

            byte[] bytes = enc.GetBytes(str);
            return new string(UrlEncodeToBytes(bytes, 0, bytes.Length).Select(b => (char)b).ToArray());
        }
        #endregion

        #region Private Methods
        
        // encodes the specified bytes array.
        private static byte[] UrlEncodeToBytes(byte[] bytes, int offset, int count)
        {
            if (bytes == null)
                return null;

            int len = bytes.Length;
            if (len == 0)
                return new byte[0];

            if (offset < 0 || offset >= len)
                throw new ArgumentOutOfRangeException("offset");

            if (count < 0 || count > len - offset)
                throw new ArgumentOutOfRangeException("count");

            using (MemoryStream result = new MemoryStream(count))
            {
                int end = offset + count;
                for (int i = offset; i < end; i++)
                    UrlEncodeChar((char)bytes[i], result, false);

                return result.ToArray();
            }
        }

        // Encodes specified char and stores the result in specified stream.
        private static void UrlEncodeChar(char ch, Stream result, bool isUnicode)
        {
            if (ch > 255)
            {
                if (!isUnicode)
                    throw new ArgumentOutOfRangeException("ch", ch, Properties.Resource.ParamCHmustbeLessThan256);
                int idx;
                int i = (int)ch;

                result.WriteByte((byte)'%');
                result.WriteByte((byte)'u');
                idx = i >> 12;
                result.WriteByte((byte)hexChars[idx]);
                idx = (i >> 8) & 0x0F;
                result.WriteByte((byte)hexChars[idx]);
                idx = (i >> 4) & 0x0F;
                result.WriteByte((byte)hexChars[idx]);
                idx = i & 0x0F;
                result.WriteByte((byte)hexChars[idx]);
                return;
            }

            if (ch > ' ' && notEncodedChars.IndexOf(ch) != -1)
            {
                result.WriteByte((byte)ch);
                return;
            }
            if (ch == ' ')
            {
                result.WriteByte((byte)'+');
                return;
            }
            if ((ch < '0') ||
                (ch < 'A' && ch > '9') ||
                (ch > 'Z' && ch < 'a') ||
                (ch > 'z'))
            {
                if (isUnicode && ch > 127)
                {
                    result.WriteByte((byte)'%');
                    result.WriteByte((byte)'u');
                    result.WriteByte((byte)'0');
                    result.WriteByte((byte)'0');
                }
                else
                    result.WriteByte((byte)'%');

                int idx = ((int)ch) >> 4;
                result.WriteByte((byte)hexChars[idx]);
                idx = ((int)ch) & 0x0F;
                result.WriteByte((byte)hexChars[idx]);
            }
            else
                result.WriteByte((byte)ch);
        }
        #endregion
    }
}
