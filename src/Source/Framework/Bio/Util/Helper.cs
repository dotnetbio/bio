using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Bio.IO.GenBank;
using System.Linq;

#if (SILVERLIGHT == false)
using Bio.Algorithms.Alignment;
#endif

namespace Bio.Util
{
    /// <summary>
    /// Generally useful static methods.
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// The .gz extension to indicate gzipped files
        /// </summary>
        public const string ZippedFileExtension = ".gz";

        /// <summary>
        /// Stores the number of alphabets to show in ToString function of a class.
        /// </summary>
        public const int AlphabetsToShowInToString = 64;

        /// <summary>
        /// Key to get GenBankMetadata object from Metadata of a sequence which is parsed from GenBankParser.
        /// </summary>
        public const string GenBankMetadataKey = "GenBank";

        /// <summary>
        /// Delimitar "!" used to seperate the PairedRead information with other info in the sequence id.
        /// </summary>
        public static char PairedReadDelimiter = '!';

        private const string Space = " ";
        private const string Colon = ":";
        private const string Comma = ",";
        private const string ProjectDBLink = "Project";
        private const string BioProjectDBLink = "BioProject";
        private const string TraceAssemblyArchiveDBLink = "Trace Assembly Archive";
        private const string SegmentDelim = " of ";
        private const string SingleStrand = "ss-";
        private const string DoubleStrand = "ds-";
        private const string MixedStrand = "ms-";
        private const string LinearStrandTopology = "linear";
        private const string CircularStrandTopology = "circular";

        /// <summary>
        /// Key to get SAMAlignmentHeader object from Metadata of a sequence alignment which is parsed from SAMParser.
        /// </summary>
        public const string SAMAlignmentHeaderKey = "SAMAlignmentHeader";

        /// <summary>
        /// Key to get SAMAlignedSequenceHeader object from Metadata of a aligned sequence which is parsed from SAMParser.
        /// </summary>
        public const string SAMAlignedSequenceHeaderKey = "SAMAlignedSequenceHeader";

        private static Random random = new Random();

        /// <summary>
        /// Convert sequence data to string.
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        /// <returns>Returns string for sequence data.</returns>
        public static string ConvertSequenceToString(ISequence sequence)
        {
            return (new string(sequence.Select(a => (char)a).ToArray()));
        }

        /// <summary>
        /// Convert Ienumerable sequence ToList.
        /// </summary>
        /// <param name="enumSeq">Enumerable sequence.</param>
        /// <returns>List of sequence.</returns>
        public static List<ISequence> ConvertIenumerableToList(IEnumerable<ISequence> enumSeq)
        {
            return enumSeq.ToList();
        }

