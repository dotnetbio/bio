using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Bio.Algorithms.Alignment;
using Bio.Algorithms.MUMmer.LIS;
using Bio.Algorithms.SuffixTree;
using Bio.Extensions;
using Bio.IO.FastA;
using Bio.TestAutomation.Util;
using Bio.Tests.Framework;
using Bio.Util.Logging;

using NUnit.Framework;

namespace Bio.Tests.Algorithms.MUMmer
{
    /// <summary>
    /// MUMmer Bvt Test case implementation.
    /// </summary>
    [TestFixture]
    public class MUMmerBvtTestCases
    {
        Utility utilityObj = new Utility(@"TestUtils\MUMmerTestsConfig.xml");
        ASCIIEncoding encodingObj = new ASCIIEncoding();

        # region Enum

        /// <summary>
        /// Lis Parameters which are used for different test cases 
        /// based on which the test cases are executed.
        /// </summary>
        enum LISParameters
        {
            FindUniqueMatches,
            PerformLIS,
        };


        #endregion Enum

        #region Suffix Tree Test Cases

        /// <summary>
        /// Validate FindMatches() method with one line sequence 
        /// for both reference and query parameter and validate
        /// the unique matches
        /// Input : One line sequence for both reference and query parameter
        /// Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void SuffixTreeFindMatchesOneLineSequence()
        {
            this.ValidateFindMatchSuffixGeneralTestCases(Constants.OneLineSequenceNodeName, false);
        }

        /// <summary>
        /// Validate FindMatches() method with small size (less than 35kb) sequence 
        /// for reference and query parameter and validate
        /// the unique matches
        /// Input : Small size sequence for both reference and query parameter
        /// Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void SuffixTreeFindMatchesSmallSizeSequence()
        {
            this.ValidateFindMatchSuffixGeneralTestCases(Constants.SmallSizeSequenceNodeName,
                true);
        }

