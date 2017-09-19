using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Bio.Util.ArgumentParser
{
    /// <summary>
    /// HelpException
    /// </summary>
    public class HelpException : Exception
    {
        /// <summary>
        /// Raw Message.
        /// </summary>
        public string RawMessage { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">The Message.</param>
        /// <param name="includeDateStamp">Include Date Stamp Optional.</param>
       public HelpException(string message, bool includeDateStamp)
            : base(FormatMessage(message, includeDateStamp))
        {
            RawMessage = message;
        }

        /// <summary>
       /// Constructor.
        /// </summary>
        /// <param name="messageFormat">The Message Format.</param>
        /// <param name="args">Argument list.</param>
        public HelpException(string messageFormat, params object[] args) : this(string.Format(messageFormat, args), true) { }

        /// <summary>
        /// Format Message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="includeDateStamp">Include Date Stamp Optional.</param>
        /// <returns>Formatted Message.</returns>
        private static string FormatMessage(string message, bool includeDateStamp)
        {
            int windowWidth =
#if !SILVERLIGHT
                Console.BufferWidth;
#else
                80;
#endif
            int indentWidth = 5;
            string indentString = Enumerable.Repeat(' ', 5).StringJoin("");

            StringBuilder result = new StringBuilder();

            StringBuilder line = new StringBuilder(windowWidth);
            int indents = 0;

            foreach (string word in Tokens(message))
            {
                switch (word)
                {
                    case "<br>":
                        result.AppendLine(line.ToString());
                        line = new StringBuilder(windowWidth);
                        line.Append(IndentString(indents, indentWidth));
                        break;
                    case "<indent>":
                        indents++;
                        line.Append(IndentString(1, indentWidth));  // add one indent to the current line.
                        break;
                    case "</indent>":
                        indents--; break;
                    default:
                        if (line.Length + word.Length + 1 < windowWidth)
                            line.Append(word + " ");
                        else
                        {
                            result.AppendLine(line.ToString());
                            line = new StringBuilder(windowWidth);
                            line.Append(IndentString(indents, indentWidth) + word + " ");
                        }
                        break;
                }
            }
            result.AppendLine(line.ToString());
            if (includeDateStamp)
            {
                result.Append(GetDateCompiledString());
            }

            return result.ToString();
        }

        /// <summary>
        /// Get Date Compiled String.
        /// </summary>
        /// <returns>DateCompiledString.</returns>
        private static string GetDateCompiledString()
        {
            DateTime compileDate = SpecialFunctions.DateProgramWasCompiled();
            if (compileDate.Ticks > 0)
                return "Program last modified " + compileDate;
            else
                return "";
        }

        /// <summary>
        /// Indent String.
        /// </summary>
        /// <param name="indents">Indents size.</param>
        /// <param name="indentWidth">Indent width.</param>
        /// <returns>Indent String.</returns>
        private static string IndentString(int indents, int indentWidth)
        {
            return Enumerable.Repeat(' ', indents * indentWidth).StringJoin("");
        }

        /// <summary>
        /// Tokens inside passed string.
        /// </summary>
        /// <param name="str">String passed.</param>
        /// <returns>List of tokens inside string.</returns>
        private static IEnumerable<string> Tokens(string str)
        {
            str = str.Replace("\n\n", "<br><br>");
            string wordBeforeTag = "";
            StringBuilder word = new StringBuilder();

            int tagDepth = 0;
            foreach (var c in str)
            {
                if (c == '<')
                {
                    tagDepth++;
                    if (word.Length > 0 && tagDepth == 1 /*is start of new tag*/)
                    {
                        wordBeforeTag = word.ToString();
                        word = new StringBuilder();
                    }
                    word.Append(c);
                }
                else if (tagDepth > 0)
                {
                    word.Append(c);
                    if (c == '>')
                    {
                        tagDepth--;
                        if (tagDepth == 0)
                        {
                            string tag = word.ToString();
                            if (KnownTag(tag))
                            {
                                if (wordBeforeTag.Length > 0)
                                    yield return wordBeforeTag;
                                yield return tag.ToLower();
                            }
                            else
                            {
                                yield return wordBeforeTag + tag;
                            }
                            wordBeforeTag = "";
                            word = new StringBuilder();
                        }
                    }
                }
                else if (Char.IsWhiteSpace(c))
                {
                    if (word.Length > 0)
                    {
                        yield return word.ToString();
                        word = new StringBuilder();
                    }
                }
                else
                {
                    word.Append(c);
                }
            }
            if (word.Length > 0)
                yield return word.ToString();
        }

        /// <summary>
        /// Known Tag.
        /// </summary>
        /// <param name="tag">Tag passed.</param>
        /// <returns>True if found.</returns>
        private static bool KnownTag(string tag)
        {
            return tag.Equals("<indent>", StringComparison.CurrentCultureIgnoreCase) ||
                tag.Equals("</indent>", StringComparison.CurrentCultureIgnoreCase) ||
                tag.Equals("<br>", StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