        /// <summary>
        /// Get a range of sequence.
        /// </summary>
        /// <param name="sequence">Original sequence.</param>
        /// <param name="start">Start position.</param>
        /// <param name="length">Length of sequence.</param>
        /// <returns>New sequence with range specified.</returns>
        public static ISequence GetSequenceRange(ISequence sequence, long start, long length)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            if (start < 0 || start >= sequence.Count)
            {
                throw new ArgumentOutOfRangeException(
                    Properties.Resource.ParameterNameStart,
                    Properties.Resource.ParameterMustLessThanCount);
            }

            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "length",
                    Properties.Resource.ParameterMustNonNegative);
            }

            if ((sequence.Count - start) < length)
            {
                throw new ArgumentOutOfRangeException("length");
            }

            byte[] newSeqData = new byte[length];

            long j = 0;
            for (long i = start; i < start + length; i++, j++)
            {
                newSeqData[j] = sequence[i];
            }

            ISequence newSequence = new Sequence(sequence.Alphabet, newSeqData);
            newSequence.ID = sequence.ID;
            return newSequence;
        }

        /// <summary>
        /// Get a Sequence Poly Tail.
        /// </summary>
        /// <param name="sequence">Original sequence.</param>
        /// <param name="start">Start position.</param>
        /// <param name="length">Length of sequence.</param>
        /// <returns>New sequence with range specified.</returns>
        public static ISequence RemoveSequencePolyTail(ISequence sequence, long start, long length)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            if (start < 0 || start >= sequence.Count)
            {
                throw new ArgumentOutOfRangeException(
                    Properties.Resource.ParameterNameStart,
                    Properties.Resource.ParameterMustLessThanCount);
            }

            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "length",
                    Properties.Resource.ParameterMustNonNegative);
            }

            if ((sequence.Count - start) < length)
            {
                throw new ArgumentOutOfRangeException("length");
            }

            byte[] tmpData = new byte[sequence.Count - length];
            for (long i = 0; i < start; i++)
            {
                tmpData[i] = sequence[i];
            }

            ISequence newSequence = new Sequence(sequence.Alphabet, tmpData);
            newSequence.ID = sequence.ID;
            return newSequence;
        }

        /// <summary>
        /// Split sequence for Iron poython.
        /// </summary>
        /// <param name="seq">The sequence.</param>
        /// <param name="coverage">The coverage.</param>
        /// <param name="fragment_length">Fragment Length.</param>
        /// <returns>List of split sequences.</returns>
        public static List<ISequence> SplitSequence(Sequence seq, long coverage, long fragment_length)
        {
            if (seq == null)
            {
                throw new ArgumentNullException("seq");
            }

            long num_fragments = seq.Count * coverage / fragment_length;
            List<ISequence> fragment_list = new List<ISequence>();
            System.Random RandNum = new System.Random();

            for (int j = 0; j < num_fragments; j++)
            {
                long start = RandNum.Next((int)seq.Count - (int)fragment_length + 1);
                ISequence tempSeq = Helper.GetSequenceRange(seq, start, fragment_length);
                byte[] tmpFragment = new byte[tempSeq.Count];
                for (int i = 0; i < tempSeq.Count; i++)
                {
                    tmpFragment[i] = tempSeq[i];
                }

                ISequence tempSequence = new Sequence(seq.Alphabet, tmpFragment);
                tempSequence.ID = seq.ID + " (Split " + "i" + ")";
                fragment_list.Add(tempSequence);
            }

            return fragment_list;
        }

        /// <summary>
        /// Get reverse complement of sequence string.
        /// Handles only unambiguous DNA sequence strings.
        /// Note: This method is a light-weight implementation of sequence.ReverseComplement.
        /// This only works for unambiguous DNA sequences, which is characteristic of the input for de-novo.
        /// </summary>
        /// <param name="sequence">Sequence string.</param>
        /// <param name="reverseComplementBuilder">String builder for building reverse complement.</param>
        /// <returns>Reverse Complement sequence string.</returns>
        public static string GetReverseComplement(this string sequence, char[] reverseComplementBuilder)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            if (reverseComplementBuilder == null)
            {
                throw new ArgumentNullException("reverseComplementBuilder");
            }

            if (sequence.Length != reverseComplementBuilder.Length)
            {
                throw new ArgumentException(Properties.Resource.BuilderIncorrectLength);
            }

            for (int i = sequence.Length - 1; i >= 0; i--)
            {
                char rc;
                switch (sequence[i])
                {
                    case 'A':
                    case 'a':
                        rc = 'T';
                        break;
                    case 'T':
                    case 't':
                        rc = 'A';
                        break;
                    case 'G':
                    case 'g':
                        rc = 'C';
                        break;
                    case 'C':
                    case 'c':
                        rc = 'G';
                        break;
                    default:
                        throw new ArgumentException(string.Format(
                            CultureInfo.CurrentCulture, Properties.Resource.InvalidSymbol, sequence[i]));
                }

                reverseComplementBuilder[sequence.Length - 1 - i] = rc;
            }

            return new string(reverseComplementBuilder);
        }

        /// <summary>
        /// Identifies if a file extension is a
        /// valid extension for FASTA formats.
        /// </summary>
        /// <returns>
        /// True  : if it is a valid fasta file extension.
        /// False : if it is a in-valid fasta file extension.
        /// </returns>
        public static bool IsFasta(string fileName)
        {

            bool isfasta = false;
            int extensionDelimiter = fileName.LastIndexOf('.');
            if (-1 < extensionDelimiter)
            {
                string fileExtension = fileName.Substring(extensionDelimiter);
                string fastaExtensions = Properties.Resource.FASTA_FILEEXTENSION;
                string[] extensions = fastaExtensions.Split(',');
                foreach (string extension in extensions)
                {
                    if (fileExtension.Equals(extension, StringComparison.CurrentCultureIgnoreCase))
                    {
                        isfasta = true;
                        break;
                    }
                }
            }

            return isfasta;
        }

		/// <summary>
		/// Identifies if a file extension is a
		/// valid extension for FASTA formats that is gzipped.
		/// </summary>
		/// <returns>
		/// True  : if it is a valid gzipped fasta file extension.
		/// False : if it is a in-valid gzipped fasta file extension.
		/// </returns>
		public static bool IsZippedFasta(string fileName)
		{
			if(FileEndsWithZippedExtension(fileName))
			{
				fileName = fileName.Substring(0,fileName.Length-ZippedFileExtension.Length);
                return IsFasta (fileName);
			}
			return false;
		}
        /// <summary>
        /// Identifies if a file extension is a
        /// valid extension for a gzipped FastQ formats. 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
		public static bool IsZippedFastQ(string fileName)
		{
			if(FileEndsWithZippedExtension(fileName))
			{
                fileName = fileName.Substring(0, fileName.Length - ZippedFileExtension.Length);
				return IsFastQ(fileName);
			}
			return false;
		}

		/// <summary>
		/// Determine if file ends with extension ".gz"
		/// </summary>
		/// <returns><c>true</c>, if file name ends with zipped extension <c>false</c> otherwise.</returns>
		/// <param name="fileName">File name.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.EndsWith(System.String)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public static bool FileEndsWithZippedExtension(string fileName)
		{
			return fileName.EndsWith (ZippedFileExtension);
		}

        /// <summary>
        /// Identifies if a file extension is a
        /// valid extension for FastQ formats.
        /// </summary>
        /// <returns>
        /// True  : if it is a valid fastq file extension.
        /// False : if it is a in-valid fastq file extension.
        /// </returns>
        public static bool IsFastQ(string fileName)
        {
            bool isfastQ = false;
            int extensionDelimiter = fileName.LastIndexOf('.');
            if (-1 < extensionDelimiter)
            {
                string fileExtension = fileName.Substring(extensionDelimiter);
                string fastQExtensions = Properties.Resource.FASTQ_FILEEXTENSION;
                string[] extensions = fastQExtensions.Split(',');
                foreach (string extension in extensions)
                {
                    if (fileExtension.Equals(extension, StringComparison.CurrentCultureIgnoreCase))
                    {
                        isfastQ = true;
                        break;
                    }
                }
            }

            return isfastQ;
        }

        /// <summary>
        /// Identifies if a file extension is a
        /// valid extension for GenBank formats.
        /// </summary>
        /// <returns>
        /// true  : if it is a valid GenBank file extension.
        /// false : if it is a in-valid GenBank file extension.
        /// </returns>
        public static bool IsGenBank(string fileName)
        {
            bool isGenBank = false;
            int extensionDelimiter = fileName.LastIndexOf('.');
            if (-1 < extensionDelimiter)
            {
                string fileExtension = fileName.Substring(extensionDelimiter);
                string genBankExtensions = Properties.Resource.GENBANK_FILEEXTENSION;
                string[] extensions = genBankExtensions.Split(',');
                foreach (string extension in extensions)
                {
                    if (fileExtension.Equals(extension, StringComparison.CurrentCultureIgnoreCase))
                    {
                        isGenBank = true;
                        break;
                    }
                }
            }

            return isGenBank;
        }


        /// <summary>
        /// Returns a string which represents specified GenBankAccession.
        /// </summary>
        /// <param name="accession">GenBankAccession instance.</param>
        /// <returns>Returns string.</returns>
        public static string GetGenBankAccession(GenBankAccession accession)
        {
            if (accession == null)
            {
                throw new ArgumentNullException("accession");
            }

            StringBuilder strBuilder = new StringBuilder();
            if (accession.Primary != null)
            {
                strBuilder.Append(accession.Primary);
            }

            foreach (string str in accession.Secondary)
            {
                strBuilder.Append(Space);
                strBuilder.Append(str);
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Returns a string which represents specified ProjectIdentifier.
        /// </summary>
        /// <param name="projectIdentifier">ProjectIdentifier instance.</param>
        /// <returns>Returns string.</returns>
        public static string GetProjectIdentifier(ProjectIdentifier projectIdentifier)
        {
            if (projectIdentifier == null)
            {
                throw new ArgumentNullException("projectIdentifier");
            }

            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(projectIdentifier.Name);
            strBuilder.Append(Colon);

            for (int i = 0; i < projectIdentifier.Numbers.Count; i++)
            {
                strBuilder.Append(projectIdentifier.Numbers[i]);
                if (i != projectIdentifier.Numbers.Count - 1)
                {
                    strBuilder.Append(Comma);
                }
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Returns a string which represents specified CrossReferenceLink.
        /// </summary>
        /// <param name="crossReferenceLinks">CrossReferenceLinks.</param>
        /// <returns>Returns string.</returns>
        public static string GetCrossReferenceLink(IList<CrossReferenceLink> crossReferenceLinks)
        {
            if (crossReferenceLinks == null)
            {
                throw new ArgumentNullException("crossReferenceLinks");
            }

            StringBuilder strBuilder = new StringBuilder();
            List<string> toReturn = new List<string>();
            foreach(var crossReferenceLink in crossReferenceLinks)
            {
                string referenceType = string.Empty;
                if (crossReferenceLink.Type == CrossReferenceType.Project)
                {
                    referenceType = ProjectDBLink;
                }
                else if (crossReferenceLink.Type == CrossReferenceType.TraceAssemblyArchive)
                {
                    referenceType = TraceAssemblyArchiveDBLink;
                }
                else if (crossReferenceLink.Type == CrossReferenceType.BioProject)
                {
                    referenceType = BioProjectDBLink;
                }
                strBuilder.Append(referenceType);
                strBuilder.Append(Colon);
                for (int i = 0; i < crossReferenceLink.Numbers.Count; i++)
                {
                    strBuilder.Append(crossReferenceLink.Numbers[i]);
                    if (i != crossReferenceLink.Numbers.Count - 1)
                    {
                        strBuilder.Append(Comma);
                    }
                }
                toReturn.Add(strBuilder.ToString());
                strBuilder.Clear();
            }
            return String.Join("\n", toReturn);
        }

        /// <summary>
        /// Returns a string which represents specified SequenceSegment.
        /// </summary>
        /// <param name="segment">SequenceSegment instance.</param>
        /// <returns>Returns string.</returns>
        public static string GetSequenceSegment(SequenceSegment segment)
        {
            if (segment == null)
            {
                throw new ArgumentNullException("segment");
            }

            return segment.Current.ToString(CultureInfo.InvariantCulture) + SegmentDelim + segment.Count.ToString(CultureInfo.InvariantCulture);
        }


        /// <summary>
        /// Returns a SequenceStrandType corresponds to the specified string.
        /// </summary>
        /// <param name="strand">Strand type.</param>
        /// <returns>Returns SequenceStrandType.</returns>
        public static SequenceStrandType GetStrandType(string strand)
        {
            if (string.IsNullOrEmpty(strand))
            {
                return SequenceStrandType.None;
            }

            strand = strand.ToLower(CultureInfo.InvariantCulture);

            if (strand.Equals(SingleStrand))
            {
                return SequenceStrandType.Single;
            }

            if (strand.Equals(DoubleStrand))
            {
                return SequenceStrandType.Double;
            }

            if (strand.Equals(MixedStrand))
            {
                return SequenceStrandType.Mixed;
            }

            return SequenceStrandType.None;
        }

        /// <summary>
        /// Returns a string which represents specified SequenceStrandType.
        /// </summary>
        /// <param name="strand">Strand type.</param>
        /// <returns>Returns string.</returns>
        public static string GetStrandType(SequenceStrandType strand)
        {
            switch (strand)
            {
                case SequenceStrandType.Single:
                    return SingleStrand;

                case SequenceStrandType.Double:
                    return DoubleStrand;

                case SequenceStrandType.Mixed:
                    return MixedStrand;
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns a SequenceStrandTopology corresponds to the specified string.
        /// </summary>
        /// <param name="strandTopology">Strand topology.</param>
        /// <returns>Returns SequenceStrandTopology.</returns>
        public static SequenceStrandTopology GetStrandTopology(string strandTopology)
        {
            if (string.IsNullOrEmpty(strandTopology))
            {
                return SequenceStrandTopology.None;
            }

            strandTopology = strandTopology.ToLower(CultureInfo.InvariantCulture);
            if (strandTopology.Equals(LinearStrandTopology))
            {
                return SequenceStrandTopology.Linear;
            }

            if (strandTopology.Equals(CircularStrandTopology))
            {
                return SequenceStrandTopology.Circular;
            }

            return SequenceStrandTopology.None;
        }

        /// <summary>
        /// Returns a string which represents specified SequenceStrandTopology.
        /// </summary>
        /// <param name="strandTopology">Strand topology.</param>
        /// <returns>Returns string.</returns>
        public static string GetStrandTopology(SequenceStrandTopology strandTopology)
        {
            switch (strandTopology)
            {
                case SequenceStrandTopology.Linear:
                    return LinearStrandTopology;
                case SequenceStrandTopology.Circular:
                    return CircularStrandTopology;
            }

            return string.Empty;
        }

        /// <summary>
        /// String Multiply. Build a string by concatenating copies of the input string.
        /// </summary>
        /// <param name="str">The string to multiply.</param>
        /// <param name="count">The number of copies wanted.</param>
        /// <returns>The multiplied string.</returns>
        public static string StringMultiply(string str, int count)
        {
            StringBuilder sb = new StringBuilder(count);
            for (int i = 0; i < count; ++i)
            {
                sb.Append(str);
            }
            return sb.ToString();
        }

        /// <summary>
        /// See if test string is identical to any of the passed list of strings.
        /// </summary>
        /// <param name="test">The string to test.</param>
        /// <param name="args">Variable number of strings to test against.</param>
        /// <returns>True if test matches one of the subsequent arguments.</returns>
        public static bool StringHasMatch(string test, params string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            foreach (string s in args)
            {
                if (test == s)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Determines if the string contains any characters specified in the list.
        /// </summary>
        /// <param name="toTest"></param>
        /// <param name="illegalCharacters"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
        public static bool StringContainsIllegalCharacters(string toTest, char[] illegalCharacters)
        {
            for (int j = 0; j < illegalCharacters.Length; j++)
            {
                char badVal=illegalCharacters[j];
                for (int i = 0; i < toTest.Length; i++)
                {
                    if (toTest[i] == badVal)
                    {
                        return true; 
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Validates specified value with the specified regular expression pattern.
        /// </summary>
        /// <param name="name">Name of the field.</param>
        /// <param name="value">Value to validate.</param>
        /// <param name="pattern">Regular exression pattern.</param>
        /// <returns>Returns empty string if valid; otherwise error message.</returns>
        public static string IsValidPatternValue(string name, string value, string pattern)
        {
            if (string.IsNullOrEmpty(value) || !IsValidRegexValue(pattern, value))
            {
                string message = string.Format(CultureInfo.CurrentCulture,
                                Properties.Resource.InvalidPatternMessage,
                                name,
                                value,
                                pattern);

                return message;
            }

            return string.Empty;
        }

        /// <summary>
        /// Validates specified value with the specified regular expression.
        /// </summary>
        /// <param name="name">Name of the field.</param>
        /// <param name="value">Value to validate.</param>
        /// <param name="regx">Regular exression object.</param>
        /// <returns>Returns empty string if valid; otherwise error message.</returns>
        public static string IsValidPatternValue(string name, string value, Regex regx)
        {
            if (regx == null)
            {
                throw new ArgumentNullException("regx");
            }

           
            if (string.IsNullOrEmpty(value) || !IsValidRegexValue(regx, value))
            {
                string message = string.Format(CultureInfo.CurrentCulture,
                                Properties.Resource.InvalidPatternMessage,
                                name,
                                value,
                                regx.ToString());

                return message;
            }

            return string.Empty;
        }

        /// <summary>
        /// Validates int value.
        /// </summary>
        /// <param name="name">Name of the field.</param>
        /// <param name="value">Value to validate.</param>
        /// <param name="minValue">Minimum value.</param>
        /// <param name="maxValue">Maximum value.</param>
        /// <returns>Returns empty string if valid; otherwise error message.</returns>
        public static string IsValidRange(string name, int value, int minValue, int maxValue)
        {
            if (value < minValue || value > maxValue)
            {
                return string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidRangeMessage, name, value, minValue, maxValue);
            }

            return string.Empty;
        }

        /// <summary>
        /// Validates specified value with the specified regular expression. 
        /// </summary>
        /// <param name="pattern">Regular expression.</param>
        /// <param name="value">Value to validate.</param>
        /// <returns>Returns true if value completely match with the specified 
        /// regular expression; otherwise false.</returns>
        public static bool IsValidRegexValue(string pattern, string value)
        {
            Regex regx = new Regex(pattern);
            Match match = regx.Match(value);
            if (!match.Success || match.Value.Length != value.Length)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates specified value with the specified regular expression. 
        /// </summary>
        /// <param name="regx">Regular expression object.</param>
        /// <param name="value">Value to validate.</param>
        /// <returns>Returns true if value completely match with the specified 
        /// regular expression; otherwise false.</returns>
        public static bool IsValidRegexValue(Regex regx, string value)
        {
            if (regx == null)
            {
                throw new ArgumentNullException("regx");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            Match match = regx.Match(value);
            if (!match.Success || match.Value.Length != value.Length)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Confirms that a condition is true. Raise an exception if it is not.
        /// </summary>
        /// <param name="condition">The condition to check</param>
        public static void CheckCondition(bool condition)
        {
            CheckCondition(condition, "A condition failed");
        }

        /// <summary>
        /// Confirms that a condition is true. Raise an exception if it is not.
        /// </summary>
        /// <remarks>
        /// Warning: The message with be evaluated even if the condition is true, so don't make it's calculation slow.
        ///           Avoid this with the "messageFunction" version.
        /// </remarks>
        /// <param name="condition">The condition to check</param>
        /// <param name="message">A message for the exception</param>
        public static void CheckCondition(bool condition, string message)
        {
            if (!condition)
            {
                throw new ArgumentException(message);
            }
        }

        /// <summary>
        /// Confirms that a condition is true. Raise an exception if it is not.
        /// </summary>
        /// <remarks>
        /// Warning: The message with be evaluated even if the condition is true, so don't make it's calculation slow.
        ///           Avoid this with the "messageFunction" version.
        /// </remarks>
        /// <param name="condition">The condition to check</param>
        /// <param name="messageToFormat">A message for the exception</param>
        /// <param name="formatValues">Values for the exception's message.</param>
        public static void CheckCondition(bool condition, string messageToFormat, params object[] formatValues)
        {
            if (!condition)
            {
                throw new Exception(string.Format(CultureInfo.InvariantCulture, messageToFormat, formatValues));
            }
        }

        /// <summary>
        /// Confirms that a condition is true. Raise an exception if it is not.
        /// </summary>
        /// <remarks>
        /// messageFunction will only be evaluated of condition is false. Use this version for messages that are costly to compute.
        /// </remarks>
        /// <param name="condition">The condition to check</param>
        /// <param name="messageFunction">Function that will generate the message if the condition is false.</param>
        public static void CheckCondition(bool condition, Func<string> messageFunction)
        {
            if (!condition)
            {
                string message = messageFunction();
                throw new Exception(message);
            }
        }

        /// <summary>
        /// Confirms that a condition is true. Raise an exception of type T if it is not.
        /// </summary>
        /// <param name="condition">The condition to check</param>
        /// <typeparam name="T">The type of exception that will be raised.</typeparam>
        public static void CheckCondition<T>(bool condition) where T : Exception
        {
            CheckCondition<T>(condition, Properties.Resource.ErrorCheckConditionFailed);
        }

        /// <summary>
        /// Confirms that a condition is true. Raise an exception of type T if it is not.
        /// </summary>
        /// <remarks>
        /// Warning: The message with be evaluated even if the condition is true, so don't make it's calculation slow.
        ///           Avoid this with the "messageFunction" version.
        /// </remarks>
        /// <param name="condition">The condition to check</param>
        /// <param name="message">A message for the exception</param>
        /// <typeparam name="T">The type of exception that will be raised.</typeparam>
        public static void CheckCondition<T>(bool condition, string message) where T : Exception
        {
            if (!condition)
            {
                Type t = typeof(T);
                System.Reflection.ConstructorInfo constructor = t.GetConstructor(new Type[] { typeof(string) });
                T exception = (T)constructor.Invoke(new object[] { message });
                throw exception;
            }
        }

        /// <summary>
        /// Confirms that a condition is true. Raise an exception if it is not.
        /// </summary>
        /// <remarks>
        /// Warning: The message with be evaluated even if the condition is true, so don't make it's calculation slow.
        ///           Avoid this with the "messageFunction" version.
        /// </remarks>
        /// <param name="condition">The condition to check</param>
        /// <param name="messageToFormat">A message for the exception</param>
        /// <param name="formatValues">Values for the exception's message.</param>
        /// <typeparam name="T">The type of exception that will be raised.</typeparam>
        public static void CheckCondition<T>(bool condition, string messageToFormat, params object[] formatValues) where T : Exception
        {
            if (!condition)
            {
                CheckCondition<T>(condition, string.Format(messageToFormat, formatValues));
            }
        }

        /// <summary>
        /// Confirms that a condition is true. Raise an exception if it is not.
        /// </summary>
        /// <remarks>
        /// messageFunction will only be evaluated of condition is false. Use this version for messages that are costly to compute.
        /// </remarks>
        /// <param name="condition">The condition to check</param>
        /// <param name="messageFunction">Function that will generate the message if the condition is false.</param>
        public static void CheckCondition<T>(bool condition, Func<string> messageFunction) where T : Exception
        {
            if (!condition)
            {
                string message = messageFunction();
                CheckCondition<T>(condition, message);
            }
        }

        /// <summary>
        /// Creates a tab-delimited string containing the object's string values.
        /// </summary>
        /// <param name="objectCollection">The objects to put in the string</param>
        /// <returns>A tab-delimited string</returns>
        public static string CreateTabString(params object[] objectCollection)
        {
            return objectCollection.StringJoin("\t");
        }

        /// <summary>
        /// Creates a delimited string containing the object's string values.
        /// </summary>
        /// <param name="delimiter">The string that will delimit the objects</param>
        /// <param name="objectCollection">The objects to put in the string</param>
        /// <returns>A delimiter-delimited string</returns>
        public static string CreateDelimitedString(string delimiter, params object[] objectCollection)
        {
            return objectCollection.StringJoin(delimiter);
        }


        /// <summary>
        /// Returns the first item in sequence that is one item long. (Raises an
        /// exception of the sequence is more than one item long).
        /// </summary>
        /// <typeparam name="T">The type of elements of the sequence</typeparam>
        /// <param name="sequence">The one-item long sequence</param>
        /// <returns>The first item in the sequence.</returns>
        public static T FirstAndOnly<T>(IEnumerable<T> sequence)
        {
            IEnumerator<T> enumor = sequence.GetEnumerator();
            CheckCondition(enumor.MoveNext(), Properties.Resource.ErrorCheckConditionFirstAndOnlyTooFew);
            T t = enumor.Current;
            CheckCondition(!enumor.MoveNext(), Properties.Resource.ErrorCheckConditionFirstAndOnlyTooMany);
            return t;
        }

        /// <summary>
        /// Efficently (log n) test if two dictionaries have the same key set.
        /// </summary>
        /// <typeparam name="TKey">The key type of the dictionaries</typeparam>
        /// <typeparam name="TValue1">The value type of dictionary 1</typeparam>
        /// <typeparam name="TValue2">The value type of dictionary 2</typeparam>
        /// <param name="dictionary1">The first dictionary</param>
        /// <param name="dictionary2">The second dictonary</param>
        /// <returns>True if the two key sets are "set equal"; false, otherwise.</returns>
        public static bool KeysEqual<TKey, TValue1, TValue2>(IDictionary<TKey, TValue1> dictionary1, IDictionary<TKey, TValue2> dictionary2)
        {
            if (dictionary1.Count != dictionary2.Count)
            {
                return false;
            }

            foreach (TKey key in dictionary1.Keys)
            {
                // real assert - all values must be "true"
                if (!dictionary2.ContainsKey(key))
                {
                    return false;
                }
                else
                {
                    // real assert - all values must be "true"
                }
            }
            return true;
        }

        /// <summary>
        /// Shifts the bits of an int around in a wrapped way. It is useful for creating hashcodes of collections.
        /// </summary>
        /// <param name="someInt">the int to shift</param>
        /// <param name="count">The number of bits to shift the int</param>
        /// <returns>The shifted int.</returns>
        public static int WrapAroundLeftShift(int someInt, int count)
        {
            //Tip: Use "?Convert.ToString(WrapAroundLeftShift(someInt,count),2)" to see this work
            int result = (someInt << count) | ((~(-1 << count)) & (someInt >> (8 * sizeof(int) - count)));
            return result;
        }

        /// <summary>
        /// Create a useful error message when a sequence fails validation.
        /// </summary>
        /// <param name="alphabet"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ArgumentOutOfRangeException GenerateAlphabetCheckFailureException(IAlphabet alphabet, byte[] data)
        {
            string badSequence = "Could not parse sequence as ASCII";
            try
            { badSequence = new String(ASCIIEncoding.ASCII.GetChars(data)); }
            catch { }
            return new ArgumentOutOfRangeException("Sequence contains an illegal character not allowed in alphabet: " + alphabet.Name + ".  Sequence was:\r\n" + badSequence);

        }


        #region BAM related methods
        /// <summary>
        /// Gets a byte array which represents value of 16 bit singed integer in LittleEndian format.
        /// </summary>
        /// <param name="value">16 bit singed integer value.</param>
        public static byte[] GetLittleEndianByteArray(Int16 value)
        {
            byte[] array = new byte[2];

            array[0] = (byte)(value & 0x00FF);
            array[1] = (byte)((value & 0xFF00) >> 8);
            return array;
        }

        /// <summary>
        /// Gets a byte array which represents value of 16 bit unsinged integer in LittleEndian format.
        /// </summary>
        /// <param name="value">16 bit unsinged integer value.</param>
        public static byte[] GetLittleEndianByteArray(UInt16 value)
        {
            byte[] array = new byte[2];

            array[0] = (byte)(value & 0x00FF);
            array[1] = (byte)((value & 0xFF00) >> 8);
            return array;
        }

        /// <summary>
        /// Gets a byte array which represents value of 32 bit singed integer in LittleEndian format.
        /// </summary>
        /// <param name="value">32 bit singed integer value.</param>
        public static byte[] GetLittleEndianByteArray(int value)
        {
            byte[] array = new byte[4];

            array[0] = (byte)(value & 0x000000FF);
            array[1] = (byte)((value & 0x0000FF00) >> 8);
            array[2] = (byte)((value & 0x00FF0000) >> 16);
            array[3] = (byte)((value & 0xFF000000) >> 24);
            return array;
        }

        /// <summary>
        /// Gets a byte array which represents value of 32 bit unsinged integer in LittleEndian format.
        /// </summary>
        /// <param name="value">32 bit unsinged integer value.</param>
        public static byte[] GetLittleEndianByteArray(uint value)
        {
            byte[] array = new byte[4];

            array[0] = (byte)(value & 0x000000FF);
            array[1] = (byte)((value & 0x0000FF00) >> 8);
            array[2] = (byte)((value & 0x00FF0000) >> 16);
            array[3] = (byte)((value & 0xFF000000) >> 24);
            return array;
        }

        /// <summary>
        /// Gets byte array which represents value of float in LittleEndian format.
        /// </summary>
        /// <param name="value">Float value.</param>
        public static byte[] GetLittleEndianByteArray(float value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        /// Returns 32 bit signed integer from the byte array stored as little-endian.
        /// </summary>
        /// <param name="byteArray">byte array.</param>
        /// <param name="startIndex">Start index of the byte array.</param>
        public static Int32 GetInt32(byte[] byteArray, int startIndex)
        {
            if (byteArray == null)
            {
                throw new ArgumentNullException("byteArray");
            }

            if (startIndex < 0 || startIndex >= byteArray.Length)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }

            return (byteArray[startIndex + 3] << 24) + (byteArray[startIndex + 2] << 16) + (byteArray[startIndex + 1] << 8) + byteArray[startIndex];
        }

        /// <summary>
        /// Returns 32 bit unsigned integer from the byte array stored as little-endian.
        /// </summary>
        /// <param name="byteArray">byte array.</param>
        /// <param name="startIndex">Start index of the byte array.</param>
        public static UInt32 GetUInt32(byte[] byteArray, int startIndex)
        {
            if (byteArray == null)
            {
                throw new ArgumentNullException("byteArray");
            }

            if (startIndex < 0 || startIndex >= byteArray.Length)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }

            return (UInt32)(byteArray[startIndex + 3] << 24) + (UInt32)(byteArray[startIndex + 2] << 16) + (UInt32)(byteArray[startIndex + 1] << 8) + (UInt32)byteArray[startIndex];
        }

        /// <summary>
        /// Returns 32 bit unsigned integer from the byte array stored as little-endian.
        /// </summary>
        /// <param name="byteArray">byte array.</param>
        /// <param name="startIndex">Start index of the byte array.</param>
        public static UInt32 GetUInt64(byte[] byteArray, int startIndex)
        {
            if (byteArray == null)
            {
                throw new ArgumentNullException("byteArray");
            }

            if (startIndex < 0 || startIndex >= byteArray.Length)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }

            return (UInt32)(byteArray[startIndex + 7] << 56)+
                (UInt32)(byteArray[startIndex + 6] << 48)+
                (UInt32)(byteArray[startIndex + 5] << 40)+
                (UInt32)(byteArray[startIndex + 4] << 32)+
                (UInt32)(byteArray[startIndex + 3] << 24)+
                (UInt32)(byteArray[startIndex + 2] << 16)+
                (UInt32)(byteArray[startIndex + 1] << 8)+
                (UInt32)byteArray[startIndex];
        }



        /// <summary>
        /// Returns 16 bit signed integer from the byte array stored as little-endian.
        /// </summary>
        /// <param name="byteArray">byte array.</param>
        /// <param name="startIndex">Start index of the byte array.</param>
        public static Int16 GetInt16(byte[] byteArray, int startIndex)
        {
            if (byteArray == null)
            {
                throw new ArgumentNullException("byteArray");
            }

            if (startIndex < 0 || startIndex >= byteArray.Length)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }

            Int16 value = 0;
            value = (Int16)(byteArray[startIndex + 1] << 8);
            value += (Int16)byteArray[startIndex];
            return value;
        }

        /// <summary>
        /// Returns 16 bit unsigned integer from the byte array stored as little-endian.
        /// </summary>
        /// <param name="byteArray">byte array.</param>
        /// <param name="startIndex">Start index of the byte array.</param>
        public static UInt16 GetUInt16(byte[] byteArray, int startIndex)
        {
            if (byteArray == null)
            {
                throw new ArgumentNullException("byteArray");
            }

            if (startIndex < 0 || startIndex >= byteArray.Length)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }

            UInt16 value = 0;
            value = (UInt16)(byteArray[startIndex + 1] << 8);
            value += (UInt16)byteArray[startIndex];
            return value;
        }

        /// <summary>
        /// Returns float from the byte array.
        /// </summary>
        /// <param name="byteArray">byte array.</param>
        /// <param name="startIndex">Start index of the byte array.</param>
        public static float GetSingle(byte[] byteArray, int startIndex)
        {
            if (byteArray == null)
            {
                throw new ArgumentNullException("byteArray");
            }

            if (startIndex < 0 || startIndex >= byteArray.Length)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }

            return BitConverter.ToSingle(byteArray, startIndex);
        }

        /// <summary>
        /// Gets the HexString from the specified byte array.
        /// </summary>
        /// <param name="byteArray">Byte array.</param>
        /// <param name="startIndex">Start index of array from which HexString is stored.</param>
        /// <param name="length">Length of HexString to read.</param>
        public static string GetHexString(byte[] byteArray, int startIndex, int length)
        {
            if (byteArray == null)
            {
                throw new ArgumentNullException("byteArray");
            }

            if (startIndex < 0 || startIndex >= byteArray.Length)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }

            if (startIndex + length > byteArray.Length)
            {
                throw new ArgumentOutOfRangeException("length");
            }

            StringBuilder Result = new StringBuilder();
            string HexAlphabet = "0123456789ABCDEF";
            for (int i = startIndex; i < startIndex + length; i++)
            {
                Result.Append(HexAlphabet[(byteArray[i] >> 4)]);
                Result.Append(HexAlphabet[(byteArray[i] & 0xF)]);
            }

            return Result.ToString();
        }

        /// <summary>
        /// Returns random numbers according to an approximate normal distribution
        /// with an average and standard deviation set by the caller.
        /// </summary>
        /// <param name="average">Average result returned from calling the method</param>
        /// <param name="standardDeviation">Standard deviation applied to the normal curve</param>
        /// <returns>A random value</returns>
        public static double GetNormalRandom(double average, double standardDeviation)
        {
            return GetNormalRandom(average, standardDeviation, 8);
        }

        /// <summary>
        /// Returns random numbers according to an approximate normal distribution
        /// with an average and standard deviation set by the caller. This is done iteratively
        /// using the central limit theorem.
        /// </summary>
        /// <param name="average">Average result returned from calling the method</param>
        /// <param name="standardDeviation">Standard deviation applied to the normal curve</param>
        /// <param name="steps">
        /// The number of iterative steps to take in generating each number. The higher this number
        /// is, the closer to a true normal distribution the results will be, but the higher the
        /// computation cost. A value between 4 and 8 should be sufficient for most uses.
        /// </param>
        /// <returns>A random value</returns>
        public static double GetNormalRandom(double average, double standardDeviation, int steps)
        {
            if (steps < 0)
                throw new ArgumentException("Step count can not be negative");

            double sum = 0.0;
            for (int i = 0; i < steps; i++)
                sum += random.NextDouble();

            // RandomDouble gives us a uniformly distributed number between 0 and 1.
            // This value has average=0.5 and var=1/12. For the sum, this is
            // average=steps*0.5 and var=steps/12.

            sum -= ((double)steps / 2.0);						// Go to N(0, 1/12n)
            sum *= (standardDeviation / (Math.Sqrt((double)steps / 12.0)));	// Go to N(0, var)
            sum += average;										// Go to N(mu, var)

            return sum;
        }

        /// <summary>
        /// Identifies if a file extension is a
        /// valid extension for BAM formats.
        /// </summary>
        /// <returns>
        /// true  : if it is a valid BAM file extension.
        /// false : if it is a in-valid BAM file extension.
        /// </returns>
        public static bool IsBAM(string fileName)
        {
            bool isBAM = false;
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return isBAM;
            }

            int extensionDelimiter = fileName.LastIndexOf('.');
            if (-1 < extensionDelimiter)
            {
                string fileExtension = fileName.Substring(extensionDelimiter);
                string bamExtensions = Properties.Resource.BAM_FILEEXTENSION;
                string[] extensions = bamExtensions.Split(Comma.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (string extension in extensions)
                {
                    if (fileExtension.Equals(extension, StringComparison.CurrentCultureIgnoreCase))
                    {
                        isBAM = true;
                        break;
                    }
                }
            }

            return isBAM;
        }
        #endregion

        /// <summary>
        /// Validates the specified sequence id is in the format of paired read or not.
        /// If so then gets the original sequence id, paired read type and library name from the paired sequence id.
        /// For Example:
        ///  if the sequence id is  "seq1.F:10K!324;abcd;345" then this method will return true and 
        ///    originalSequenceId  -  "seq1"
        ///    pairedReadType      -  "F"
        ///    libraryName         -  "10K"
        ///  if the sequence id is not in the format of "originalSequenceId.{F,R}:LibraryName"
        ///  or "originalSequenceId.{F,R}:LibraryName!otherInfo" then this method will return false.
        /// </summary>
        /// <param name="pairedSequenceId">Paired sequence id.</param>
        /// <param name="originalSequenceId">Original sequence id part from the specified sequence id.</param>
        /// <param name="forwardRead">Flag to indicate whether forward read or not.</param>
        /// <param name="pairedReadType">Paired sequence type part from the specified sequence id.</param>
        /// <param name="libraryName">Library name part from the specified sequence id.</param>
        public static bool ValidatePairedSequenceId(string pairedSequenceId, out string originalSequenceId, out bool forwardRead, out string pairedReadType, out string libraryName)
        {
            originalSequenceId = string.Empty;
            pairedReadType = string.Empty;
            forwardRead = false;
            libraryName = string.Empty;

            if (string.IsNullOrWhiteSpace(pairedSequenceId))
            {
                return false;
            }

            try
            {
                string id = pairedSequenceId;
                int index = id.LastIndexOf(Helper.PairedReadDelimiter);
                int endindex = id.Length;
                if (index > 0)
                {
                    id = id.Substring(0, index);
                    endindex = index;
                }

                index = id.LastIndexOf(':');
                if (index == -1)
                {
                    return false;
                }

                libraryName = id.Substring(index + 1, endindex - (index + 1));

                endindex = index;
                index = id.LastIndexOf('.', index);
                if (index == -1)
                {
                    return false;
                }

                pairedReadType = id.Substring(index + 1, endindex - (index + 1));

                switch (pairedReadType)
                {
                    case "F":
                    case "f":
                    case "1":
                    case "X1":
                    case "x1":
                    case "A":
                    case "a":
                        forwardRead = true;
                        break;
                    case "R":
                    case "r":
                    case "2":
                    case "Y1":
                    case "y1":
                    case "B":
                    case "b":
                        forwardRead = false;
                        break;
                    default:
                        return false;
                }

                originalSequenceId = id.Substring(0, index);
                return true;
            }
            catch
            {
            }

            return false;
        }

        /// <summary>
        /// Gets the mate paired read type.
        /// That is for F/1/X1 it provides R/2/Y1.
        /// </summary>
        /// <param name="pairedReadType">Paired read type.</param>
        public static string GetMatePairedReadType(string pairedReadType)
        {
            switch (pairedReadType)
            {
                case "F":
                    return "R";
                case "f":
                    return "r";
                case "1":
                    return "2";
                case "X1":
                    return "Y1";
                case "x1":
                    return "y1";
                case "R":
                    return "F";
                case "r":
                    return "f";
                case "2":
                    return "1";
                case "Y1":
                    return "X1";
                case "y1":
                    return "x1";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Concatenates the specified original sequence id, paired read type and library name to paired read id format.
        /// </summary>
        /// <param name="originalSequenceId">Original sequence id.</param>
        /// <param name="pairedReadType">Paired read type.</param>
        /// <param name="libraryName">Library name.</param>
        public static string GetPairedReadId(string originalSequenceId, string pairedReadType, string libraryName)
        {
            return originalSequenceId + "." + pairedReadType + ":" + libraryName;
        }

        /// <summary>
        /// Returns the id exluding the otherinformation part from it.
        /// </summary>
        /// <param name="readId">Read id.</param>
        public static string GetReadIdExcludingOtherInfo(string readId)
        {
            if (string.IsNullOrEmpty(readId))
            {
                return readId;
            }

            string result = readId;
            int index = readId.LastIndexOf(Helper.PairedReadDelimiter);
            if (index > 0)
            {
                result = readId.Substring(0, index);
            }

            return result;
        }

#if (SILVERLIGHT == false)
        /// <summary>
        /// Gets string representing specified delta alignment.
        /// This method is used in comparative Utilities to write delta alignments to file.
        /// </summary>
        /// <param name="deltaAlignment">Delta alignment</param>
        public static string GetString(DeltaAlignment deltaAlignment)
        {
            if (deltaAlignment == null)
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("@" + deltaAlignment.Id);
            builder.AppendLine(">" + deltaAlignment.ReferenceSequence.ID);
            if (deltaAlignment.QuerySequence != null)
            {
                builder.AppendLine(deltaAlignment.QuerySequence.ID);
            }
            else
            {
                //To provide empty lines in place of query sequence id and query sequence
                builder.AppendLine();
                builder.AppendLine();
            }

            builder.AppendLine(string.Format(CultureInfo.InvariantCulture,
                "{0,10} {1,10} {2,10} {3,10} {4} {5} {6}",
                deltaAlignment.FirstSequenceStart,
                deltaAlignment.FirstSequenceEnd,
                ((deltaAlignment.IsReverseQueryDirection) 
                ? deltaAlignment.SecondSequenceEnd : deltaAlignment.SecondSequenceStart),
                ((deltaAlignment.IsReverseQueryDirection)
                ? deltaAlignment.SecondSequenceStart : deltaAlignment.SecondSequenceEnd),
                deltaAlignment.Errors,
                deltaAlignment.SimilarityErrors,
                deltaAlignment.NonAlphas));

            foreach (long deltas in deltaAlignment.Deltas)
            {
                builder.AppendLine(deltas.ToString(CultureInfo.InvariantCulture));
            }

            builder.AppendLine("*");
            return builder.ToString();
        }
#endif

        /// <summary>
        /// Copies source array to destination array.
        /// In case of silverlight Length will be converted to integer before copying.
        /// </summary>
        /// <param name="sourceArray">Source array</param>
        /// <param name="destinationArray">Destination array</param>
        /// <param name="length">No of elements to copy</param>
        public static void Copy(Array sourceArray, Array destinationArray, long length)
        {
#if (SILVERLIGHT == false)
            Array.Copy(sourceArray, destinationArray, length);
#else
            Array.Copy(sourceArray, destinationArray, (int)length);
#endif
        }

        /// <summary>
        ///  Copies source array to destination array.
        /// In case of silverlight Length will be converted to integer before copying.
        /// </summary>
        /// <param name="sourceArray">Source array</param>
        /// <param name="sourceIndex">Source stating index.</param>
        /// <param name="destinationArray">Destination array</param>
        /// <param name="destinationIndex">Destination stating index.</param>
        /// <param name="length">No of elements to copy</param>
        public static void Copy(Array sourceArray, long sourceIndex, Array destinationArray, long destinationIndex, long length)
        {
#if (SILVERLIGHT == false)
            Array.Copy(sourceArray, sourceIndex, destinationArray, destinationIndex, length);
#else
            Array.Copy(sourceArray, (int)sourceIndex, destinationArray, (int)destinationIndex, (int)length);
#endif
        }
    }
}