        /// <summary>      
        /// Validates Edge count for a Suffix tree.
        /// Input:A Dna Sequence.
        /// Output:Edge Count.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateEdgeCount()
        {
            string dnaSequence = "ATGCA";
            Sequence sequence = new Sequence(Alphabets.DNA, dnaSequence);
            MultiWaySuffixTree dnaSuffixTree = new MultiWaySuffixTree(sequence);
            Assert.AreEqual(8, dnaSuffixTree.EdgesCount);
            ApplicationLog.WriteLine(@"MUMmer BVT : Validation of edge
                        count for a Dna sequence completed successfully");

            string ambiguousDnasequence = "RSVTW";

            sequence = new Sequence(AmbiguousDnaAlphabet.Instance, ambiguousDnasequence);
            MultiWaySuffixTree ambiguousDnaSuffixTree = new MultiWaySuffixTree(sequence);
            Assert.AreEqual(7, ambiguousDnaSuffixTree.EdgesCount);
            ApplicationLog.WriteLine(@"MUMmer BVT : Validation of edge
                        count for a Ambiguous Dna sequence completed successfully");
        }

        #endregion Suffix Tree Test Cases

        #region Edges Test cases

        /// <summary>      
        /// Validates the public properties for a leaf node.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateEdgesForALeaf()
        {
            MultiWaySuffixEdge rootEdge = new MultiWaySuffixEdge();
            Assert.AreEqual(rootEdge.IsLeaf, true);
            ApplicationLog.WriteLine("MUMmer BVT : Successfully Validated Is leaf property.");
            Assert.AreEqual(rootEdge.Children, null);
            ApplicationLog.WriteLine("MUMmer BVT : Successfully Validated Children property for a Leaf. ");
            Assert.AreEqual(rootEdge.StartIndex, 0);
            ApplicationLog.WriteLine("MUMmer BVT : Successfully Validated start index of a Leaf.");
        }

        # endregion Edges Test cases

        #region LIS test cases

        /// <summary>
        /// Validate GetLongestSequence() method with two match
        /// for reference and query parameter. 
        /// Input : One sequence for both reference and query parameter
        /// Validation : Validate longest sequence.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateLongestIncreasingSubsequenceTwoUniqueMatch()
        {
            this.ValidateLongestIncreasingSubsequenceTestCases(Constants.OneLineTwoMatchSequenceNodeName,
                false);
        }

        /// <summary>
        /// Validate GetLongestSequence() method with one match
        /// for reference and query parameter. 
        /// Input : One sequence for both reference and query parameter
        /// Validation : Validate longest sequence.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateLongestIncreasingSubsequenceOneUniqueMatch()
        {
            this.ValidateLongestIncreasingSubsequenceTestCases(Constants.OneLineSequenceNodeName,
                false);
        }

        #endregion LIS test cases

        #region MUMmer Align Test Cases

        /// <summary>
        /// Validate Align() method with one line sequence 
        /// and validate the aligned sequences
        /// Input : One line sequence
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void MUMmerAlignOneLineSequence()
        {
            this.ValidateMUMmerAlignGeneralTestCases(Constants.OneLineSequenceNodeName,
                false, false);
        }

        /// <summary>
        /// Validate Align() method with small size (less than 35kb) sequence 
        /// and validate the aligned sequences
        /// Input : small size sequence file
        /// Validation : Validate the aligned sequences.
        /// </summary>        
        [Test]
        [Category("Priority0")]
        public void MUMmerAlignSmallSizeSequence()
        {
            this.ValidateMUMmerAlignGeneralTestCases(Constants.SmallSizeSequenceNodeName,
                true, false);
        }

        /// <summary>
        /// Validate Align(QuerySeqList) method with one line sequence 
        /// and validate the aligned sequences
        /// Input : One line multiple sequences
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void MUMmerAlignQuerySeqList()
        {
            this.ValidateMUMmerAlignGeneralTestCases(Constants.QuerySeqeunceListNode, false, true);
        }

        #endregion MUMmer Align Test Cases

        #region MUMs validation Test Cases

        /// <summary>
        /// Validate GetMatches() method with one line sequence 
        /// and validate the Mumms.
        /// Input : One line sequence
        /// Validation : Validate the MUMs Output.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateMUMsWithOneLineSequence()
        {
            this.ValidateMUMsGeneralTestCases(Constants.OneLineSequenceNodeName,
                false);
        }

        /// <summary>
        /// Validate GetMumsWithMaxMatch() method with one line sequence 
        /// and validate the Mumms.
        /// Input : One line sequence
        /// Validation : Validate the MUMs Output.
        /// </summary>
        [Category("Priority0")]
        public void ValidateGetMumsWithMaxMatchWithOneLineSequence()
        {
            // Gets the reference sequence from the configurtion file
            string referenceSequence = this.utilityObj.xmlUtil.GetTextValue(Constants.OneLineSequenceNodeName,
                    Constants.SequenceNode);

            string referenceSeqAlphabet = this.utilityObj.xmlUtil.GetTextValue(Constants.OneLineSequenceNodeName,
                Constants.AlphabetNameNode);

            ISequence referenceSeq = new Sequence(Utility.GetAlphabet(referenceSeqAlphabet),
                this.encodingObj.GetBytes(referenceSequence));

            string querySequence = this.utilityObj.xmlUtil.GetTextValue(Constants.OneLineSequenceNodeName,
                Constants.SearchSequenceNode);

            referenceSeqAlphabet = this.utilityObj.xmlUtil.GetTextValue(Constants.OneLineSequenceNodeName,
                Constants.SearchSequenceAlphabetNode);

            ISequence querySeq = new Sequence(Utility.GetAlphabet(referenceSeqAlphabet),
                 this.encodingObj.GetBytes(querySequence));

            string mumLength = this.utilityObj.xmlUtil.GetTextValue(Constants.OneLineSequenceNodeName, Constants.MUMLengthNode);

            Bio.Algorithms.MUMmer.MUMmer mum = new Bio.Algorithms.MUMmer.MUMmer(referenceSeq as Sequence);
            mum.LengthOfMUM = long.Parse(mumLength, null);
            IEnumerable<Match> actualResult = null;
            actualResult = mum.GetMatches(querySeq);

            // Validate MUMs output.
            Assert.IsTrue(this.ValidateMums(Constants.OneLineSequenceNodeName, actualResult));

            ApplicationLog.WriteLine("MUMmer BVT : Successfully validated the Mumms.");
        }

        /// <summary>
        /// Validate GetMUMs() method with one line sequence 
        /// and validate the MUMs up to LIS.
        /// Input : One line sequence
        /// Validation : Validate the MUMs Output.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateMUMsUpToLISWithOneLineSequence()
        {
            this.ValidateMUMsGeneralTestCases(Constants.OneLineSequenceNodeName,
                false);
        }

        /// <summary>
        /// Validates SearchMatch() with One line sequence as input.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSearchMatchesWithOneLineSequence()
        {
            this.ValidateSearchMatch(Constants.OneLineSequenceNodeName);
        }

        #endregion MUMs validation Test Cases

        #region Simple Suffix Tree Test Cases

        /// <summary>
        /// Validate SimpleSuffixTree Find Match() for one line sequence
        /// Input : One line sequence for both reference and query parameter
        /// Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void SimpleSuffixTreeFindMatchesOneLineSequence()
        {
            this.ValidateFindMatchSimpleSuffixGeneralTestCases(Constants.OneLineSequenceNodeName,
                false);
        }

        /// <summary>
        /// Validate SimpleSuffixTree Find Match() for small size sequences
        /// Input : Small size sequence for both reference and query parameter
        /// Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void SimpleSuffixTreeFindMatchesSmallSizeSequence()
        {
            this.ValidateFindMatchSimpleSuffixGeneralTestCases(Constants.SmallSizeSequenceNodeName,
                true);
        }

        #endregion  Simple Suffix Tree Test Cases

        #region Supported Methods

        /// <summary>
        /// Validates most of the find matches suffix tree test cases with varying parameters.
        /// </summary>
        /// <param name="nodeName">Node name which needs to be read for execution.</param>
        /// <param name="isFilePath">Is File Path?</param>
        /// <param name="LISActionType">LIS action type enum</param>
        void ValidateFindMatchSuffixGeneralTestCases(string nodeName, bool isFilePath)
        {
            ISequence referenceSeq = null;
            ISequence querySeq = null;
            string referenceSequence = string.Empty;
            string querySequence = string.Empty;
            IEnumerable<ISequence> referenceSeqs = null;

            if (isFilePath)
            {
                // Gets the reference sequence from the configuration file
                string filePath = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.FilePathNode);

                Assert.IsNotNull(filePath);
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "MUMmer BVT : Successfully validated the File Path '{0}'.", filePath));

                FastAParser parser = new FastAParser();
                referenceSeqs = parser.Parse(filePath);
                referenceSeq = referenceSeqs.ElementAt(0);
                referenceSequence = new string(referenceSeq.Select(a => (char)a).ToArray());

                // Gets the reference sequence from the configuration file
                string queryFilePath = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.SearchSequenceFilePathNode);

