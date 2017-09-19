using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Bio.Algorithms.Assembly.Padena.Scaffold;

namespace Bio.Algorithms.Assembly
{
    /// <summary>
    /// Class maps reads to mate pairs using sequence ID of sequence in FASTA file.
    /// Supported mate pair formats
    /// >chrI0.X1:abc
    /// ATGC
    /// >chrI0.Y1:abc
    /// TACG
    /// >chrI0.F:abc
    /// ATGC
    /// >chrI0.R:abc
    /// TACG
    /// >chrI0.1:abc
    /// ATGC
    /// >chrI0.2:abc
    /// TACG
    /// Where X1,F,1 denotes forward reads and Y1,R,2 denotes reverse reads
    /// abc denotes library name 
    /// chrI0 is the sequence id.
    /// </summary>
    public class MatePairMapper : IMatePairMapper
    {
        #region Field Variables
        
        /// <summary>
        /// Regular Expression matching reads in supported formats.
        /// </summary>
        private Regex readExpression = PlatformManager.Services.CreateCompiledRegex(
            "^(.*)\\.(X1|Y1|F|R|1|2|x1|y1|f|r|a|b|A|B)\\:(.*)$");

        #endregion

        #region IMapMatePairs Members

        /// <summary>
        /// Map reads to mate pairs.
        /// </summary>
        /// <param name="reads">List of Reads.</param>
        /// <returns>List of mate pairs.</returns>
        public IList<MatePair> Map(IEnumerable<ISequence> reads)
        {
            if (null == reads)
            {
                throw new ArgumentNullException("reads");
            }

            Dictionary<string, MatePair> pairs = new Dictionary<string, MatePair>();
            MatePair mate;
            string exp = string.Empty;

            foreach (ISequence read in reads)
            {
                if (read == null)
                {
                    throw new ArgumentException(Properties.Resource.ReadCannotBeNull);
                }

                if (pairs.TryGetValue(read.ID, out mate))
                {
                    if (string.IsNullOrEmpty(mate.ForwardReadID))
                    {
                        mate.ForwardReadID = read.ID;
                    }
                    else
                    {
                        mate.ReverseReadID = read.ID;
                    }
                }
                else
                {
                    Match match = this.readExpression.Match(read.ID);
                    if (match.Success)
                    {
                        mate = new MatePair(match.Groups[3].Value);
                        if (match.Groups[2].Value == "X1" || match.Groups[2].Value == "F" ||
                            match.Groups[2].Value == "1" || match.Groups[2].Value == "x1" ||
                            match.Groups[2].Value == "f" || match.Groups[2].Value == "a" ||
                            match.Groups[2].Value == "A")
                        {
                            mate.ForwardReadID = read.ID;
                        }
                        else
                        {
                            mate.ReverseReadID = read.ID;
                        }

                        exp = GenerateExpression(match);
                        if (!pairs.ContainsKey(exp))
                        {
                            pairs.Add(exp, mate);
                        }
                        else
                        {
                            throw new FormatException(
                                string.Format(CultureInfo.CurrentCulture,Properties.Resource.DuplicatingReadIds, read.ID));
                        }
                    }
                }
            }

            return pairs.Values.Where(pair => !string.IsNullOrEmpty(pair.ForwardReadID)
                && !string.IsNullOrEmpty(pair.ReverseReadID)).ToList();
        }

