using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using Bio.Algorithms.Alignment;
using Bio.Algorithms.MUMmer;
using Bio.Algorithms.SuffixTree;
using Bio.Extensions;
using Bio.IO.FastA;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;

using NUnit.Framework;

namespace Bio.Tests.Algorithms.Alignment.NUCmer
{
    /// <summary>
    /// NUCmer Bvt Test case implementation.
    /// </summary>
    [TestFixture]
    public class NUCmerBvtTestCases
    {
        /// <summary>
        /// Lis Parameters which are used for different test cases 
        /// based on which the test cases are executed.
        /// </summary>
        enum AdditionalParameters
        {
            FindUniqueMatches,
            PerformClusterBuilder
        };

        readonly Utility utilityObj = new Utility(@"TestUtils\NUCmerTestsConfig.xml");
        readonly ASCIIEncoding encodingObj = new ASCIIEncoding();

        #region Suffix Tree Test Cases

        /// <summary>
        /// Validate FindMatches() method with one line sequences
        /// for both reference and query parameter and validate
        /// the unique matches
        /// Input : One line sequence for both reference and query parameter
        /// Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void SuffixTreeFindMatchesOneLineSequence()
        {
            this.ValidateFindMatchSuffixGeneralTestCases(Constants.OneLineSequenceNodeName,
                false, AdditionalParameters.FindUniqueMatches);
        }

        /// <summary>
        /// Validate FindMatches() method with small size (less than 35kb) sequences 
        /// for reference and query parameter and validate
        /// the unique matches
        /// Input : Small size sequence for both reference and query parameter
        /// Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void SuffixTreeFindMatchesSmallSizeSequence()
        {
            this.ValidateFindMatchSuffixGeneralTestCases(Constants.SmallSizeSequenceNodeName, true,
                AdditionalParameters.FindUniqueMatches);
        }

        /// <summary>
        /// Validate BuildCluster() method with one unique match
        /// and validate the clusters
        /// Input : one unique matches
        /// Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ClusterBuilderOneUniqueMatches()
        {
            this.ValidateFindMatchSuffixGeneralTestCases(Constants.OneUniqueMatchSequenceNodeName,
                false, AdditionalParameters.PerformClusterBuilder);
        }

        /// <summary>
        /// Validate BuildCluster() method with two unique match
        /// without cross overlap and validate the clusters
        /// Input : two unique matches with out cross overlap
        /// Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ClusterBuilderTwoUniqueMatchesWithoutCrossOverlap()
        {
            this.ValidateFindMatchSuffixGeneralTestCases(
                Constants.TwoUniqueMatchWithoutCrossOverlapSequenceNodeName,
                false, AdditionalParameters.PerformClusterBuilder);
        }

        /// <summary>
        /// Validate BuildCluster() method with two unique match
        /// with cross overlap and validate the clusters
        /// Input : two unique matches with cross overlap
        /// Validation : Validate the unique matches
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ClusterBuilderTwoUniqueMatchesWithCrossOverlap()
        {
            this.ValidateFindMatchSuffixGeneralTestCases(
                Constants.TwoUniqueMatchWithCrossOverlapSequenceNodeName,
                false, AdditionalParameters.PerformClusterBuilder);
        }

        #endregion Suffix Tree Test Cases

        #region NUCmer Align Test Cases

