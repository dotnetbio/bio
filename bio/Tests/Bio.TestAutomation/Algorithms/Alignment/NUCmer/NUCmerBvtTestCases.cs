/****************************************************************************
 * NUCmerBvtTestCases.cs
 * 
 *   This file contains the NUCmer Bvt test cases
 * 
***************************************************************************/

using System;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using Bio.Algorithms.Alignment;
using Bio.Algorithms.SuffixTree;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;
using Bio.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.IO.FastA;
using Bio.Algorithms.MUMmer;
using System.IO;

namespace Bio.TestAutomation.Algorithms.Alignment
{
    /// <summary>
    /// NUCmer Bvt Test case implementation.
    /// </summary>
    [TestClass]
    public class NUCmerBvtTestCases
    {

        #region Enums

        /// <summary>
        /// Lis Parameters which are used for different test cases 
        /// based on which the test cases are executed.
        /// </summary>
        enum AdditionalParameters
        {
            FindUniqueMatches,
            PerformClusterBuilder
        };

        #endregion Enums

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\NUCmerTestsConfig.xml");
        ASCIIEncoding encodingObj = new ASCIIEncoding();

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static NUCmerBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region Suffix Tree Test Cases

        /// <summary>
        /// Validate FindMatches() method with one line sequences
        /// for both reference and query parameter and validate
        /// the unique matches
        /// Input : One line sequence for both reference and query parameter
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void SuffixTreeFindMatchesOneLineSequence()
        {
            ValidateFindMatchSuffixGeneralTestCases(Constants.OneLineSequenceNodeName,
                false, AdditionalParameters.FindUniqueMatches);
        }

        /// <summary>
        /// Validate FindMatches() method with small size (less than 35kb) sequences 
        /// for reference and query parameter and validate
        /// the unique matches
        /// Input : Small size sequence for both reference and query parameter
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void SuffixTreeFindMatchesSmallSizeSequence()
        {
            ValidateFindMatchSuffixGeneralTestCases(Constants.SmallSizeSequenceNodeName, true,
                AdditionalParameters.FindUniqueMatches);
        }

        /// <summary>
        /// Validate BuildCluster() method with one unique match
        /// and validate the clusters
        /// Input : one unique matches
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ClusterBuilderOneUniqueMatches()
        {
            ValidateFindMatchSuffixGeneralTestCases(Constants.OneUniqueMatchSequenceNodeName,
                false, AdditionalParameters.PerformClusterBuilder);
        }

        /// <summary>
        /// Validate BuildCluster() method with two unique match
        /// without cross overlap and validate the clusters
        /// Input : two unique matches with out cross overlap
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ClusterBuilderTwoUniqueMatchesWithoutCrossOverlap()
        {
            ValidateFindMatchSuffixGeneralTestCases(
                Constants.TwoUniqueMatchWithoutCrossOverlapSequenceNodeName,
                false, AdditionalParameters.PerformClusterBuilder);
        }

        /// <summary>
        /// Validate BuildCluster() method with two unique match
        /// with cross overlap and validate the clusters
        /// Input : two unique matches with cross overlap
        /// Validation : Validate the unique matches
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ClusterBuilderTwoUniqueMatchesWithCrossOverlap()
        {
            ValidateFindMatchSuffixGeneralTestCases(
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
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void NUCmerAlignOneLineSequence()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.OneLineSequenceNodeName, false);
        }

        /// <summary>
        /// Validate Align() method with small size (less than 35kb) sequence 
        /// and validate the aligned sequences
        /// Input : small size sequence file
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void NUCmerAlignSmallSizeSequence()
        {
            ValidateNUCmerAlignGeneralTestCases(Constants.SmallSizeSequenceNodeName, true);
        }

