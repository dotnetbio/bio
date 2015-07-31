using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using Bio.Extensions;

namespace Bio.IO.Wiggle
{
    /// <summary>
    /// Implementation of wiggle parser with support for fixed/variable step formats.
    /// BED Wiggle is not supported in this implementation as its obsolete.
    /// </summary>
    public class WiggleParser : IParser<WiggleAnnotation>
    {
        /// <summary>
        /// Gets the name of this parser.
        /// </summary>
        public string Name
        {
            get { return Properties.Resource.WiggleName; }
        }

        /// <summary>
        /// Gets a short description of this parser.
        /// </summary>
        public string Description
        {
            get { return Properties.Resource.WiggleParserDescription; }
        }

        /// <summary>
        /// Gets the known file extensions for Wiggle files.
        /// </summary>
        public string SupportedFileTypes
        {
            get { return Properties.Resource.Wiggle_FileExtension; }
        }

        /// <summary>
        /// Parses a list of biological sequence texts from a given stream.
        /// </summary>
        /// <param name="stream">The stream to pull the data from</param>
        /// <returns>The collection of parsed annotations.</returns>
        public IEnumerable<WiggleAnnotation> Parse(Stream stream)
        {
            var annotation = this.ParseOne(stream);
            return annotation == null 
                ? Enumerable.Empty<WiggleAnnotation>() 
                : new[] { annotation };
        }

        /// <summary>
        /// Return the single annotation from the stream.
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>WiggleAnnotation</returns>
        public WiggleAnnotation ParseOne(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            using (var reader = stream.OpenRead())
            {
                return Parse(reader);
            }
        }

        /// <summary>
        /// Parse a wiggle annotation from a stream reader.
        /// </summary>
        /// <param name="reader">Stream to parse.</param>
        /// <returns>WiggleAnnotation object.</returns>
        WiggleAnnotation Parse(StreamReader reader)
        {
            if (reader.EndOfStream)
            {
                return null;
            }

            string line;
            WiggleAnnotation result = ParseHeader(reader);

            if (result.AnnotationType == WiggleAnnotationType.FixedStep)
            {
                var fixedStepValues = new List<float>();
                while ((line = reader.ReadLine()) != null)
                {
                    fixedStepValues.Add(float.Parse(line.Trim(), CultureInfo.InvariantCulture));
                }

                result.SetFixedStepAnnotationData(fixedStepValues.ToArray());
            }
            else
            {
                var variableStepValues = new List<KeyValuePair<long, float>>();
                try
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }

                        string[] keyValue = line.Split(' ', '\t');
                        variableStepValues.Add(new KeyValuePair<long, float>(long.Parse(keyValue[0], CultureInfo.InvariantCulture), float.Parse(keyValue[1], CultureInfo.InvariantCulture)));
                    }
                }
                catch
                {
                    throw new Exception(Properties.Resource.WiggleBadInputInFile);
                }

                result.SetVariableStepAnnotationData(variableStepValues.ToArray());
            }

            return result;
        }

        /// <summary>
        /// Parse wiggle header including track line and metadata.
        /// </summary>
        /// <param name="reader">Stream reader to parse.</param>
        /// <returns>Wiggle annotation object initialized with data from the header.</returns>
        private static WiggleAnnotation ParseHeader(StreamReader reader)
        {
            WiggleAnnotation result = new WiggleAnnotation();

            string line = reader.ReadLine();
            
            // read comments
            while (line != null && (line.StartsWith(WiggleSchema.CommentLineStart, StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(line)))
            {
                result.Comments.Add(line);
                line = reader.ReadLine();
            }

            if (line == null || !line.StartsWith(WiggleSchema.Track + " " , StringComparison.Ordinal))
            {
                throw new FormatException(Properties.Resource.WiggleInvalidHeader);
            }

            try
            {
                result.Metadata = ExtractMetadata(line.Substring((WiggleSchema.Track + " ").Length));
            }
            catch
            {
                throw new FormatException(Properties.Resource.WiggleInvalidHeader);
            }

            // step and span details
            line = reader.ReadLine();
            if (line.StartsWith(WiggleSchema.FixedStep + " ", StringComparison.Ordinal))
            {
                result.AnnotationType = WiggleAnnotationType.FixedStep;
            }
            else if (line.StartsWith(WiggleSchema.VariableStep + " ", StringComparison.Ordinal))
            {
                result.AnnotationType = WiggleAnnotationType.VariableStep;
            }
            else
            {
                throw new FormatException(Properties.Resource.WiggleInvalidHeader);
            }

            string[] tokens = line.Split(' ');
            Dictionary<string, string> encodingDetails = new Dictionary<string, string>();
            for (int i = 1; i < tokens.Length; i++)
            {
                string[] metadataArray = tokens[i].Split('=');
                encodingDetails.Add(metadataArray[0], metadataArray[1]);
            }

            try
            {
                result.Chromosome = encodingDetails[WiggleSchema.Chrom];
                string spanString;
                if (encodingDetails.TryGetValue(WiggleSchema.Span, out spanString))
                {
                    result.Span = int.Parse(spanString, CultureInfo.InvariantCulture);
                }

                if (result.AnnotationType == WiggleAnnotationType.FixedStep)
                {
                    result.Step = int.Parse(encodingDetails[WiggleSchema.Step], CultureInfo.InvariantCulture);
                    result.BasePosition = long.Parse(encodingDetails[WiggleSchema.Start], CultureInfo.InvariantCulture);
                }
            }
            catch
            {
                throw new FormatException(Properties.Resource.WiggleInvalidHeader);
            }

            return result;
        }

        /// <summary>
        /// Reads the track line and converts to key value pairs.
        /// </summary>
        /// <param name="trackLine">Track line.</param>
        /// <returns>Track line data in key-value format.</returns>
        private static Dictionary<string, string> ExtractMetadata(string trackLine)
        {
            List<string> tokens = trackLine.Split(' ').ToList();
            int i = 0;

            // Values might be enclosed in double-quotes to support spaces in values. 
            // (space is a delimited if not inside double-quotes.)
            while (i < tokens.Count)
            {
                if (!tokens[i].Contains("="))
                {
                    tokens[i - 1] += " " + tokens[i];
                    tokens.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            // Remove quotes from values and add to result.
            return tokens
                .Select(token => token.Split('='))
                .ToDictionary(metadataArray => metadataArray[0], metadataArray => metadataArray[1]
                .Replace("\"", string.Empty));
        }
    }
}