                Assert.IsNotNull(queryFilePath);
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "MUMmer BVT : Successfully validated the Search File Path '{0}'.", queryFilePath));

                IEnumerable<ISequence> querySeqs = null;
                FastAParser queryParser = new FastAParser();
                querySeqs = queryParser.Parse(queryFilePath);
                querySeq = querySeqs.ElementAt(0);
                querySequence = new string(querySeq.Select(a => (char)a).ToArray());
            }
            else
            {
                // Gets the reference sequence from the configurtion file
                referenceSequence = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.SequenceNode);

                string seqAlp = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.AlphabetNameNode);

                referenceSeq = new Sequence(Utility.GetAlphabet(seqAlp),
                    this.encodingObj.GetBytes(referenceSequence));

                querySequence = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.SearchSequenceNode);

                seqAlp = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.SearchSequenceAlphabetNode);

                querySeq = new Sequence(Utility.GetAlphabet(seqAlp),
                    this.encodingObj.GetBytes(querySequence));
            }

            string mumLength = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMLengthNode);

            // Builds the suffix for the reference sequence passed.           
            MultiWaySuffixTree suffixTreeBuilder = new MultiWaySuffixTree(referenceSeq as Sequence);
            suffixTreeBuilder.MinLengthOfMatch = long.Parse(mumLength, null);
            IEnumerable<Match> matches = null;
            matches = suffixTreeBuilder.SearchMatchesUniqueInReference(querySeq);

            // Validates the Unique Matches.
            ApplicationLog.WriteLine("MUMmer BVT : Validating the Unique Matches");
            Assert.IsTrue(this.ValidateUniqueMatches(matches, nodeName, LISParameters.FindUniqueMatches));

            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "MUMmer BVT : Successfully validated the all the unique matches for the sequence '{0}' and '{1}'.",
                referenceSequence, querySequence));
        }

        /// <summary>
        /// Validates the Mummer align method for several test cases for the parameters passed.
        /// </summary>
        /// <param name="nodeName">Node name to be read from xml</param>
        /// <param name="isFilePath"></param>
        /// <param name="isSeqList">Is MUMmer alignment with List of sequences</param>
        void ValidateMUMmerAlignGeneralTestCases(string nodeName, bool isFilePath, bool isSeqList)
        {
            ISequence referenceSeq;
            ISequence querySeq;
            IList<ISequence> querySeqs = new List<ISequence>();
            string referenceSequence;
            string querySequence;
            IList<IPairwiseSequenceAlignment> align;

            if (isFilePath)
            {
                // Gets the reference sequence from the configuration file
                string filePath = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);

                Assert.IsNotNull(filePath);
                ApplicationLog.WriteLine(string.Format(null, "MUMmer BVT : Successfully validated the File Path '{0}'.", filePath));

                FastAParser parser = new FastAParser();
                IEnumerable<ISequence> referenceSeqs = parser.Parse(filePath);
                referenceSeq = referenceSeqs.FirstOrDefault();
                Assert.IsNotNull(referenceSeq);
                referenceSequence = referenceSeq.ConvertToString();
                parser.Close();

                // Gets the reference sequence from the configuration file
                string queryFilePath = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SearchSequenceFilePathNode);

                Assert.IsNotNull(queryFilePath);
                ApplicationLog.WriteLine(string.Format(null, "MUMmer BVT : Successfully validated the Search File Path '{0}'.", queryFilePath));

                FastAParser queryParserObj = new FastAParser();
                querySeqs = queryParserObj.Parse(queryFilePath).ToList();
                querySeq = querySeqs.FirstOrDefault();
                Assert.IsNotNull(querySeq);
                querySequence = querySeq.ConvertToString();
                queryParserObj.Close();
            }
            else
            {
                // Gets the reference sequence from the configuration file
                referenceSequence = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SequenceNode);
                string referenceSeqAlphabet = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.AlphabetNameNode);
                referenceSeq = new Sequence(Utility.GetAlphabet(referenceSeqAlphabet), referenceSequence);
                
                querySequence = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SearchSequenceNode);
                referenceSeqAlphabet = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SearchSequenceAlphabetNode);
                querySeq = new Sequence(Utility.GetAlphabet(referenceSeqAlphabet), querySequence);
                querySeqs = new List<ISequence> {querySeq};
            }

            string mumLength = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMAlignLengthNode);

            var mumAlignObj = new Bio.Algorithms.MUMmer.MUMmerAligner
            {
                LengthOfMUM = long.Parse(mumLength, null),
                GapOpenCost = int.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.GapOpenCostNode), null)
            };

            if (isSeqList)
            {
                querySeqs.Add(referenceSeq);
                align = mumAlignObj.Align(querySeqs);
            }
            else
            {
                align = mumAlignObj.AlignSimple(referenceSeq, querySeqs);
            }

            string expectedScore = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ScoreNodeName);
            Assert.AreEqual(expectedScore, align[0].PairwiseAlignedSequences[0].Score.ToString((IFormatProvider)null));
            ApplicationLog.WriteLine(string.Format(null, "MUMmer BVT : Successfully validated the score for the sequence '{0}' and '{1}'.", referenceSequence, querySequence));

            string[] expectedSequences = this.utilityObj.xmlUtil.GetTextValues(nodeName, Constants.ExpectedSequencesNode);
            IList<IPairwiseSequenceAlignment> expectedOutput = new List<IPairwiseSequenceAlignment>();

            IPairwiseSequenceAlignment seqAlign = new PairwiseSequenceAlignment();
            var alignedSeq = new PairwiseAlignedSequence
            {
                FirstSequence = new Sequence(referenceSeq.Alphabet, expectedSequences[0]),
                SecondSequence = new Sequence(referenceSeq.Alphabet, expectedSequences[1]),
                Score = Convert.ToInt32(expectedScore, null),
                FirstOffset = Int32.MinValue,
                SecondOffset = Int32.MinValue
            };
            seqAlign.PairwiseAlignedSequences.Add(alignedSeq);
            expectedOutput.Add(seqAlign);
            Assert.IsTrue(CompareAlignment(align, expectedOutput));
            ApplicationLog.WriteLine("MUMmer BVT : Successfully validated the aligned sequences.");
        }

        /// <summary>
        /// Validates the Unique Matches for the input provided.
        /// </summary>
        /// <param name="matches">Max Unique Match list</param>
        /// <param name="nodeName">Node name to be read from xml</param>
        /// <param name="lisActionType"></param>
        /// <returns>True, if successfully validated</returns>        
        bool ValidateUniqueMatches(IEnumerable<Match> matches, string nodeName, LISParameters lisActionType)
        {
            // Gets all the unique matches properties to be validated as in xml.
            string[] firstSeqStart = null;
            string[] length = null;
            string[] secondSeqStart = null;

            switch (lisActionType)
            {
                case LISParameters.PerformLIS:
                    firstSeqStart = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                        Constants.LisFirstSequenceStartNode).Split(',');
                    length = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.LisLengthNode).Split(',');
                    secondSeqStart = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                        Constants.LisSecondSequenceStartNode).Split(',');
                    break;
                case LISParameters.FindUniqueMatches:
                    firstSeqStart = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                        Constants.FirstSequenceStartNode).Split(',');
                    length = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.LengthNode).Split(',');
                    secondSeqStart = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                        Constants.SecondSequenceStartNode).Split(',');
                    break;
                default:
                    break;
            }

            int i = 0;
            // Loops through all the matches and validates the same.            
            foreach (Match match in matches)
            {
                if ((0 != string.Compare(firstSeqStart[i],
                    match.ReferenceSequenceOffset.ToString((IFormatProvider)null), true, CultureInfo.CurrentCulture))
                    || (0 != string.Compare(length[i],
                    match.Length.ToString((IFormatProvider)null), true, CultureInfo.CurrentCulture))
                    || (0 != string.Compare(secondSeqStart[i],
                    match.QuerySequenceOffset.ToString((IFormatProvider)null), true, CultureInfo.CurrentCulture)))
                {
                    ApplicationLog.WriteLine(string.Format("MUMmer BVT : Unique match not matching at index '{0}'", i.ToString((IFormatProvider)null)));
                    return false;
                }
                i++;
            }

            return true;
        }

        /// <summary>
        /// Validate the Mummer GetMUMs method for different test cases.
        /// </summary>
        /// <param name="nodeName">Name of the XML node to be read.</param>
        /// <param name="isFilePath">Is Sequence saved in File</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        void ValidateMUMsGeneralTestCases(string nodeName, bool isFilePath)
        {
            ISequence referenceSeq = null;
            ISequence querySeq = null;
            IEnumerable<ISequence> querySeqs = null;
            string referenceSequence = string.Empty;
            string querySequence = string.Empty;
            IEnumerable<ISequence> referenceSeqs = null;

            if (isFilePath)
            {
                // Gets the reference sequence from the configurtion file
                string filePath = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.FilePathNode);

                Assert.IsNotNull(filePath);
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "MUMmer BVT : Successfully validated the File Path '{0}'.", filePath));

                FastAParser parser = new FastAParser();
                parser.Alphabet = Alphabets.DNA;
                referenceSeqs = parser.Parse(filePath);
                referenceSeq = referenceSeqs.ElementAt(0);
                referenceSequence = new string(referenceSeq.Select(a => (char)a).ToArray());

                // Gets the reference sequence from the configuration file
                string queryFilePath = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.SearchSequenceFilePathNode);

                Assert.IsNotNull(queryFilePath);
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "MUMmer BVT : Successfully validated the Search File Path '{0}'.", queryFilePath));

                querySeqs = parser.Parse(queryFilePath);
                querySeq = querySeqs.ElementAt(0);
                querySequence = new string(querySeq.Select(a => (char)a).ToArray());
            }
            else
            {
                // Gets the reference sequence from the configurtion file
                referenceSequence = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.SequenceNode);

                string referenceSeqAlphabet = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.AlphabetNameNode);

                referenceSeq = new Sequence(Utility.GetAlphabet(referenceSeqAlphabet),
                    this.encodingObj.GetBytes(referenceSequence));

                querySequence = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.SearchSequenceNode);

                referenceSeqAlphabet = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.SearchSequenceAlphabetNode);

                querySeq = new Sequence(Utility.GetAlphabet(referenceSeqAlphabet),
                    this.encodingObj.GetBytes(querySequence));

                querySeqs = new List<ISequence>() { querySeq };
            }

            string mumLength = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMLengthNode);

            Bio.Algorithms.MUMmer.MUMmer mum = new Bio.Algorithms.MUMmer.MUMmer(referenceSeq as Sequence);
            mum.LengthOfMUM = long.Parse(mumLength, null);
            IEnumerable<Match> actualResult = null;
            actualResult = mum.GetMatchesUniqueInReference(querySeqs.ElementAt(0));

            // Validate MUMs output.
            Assert.IsTrue(this.ValidateMums(nodeName, actualResult));

            ApplicationLog.WriteLine("MUMmer BVT : Successfully validated the Mumms.");
        }

        /// <summary>
        /// Validate the Mums output.
        /// </summary>
        /// <param name="result">Mumms Output</param>
        /// <param name="nodeName">Node name to be read from xml</param>       
        bool ValidateMums(string nodeName, IEnumerable<Match> result)
        {
            string[] firstSeqStart = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.LisFirstSequenceStartNode).Split(',');
            string[] length = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.LisLengthNode).Split(',');
            string[] secondSeqStart = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.LisSecondSequenceStartNode).Split(',');

            var mums = result;
            for (int i = 0; i < mums.Count(); i++)
            {
                if ((0 != string.Compare(firstSeqStart[i], mums.ElementAt(i).ReferenceSequenceOffset.ToString((IFormatProvider)null), StringComparison.CurrentCulture))
                   || (0 != string.Compare(length[i], mums.ElementAt(i).Length.ToString((IFormatProvider)null), StringComparison.CurrentCulture))
                   || (0 != string.Compare(secondSeqStart[i], mums.ElementAt(i).QuerySequenceOffset.ToString((IFormatProvider)null), StringComparison.CurrentCulture)))
                {
                    ApplicationLog.WriteLine(string.Format((IFormatProvider)null, "MUMmer P1 : There is no match at '{0}'", i.ToString((IFormatProvider)null)));
                    return false;
                }

            }

            return true;
        }

        /// <summary>
        /// Validates most of the find matches suffix tree test cases with varying parameters.
        /// </summary>
        /// <param name="nodeName">Node name which needs to be read for execution.</param>
        /// <param name="isFilePath">Is File Path?</param>
        /// <param name="LISActionType">LIS action type enum</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        void ValidateFindMatchSimpleSuffixGeneralTestCases(string nodeName, bool isFilePath)
        {
            ISequence referenceSeq = null;
            ISequence querySeq = null;
            string referenceSequence = string.Empty;
            string querySequence = string.Empty;
            IEnumerable<ISequence> referenceSeqs = null;

            if (isFilePath)
            {
                // Gets the reference sequence from the configurtion file
                string filePath = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.FilePathNode);

                Assert.IsNotNull(filePath);
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "MUMmer BVT : Successfully validated the File Path '{0}'.", filePath));

                FastAParser parser = new FastAParser();
                referenceSeqs = parser.Parse(filePath);
                referenceSeq = referenceSeqs.ElementAt(0);
                referenceSequence = new string(referenceSeq.Select(a => (char)a).ToArray());

                // Gets the reference sequence from the configurtion file
                string queryFilePath = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.SearchSequenceFilePathNode);

                Assert.IsNotNull(queryFilePath);
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "MUMmer BVT : Successfully validated the Search File Path '{0}'.", queryFilePath));

                IEnumerable<ISequence> querySeqs = null;
                querySeqs = parser.Parse(queryFilePath);
                querySeq = querySeqs.ElementAt(0);
                querySequence = new string(querySeq.Select(a => (char)a).ToArray());
            }
            else
            {
                // Gets the reference sequence from the configuration file
                referenceSequence = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.SequenceNode);

                string seqAlp = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.AlphabetNameNode);

                referenceSeq = new Sequence(Utility.GetAlphabet(seqAlp),
                    this.encodingObj.GetBytes(referenceSequence));

                querySequence = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.SearchSequenceNode);

                seqAlp = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.SearchSequenceAlphabetNode);

                querySeq = new Sequence(Utility.GetAlphabet(seqAlp),
                    this.encodingObj.GetBytes(querySequence));
            }

            string mumLength = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMLengthNode);

            // Builds the suffix for the reference sequence passed.

            MultiWaySuffixTree suffixTreeBuilder = new MultiWaySuffixTree(referenceSeq as Sequence);
            suffixTreeBuilder.MinLengthOfMatch = long.Parse(mumLength, null);
            IEnumerable<Match> matches = null;
            matches = suffixTreeBuilder.SearchMatchesUniqueInReference(querySeq);

            // Validates the Unique Matches.
            ApplicationLog.WriteLine("MUMmer BVT : Validating the Unique Matches");
            Assert.IsTrue(this.ValidateUniqueMatches(matches, nodeName, LISParameters.FindUniqueMatches));

            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "MUMmer BVT : Successfully validated the all the unique matches for the sequence '{0}' and '{1}'.",
                referenceSequence, querySequence));
        }

        /// <summary>
        /// Validates SearchMatch() with different inputs.
        /// </summary>
        /// <param name="nodeName">Parent Node from Xml.</param>
        void ValidateSearchMatch(string nodeName)
        {
            string referenceSequence = string.Empty;
            string querySequence = string.Empty;
            string seqAlp = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.AlphabetNameNode);

            // Gets the reference sequence from the configurtion file
            referenceSequence = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.SequenceNode);

            querySequence = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.SearchSequenceNode);

            seqAlp = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.SearchSequenceAlphabetNode);

            IEnumerable<Match> matches = null;
            string mumLength = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMLengthNode);
            Sequence referenceSequenceForMatches = new Sequence(Utility.GetAlphabet(seqAlp), referenceSequence);
            MultiWaySuffixTree suffixTree = new MultiWaySuffixTree(referenceSequenceForMatches);
            suffixTree.MinLengthOfMatch = long.Parse(mumLength, null);
            Sequence querySequenceForMatches = new Sequence(Utility.GetAlphabet(seqAlp), querySequence);
            matches = suffixTree.SearchMatches(querySequenceForMatches);
            // Validates the Unique Matches.
            ApplicationLog.WriteLine("MUMmer BVT : Validating the Unique Matches");
            Assert.IsTrue(this.ValidateUniqueMatches(matches, nodeName, LISParameters.FindUniqueMatches));

            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "MUMmer BVT : Successfully validated the all the unique matches for the sequence '{0}' and '{1}'.",
                referenceSequence, querySequence));
        }

        /// <summary>
        /// Validates Longest Increasing sequences.
        /// </summary>
        /// <param name="nodeName">Node name which needs to be read for execution.</param>
        /// <param name="isFilePath">Is File Path?</param>
        void ValidateLongestIncreasingSubsequenceTestCases(string nodeName, bool isFilePath)
        {
            ISequence referenceSeq = null;
            ISequence querySeq = null;
            string referenceSequence = string.Empty;
            string querySequence = string.Empty;
            IEnumerable<ISequence> referenceSeqs = null;

            if (isFilePath)
            {
                // Gets the reference sequence from the configurtion file
                string filePath = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.FilePathNode);

                Assert.IsNotNull(filePath);
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "MUMmer BVT : Successfully validated the File Path '{0}'.", filePath));

                FastAParser parser = new FastAParser();
                referenceSeqs = parser.Parse(filePath);
                referenceSeq = referenceSeqs.ElementAt(0);
                referenceSequence = new string(referenceSeq.Select(a => (char)a).ToArray());

                // Gets the reference sequence from the configuration file
                string queryFilePath = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.SearchSequenceFilePathNode);

                Assert.IsNotNull(queryFilePath);
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "MUMmer BVT : Successfully validated the Search File Path '{0}'.", queryFilePath));

                IEnumerable<ISequence> querySeqs = null;
                querySeqs = parser.Parse(queryFilePath);
                querySeq = querySeqs.ElementAt(0);
                querySequence = new string(querySeq.Select(a => (char)a).ToArray());
            }
            else
            {
                // Gets the reference sequence from the configuration file
                referenceSequence = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.SequenceNode);

                string seqAlp = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.AlphabetNameNode);

                referenceSeq = new Sequence(Utility.GetAlphabet(seqAlp),
                    referenceSequence);

                querySequence = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.SearchSequenceNode);

                seqAlp = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.SearchSequenceAlphabetNode);

                querySeq = new Sequence(Utility.GetAlphabet(seqAlp),
                    querySequence);
            }

            string mumLength = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMLengthNode);

            IEnumerable<Match> matches;
            Bio.Algorithms.MUMmer.MUMmer mum = new Bio.Algorithms.MUMmer.MUMmer(referenceSeq as Sequence);
            mum.LengthOfMUM = long.Parse(mumLength);
            matches = mum.GetMatchesUniqueInReference(querySeq);

            // Validates the Unique Matches.
            ApplicationLog.WriteLine("MUMmer BVT : Validating the Unique Matches using LIS");
            LongestIncreasingSubsequence lisObj = new LongestIncreasingSubsequence();

            List<Match> listMatch = new List<Match>();

            foreach (Match mtch in matches)
            {
                listMatch.Add(mtch);
            }

            IList<Match> lisSorted = null, actualLis = null;
            lisSorted = lisObj.SortMum(listMatch);
            actualLis = lisObj.GetLongestSequence(lisSorted);
            Assert.IsTrue(this.ValidateUniqueMatches(actualLis, nodeName, LISParameters.PerformLIS));

            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "MUMmer BVT : Successfully validated the all the unique matches for the sequence '{0}' and '{1}'.",
                referenceSequence, querySequence));
        }

        /// <summary>
        /// Compare the alignment of mummer and defined alignment
        /// </summary>
        /// <param name="actualAlignment"></param>
        /// <param name="expectedAlignment">expected output</param>
        /// <returns>Compare result of alignments</returns>
        private static bool CompareAlignment(IList<IPairwiseSequenceAlignment> actualAlignment,
             IList<IPairwiseSequenceAlignment> expectedAlignment)
        {
            return AlignmentHelpers.CompareAlignment(actualAlignment, expectedAlignment);
        }

        #endregion Supported Methods
    }
}