        /// <summary>
        /// Validate Align() method with one line sequence 
        /// and validate the aligned sequences
        /// Input : One line sequence
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void NUCmerAlignOneLineSequence()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.OneLineSequenceNodeName, false);
        }

        /// <summary>
        /// Validate Align() method with small size (less than 35kb) sequence 
        /// and validate the aligned sequences
        /// Input : small size sequence file
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void NUCmerAlignSmallSizeSequence()
        {
            this.ValidateNUCmerAlignGeneralTestCases(Constants.SmallSizeSequenceNodeName, true);
        }

        /// <summary>
        /// Validate Align(seq, seqList) method with small size (less than 35kb) sequence 
        /// and validate the aligned sequences
        /// Input : small size sequence file
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void NUCmerAlignSmallSizeAlignSequence()
        {
            // Gets the reference sequence from the FastA file
            string filePath = this.utilityObj.xmlUtil.GetTextValue(Constants.SmallSizeSequenceNodeName,
                Constants.FilePathNode);

            Assert.IsNotNull(filePath);

            FastAParser parser = new FastAParser();
            IEnumerable<ISequence> refSeqList = parser.Parse(filePath);

            // Gets the query sequence from the FastA file
            string queryFilePath = this.utilityObj.xmlUtil.GetTextValue(Constants.SmallSizeSequenceNodeName,
                Constants.SearchSequenceFilePathNode);

            Assert.IsNotNull(queryFilePath);

            FastAParser queryParser = new FastAParser();
            IEnumerable<ISequence> searchSeqList = queryParser.Parse(queryFilePath);

            string mumLength = this.utilityObj.xmlUtil.GetTextValue(Constants.SmallSizeSequenceNodeName, Constants.MUMAlignLengthNode);

            NucmerPairwiseAligner nucmerObj = new NucmerPairwiseAligner
                {
                    MaximumSeparation = 0,
                    MinimumScore = 2,
                    SeparationFactor = 0.12f,
                    BreakLength = 2,
                    LengthOfMUM = long.Parse(mumLength, null)
                };

            IList<ISequenceAlignment> align = nucmerObj.Align(refSeqList.ElementAt(0), searchSeqList);

            string expectedSequences = this.utilityObj.xmlUtil.GetFileTextValue(Constants.SmallSizeSequenceNodeName,
                Constants.ExpectedSequencesNode);

            string[] expSeqArray = expectedSequences.Split(',');

            int j = 0;

            // Gets all the aligned sequences in comma separated format
            foreach (IPairwiseSequenceAlignment seqAlignment in align)
            {
                foreach (PairwiseAlignedSequence alignedSeq in seqAlignment)
                {
                    Assert.AreEqual(expSeqArray[j], alignedSeq.FirstSequence.ConvertToString());
                    ++j;
                    Assert.AreEqual(expSeqArray[j], alignedSeq.SecondSequence.ConvertToString());
                    j++;
                }
            }
        }

        /// <summary>
        /// Validate Align() method with one line sequence 
        /// with cross over lap and validate the aligned sequences
        /// Input : One line sequence
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void NUCmerAlignSequenceWithCrossOverlap()
        {
            this.ValidateNUCmerAlignGeneralTestCases(
                Constants.TwoUniqueMatchWithCrossOverlapSequenceNodeName, false);
        }

        /// <summary>
        /// Validate All properties in NUCmer class
        /// Input : Create a NUCmer object.
        /// Validation : Validate the properties
        /// </summary>
        [Category("Priority0")]
        public void NUCmerAlignerProperties()
        {
            NucmerPairwiseAligner nucmerObj = new NucmerPairwiseAligner();
            Assert.AreEqual(Constants.NUCLength, nucmerObj.LengthOfMUM.ToString((IFormatProvider)null));
            Assert.AreEqual(200, nucmerObj.BreakLength);
            Assert.AreEqual(-8, nucmerObj.GapExtensionCost);
            Assert.AreEqual(-13, nucmerObj.GapOpenCost);
            Assert.AreEqual(Constants.NUCFixedSeperation, nucmerObj.FixedSeparation.ToString(CultureInfo.InvariantCulture));
            Assert.AreEqual(Constants.NUCMaximumSeparation, nucmerObj.MaximumSeparation.ToString(CultureInfo.InvariantCulture));
            Assert.AreEqual(Constants.NUCMinimumScore, nucmerObj.MinimumScore.ToString(CultureInfo.InvariantCulture));
            Assert.AreEqual(Constants.NUCSeparationFactor, nucmerObj.SeparationFactor.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Validate GetClusters() method by passing valid values.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateNUCmerGetClusters()
        {
            // NOTE: Nigel ran this test with the same data through mmummer and mgaps and got the same result.

            // Gets the reference sequence from the FastA file
            string filePath = this.utilityObj.xmlUtil.GetTextValue(Constants.MediumSizeSequenceNodeName,
                Constants.FilePathNode);

            // Gets the query sequence from the FastA file
            string queryFilePath = this.utilityObj.xmlUtil.GetTextValue(Constants.MediumSizeSequenceNodeName,
                Constants.SearchSequenceFilePathNode);

            FastAParser parser = new FastAParser();
            IEnumerable<ISequence> seqs1 = parser.Parse(filePath);
            IEnumerable<ISequence> seqs2 = parser.Parse(queryFilePath);
            var nuc = new Bio.Algorithms.Alignment.NUCmer(seqs1.First()) {
                LengthOfMUM = 5,
                MinimumScore = 0,
            };
            var clusts = nuc.GetClusters(seqs2.First());
            string clustCount1 = this.utilityObj.xmlUtil.GetTextValue(
                Constants.MediumSizeSequenceNodeName, Constants.ClustCount1Node);

            Assert.AreEqual(clustCount1, clusts.Count.ToString(CultureInfo.InvariantCulture));
        }

        #endregion NUCmer Align Test Cases

        #region NUCmer Simple Align Test Cases

        /// <summary>
        /// Validate SimpleAlign() method with one line Dna list of sequence 
        /// and validate the aligned sequences
        /// Input : One line Dna list of sequence
        /// Validation : Validate the aligned sequences
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void NUCmerAlignSimpleOneLineDnaListOfSequence()
        {
            this.ValidateNUCmerAlignSimpleGeneralTestCases(Constants.SingleDnaNucmerSequenceNodeName, true);
        }

        /// <summary>
        /// Validate SimpleAlign() method with one line Rna list of sequence 
        /// and validate the aligned sequences
        /// Input : One line Rna list of sequence
        /// Validation : Validate the aligned sequences
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void NUCmerAlignSimpleOneLineRnaListOfSequence()
        {
            this.ValidateNUCmerAlignSimpleGeneralTestCases(Constants.SingleRnaNucmerSequenceNodeName, true);
        }

        #endregion NUCmer Simple Align Test Cases

        #region Supported Methods

        /// <summary>
        /// Validates most of the find matches suffix tree test cases with varying parameters.
        /// </summary>
        /// <param name="nodeName">Node name which needs to be read for execution.</param>
        /// <param name="isFilePath">Is File Path?</param>
        /// <param name="additionalParam">LIS action type enum</param>
        void ValidateFindMatchSuffixGeneralTestCases(string nodeName, bool isFilePath,
            AdditionalParameters additionalParam)
        {
            ISequence referenceSeqs;
            var searchSeqList = new List<ISequence>();

            if (isFilePath)
            {
                // Gets the reference sequence from the FastA file
                string filePath = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.FilePathNode);

                Assert.IsNotNull(filePath);
                
                FastAParser parser = new FastAParser();
                IEnumerable<ISequence> referenceSeqList = parser.Parse(filePath);
                List<Byte> byteList = new List<Byte>();
                foreach (ISequence seq in referenceSeqList)
                {
                    byteList.AddRange(seq);
                    byteList.Add((byte)'+');
                }
                referenceSeqs = new Sequence(referenceSeqList.First().Alphabet.GetMummerAlphabet(), byteList.ToArray());

                // Gets the query sequence from the FastA file
                string queryFilePath = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.SearchSequenceFilePathNode);

                Assert.IsNotNull(queryFilePath);

                IEnumerable<ISequence> querySeqList = parser.Parse(queryFilePath);
                searchSeqList.AddRange(querySeqList);
            }
            else
            {
                // Gets the reference & search sequences from the configuration file
                string[] referenceSequences = this.utilityObj.xmlUtil.GetTextValues(nodeName,
                    Constants.ReferenceSequencesNode);
                string[] searchSequences = this.utilityObj.xmlUtil.GetTextValues(nodeName,
                    Constants.SearchSequencesNode);

                IAlphabet seqAlphabet = Utility.GetAlphabet(this.utilityObj.xmlUtil.GetTextValue(nodeName,
                       Constants.AlphabetNameNode));

                List<ISequence> refSeqList = referenceSequences.Select(t => new Sequence(seqAlphabet, this.encodingObj.GetBytes(t))).Cast<ISequence>().ToList();

                List<Byte> byteListQuery = new List<Byte>();
                foreach (ISequence seq in refSeqList)
                {
                    byteListQuery.AddRange(seq);
                    byteListQuery.Add((byte)'+');
                }
                referenceSeqs = new Sequence(refSeqList.First().Alphabet.GetMummerAlphabet(),
                    byteListQuery.ToArray());

                searchSeqList.AddRange(searchSequences.Select(t => new Sequence(seqAlphabet, this.encodingObj.GetBytes(t))).Cast<ISequence>());
            }

            string mumLength = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMLengthNode);

            // Builds the suffix for the reference sequence passed.           
            MultiWaySuffixTree suffixTreeBuilder = new MultiWaySuffixTree(referenceSeqs as Sequence)
                {
                    MinLengthOfMatch = long.Parse(mumLength, null)
                };

            var matches = new Dictionary<ISequence, IEnumerable<Match>>();
            foreach (ISequence sequence in searchSeqList)
            {
                matches.Add(sequence,
                    suffixTreeBuilder.SearchMatchesUniqueInReference(sequence));
            }

            List<Match> mums = new List<Match>();
            foreach (var a in matches.Values)
            {
                mums.AddRange(a);
            }

            switch (additionalParam)
            {
                case AdditionalParameters.FindUniqueMatches:
                    // Validates the Unique Matches.
                    Assert.IsTrue(this.ValidateUniqueMatches(mums, nodeName, additionalParam, isFilePath));
                    break;
                case AdditionalParameters.PerformClusterBuilder:
                    // Validates the Unique Matches.
                    Assert.IsTrue(this.ValidateUniqueMatches(mums, nodeName, additionalParam, isFilePath));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Validates the NUCmer align method for several test cases for the parameters passed.
        /// </summary>
        /// <param name="nodeName">Node name to be read from xml</param>
        /// <param name="isFilePath">Is Sequence saved in File</param>
        void ValidateNUCmerAlignGeneralTestCases(string nodeName, bool isFilePath)
        {
            IList<ISequence> refSeqList, searchSeqList;

            if (isFilePath)
            {
                // Gets the reference sequence from the FastA file
                string filePath = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
                Assert.IsNotNull(filePath);
                Assert.IsTrue(File.Exists(filePath));
                ApplicationLog.WriteLine(string.Format(null, "NUCmer BVT : Successfully validated the File Path '{0}'.", filePath));

                FastAParser parser = new FastAParser();
                refSeqList = parser.Parse(filePath).ToList();

                // Gets the query sequence from the FastA file
                string queryFilePath = this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SearchSequenceFilePathNode);
                Assert.IsNotNull(queryFilePath);
                Assert.IsTrue(File.Exists(queryFilePath));
                ApplicationLog.WriteLine(string.Format(null, "NUCmer BVT : Successfully validated the File Path '{0}'.", queryFilePath));

                searchSeqList = parser.Parse(queryFilePath).ToList();
            }
            else
            {
                // Gets the reference & search sequences from the configuration file
                IAlphabet seqAlphabet = Utility.GetAlphabet(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.AlphabetNameNode));

                string[] sequences = this.utilityObj.xmlUtil.GetTextValues(nodeName, Constants.ReferenceSequencesNode);
                refSeqList = sequences.Select((t, i) => new Sequence(seqAlphabet, this.encodingObj.GetBytes(t)) {ID = "ref " + i}).Cast<ISequence>().ToList();

                sequences = this.utilityObj.xmlUtil.GetTextValues(nodeName, Constants.SearchSequencesNode);
                searchSeqList = sequences.Select((t, i) => new Sequence(seqAlphabet, this.encodingObj.GetBytes(t)) { ID = "qry " + i }).Cast<ISequence>().ToList();
            }

            var aligner = new NucmerPairwiseAligner
            {
                MaximumSeparation = 0,
                MinimumScore = 2,
                SeparationFactor = 0.12f,
                BreakLength = 2,
                LengthOfMUM = long.Parse(this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMAlignLengthNode), null)
            };

            IList<ISequenceAlignment> align = aligner.Align(refSeqList, searchSeqList);

            string expectedSequences = isFilePath
                    ? this.utilityObj.xmlUtil.GetFileTextValue(nodeName, Constants.ExpectedSequencesNode)
                    : this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequencesNode);

            string[] expSeqArray = expectedSequences.Split(',');

            // Gets all the aligned sequences in comma separated format
            foreach (IPairwiseSequenceAlignment seqAlignment in align)
            {
                foreach (PairwiseAlignedSequence alignedSeq in seqAlignment)
                {
                    string actualStr = alignedSeq.FirstSequence.ConvertToString();
                    Assert.IsTrue(expSeqArray.Contains(actualStr));

                    actualStr = alignedSeq.SecondSequence.ConvertToString();
                    Assert.IsTrue(expSeqArray.Contains(actualStr));
                }
            }
        }

        /// <summary>
        /// Validates the Unique Matches for the input provided.
        /// </summary>
        /// <param name="matches">Max Unique Match list</param>
        /// <param name="nodeName">Node name to be read from xml</param>
        /// <param name="additionalParam">Unique Match/Sub level LIS/LIS</param>
        /// <param name="isFilePath">Nodes to be read from Text file?</param>
        /// <returns>True, if successfully validated</returns>
        bool ValidateUniqueMatches(IList<Match> matches,
            string nodeName, AdditionalParameters additionalParam, bool isFilePath)
        {
            switch (additionalParam)
            {
                case AdditionalParameters.PerformClusterBuilder:
                    // Validates the Cluster builder MUMs
                    string firstSeqOrderExpected =
                        this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ClustFirstSequenceStartNode);
                    string lengthExpected =
                        this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ClustLengthNode);
                    string secondSeqOrderExpected =
                        this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ClustSecondSequenceStartNode);

                    StringBuilder firstSeqOrderActual = new StringBuilder();
                    StringBuilder lengthActual = new StringBuilder();
                    StringBuilder secondSeqOrderActual = new StringBuilder();

                    ClusterBuilder cb = new ClusterBuilder { MinimumScore = 0 };

                    List<MatchExtension> meObj = matches.Select(m => new MatchExtension(m)).ToList();

                    // Order the mum list with query sequence order and
                    // Assign query sequence to the MUM's
                    for (int index = 0; index < meObj.Count(); index++)
                    {
                        meObj.ElementAt(index).ReferenceSequenceMumOrder = index + 1;
                        meObj.ElementAt(index).QuerySequenceMumOrder = index + 1;
                    }

                    List<Cluster> clusts = cb.BuildClusters(meObj);

                    foreach (Cluster clust in clusts)
                    {
                        foreach (MatchExtension maxMatchExtension in clust.Matches)
                        {
                            firstSeqOrderActual.Append(maxMatchExtension.ReferenceSequenceMumOrder);
                            secondSeqOrderActual.Append(maxMatchExtension.QuerySequenceMumOrder);
                            lengthActual.Append(maxMatchExtension.Length);
                        }
                    }

                    if ((0 != string.Compare(firstSeqOrderExpected.Replace(",", ""),
                        firstSeqOrderActual.ToString(), true, CultureInfo.CurrentCulture))
                        || (0 != string.Compare(lengthExpected.Replace(",", ""),
                        lengthActual.ToString(), true, CultureInfo.CurrentCulture))
                        || (0 != string.Compare(secondSeqOrderExpected.Replace(",", ""),
                        secondSeqOrderActual.ToString(), true, CultureInfo.CurrentCulture)))
                    {
                        ApplicationLog.WriteLine("NUCmer BVT : Unique match not matching");
                        return false;
                    }
                    break;
                case AdditionalParameters.FindUniqueMatches:
                    // Gets all the unique matches properties to be validated as in xml.
                    string[] firstSeqOrder = null;
                    string[] length = null;
                    string[] secondSeqOrder = null;

                    if (isFilePath)
                    {
                        firstSeqOrder = this.utilityObj.xmlUtil.GetFileTextValue(nodeName,
                            Constants.FirstSequenceMumOrderNode).Split(',');
                        length = this.utilityObj.xmlUtil.GetFileTextValue(nodeName,
                            Constants.LengthNode).Split(',');
                        secondSeqOrder = this.utilityObj.xmlUtil.GetFileTextValue(nodeName,
                            Constants.SecondSequenceMumOrderNode).Split(',');
                    }
                    else
                    {
                        firstSeqOrder = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                            Constants.FirstSequenceMumOrderNode).Split(',');
                        length = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                            Constants.LengthNode).Split(',');
                        secondSeqOrder = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                            Constants.SecondSequenceMumOrderNode).Split(',');
                    }

                    int i = 0;

                    IList<MatchExtension> meNewObj = matches.Select(m => new MatchExtension(m)).ToList();

                    // Order the mum list with query sequence order and
                    // Assign query sequence to the MUM's
                    for (int index = 0; index < meNewObj.Count(); index++)
                    {
                        meNewObj.ElementAt(index).ReferenceSequenceMumOrder = index + 1;
                        meNewObj.ElementAt(index).QuerySequenceMumOrder = index + 1;
                    }

                    // Loops through all the matches and validates the same.
                    foreach (MatchExtension match in meNewObj)
                    {
                        if ((0 != string.Compare(firstSeqOrder[i],
                            match.ReferenceSequenceMumOrder.ToString((IFormatProvider)null), true,
                            CultureInfo.CurrentCulture))
                            || (0 != string.Compare(length[i],
                            match.Length.ToString((IFormatProvider)null), true,
                            CultureInfo.CurrentCulture))
                            || (0 != string.Compare(secondSeqOrder[i],
                            match.QuerySequenceMumOrder.ToString((IFormatProvider)null), true,
                            CultureInfo.CurrentCulture)))
                        {
                            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                                "NUCmer BVT : Unique match not matching at index '{0}'", i.ToString((IFormatProvider)null)));
                            return false;
                        }
                        i++;
                    }
                    break;
                default:
                    break;
            }
            return true;
        }

        /// <summary>
        /// Validates the NUCmer align method for several test cases for the parameters passed.
        /// </summary>
        /// <param name="nodeName">Node name to be read from xml</param>
        /// <param name="isAlignList">Is align method to take list?</param>
        void ValidateNUCmerAlignSimpleGeneralTestCases(string nodeName,
            bool isAlignList)
        {
            string[] referenceSequences = null;
            string[] searchSequences = null;
            List<ISequence> refSeqList = new List<ISequence>();
            List<ISequence> searchSeqList = new List<ISequence>();

            // Gets the reference & search sequences from the configurtion file
            referenceSequences = this.utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.ReferenceSequencesNode);
            searchSequences = this.utilityObj.xmlUtil.GetTextValues(nodeName,
              Constants.SearchSequencesNode);

            IAlphabet seqAlphabet = Utility.GetAlphabet(
                this.utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.AlphabetNameNode));

            for (int i = 0; i < referenceSequences.Length; i++)
            {
                ISequence referSeq = new Sequence(seqAlphabet,
                    this.encodingObj.GetBytes(referenceSequences[i]));
                refSeqList.Add(referSeq);
            }

            for (int i = 0; i < searchSequences.Length; i++)
            {
                ISequence searchSeq = new Sequence(seqAlphabet,
                    this.encodingObj.GetBytes(searchSequences[i]));
                searchSeqList.Add(searchSeq);
            }

            // Gets the mum length from the xml
            string mumLength = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.MUMAlignLengthNode);

            NucmerPairwiseAligner nucmerObj = new NucmerPairwiseAligner();

            // Update other values for NUCmer object
            nucmerObj.MaximumSeparation = int.Parse
                (this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMAlignLengthNode), (IFormatProvider)null);
            nucmerObj.MinimumScore = int.Parse(
                this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMAlignLengthNode), (IFormatProvider)null);
            nucmerObj.SeparationFactor = int.Parse(
                this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMAlignLengthNode), (IFormatProvider)null);
            nucmerObj.BreakLength = int.Parse(
                this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMAlignLengthNode), (IFormatProvider)null);
            nucmerObj.LengthOfMUM = long.Parse(mumLength, null);

            IList<ISequenceAlignment> alignSimple = null;

            if (isAlignList)
            {
                List<ISequence> listOfSeq = new List<ISequence>();
                listOfSeq.Add(refSeqList[0]);
                listOfSeq.Add(searchSeqList[0]);
                alignSimple = nucmerObj.AlignSimple(listOfSeq);
            }

            string expectedSequences = string.Empty;
            expectedSequences = this.utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.ExpectedSequencesNode);

            string[] expSeqArray = expectedSequences.Split(',');

            int j = 0;

            // Gets all the aligned sequences in comma seperated format
            foreach (IPairwiseSequenceAlignment seqAlignment in alignSimple)
            {
                foreach (PairwiseAlignedSequence alignedSeq in seqAlignment)
                {
                    Assert.AreEqual(expSeqArray[j], new string(alignedSeq.FirstSequence.Select(a => (char)a).ToArray()));
                    ++j;
                    Assert.AreEqual(expSeqArray[j], new string(alignedSeq.SecondSequence.Select(a => (char)a).ToArray()));
                    j++;
                }
            }

            ApplicationLog.WriteLine("NUCmer BVT : Successfully validated all the aligned sequences.");
        }

        #endregion Supported Methods
    }
}