        /// <summary>
        /// Validate Align(seq, seqList) method with small size (less than 35kb) sequence 
        /// and validate the aligned sequences
        /// Input : small size sequence file
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void NUCmerAlignSmallSizeAlignSequence()
        {
            IEnumerable<ISequence> refSeqList = new List<ISequence>();
            IEnumerable<ISequence> searchSeqList = new List<ISequence>();

            // Gets the reference sequence from the FastA file
            string filePath = utilityObj.xmlUtil.GetTextValue(Constants.SmallSizeSequenceNodeName,
                Constants.FilePathNode);

            Assert.IsNotNull(filePath);
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "NUCmer BVT : Successfully validated the File Path '{0}'.", filePath));

            FastAParser parser = new FastAParser(filePath);
            refSeqList = parser.Parse();

            // Gets the query sequence from the FastA file
            string queryFilePath = utilityObj.xmlUtil.GetTextValue(Constants.SmallSizeSequenceNodeName,
                Constants.SearchSequenceFilePathNode);

            Assert.IsNotNull(queryFilePath);
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "NUCmer BVT : Successfully validated the File Path '{0}'.", queryFilePath));

            FastAParser queryParser = new FastAParser(queryFilePath);

            searchSeqList = queryParser.Parse();

            string mumLength = utilityObj.xmlUtil.GetTextValue(Constants.SmallSizeSequenceNodeName, Constants.MUMAlignLengthNode);

            NucmerPairwiseAligner nucmerObj = new NucmerPairwiseAligner();
            nucmerObj.MaximumSeparation = 0;
            nucmerObj.MinimumScore = 2;
            nucmerObj.SeparationFactor = 0.12f;
            nucmerObj.BreakLength = 2;
            nucmerObj.LengthOfMUM = long.Parse(mumLength, null);

            IList<ISequenceAlignment> align = nucmerObj.Align(refSeqList.ElementAt(0), searchSeqList);

            string expectedSequences = string.Empty;

            expectedSequences = utilityObj.xmlUtil.GetFileTextValue(Constants.SmallSizeSequenceNodeName,
                Constants.ExpectedSequencesNode);

            string[] expSeqArray = expectedSequences.Split(',');

            int j = 0;

            // Gets all the aligned sequences in comma seperated format
            foreach (IPairwiseSequenceAlignment seqAlignment in align)
            {
                foreach (PairwiseAlignedSequence alignedSeq in seqAlignment)
                {
                    Assert.AreEqual(expSeqArray[j], new string(alignedSeq.FirstSequence.Select(a => (char)a).ToArray()));
                    ++j;
                    Assert.AreEqual(expSeqArray[j], new string(alignedSeq.SecondSequence.Select(a => (char)a).ToArray()));
                    j++;
                }
            }

            Console.WriteLine("NUCmer BVT : Successfully validated all the aligned sequences.");
            ApplicationLog.WriteLine("NUCmer BVT : Successfully validated all the aligned sequences.");
        }

        /// <summary>
        /// Validate Align() method with one line sequence 
        /// with cross over lap and validate the aligned sequences
        /// Input : One line sequence
        /// Validation : Validate the aligned sequences.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void NUCmerAlignSequenceWithCrossOverlap()
        {
            ValidateNUCmerAlignGeneralTestCases(
                Constants.TwoUniqueMatchWithCrossOverlapSequenceNodeName, false);
        }

        /// <summary>
        /// Validate All properties in NUCmer class
        /// Input : Create a NUCmer object.
        /// Validation : Validate the properties
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)"), TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void NUCmerAlignerProperties()
        {
            NucmerPairwiseAligner nucmerObj = new NucmerPairwiseAligner();
            Assert.AreEqual(Constants.NUCLength,
                nucmerObj.LengthOfMUM.ToString((IFormatProvider)null));
            Assert.AreEqual(200, nucmerObj.BreakLength);
            Assert.AreEqual(-8, nucmerObj.GapExtensionCost);
            Assert.AreEqual(-13, nucmerObj.GapOpenCost);
            Assert.AreEqual(Constants.NUCFixedSeperation,
                nucmerObj.FixedSeparation.ToString((IFormatProvider)null));
            Assert.AreEqual(Constants.NUCMaximumSeparation,
                nucmerObj.MaximumSeparation.ToString((IFormatProvider)null));
            Assert.AreEqual(Constants.NUCMinimumScore,
                nucmerObj.MinimumScore.ToString((IFormatProvider)null));
            Assert.AreEqual(Constants.NUCSeparationFactor,
                nucmerObj.SeparationFactor.ToString((IFormatProvider)null));
            Console.WriteLine("Successfully validated all the properties of NUCmer Aligner class.");
            ApplicationLog.WriteLine("Successfully validated all the properties of NUCmer Aligner class.");
        }

        /// <summary>
        /// Validate GetClusters() method by passing valid values.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateNUCmerGetClusters()
        {
            // Gets the reference sequence from the FastA file
            string filePath = utilityObj.xmlUtil.GetTextValue(Constants.MediumSizeSequenceNodeName,
                Constants.FilePathNode);

            // Gets the query sequence from the FastA file
            string queryFilePath = utilityObj.xmlUtil.GetTextValue(Constants.MediumSizeSequenceNodeName,
                Constants.SearchSequenceFilePathNode);

            using (FastAParser parser1 = new FastAParser(filePath))
            {
                using (FastAParser parser2 = new FastAParser(queryFilePath))
                {
                    IEnumerable<ISequence> seqs1 = parser1.Parse();
                    IEnumerable<ISequence> seqs2 = parser2.Parse();
                    NUCmer nuc = new NUCmer((Sequence)seqs1.ElementAt(0));
                    nuc.LengthOfMUM = 5;
                    nuc.MinimumScore = 0;
                    nuc.BreakLength = 0;
                    IList<Cluster> clusts = nuc.GetClusters(seqs2.ElementAt(0), true);

                    string clustCount1 = utilityObj.xmlUtil.GetTextValue(Constants.MediumSizeSequenceNodeName,
                Constants.ClustCount1Node);

                    Assert.AreEqual(clustCount1, clusts.Count.ToString((IFormatProvider)null));
                }
            }
        }

        #endregion NUCmer Align Test Cases

        #region NUCmer Simple Align Test Cases

        /// <summary>
        /// Validate SimpleAlign() method with one line Dna list of sequence 
        /// and validate the aligned sequences
        /// Input : One line Dna list of sequence
        /// Validation : Validate the aligned sequences
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void NUCmerAlignSimpleOneLineDnaListOfSequence()
        {
            ValidateNUCmerAlignSimpleGeneralTestCases(Constants.SingleDnaNucmerSequenceNodeName, true);
        }

        /// <summary>
        /// Validate SimpleAlign() method with one line Rna list of sequence 
        /// and validate the aligned sequences
        /// Input : One line Rna list of sequence
        /// Validation : Validate the aligned sequences
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void NUCmerAlignSimpleOneLineRnaListOfSequence()
        {
            ValidateNUCmerAlignSimpleGeneralTestCases(Constants.SingleRnaNucmerSequenceNodeName, true);
        }

        #endregion NUCmer Simple Align Test Cases

        #region Supported Methods

        /// <summary>
        /// Validates most of the find matches suffix tree test cases with varying parameters.
        /// </summary>
        /// <param name="nodeName">Node name which needs to be read for execution.</param>
        /// <param name="isFilePath">Is File Path?</param>
        /// <param name="additionalParam">LIS action type enum</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)")]
        void ValidateFindMatchSuffixGeneralTestCases(string nodeName, bool isFilePath,
            AdditionalParameters additionalParam)
        {
            ISequence referenceSeqs = null;
            string[] referenceSequences = null;
            string[] searchSequences = null;

            List<ISequence> searchSeqList = new List<ISequence>();

            if (isFilePath)
            {
                // Gets the reference sequence from the FastA file
                string filePath = utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.FilePathNode);

                Assert.IsNotNull(filePath);
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "NUCmer BVT : Successfully validated the File Path '{0}'.", filePath));

                using (FastAParser parser = new FastAParser(filePath))
                {
                    IEnumerable<ISequence> referenceSeqList = parser.Parse();
                    List<Byte> byteList = new List<Byte>();
                    foreach (ISequence seq in referenceSeqList)
                    {
                        byteList.AddRange(seq);
                        byteList.Add((byte)'+');
                    }
                    referenceSeqs = new Sequence(referenceSeqList.First().Alphabet.GetMummerAlphabet(),
                        byteList.ToArray());

                    // Gets the query sequence from the FastA file
                    string queryFilePath = utilityObj.xmlUtil.GetTextValue(nodeName,
                        Constants.SearchSequenceFilePathNode);

                    Assert.IsNotNull(queryFilePath);
                    ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                        "NUCmer BVT : Successfully validated the File Path '{0}'.", queryFilePath));

                    FastAParser queryParser = new FastAParser(queryFilePath);
                    IEnumerable<ISequence> querySeqList = queryParser.Parse();

                    foreach (ISequence seq in querySeqList)
                    {
                        searchSeqList.Add(seq);
                    }
                }
            }
            else
            {
                // Gets the reference & search sequences from the configurtion file
                referenceSequences = utilityObj.xmlUtil.GetTextValues(nodeName,
                    Constants.ReferenceSequencesNode);
                searchSequences = utilityObj.xmlUtil.GetTextValues(nodeName,
                  Constants.SearchSequencesNode);

                IAlphabet seqAlphabet = Utility.GetAlphabet(utilityObj.xmlUtil.GetTextValue(nodeName,
                       Constants.AlphabetNameNode));

                List<ISequence> refSeqList = new List<ISequence>();

                for (int i = 0; i < referenceSequences.Length; i++)
                {
                    ISequence referSeq = new Sequence(seqAlphabet, encodingObj.GetBytes(referenceSequences[i]));
                    refSeqList.Add(referSeq);
                }

                List<Byte> byteListQuery = new List<Byte>();
                foreach (ISequence seq in refSeqList)
                {
                    byteListQuery.AddRange(seq);
                    byteListQuery.Add((byte)'+');
                }
                referenceSeqs = new Sequence(refSeqList.First().Alphabet.GetMummerAlphabet(),
                    byteListQuery.ToArray());

                for (int i = 0; i < searchSequences.Length; i++)
                {
                    ISequence searchSeq = new Sequence(seqAlphabet, encodingObj.GetBytes(searchSequences[i]));
                    searchSeqList.Add(searchSeq);
                }
            }

            string mumLength = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMLengthNode);

            // Builds the suffix for the reference sequence passed.           
            MultiWaySuffixTree suffixTreeBuilder = new MultiWaySuffixTree(referenceSeqs as Sequence);
            suffixTreeBuilder.MinLengthOfMatch = long.Parse(mumLength, null);

            Dictionary<ISequence, IEnumerable<Match>> matches =
                new Dictionary<ISequence, IEnumerable<Match>>();

            for (int i = 0; i < searchSeqList.Count; i++)
            {
                matches.Add(searchSeqList[i],
                    suffixTreeBuilder.SearchMatchesUniqueInReference(searchSeqList[i]));
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
                    ApplicationLog.WriteLine("NUCmer BVT : Validating the Unique Matches");
                    Assert.IsTrue(ValidateUniqueMatches(mums, nodeName, additionalParam, isFilePath));
                    Console.WriteLine(
                        "NUCmer BVT : Successfully validated the all the unique matches for the sequences.");
                    break;
                case AdditionalParameters.PerformClusterBuilder:
                    // Validates the Unique Matches.
                    ApplicationLog.WriteLine(
                        "NUCmer BVT : Validating the Unique Matches using Cluster Builder");
                    Assert.IsTrue(ValidateUniqueMatches(mums, nodeName, additionalParam, isFilePath));
                    Console.WriteLine(
                        "NUCmer BVT : Successfully validated the all the cluster builder matches for the sequences.");
                    break;
                default:
                    break;
            }


            ApplicationLog.WriteLine(
                "NUCmer BVT : Successfully validated the all the unique matches for the sequences.");
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
                string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
                Assert.IsNotNull(filePath);
                Assert.IsTrue(File.Exists(filePath));
                ApplicationLog.WriteLine(string.Format(null, "NUCmer BVT : Successfully validated the File Path '{0}'.", filePath));

                using (var parser = new FastAParser(filePath))
                    refSeqList = parser.Parse().ToList();

                // Gets the query sequence from the FastA file
                string queryFilePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SearchSequenceFilePathNode);
                Assert.IsNotNull(queryFilePath);
                Assert.IsTrue(File.Exists(queryFilePath));
                ApplicationLog.WriteLine(string.Format(null, "NUCmer BVT : Successfully validated the File Path '{0}'.", queryFilePath));

                using (var queryParser = new FastAParser(queryFilePath))
                    searchSeqList = queryParser.Parse().ToList();
            }
            else
            {
                // Gets the reference & search sequences from the configuration file
                IAlphabet seqAlphabet = Utility.GetAlphabet(utilityObj.xmlUtil.GetTextValue(nodeName, Constants.AlphabetNameNode));

                string[] sequences = utilityObj.xmlUtil.GetTextValues(nodeName, Constants.ReferenceSequencesNode);
                refSeqList = sequences.Select((t, i) => new Sequence(seqAlphabet, encodingObj.GetBytes(t)) {ID = "ref " + i}).Cast<ISequence>().ToList();

                sequences = utilityObj.xmlUtil.GetTextValues(nodeName, Constants.SearchSequencesNode);
                searchSeqList = sequences.Select((t, i) => new Sequence(seqAlphabet, encodingObj.GetBytes(t)) { ID = "qry " + i }).Cast<ISequence>().ToList();
            }

            var aligner = new NucmerPairwiseAligner
            {
                MaximumSeparation = 0,
                MinimumScore = 2,
                SeparationFactor = 0.12f,
                BreakLength = 2,
                LengthOfMUM = long.Parse(utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMAlignLengthNode), null)
            };

            IList<ISequenceAlignment> align = aligner.Align(refSeqList, searchSeqList);

            string expectedSequences = isFilePath
                    ? utilityObj.xmlUtil.GetFileTextValue(nodeName, Constants.ExpectedSequencesNode)
                    : utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequencesNode);

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

            ApplicationLog.WriteLine("NUCmer BVT : Successfully validated all the aligned sequences.");
        }

        /// <summary>
        /// Validates the Unique Matches for the input provided.
        /// </summary>
        /// <param name="matches">Max Unique Match list</param>
        /// <param name="nodeName">Node name to be read from xml</param>
        /// <param name="additionalParam">Unique Match/Sub level LIS/LIS</param>
        /// <param name="isFilePath">Nodes to be read from Text file?</param>
        /// <returns>True, if successfully validated</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)")]
        bool ValidateUniqueMatches(IList<Match> matches,
            string nodeName, AdditionalParameters additionalParam, bool isFilePath)
        {
            switch (additionalParam)
            {
                case AdditionalParameters.PerformClusterBuilder:
                    // Validates the Cluster builder MUMs
                    string firstSeqOrderExpected =
                        utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ClustFirstSequenceStartNode);
                    string lengthExpected =
                        utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ClustLengthNode);
                    string secondSeqOrderExpected =
                        utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ClustSecondSequenceStartNode);

                    StringBuilder firstSeqOrderActual = new StringBuilder();
                    StringBuilder lengthActual = new StringBuilder();
                    StringBuilder secondSeqOrderActual = new StringBuilder();

                    ClusterBuilder cb = new ClusterBuilder();
                    cb.MinimumScore = 0;

                    List<MatchExtension> meObj = new List<MatchExtension>();

                    foreach (Match m in matches)
                    {
                        meObj.Add(new MatchExtension(m));
                    }

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
                        Console.WriteLine("NUCmer BVT : Unique match not matching");
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
                        firstSeqOrder = utilityObj.xmlUtil.GetFileTextValue(nodeName,
                            Constants.FirstSequenceMumOrderNode).Split(',');
                        length = utilityObj.xmlUtil.GetFileTextValue(nodeName,
                            Constants.LengthNode).Split(',');
                        secondSeqOrder = utilityObj.xmlUtil.GetFileTextValue(nodeName,
                            Constants.SecondSequenceMumOrderNode).Split(',');
                    }
                    else
                    {
                        firstSeqOrder = utilityObj.xmlUtil.GetTextValue(nodeName,
                            Constants.FirstSequenceMumOrderNode).Split(',');
                        length = utilityObj.xmlUtil.GetTextValue(nodeName,
                            Constants.LengthNode).Split(',');
                        secondSeqOrder = utilityObj.xmlUtil.GetTextValue(nodeName,
                            Constants.SecondSequenceMumOrderNode).Split(',');
                    }

                    int i = 0;

                    IList<MatchExtension> meNewObj = new List<MatchExtension>();

                    foreach (Match m in matches)
                    {
                        meNewObj.Add(new MatchExtension(m));
                    }

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
                            Console.WriteLine(string.Format((IFormatProvider)null,
                                "NUCmer BVT : Unique match not matching at index '{0}'", i.ToString((IFormatProvider)null)));
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)")]
        void ValidateNUCmerAlignSimpleGeneralTestCases(string nodeName,
            bool isAlignList)
        {
            string[] referenceSequences = null;
            string[] searchSequences = null;
            List<ISequence> refSeqList = new List<ISequence>();
            List<ISequence> searchSeqList = new List<ISequence>();

            // Gets the reference & search sequences from the configurtion file
            referenceSequences = utilityObj.xmlUtil.GetTextValues(nodeName,
                Constants.ReferenceSequencesNode);
            searchSequences = utilityObj.xmlUtil.GetTextValues(nodeName,
              Constants.SearchSequencesNode);

            IAlphabet seqAlphabet = Utility.GetAlphabet(
                utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.AlphabetNameNode));

            for (int i = 0; i < referenceSequences.Length; i++)
            {
                ISequence referSeq = new Sequence(seqAlphabet,
                    encodingObj.GetBytes(referenceSequences[i]));
                refSeqList.Add(referSeq);
            }

            for (int i = 0; i < searchSequences.Length; i++)
            {
                ISequence searchSeq = new Sequence(seqAlphabet,
                    encodingObj.GetBytes(searchSequences[i]));
                searchSeqList.Add(searchSeq);
            }

            // Gets the mum length from the xml
            string mumLength = utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.MUMAlignLengthNode);

            NucmerPairwiseAligner nucmerObj = new NucmerPairwiseAligner();

            // Update other values for NUCmer object
            nucmerObj.MaximumSeparation = int.Parse
                (utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMAlignLengthNode), (IFormatProvider)null);
            nucmerObj.MinimumScore = int.Parse(
                utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMAlignLengthNode), (IFormatProvider)null);
            nucmerObj.SeparationFactor = int.Parse(
                utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMAlignLengthNode), (IFormatProvider)null);
            nucmerObj.BreakLength = int.Parse(
                utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMAlignLengthNode), (IFormatProvider)null);
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
            expectedSequences = utilityObj.xmlUtil.GetTextValue(nodeName,
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

            Console.WriteLine(
                "NUCmer BVT : Successfully validated all the aligned sequences.");
            ApplicationLog.WriteLine(
                "NUCmer BVT : Successfully validated all the aligned sequences.");
        }

        #endregion Supported Methods
    }
}