        /// <summary>
        /// Finds contig pairs having valid mate pairs connection between them.
        /// </summary>
        /// <param name="reads">Input list of reads.</param>
        /// <param name="alignment">Reads contig alignment.</param>
        /// <returns>Contig Mate pair map.</returns>
        public ContigMatePairs MapContigToMatePairs(IEnumerable<ISequence> reads, ReadContigMap alignment)
        {
            if (alignment == null)
            {
                throw new ArgumentNullException("alignment");
            }

            if (reads == null)
            {
                throw new ArgumentNullException("reads");
            }

            Dictionary<ISequence, IList<ReadMap>> contigs1;
            Dictionary<ISequence, IList<ReadMap>> contigs2;
            ContigMatePairs contigMatePairs = new ContigMatePairs();
            foreach (ISequence read in reads)
            {
                Match match = this.readExpression.Match(read.ID);
                if (match.Success)
                {
                    string mateDisplayID = GenerateExpression(match);
                    if (alignment.TryGetValue(read.ID, out contigs1) && alignment.TryGetValue(mateDisplayID, out contigs2))
                    {
                        MatePair pair;
                        if (match.Groups[2].Value == "X1" || match.Groups[2].Value == "F" ||
                            match.Groups[2].Value == "1" || match.Groups[2].Value == "x1" ||
                            match.Groups[2].Value == "f" || match.Groups[2].Value == "a" ||
                            match.Groups[2].Value == "A")
                        {
                            pair = new MatePair(read.ID, mateDisplayID, match.Groups[3].Value);
                            ContigMatePairMapper(contigs1, contigs2, pair, contigMatePairs);
                        }
                        else
                        {
                            pair = new MatePair(mateDisplayID, read.ID, match.Groups[3].Value);
                            ContigMatePairMapper(contigs2, contigs1, pair, contigMatePairs);
                        }

                        alignment.Remove(read.ID);
                        alignment.Remove(mateDisplayID);
                    }
                }
            }

            return contigMatePairs;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates Paired Read Contig Map.
        /// </summary>
        /// <param name="forwardContigs">Contigs aligning to forward read.</param>
        /// <param name="reverseContigs">Contigs aligning to reverse read.</param>
        /// <param name="pair">Mate Pair.</param>
        /// <param name="contigMatePairs">Contig mate pair.</param>
        private static void ContigMatePairMapper(
            Dictionary<ISequence, IList<ReadMap>> forwardContigs,
            Dictionary<ISequence, IList<ReadMap>> reverseContigs,
            MatePair pair,
            ContigMatePairs contigMatePairs)
        {
            foreach (KeyValuePair<ISequence, IList<ReadMap>> forwardContigMaps in forwardContigs)
            {
                Dictionary<ISequence, IList<ValidMatePair>> forwardContig;
                if (!contigMatePairs.TryGetValue(forwardContigMaps.Key, out forwardContig))
                {
                    forwardContig = new Dictionary<ISequence, IList<ValidMatePair>>();
                    contigMatePairs.Add(forwardContigMaps.Key, forwardContig);
                }

                foreach (KeyValuePair<ISequence, IList<ReadMap>> reverseContigMaps in reverseContigs)
                {
                    IList<ValidMatePair> matePairs;
                    if (!forwardContig.TryGetValue(reverseContigMaps.Key, out matePairs))
                    {
                        matePairs = new List<ValidMatePair>();
                        forwardContig.Add(reverseContigMaps.Key, matePairs);
                    }
                    
                    foreach (ReadMap forwardMap in forwardContigMaps.Value)
                    {
                        foreach (ReadMap reverseMap in reverseContigMaps.Value)
                        {
                            ValidMatePair validPairedRead = new ValidMatePair();
                            validPairedRead.PairedRead = pair;
                            validPairedRead.ForwardReadStartPosition.Add(forwardMap.StartPositionOfContig);
                            validPairedRead.ReverseReadStartPosition.Add(
                                reverseMap.StartPositionOfContig + reverseMap.Length - 1);
                            validPairedRead.ReverseReadReverseComplementStartPosition.Add(
                                reverseContigMaps.Key.Count - reverseMap.StartPositionOfContig - 1);
                            matePairs.Add(validPairedRead);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generates expression for other read using expression on read.
        /// </summary>
        /// <param name="match">Pattern match on read whose partner has to be determined.</param>
        /// <returns>Expression for other read.</returns>
        private static string GenerateExpression(Match match)
        {
            string expression = string.Empty;
            switch (match.Groups[2].Value)
            {
                case "X1":
                    expression = match.Groups[1].Value + ".Y1:" + match.Groups[3].Value;
                    break;
                case "Y1":
                    expression = match.Groups[1].Value + ".X1:" + match.Groups[3].Value;
                    break;
                case "F":
                    expression = match.Groups[1].Value + ".R:" + match.Groups[3].Value;
                    break;
                case "R":
                    expression = match.Groups[1].Value + ".F:" + match.Groups[3].Value;
                    break;
                case "1":
                    expression = match.Groups[1].Value + ".2:" + match.Groups[3].Value;
                    break;
                case "2":
                    expression = match.Groups[1].Value + ".1:" + match.Groups[3].Value;
                    break;
                case "x1":
                    expression = match.Groups[1].Value + ".y1:" + match.Groups[3].Value;
                    break;
                case "y1":
                    expression = match.Groups[1].Value + ".x1:" + match.Groups[3].Value;
                    break;
                case "f":
                    expression = match.Groups[1].Value + ".r:" + match.Groups[3].Value;
                    break;
                case "r":
                    expression = match.Groups[1].Value + ".f:" + match.Groups[3].Value;
                    break;
                case "A":
                    expression = match.Groups[1].Value + ".B:" + match.Groups[3].Value;
                    break;
                case "B":
                    expression = match.Groups[1].Value + ".A:" + match.Groups[3].Value;
                    break;
                case "a":
                    expression = match.Groups[1].Value + ".b:" + match.Groups[3].Value;
                    break;
                case "b":
                    expression = match.Groups[1].Value + ".a:" + match.Groups[3].Value;
                    break;
            }

            return expression;
        }

        #endregion
    }
}
