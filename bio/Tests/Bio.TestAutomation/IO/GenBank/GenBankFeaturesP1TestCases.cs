/****************************************************************************
 * GenBankFeaturesP1TestCases.cs
 * 
 *   This file contains the GenBank Features P1 test cases.
 * 
***************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Bio.IO;
using Bio.IO.GenBank;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio;

#if (SILVERLIGHT == false)
    namespace Bio.TestAutomation.IO.GenBank
#else
    namespace Bio.Silverlight.TestAutomation.IO.GenBank
#endif
{
    /// <summary>
    /// GenBank Features P1 test case implementation.
    /// </summary>
    [TestClass]
    public class GenBankFeaturesP1TestCases
    {

        #region Enums

        /// <summary>
        /// GenBank Feature parameters which are used for different test cases 
        /// based on which the test cases are executed.
        /// </summary>
        enum FeatureGroup
        {
            CDS,
            Exon,
            Intron,
            mRNA,
            Enhancer,
            Promoter,
            miscDifference,
            variation,
            MiscStructure,
            TrnsitPeptide,
            StemLoop,
            ModifiedBase,
            PrecursorRNA,
            PolySite,
            MiscBinding,
            GCSignal,
            LTR,
            Operon,
            UnsureSequenceRegion,
            NonCodingRNA,
            RibosomeBindingSite,
            Default
        };

        /// <summary>
        /// GenBank Feature location operators used for different test cases.
        /// </summary>
        enum FeatureOperator
        {
            Join,
            Complement,
            Order,
            Default
        };

        #endregion Enums

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\GenBankFeaturesTestConfig.xml");

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static GenBankFeaturesP1TestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region GenBank P1 TestCases

        /// <summary>
        /// Parse a valid medium size DNA GenBank file.
        /// and validate GenBank features.
        /// Input : DNA medium size Sequence
        /// Output : Validate GenBank features.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGenBankFeaturesForMediumSizeDnaSequence()
        {
            ValidateGenBankFeatures(Constants.MediumSizeDNAGenBankFeaturesNode,
                "DNA");
        }

        /// <summary>
        /// Parse a valid medium size Protein GenBank file.
        /// and validate GenBank features.
        /// Input : Protein medium size Sequence
        /// Output : Validate GenBank features.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGenBankFeaturesForMediumSizeProteinSequence()
        {
            ValidateGenBankFeatures(Constants.MediumSizePROTEINGenBankFeaturesNode,
                "Protein");
        }

        /// <summary>
        /// Parse a valid medium size RNA GenBank file.
        /// and validate GenBank features.
        /// Input : RNA medium size Sequence
        /// Output : Validate GenBank features.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGenBankFeaturesForMediumSizeRnaSequence()
        {
            ValidateGenBankFeatures(Constants.MediumSizeRNAGenBankFeaturesNode,
                "RNA");
        }

        /// <summary>
        /// Parse a valid medium size DNA GenBank file.
        /// and validate cloned GenBank features.
        /// Input : DNA medium size Sequence
        /// Output : alidate cloned GenBank features.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateClonedGenBankFeaturesForMediumSizeDnaSequence()
        {
            ValidateCloneGenBankFeatures(Constants.MediumSizeDNAGenBankFeaturesNode);
        }

        /// <summary>
        /// Parse a valid medium size Protein GenBank file.
        /// and validate cloned GenBank features.
        /// Input : Protein medium size Sequence
        /// Output : validate cloned GenBank features.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateClonedGenBankFeaturesForMediumSizeProteinSequence()
        {
            ValidateCloneGenBankFeatures(Constants.MediumSizePROTEINGenBankFeaturesNode);
        }

        /// <summary>
        /// Parse a valid medium size RNA GenBank file.
        /// and validate GenBank features.
        /// Input : RNA medium size Sequence
        /// Output : Validate GenBank features.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateClonedGenBankFeaturesForMediumSizeRnaSequence()
        {
            ValidateCloneGenBankFeatures(Constants.MediumSizeRNAGenBankFeaturesNode);

        }

        /// <summary>
        /// Parse a valid medium size DNA GenBank file.
        /// and validate GenBank DNA sequence standard features.
        /// Input : Valid DNA sequence.
        /// Output : Validation of GenBank standard Features 
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateMediumSizeDnaSequenceStandardFeatures()
        {
            ValidateGenBankStandardFeatures(Constants.MediumSizeDNAGenBankFeaturesNode,
               "DNA");
        }

        /// <summary>
        /// Parse a valid medium size Protein GenBank file.
        /// and validate GenBank Protein seq standard features.
        /// Input : Valid Protein sequence.
        /// Output : Validation of GenBank standard Features 
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateMediumSizeProteinSequenceStandardFeatures()
        {
            ValidateGenBankStandardFeatures(Constants.MediumSizePROTEINGenBankFeaturesNode,
                "Protein");
        }

        /// <summary>
        /// Parse a valid medium size RNA GenBank file.
        /// and validate GenBank RNA seq standard features.
        /// Input : Valid RNA sequence.
        /// Output : Validation of GenBank standard Features 
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateMediumSizeRNaSequenceStandardFeatures()
        {
            ValidateGenBankStandardFeatures(Constants.MediumSizeRNAGenBankFeaturesNode,
                "RNA");
        }

        /// <summary>
        /// Parse a valid multiSequence Protein GenBank file.
        /// validate GenBank Features.
        /// Input : MultiSequence GenBank Protein file.
        /// Validation : Validate GenBank Features.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGenBankFeaturesForMultipleProteinSequence()
        {
            ValidateGenBankFeatures(Constants.MultiSeqGenBankProteinNode,
                null);
        }

        /// <summary>
        /// Parse a valid multiSequence RNA GenBank file.
        /// validate GenBank Features.
        /// Input : MultiSequence GenBank RNA file.
        /// Validation : Validate GenBank Features.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGenBankFeaturesForMultipleRnaSequence()
        {
            ValidateGenBankFeatures(Constants.MulitSequenceGenBankRNANode,
                "RNA");
        }


        /// <summary>
        /// Parse a valid medium size RNA GenBank file.
        /// and validate GenBank Gene feature qualifiers
        /// Input : RNA medium size Sequence
        /// Output : Validate of GenBank Gene feature qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateRnaSequenceGenBankGeneFeatureQualifiers()
        {
            ValidateGenBankGeneFeatureQualifiers(
                Constants.MediumSizeRNAGenBankFeaturesNode);
        }

        /// <summary>
        /// Parse a valid DNA GenBank file.
        /// and validate GenBank Gene feature qualifiers
        /// Input : DNA medium size Sequence
        /// Output : Validate of GenBank tRNA feature qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateDnaSequenceGenBanktRNAFeatureQualifiers()
        {
            ValidateGenBanktRNAFeatureQualifiers(
                Constants.DNAStandardFeaturesKeyNode);
        }

        /// <summary>
        /// Parse a valid medium size RNA GenBank file.
        /// and validate GenBank Gene feature qualifiers
        /// Input : RNA medium size Sequence
        /// Output : Validate of GenBank tRNA feature qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateRnaSequenceGenBanktRNAFeatureQualifiers()
        {
            ValidateGenBanktRNAFeatureQualifiers(
                Constants.MediumSizeRNAGenBankFeaturesNode);
        }

        /// <summary>
        /// Parse a valid DNA GenBank file.
        /// and validate GenBank Gene feature qualifiers
        /// Input : DNA medium size Sequence
        /// Output : Validate of GenBank Gene feature qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateDnaSequenceGenBankGeneFeatureQualifiers()
        {
            ValidateGenBankGeneFeatureQualifiers(
                Constants.DNAStandardFeaturesKeyNode);
        }

        /// <summary>
        /// Parse a valid medium size RNA GenBank file.
        /// and validate GenBank Gene feature qualifiers
        /// Input : RNA medium size Sequence
        /// Output : Validate of GenBank mRNA feature qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateRnaSequenceGenBankmRNAFeatureQualifiers()
        {
            ValidateGenBankmRNAFeatureQualifiers(
                Constants.MediumSizeRNAGenBankFeaturesNode);
        }

        /// <summary>
        /// Parse a valid DNA GenBank file.
        /// and validate GenBank Gene feature qualifiers
        /// Input : DNA medium size Sequence
        /// Output : Validate of GenBank mRNA feature qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateDnaSequenceGenBankmRNAFeatureQualifiers()
        {
            ValidateGenBankmRNAFeatureQualifiers(
                Constants.DNAStandardFeaturesKeyNode);
        }

        /// <summary>
        /// Parse a valid Protein GenBank file.
        /// and validate GenBank Gene feature qualifiers
        /// Input : Protein medium size Sequence
        /// Output : Validate of GenBank mRNA feature qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateProteinSequenceGenBankmRNAFeatureQualifiers()
        {
            ValidateGenBankmRNAFeatureQualifiers(
                Constants.ProteinGenBankVariationNode);
        }

        /// <summary>
        /// Parse a valid medium size RNA GenBank file.
        /// and validate addition of GenBank features.
        /// Input : RNA medium size Sequence
        /// Output : validate addition of GenBank features.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateAdditionGenBankFeatures()
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.MediumSizeRNAGenBankFeaturesNode, Constants.FilePathNode);
            string addFirstKey = utilityObj.xmlUtil.GetTextValue(
                Constants.MediumSizeRNAGenBankFeaturesNode, Constants.FirstKey);
            string addSecondKey = utilityObj.xmlUtil.GetTextValue(
                Constants.MediumSizeRNAGenBankFeaturesNode, Constants.SecondKey);
            string addFirstLocation = utilityObj.xmlUtil.GetTextValue(
                Constants.MediumSizeRNAGenBankFeaturesNode, Constants.FirstLocation);
            string addSecondLocation = utilityObj.xmlUtil.GetTextValue(
                Constants.MediumSizeRNAGenBankFeaturesNode, Constants.SecondLocation);

            // Parse a GenBank file.
            LocationBuilder locBuilder = new LocationBuilder();
            ISequenceParser parserObj = new GenBankParser(filePath);
            IEnumerable<ISequence> seqList = parserObj.Parse();

            GenBankMetadata metadata =
                (GenBankMetadata)seqList.ElementAt(0).Metadata[Constants.GenBank];

            // Add a new features to Genbank features list.
            metadata.Features = new SequenceFeatures();
            FeatureItem feature = new FeatureItem(addFirstKey, addFirstLocation);
            metadata.Features.All.Add(feature);
            feature = new FeatureItem(addSecondKey, addSecondLocation);
            metadata.Features.All.Add(feature);

            // Validate added GenBank features.
            Assert.AreEqual(metadata.Features.All[0].Key.ToString((IFormatProvider)null), addFirstKey);
            Assert.AreEqual(locBuilder.GetLocationString(metadata.Features.All[0].Location),
                addFirstLocation);
            Assert.AreEqual(metadata.Features.All[1].Key.ToString((IFormatProvider)null), addSecondKey);
            Assert.AreEqual(locBuilder.GetLocationString(metadata.Features.All[1].Location),
                addSecondLocation);

            //Log to VSTest GUI.
            Console.WriteLine(string.Format((IFormatProvider)null,
                "GenBank Features P1: Successfully validated the new feature '{0}'",
                metadata.Features.All[0].Key.ToString((IFormatProvider)null)));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "GenBank Features P1: Successfully validated the new location '{0}'",
                metadata.Features.All[0].Location.ToString()));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "GenBank Features P1: Successfully validated the new feature '{0}'",
                metadata.Features.All[1].Key.ToString((IFormatProvider)null)));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "GenBank Features P1: Successfully validated the new location '{0}'",
                metadata.Features.All[1].Location.ToString()));
        }

        /// <summary>
        /// Parse a valid medium size RNA GenBank file.
        /// and validate addition of GenBank qualifiers.
        /// Input : RNA medium size Sequence
        /// Output : validate addition of GenBank qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateAdditionGenBankQualifiers()
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.MediumSizeRNAGenBankFeaturesNode, Constants.FilePathNode);
            string addFirstKey = utilityObj.xmlUtil.GetTextValue(
                Constants.MediumSizeRNAGenBankFeaturesNode, Constants.FirstKey);
            string addSecondKey = utilityObj.xmlUtil.GetTextValue(
                Constants.MediumSizeRNAGenBankFeaturesNode, Constants.SecondKey);
            string addFirstLocation = utilityObj.xmlUtil.GetTextValue(
                Constants.MediumSizeRNAGenBankFeaturesNode, Constants.FirstLocation);
            string addSecondLocation = utilityObj.xmlUtil.GetTextValue(
                Constants.MediumSizeRNAGenBankFeaturesNode, Constants.SecondLocation);
            string addFirstQualifier = utilityObj.xmlUtil.GetTextValue(
                Constants.MediumSizeRNAGenBankFeaturesNode, Constants.FirstQualifier);
            string addSecondQualifier = utilityObj.xmlUtil.GetTextValue(
                Constants.MediumSizeRNAGenBankFeaturesNode, Constants.SecondQualifier);

            ISequenceParser parserObj = new GenBankParser(filePath);
            IEnumerable<ISequence> seqList = parserObj.Parse();

            LocationBuilder locBuilder = new LocationBuilder();

            GenBankMetadata metadata =
                (GenBankMetadata)seqList.ElementAt(0).Metadata[Constants.GenBank];

            // Add a new features to Genbank features list.
            metadata.Features = new SequenceFeatures();
            FeatureItem feature = new FeatureItem(addFirstKey,
                addFirstLocation);
            List<string> qualifierValues = new List<string>();
            qualifierValues.Add(addFirstQualifier);
            qualifierValues.Add(addFirstQualifier);
            feature.Qualifiers.Add(addFirstQualifier, qualifierValues);
            metadata.Features.All.Add(feature);

            feature = new FeatureItem(addSecondKey, addSecondLocation);
            qualifierValues = new List<string>();
            qualifierValues.Add(addSecondQualifier);
            qualifierValues.Add(addSecondQualifier);
            feature.Qualifiers.Add(addSecondQualifier, qualifierValues);
            metadata.Features.All.Add(feature);

            // Validate added GenBank features.
            Assert.AreEqual(metadata.Features.All[0].Key.ToString((IFormatProvider)null),
                addFirstKey);
            Assert.AreEqual(locBuilder.GetLocationString(
                metadata.Features.All[0].Location),
                addFirstLocation);
            Assert.AreEqual(metadata.Features.All[1].Key.ToString((IFormatProvider)null),
                addSecondKey);
            Assert.AreEqual(locBuilder.GetLocationString(
                metadata.Features.All[1].Location),
                addSecondLocation);

            //Log to VSTest GUI.
            Console.WriteLine(string.Format((IFormatProvider)null,
                "GenBank Features P1: Successfully validated the new feature '{0}'",
                metadata.Features.All[0].Key.ToString((IFormatProvider)null)));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "GenBank Features P1: Successfully validated the new location '{0}'",
                locBuilder.GetLocationString(metadata.Features.All[0].Location)));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "GenBank Features P1: Successfully validated the new feature '{0}'",
                metadata.Features.All[1].Key.ToString((IFormatProvider)null)));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "GenBank Features P1: Successfully validated the new location '{0}'",
                locBuilder.GetLocationString(metadata.Features.All[1].Location)));
        }

        /// <summary>
        /// Parse a valid medium size Protein GenBank file.
        /// and Validate CDS Qualifiers
        /// Input : Protein medium size Sequence
        /// Output : validate CDS Qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateMediumSizePROTEINSequenceCDSQualifiers()
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleGenBankProNodeName, Constants.FilePathNode);
            string expectedCDSException = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleGenBankProNodeName, Constants.CDSException);
            string expectedCDSLabel = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleGenBankProNodeName, Constants.CDSLabel);
            string expectedCDSDBReference = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleGenBankProNodeName, Constants.CDSDBReference);
            string expectedGeneSymbol = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleGenBankProNodeName, Constants.GeneSymbol);

            // Parse a GenBank file.
            ISequenceParser parserObj = new GenBankParser(filePath);
            ISequence sequence = parserObj.Parse().FirstOrDefault();

            GenBankMetadata metadata =
                sequence.Metadata[Constants.GenBank] as GenBankMetadata;

            // Get CDS qaulifier.value.
            List<CodingSequence> cdsQualifiers = metadata.Features.CodingSequences;
            List<string> dbReferenceValue = cdsQualifiers[0].DatabaseCrossReference;
            Assert.AreEqual(cdsQualifiers[0].Label, expectedCDSLabel);
            Assert.AreEqual(cdsQualifiers[0].Exception.ToString((IFormatProvider)null), expectedCDSException);
            Assert.IsTrue(string.IsNullOrEmpty(cdsQualifiers[0].Allele));
            Assert.IsFalse(string.IsNullOrEmpty(cdsQualifiers[0].Citation.ToString()));
            Assert.AreEqual(dbReferenceValue[0].ToString(), expectedCDSDBReference);
            Assert.AreEqual(cdsQualifiers[0].GeneSymbol, expectedGeneSymbol);
            Console.WriteLine(string.Format((IFormatProvider)null,
                "GenBank Features P1: Successfully validated the CDS Qualifiers '{0}'",
                cdsQualifiers[0].Label));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "GenBank Features P1: Successfully validated the CDS Qualifiers '{0}'",
                dbReferenceValue[0].ToString()));
        }

        /// <summary>
        /// Parse a valid GenBank file.
        /// and Validate Clearing feature list
        /// Input : Dna medium size Sequence
        /// Output : validate clear() featre list.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateRemoveFeatureItem()
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleGenBankDnaNodeName, Constants.FilePathNode);
            string allFeaturesCount = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleGenBankDnaNodeName, Constants.GenBankFeaturesCount);

            // Parse a file.
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                IEnumerable<ISequence> sequenceList = parserObj.Parse();

                // GenBank metadata.
                GenBankMetadata metadata =
                    sequenceList.ElementAt(0).Metadata[Constants.GenBank] as GenBankMetadata;

                // Validate GenBank features before removing feature item.
                Assert.AreEqual(metadata.Features.All.Count,
                    Convert.ToInt32(allFeaturesCount, (IFormatProvider)null));
                IList<FeatureItem> featureList = metadata.Features.All;

                // Remove feature items from feature list.
                featureList.Clear();

                // Validate feature list after clearing featureList.
                Assert.AreEqual(featureList.Count, 0);

                ApplicationLog.WriteLine(
                    "GenBank Features P1: Successfully validated the GenBank Features");
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validatedremoving GenBank features '{0}'",
                    featureList.Count));
            }
        }

        /// <summary>
        /// Parse a Protein valid GenBank file.
        /// and Validate Clearing feature list
        /// Input : Protein medium size Sequence
        /// Output : validate clear() featre list.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateRemoveFeatureItemForProteinSequence()
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleGenBankProNodeName, Constants.FilePathNode);
            string allFeaturesCount = utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleGenBankProNodeName, Constants.GenBankFeaturesCount);

            // Parse a file.
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                IEnumerable<ISequence> sequenceList = parserObj.Parse();

                // GenBank metadata.
                GenBankMetadata metadata =
                    sequenceList.ElementAt(0).Metadata[Constants.GenBank] as GenBankMetadata;

                // Validate GenBank features before removing feature item.
                Assert.AreEqual(metadata.Features.All.Count, Convert.ToInt32(allFeaturesCount, (IFormatProvider)null));
                IList<FeatureItem> featureList = metadata.Features.All;

                // Remove feature items from feature list.
                featureList.Clear();

                // Validate feature list after clearing featureList.
                Assert.AreEqual(featureList.Count, 0);

                ApplicationLog.WriteLine(
                    "GenBank Features P1: Successfully validated the GenBank Features");
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validatedremoving GenBank features '{0}'",
                    featureList.Count));
            }
        }

        /// <summary>
        /// Parse a Valid medium size Dna Sequence and Validate Features 
        /// within specified range.
        /// Input : Valid medium size Dna Sequence and specified range.
        /// Ouput : Validate features within specified range.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateFeaturesWithinRangeForMediumSizeDnaSequence()
        {
            ValidateGetFeatures(Constants.DNAStandardFeaturesKeyNode);
        }

        /// <summary>
        /// Parse a Valid medium size Rna Sequence and Validate Features 
        /// within specified range.
        /// Input : Valid medium size Rna Sequence and specified range.
        /// Ouput : Validate features within specified range.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateFeaturesWithinRangeForMediumSizeRnaSequence()
        {
            ValidateGetFeatures(Constants.MediumSizeRNAGenBankFeaturesNode);
        }

        /// <summary>
        /// Parse a Valid medium size Protein Sequence and Validate Features 
        /// within specified range.
        /// Input : Valid medium size Protein Sequence and specified range.
        /// Ouput : Validate features within specified range.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateFeaturesWithinRangeForMediumSizeProteinSequence()
        {
            ValidateGetFeatures(Constants.MediumSizeRNAGenBankFeaturesNode);
        }

        /// <summary>
        /// Parse a Valid DNA Sequence and validate citation referenced
        /// present in CDS GenBank Feature.
        /// Input : Valid DNA Sequence 
        /// Ouput : Validation of citation referneced for CDS feature.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateCitationReferencedForCDSFeature()
        {
            ValidateCitationReferenced(
                Constants.DNAStandardFeaturesKeyNode, FeatureGroup.CDS);
        }

        /// <summary>
        /// Parse a Valid DNA Sequence and validate citation referenced
        /// present in mRNA GenBank Feature.
        /// Input : Valid DNA Sequence 
        /// Ouput : Validation of citation referneced for mRNA feature.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateCitationReferencedFormRNAFeature()
        {
            ValidateCitationReferenced(
                Constants.DNAStandardFeaturesKeyNode, FeatureGroup.mRNA);
        }

        /// <summary>
        /// Parse a Valid DNA Sequence and validate citation referenced
        /// present in Exon GenBank Feature.
        /// Input : Valid DNA Sequence 
        /// Ouput : Validation of citation referneced for Exon feature.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateCitationReferencedForhExonFeature()
        {
            ValidateCitationReferenced(
                Constants.DNAStandardFeaturesKeyNode, FeatureGroup.Exon);
        }

        /// <summary>
        /// Parse a Valid DNA Sequence and validate citation referenced
        /// present in Intron GenBank Feature.
        /// Input : Valid DNA Sequence 
        /// Ouput : Validation of citation referneced for Intron feature.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateCitationReferencedForIntronFeature()
        {
            ValidateCitationReferenced(
                Constants.DNAStandardFeaturesKeyNode, FeatureGroup.Intron);
        }

        /// <summary>
        /// Parse a Valid DNA Sequence and validate citation referenced
        /// present in Promoter GenBank Feature.
        /// Input : Valid DNA Sequence 
        /// Ouput : Validation of citation referneced for Promoter feature.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateCitationReferencedForEnhancerFeature()
        {
            ValidateCitationReferenced(
                Constants.DNAStandardFeaturesKeyNode, FeatureGroup.Promoter);
        }

        /// <summary>
        /// Parse a valid medium size multiSequence RNA GenBank file.
        /// validate GenBank Features.
        /// Input : Medium size MultiSequence GenBank RNA file.
        /// Validation : Validate GenBank Features.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGenBankFeaturesForMediumSizeMultipleRnaSequence()
        {
            ValidateGenBankFeatures(Constants.MediumSizeMulitSequenceGenBankRNANode,
                "RNA");
        }

        /// <summary>
        /// Parse a valid medium size RNA GenBank file.
        /// and validate addition of single GenBank feature.
        /// Input : RNA medium size Sequence
        /// Output : validate addition of single GenBank  features.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateAdditionSingleGenBankFeature()
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.MediumSizeRNAGenBankFeaturesNode, Constants.FilePathNode);
            string addFirstKey = utilityObj.xmlUtil.GetTextValue(
                Constants.MediumSizeRNAGenBankFeaturesNode, Constants.FirstKey);
            string addFirstLocation = utilityObj.xmlUtil.GetTextValue(
                Constants.MediumSizeRNAGenBankFeaturesNode, Constants.FirstLocation);

            // Parse a GenBank file.
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                IEnumerable<ISequence> seqList = parserObj.Parse();
                LocationBuilder locBuilder = new LocationBuilder();

                GenBankMetadata metadata =
                    (GenBankMetadata)seqList.ElementAt(0).Metadata[Constants.GenBank];

                // Add a new features to Genbank features list.
                metadata.Features = new SequenceFeatures();
                FeatureItem feature = new FeatureItem(addFirstKey, addFirstLocation);
                metadata.Features.All.Add(feature);

                // Validate added GenBank features.
                Assert.AreEqual(metadata.Features.All[0].Key.ToString((IFormatProvider)null),
                    addFirstKey);
                Assert.AreEqual(
                    locBuilder.GetLocationString(metadata.Features.All[0].Location),
                    addFirstLocation);

                //Log to VSTest GUI.
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully added the new feature '{0}'",
                    metadata.Features.All[0].Key.ToString((IFormatProvider)null)));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully added the new location '{0}'",
                    locBuilder.GetLocationString(metadata.Features.All[0].Location)));
            }
        }

        /// <summary>
        /// Parse a valid medium size RNA GenBank file.
        /// and validate addition of single GenBank qualifier.
        /// Input : RNA medium size Sequence
        /// Output : validate addition of single GenBank qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateAdditionSingleGenBankQualifier()
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.MediumSizeRNAGenBankFeaturesNode, Constants.FilePathNode);
            string addFirstKey = utilityObj.xmlUtil.GetTextValue(
                Constants.MediumSizeRNAGenBankFeaturesNode, Constants.FirstKey);
            string addFirstLocation = utilityObj.xmlUtil.GetTextValue(
                Constants.MediumSizeRNAGenBankFeaturesNode, Constants.FirstLocation);
            string addFirstQualifier = utilityObj.xmlUtil.GetTextValue(
            Constants.MediumSizeRNAGenBankFeaturesNode, Constants.FirstQualifier);
            string addSecondQualifier = utilityObj.xmlUtil.GetTextValue(
            Constants.MediumSizeRNAGenBankFeaturesNode, Constants.SecondQualifier);

            // Parse a GenBank file.            
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                IEnumerable<ISequence> seqList = parserObj.Parse();
                LocationBuilder locBuilder = new LocationBuilder();

                // Validate Minus35Signal feature all qualifiers.
                GenBankMetadata metadata =
                    (GenBankMetadata)seqList.ElementAt(0).Metadata[Constants.GenBank];

                // Add a new features to Genbank features list.
                metadata.Features = new SequenceFeatures();
                FeatureItem feature = new FeatureItem(addFirstKey, addFirstLocation);
                List<string> qualifierValues = new List<string>();
                qualifierValues.Add(addFirstQualifier);
                qualifierValues.Add(addFirstQualifier);
                feature.Qualifiers.Add(addFirstQualifier, qualifierValues);
                metadata.Features.All.Add(feature);

                qualifierValues = new List<string>();
                qualifierValues.Add(addSecondQualifier);
                qualifierValues.Add(addSecondQualifier);
                feature.Qualifiers.Add(addSecondQualifier, qualifierValues);
                metadata.Features.All.Add(feature);

                // Validate added GenBank features.
                Assert.AreEqual(
                    metadata.Features.All[0].Key.ToString((IFormatProvider)null), addFirstKey);
                Assert.AreEqual(
                    locBuilder.GetLocationString(metadata.Features.All[0].Location),
                    addFirstLocation);

                //Log to VSTest GUI.
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully added the new feature '{0}'",
                    metadata.Features.All[0].Key.ToString((IFormatProvider)null)));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully added the new location '{0}'",
                    locBuilder.GetLocationString(metadata.Features.All[0].Location)));
            }
        }

        /// <summary>
        /// Parse a valid DNA GenBank file.
        /// and validate GenBank Misc feature qualifiers
        /// Input : DNA medium size Sequence
        /// Output : Validation of GenBank Misc feature qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateDnaSequenceGenBankMiscFeatureQualifiers()
        {
            ValidateGenBankMiscFeatureQualifiers(
                Constants.DNAStandardFeaturesKeyNode);
        }

        /// <summary>
        /// Parse a valid medium size RNA GenBank file.
        /// and validate GenBank Misc feature qualifiers
        /// Input : RNA medium size Sequence
        /// Output : Validation of GenBank Misc feature qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateRnaSequenceGenBankMiscFeatureQualifiers()
        {
            ValidateGenBankMiscFeatureQualifiers(
                Constants.MediumSizeRNAGenBankFeaturesNode);
        }

        /// <summary>
        /// Parse a valid DNA GenBank file.
        /// and validate GenBank Exon feature qualifiers
        /// Input : DNA medium size Sequence
        /// Output : Validation of GenBank Exon feature qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateDnaSequenceGenBankExonFeatureQualifiers()
        {
            ValidateGenBankExonFeatureQualifiers(
                Constants.DNAStandardFeaturesKeyNode);
        }

        /// <summary>
        /// Parse a valid medium size RNA GenBank file.
        /// and validate GenBank Exon feature qualifiers
        /// Input : RNA medium size Sequence
        /// Output : Validation of GenBank Exon feature qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateRnaSequenceGenBankExonFeatureQualifiers()
        {
            ValidateGenBankExonFeatureQualifiers(
                Constants.MediumSizeRNAGenBankFeaturesNode);
        }

        /// <summary>
        /// Parse a valid DNA GenBank file.
        /// and validate GenBank Intron feature qualifiers
        /// Input : DNA medium size Sequence
        /// Output : Validation of GenBank Intron feature qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateDnaSequenceGenBankIntronFeatureQualifiers()
        {
            ValidateGenBankIntronFeatureQualifiers(
                Constants.DNAStandardFeaturesKeyNode);
        }

        /// <summary>
        /// Parse a valid medium size RNA GenBank file.
        /// and validate GenBank Intron feature qualifiers
        /// Input : RNA medium size Sequence
        /// Output : Validation of GenBank Intron feature qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateRnaSequenceGenBankIntronFeatureQualifiers()
        {
            ValidateGenBankIntronFeatureQualifiers(
                Constants.MediumSizeRNAGenBankFeaturesNode);
        }

        /// <summary>
        /// Parse a valid DNA GenBank file.
        /// and validate GenBank Promoter feature qualifiers
        /// Input : DNA medium size Sequence
        /// Output : Validation of GenBank Promoter feature qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateDnaSequenceGenBankPromoterFeatureQualifiers()
        {
            ValidateGenBankPromoterFeatureQualifiers(
                Constants.DNAStandardFeaturesKeyNode);
        }

        /// <summary>
        /// Parse a valid medium size RNA GenBank file.
        /// and validate GenBank Promoter feature qualifiers
        /// Input : RNA medium size Sequence
        /// Output : Validation of GenBank Promoter feature qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateRnaSequenceGenBankPromoterFeatureQualifiers()
        {
            ValidateGenBankPromoterFeatureQualifiers(
                Constants.MediumSizeRNAGenBankFeaturesNode);
        }

        /// <summary>
        /// Parse a valid DNA GenBank file.
        /// and validate GenBank Variation feature qualifiers
        /// Input : DNA medium size Sequence
        /// Output : Validation of GenBank Variation feature qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateDnaSequenceGenBankVariationFeatureQualifiers()
        {
            ValidateGenBankVariationFeatureQualifiers(
                Constants.DNAGenBankVariationNode);
        }

        /// <summary>
        /// Parse a valid medium size RNA GenBank file.
        /// and validate GenBank Variation feature qualifiers
        /// Input : RNA medium size Sequence
        /// Output : Validation of GenBank Variation feature qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateRnaSequenceGenBankVariationFeatureQualifiers()
        {
            ValidateGenBankVariationFeatureQualifiers(
                Constants.RNAGenBankVariationNode);
        }

        /// <summary>
        /// Parse a valid medium size Protein GenBank file.
        /// and validate GenBank Variation feature qualifiers
        /// Input : Protein medium size Sequence
        /// Output : Validation of GenBank Variation feature qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateProteinSequenceGenBankVariationFeatureQualifiers()
        {
            ValidateGenBankVariationFeatureQualifiers(
                Constants.ProteinGenBankVariationNode);
        }

        /// <summary>
        /// Parse a valid DNA GenBank file.
        /// and validate GenBank Misc Difference feature qualifiers
        /// Input : DNA medium size Sequence
        /// Output : Validation of GenBank Misc Difference feature qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateDnaSequenceGenBankMiscDiffFeatureQualifiers()
        {
            ValidateGenBankMiscDiffFeatureQualifiers(
                Constants.DNAGenBankVariationNode);
        }

        /// <summary>
        /// Parse a valid medium size RNA GenBank file.
        /// and validate GenBank Misc Difference feature qualifiers
        /// Input : RNA medium size Sequence
        /// Output : Validation of GenBank Misc Difference feature qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateRnaSequenceGenBankMiscDiffFeatureQualifiers()
        {
            ValidateGenBankMiscDiffFeatureQualifiers(
                Constants.RNAGenBankVariationNode);
        }

        /// <summary>
        /// Parse a valid medium size Protein GenBank file.
        /// and validate GenBank Misc Difference feature qualifiers
        /// Input : Protein medium size Sequence
        /// Output : Validation of GenBank Misc Difference feature qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateProteinSequenceGenBankMiscDiffFeatureQualifiers()
        {
            ValidateGenBankMiscDiffFeatureQualifiers(
                Constants.ProteinGenBankVariationNode);
        }

        /// <summary>
        /// Parse a valid DNA GenBank file.
        /// and validate GenBank Protein Binding feature qualifiers
        /// Input : DNA medium size Sequence
        /// Output : Validation of GenBank Protein Binding feature qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateDnaSequenceGenBankProteinBindingFeatureQualifiers()
        {
            ValidateGenBankProteinBindingFeatureQualifiers(
                Constants.DNAGenBankVariationNode);
        }

        /// <summary>
        /// Parse a valid medium size RNA GenBank file.
        /// and validate GenBank Protein Binding feature qualifiers
        /// Input : RNA medium size Sequence
        /// Output : Validation of GenBank Protein Binding feature qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateRnaSequenceGenBankProteinBindingFeatureQualifiers()
        {
            ValidateGenBankProteinBindingFeatureQualifiers(
                Constants.RNAGenBankVariationNode);
        }

        /// <summary>
        /// Parse a valid medium size Protein GenBank file.
        /// and validate GenBank Protein Binding feature qualifiers
        /// Input : Protein medium size Sequence
        /// Output : Validation of GenBank Protein Binding feature qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateProteinSequenceGenBankProteinBindingFeatureQualifiers()
        {
            ValidateGenBankProteinBindingFeatureQualifiers(
                Constants.ProteinGenBankVariationNode);
        }

        /// <summary>
        /// Parse a valid Dna GenBank file.
        /// and validate GenBank Exon feature clonning.
        /// Input : Dna medium size Sequence
        /// Output : validate GenBank Exon feature clonning.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateDnaSequenceGenBankExonFeatureClonning()
        {
            ValidateGenBankFeaturesClonning(Constants.DNAStandardFeaturesKeyNode,
                FeatureGroup.Exon);
        }

        /// <summary>
        /// Parse a valid Rna GenBank file.
        /// and validate GenBank Exon feature clonning.
        /// Input : Rna medium size Sequence
        /// Output : validate GenBank Exon feature clonning.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateRnaSequenceGenBankExonFeatureClonning()
        {
            ValidateGenBankFeaturesClonning(Constants.MediumSizeRNAGenBankFeaturesNode,
               FeatureGroup.Exon);
        }

        /// <summary>
        /// Parse a valid Protein GenBank file.
        /// and validate GenBank Exon feature clonning.
        /// Input : Protein medium size Sequence
        /// Output : validate GenBank Exon feature clonning.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateProteinSequenceGenBankExonFeatureClonning()
        {
            ValidateGenBankFeaturesClonning(Constants.ProteinGenBankVariationNode,
               FeatureGroup.Exon);
        }

        /// <summary>
        /// Parse a valid Dna GenBank file.
        /// and validate GenBank Misc Difference feature clonning.
        /// Input : Dna Sequence
        /// Output : validate GenBank Misc Difference feature clonning.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateDnaSequenceGenBankMiscDiffFeatureClonning()
        {
            ValidateGenBankFeaturesClonning(Constants.DNAGenBankVariationNode,
                FeatureGroup.miscDifference);
        }

        /// <summary>
        /// Parse a valid Rna GenBank file.
        /// and validate GenBank Misc Difference feature clonning.
        /// Input : Rna Sequence
        /// Output : validate GenBank Misc Difference feature clonning.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateRnaSequenceGenBankMiscDiffFeatureClonning()
        {
            ValidateGenBankFeaturesClonning(Constants.RNAGenBankVariationNode,
                FeatureGroup.miscDifference);
        }

        /// <summary>
        /// Parse a valid Protein GenBank file.
        /// and validate GenBank Misc Difference feature clonning.
        /// Input : Protein Sequence
        /// Output : Validate GenBank Misc Difference feature clonning.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateProteinSequenceGenBankMiscDiffFeatureClonning()
        {
            ValidateGenBankFeaturesClonning(Constants.ProteinGenBankVariationNode,
                FeatureGroup.miscDifference);
        }

        /// <summary>
        /// Parse a valid Dna GenBank file.
        /// and validate GenBank Intron feature clonning.
        /// Input : Dna Sequence
        /// Output : validate GenBank Intron feature clonning.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateDnaSequenceGenBankIntronFeatureClonning()
        {
            ValidateGenBankFeaturesClonning(Constants.DNAStandardFeaturesKeyNode,
                FeatureGroup.Intron);
        }

        /// <summary>
        /// Parse a valid Rna GenBank file.
        /// and validate GenBank Intron feature clonning.
        /// Input : Rna Sequence
        /// Output : validate GenBank Intron feature clonning.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateRnaSequenceGenBankIntronFeatureClonning()
        {
            ValidateGenBankFeaturesClonning(Constants.MediumSizeRNAGenBankFeaturesNode,
                FeatureGroup.Intron);
        }

        /// <summary>
        /// Parse a valid Protein GenBank file.
        /// and validate GenBank Intron feature clonning.
        /// Input : Protein Sequence
        /// Output : Validate GenBank Intron feature clonning.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateProteinSequenceGenBankIntronFeatureClonning()
        {
            ValidateGenBankFeaturesClonning(Constants.ProteinGenBankVariationNode,
                FeatureGroup.Intron);
        }

        /// <summary>
        /// Parse a valid Dna GenBank file.
        /// and validate GenBank Variation feature clonning.
        /// Input : Dna Sequence
        /// Output : validate GenBank Variation feature clonning.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateDnaSequenceGenBankVariationFeatureClonning()
        {
            ValidateGenBankFeaturesClonning(Constants.DNAGenBankVariationNode,
                FeatureGroup.variation);
        }

        /// <summary>
        /// Parse a valid Rna GenBank file.
        /// and validate GenBank Variation feature clonning.
        /// Input : Rna Sequence
        /// Output : validate GenBank Variation feature clonning.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateRnaSequenceGenBankVariationFeatureClonning()
        {
            ValidateGenBankFeaturesClonning(Constants.RNAGenBankVariationNode,
                FeatureGroup.variation);
        }

        /// <summary>
        /// Parse a valid Protein GenBank file.
        /// and validate GenBank Variation feature clonning.
        /// Input : Protein Sequence
        /// Output : Validate GenBank Variation feature clonning.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateProteinSequenceGenBankVariationFeatureClonning()
        {
            ValidateGenBankFeaturesClonning(Constants.ProteinGenBankVariationNode,
                FeatureGroup.variation);
        }

        /// <summary>
        /// Validate GenBank MaturePeptide feature Qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of GenBank MaturePeptide feature Qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGenBankMPeptideFeatureQualifiers()
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.ProteinGenBankMPeptideQualifiersNode,
                Constants.FilePathNode);
            string mPeptideCount = utilityObj.xmlUtil.GetTextValue(
                Constants.ProteinGenBankMPeptideQualifiersNode,
                Constants.MiscPeptideCount);
            string geneSymbol = utilityObj.xmlUtil.GetTextValue(
                Constants.ProteinGenBankMPeptideQualifiersNode,
                Constants.GeneSymbol);
            string mPeptideLocation = utilityObj.xmlUtil.GetTextValue(
                Constants.ProteinGenBankMPeptideQualifiersNode,
                Constants.Location);

            // Parse a GenBank file.
            ISequenceParser parserObj = new GenBankParser(filePath);
            IEnumerable<ISequence> seqList = parserObj.Parse();
            LocationBuilder locBuilder = new LocationBuilder();

            // Validate Minus35Signal feature all qualifiers.
            GenBankMetadata metadata =
                (GenBankMetadata)seqList.ElementAt(0).Metadata[Constants.GenBank];
            List<MaturePeptide> mPeptideList = metadata.Features.MaturePeptides;

            // Create a clone and validate all qualifiers.
            MaturePeptide clonemPeptide = mPeptideList[0].Clone();
            Assert.AreEqual(mPeptideList.Count.ToString((IFormatProvider)null), mPeptideCount);
            Assert.AreEqual(clonemPeptide.GeneSymbol.ToString((IFormatProvider)null), geneSymbol);
            Assert.IsFalse(string.IsNullOrEmpty(clonemPeptide.DatabaseCrossReference.ToString()));
            Assert.IsTrue(string.IsNullOrEmpty(clonemPeptide.Allele.ToString((IFormatProvider)null)));
            Assert.IsFalse(string.IsNullOrEmpty(clonemPeptide.Citation.ToString()));
            Assert.IsFalse(string.IsNullOrEmpty(clonemPeptide.Experiment.ToString()));
            Assert.IsFalse(string.IsNullOrEmpty(clonemPeptide.Function.ToString()));
            Assert.IsFalse(string.IsNullOrEmpty(clonemPeptide.GeneSynonym.ToString()));
            Assert.IsTrue(string.IsNullOrEmpty(clonemPeptide.GenomicMapPosition.ToString()));
            Assert.IsFalse(string.IsNullOrEmpty(clonemPeptide.Inference.ToString()));
            Assert.IsTrue(string.IsNullOrEmpty(clonemPeptide.Label.ToString()));
            Assert.AreEqual(locBuilder.GetLocationString(
                metadata.Features.MaturePeptides[0].Location), mPeptideLocation);
            Assert.IsFalse(string.IsNullOrEmpty(clonemPeptide.Note.ToString()));
            Assert.IsFalse(mPeptideList[0].Pseudo);
            Assert.IsFalse(string.IsNullOrEmpty(mPeptideList[0].OldLocusTag.ToString()));
            Assert.IsTrue(string.IsNullOrEmpty(mPeptideList[0].EnzymeCommissionNumber.ToString((IFormatProvider)null)));
            Assert.IsTrue(string.IsNullOrEmpty(mPeptideList[0].StandardName.ToString()));
            Assert.IsFalse(string.IsNullOrEmpty(mPeptideList[0].LocusTag.ToString()));

            // Log VSTest GUI.
            ApplicationLog.WriteLine(
                "GenBank Features P1: Successfully validated the GenBank Features");
            Console.WriteLine(string.Format((IFormatProvider)null,
                "GenBank Features P1: Successfully validated the MaturePeptide feature '{0}'",
                mPeptideList[0].GeneSymbol.ToString((IFormatProvider)null)));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "GenBank Features P1: Successfully validated the MaturePeptide feature '{0}'",
                mPeptideList.Count.ToString((IFormatProvider)null)));
        }

        /// <summary>
        /// Validate GenBank Attenuator feature Qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of GenBank Attenuator feature Qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGenBankAttenuatorFeatureQualifiers()
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankAttenuatorQualifiers, Constants.FilePathNode);
            string attenuatorLocation = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankAttenuatorQualifiers, Constants.Location);
            string featureCount = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankAttenuatorQualifiers, Constants.QualifierCount);

            // Parse a GenBank file.
            ISequenceParser parserObj = new GenBankParser(filePath);
            IEnumerable<ISequence> seqList = parserObj.Parse();
            LocationBuilder locBuilder = new LocationBuilder();

            GenBankMetadata metadata =
                (GenBankMetadata)seqList.ElementAt(0).Metadata[Constants.GenBank];

            List<Attenuator> attenuatorList = metadata.Features.Attenuators;

            // Create a clone of attenuator feature.
            Attenuator attenuatorClone = attenuatorList[0].Clone();
            Assert.AreEqual(attenuatorList.Count.ToString((IFormatProvider)null), featureCount);
            Assert.IsTrue(string.IsNullOrEmpty(attenuatorList[0].GeneSymbol));
            Assert.IsFalse(string.IsNullOrEmpty(attenuatorClone.DatabaseCrossReference.ToString()));
            Assert.IsTrue(string.IsNullOrEmpty(attenuatorClone.Allele));
            Assert.IsFalse(string.IsNullOrEmpty(attenuatorList[0].ToString()));
            Assert.IsFalse(string.IsNullOrEmpty(attenuatorClone.Experiment.ToString()));
            Assert.IsTrue(string.IsNullOrEmpty(attenuatorList[0].GenomicMapPosition));
            Assert.IsFalse(string.IsNullOrEmpty(attenuatorList[0].GeneSynonym.ToString()));
            Assert.IsFalse(string.IsNullOrEmpty(attenuatorList[0].Inference.ToString()));
            Assert.IsTrue(string.IsNullOrEmpty(attenuatorList[0].Label));
            Assert.AreEqual(locBuilder.GetLocationString(
                metadata.Features.Attenuators[0].Location), attenuatorLocation);
            Assert.IsFalse(string.IsNullOrEmpty(attenuatorList[0].Note.ToString()));
            Assert.IsTrue(string.IsNullOrEmpty(attenuatorList[0].Operon));
            Assert.IsFalse(string.IsNullOrEmpty(attenuatorList[0].OldLocusTag.ToString()));
            Assert.IsFalse(string.IsNullOrEmpty(attenuatorList[0].Phenotype.ToString()));
            Assert.IsFalse(string.IsNullOrEmpty(attenuatorList[0].LocusTag.ToString()));
            Assert.IsFalse(string.IsNullOrEmpty(attenuatorList[0].OldLocusTag.ToString()));

            // Create a new Attenuator and validate the same.
            Attenuator attenuator = new Attenuator(attenuatorLocation);
            Attenuator attenuatorWithILoc = new Attenuator(
                metadata.Features.Attenuators[0].Location);

            // Set qualifiers and validate them.
            attenuator.Allele = attenuatorLocation;
            attenuator.GeneSymbol = string.Empty;
            attenuatorWithILoc.GenomicMapPosition = string.Empty;
            Assert.IsTrue(string.IsNullOrEmpty(attenuator.GeneSymbol));
            Assert.AreEqual(attenuator.Allele, attenuatorLocation);
            Assert.IsTrue(string.IsNullOrEmpty(attenuatorWithILoc.GenomicMapPosition));

            // Log VSTest GUI.
            ApplicationLog.WriteLine(
                "GenBank Features P1: Successfully validated the GenBank Features");
            Console.WriteLine(string.Format((IFormatProvider)null,
                "GenBank Features P1: Successfully validated the attenuator feature '{0}'",
                metadata.Features.Attenuators[0].Location));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "GenBank Features P1: Successfully validated the attenuator feature '{0}'",
                attenuatorList.Count.ToString((IFormatProvider)null)));
        }

        /// <summary>
        /// Validate GenBank Minus35Signal feature Qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of GenBank Minus35Signal feature Qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGenBankMinus35SignalFeatureQualifiers()
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankMinus35SignalNode, Constants.FilePathNode);
            string minus35Location = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankMinus35SignalNode, Constants.Location);
            string featureCount = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankMinus35SignalNode, Constants.QualifierCount);
            string geneSymbol = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankMinus35SignalNode, Constants.GeneSymbol);

            // Parse a GenBank file.
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                IEnumerable<ISequence> seqList = parserObj.Parse();
                LocationBuilder locBuilder = new LocationBuilder();

                // Validate Minus35Signal feature all qualifiers.
                GenBankMetadata metadata =
                    (GenBankMetadata)seqList.ElementAt(0).Metadata[Constants.GenBank];
                List<Minus35Signal> minus35Signal = metadata.Features.Minus35Signals;

                // Create a clone of Minus35Signal feature feature.
                Minus35Signal cloneMinus35Signal = minus35Signal[0].Clone();
                Assert.AreEqual(minus35Signal.Count.ToString((IFormatProvider)null), featureCount);
                Assert.IsFalse(string.IsNullOrEmpty(cloneMinus35Signal.GeneSymbol));
                Assert.IsFalse(string.IsNullOrEmpty(cloneMinus35Signal.DatabaseCrossReference.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(minus35Signal[0].Allele));
                Assert.IsFalse(string.IsNullOrEmpty(minus35Signal[0].Citation.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(minus35Signal[0].Experiment.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(minus35Signal[0].GenomicMapPosition));
                Assert.IsFalse(string.IsNullOrEmpty(minus35Signal[0].GeneSynonym.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(minus35Signal[0].Inference.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(minus35Signal[0].Label));
                Assert.AreEqual(locBuilder.GetLocationString(
                    metadata.Features.Minus35Signals[0].Location), minus35Location);
                Assert.IsFalse(string.IsNullOrEmpty(minus35Signal[0].Note.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(minus35Signal[0].Operon));
                Assert.IsFalse(string.IsNullOrEmpty(minus35Signal[0].OldLocusTag.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(minus35Signal[0].StandardName));
                Assert.IsFalse(string.IsNullOrEmpty(minus35Signal[0].LocusTag.ToString()));

                // Create a new Minus35Signal and validate the same.
                Minus10Signal minus35 = new Minus10Signal(minus35Location);
                Minus10Signal minus35WithILoc = new Minus10Signal(
                    metadata.Features.Minus35Signals[0].Location);

                // Set qualifiers and validate them.
                minus35.GeneSymbol = geneSymbol;
                minus35WithILoc.GeneSymbol = geneSymbol;
                Assert.AreEqual(minus35.GeneSymbol, geneSymbol);
                Assert.AreEqual(minus35WithILoc.GeneSymbol, geneSymbol);

                // Log VSTest GUI.
                ApplicationLog.WriteLine(
                    "GenBank Features P1: Successfully validated the GenBank Features");
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the Minus35Signalfeature feature '{0}'",
                    metadata.Features.Minus35Signals[0].Location));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the Minus35Signalfeature feature '{0}'",
                    minus35Signal.Count.ToString((IFormatProvider)null)));
            }
        }

        /// <summary>
        /// Validate GenBank Minus10Signal feature Qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of GenBank Minus10Signal feature Qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGenBankMinus10SignalFeatureQualifiers()
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankMInus10SignalNode, Constants.FilePathNode);
            string minus10Location = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankMInus10SignalNode, Constants.Location);
            string featureCount = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankMInus10SignalNode, Constants.QualifierCount);
            string geneSymbol = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankMInus10SignalNode, Constants.GeneSymbol);

            // Parse a GenBank file.
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                IEnumerable<ISequence> seqList = parserObj.Parse();
                LocationBuilder locBuilder = new LocationBuilder();

                GenBankMetadata metadata =
                    (GenBankMetadata)seqList.ElementAt(0).Metadata[Constants.GenBank];
                List<Minus10Signal> minus10Signal = metadata.Features.Minus10Signals;

                // Create a clone of Minus10Signalfeature feature.
                Minus10Signal cloneMinus10Signal = minus10Signal[0].Clone();
                Assert.AreEqual(minus10Signal.Count.ToString((IFormatProvider)null), featureCount);
                Assert.IsFalse(string.IsNullOrEmpty(cloneMinus10Signal.GeneSymbol));
                Assert.IsFalse(string.IsNullOrEmpty(cloneMinus10Signal.DatabaseCrossReference.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(minus10Signal[0].Allele));
                Assert.IsFalse(string.IsNullOrEmpty(minus10Signal[0].Citation.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(minus10Signal[0].Experiment.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(minus10Signal[0].GenomicMapPosition));
                Assert.IsFalse(string.IsNullOrEmpty(minus10Signal[0].GeneSynonym.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(minus10Signal[0].Inference.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(minus10Signal[0].Label));
                Assert.AreEqual(locBuilder.GetLocationString(
                    metadata.Features.Minus10Signals[0].Location), minus10Location);
                Assert.IsFalse(string.IsNullOrEmpty(minus10Signal[0].Note.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(minus10Signal[0].Operon));
                Assert.IsFalse(string.IsNullOrEmpty(minus10Signal[0].OldLocusTag.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(minus10Signal[0].StandardName));
                Assert.IsFalse(string.IsNullOrEmpty(minus10Signal[0].LocusTag.ToString()));

                // Create a new Minus10Signal and validate the same.
                Minus10Signal minus10 = new Minus10Signal(minus10Location);
                Minus10Signal minus10WithILoc = new Minus10Signal(
                    metadata.Features.Minus10Signals[0].Location);

                // Set qualifiers and validate them.
                minus10.GeneSymbol = geneSymbol;
                minus10WithILoc.GeneSymbol = geneSymbol;
                Assert.AreEqual(minus10.GeneSymbol, geneSymbol);
                Assert.AreEqual(minus10WithILoc.GeneSymbol, geneSymbol);

                // Log VSTest GUI.
                ApplicationLog.WriteLine(
                    "GenBank Features P1: Successfully validated the GenBank Features");
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the minus10Signal feature '{0}'",
                    metadata.Features.Minus10Signals[0].Location));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the minus10Signal feature '{0}'",
                    minus10Signal.Count.ToString((IFormatProvider)null)));
            }
        }

        /// <summary>
        /// Validate GenBank PolyASignal feature Qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of GenBank PolyASignal feature Qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGenBankPolyASignalFeatureQualifiers()
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankPolyASignalNode, Constants.FilePathNode);
            string polyALocation = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankPolyASignalNode, Constants.Location);
            string featureCount = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankPolyASignalNode, Constants.QualifierCount);
            string geneSymbol = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankPolyASignalNode, Constants.GeneSymbol);

            // Parse a GenBank file.            
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                IEnumerable<ISequence> seqList = parserObj.Parse();
                LocationBuilder locBuilder = new LocationBuilder();

                // Validate Minus35Signal feature all qualifiers.
                GenBankMetadata metadata =
                    (GenBankMetadata)seqList.ElementAt(0).Metadata[Constants.GenBank];
                List<PolyASignal> polyASignalList = metadata.Features.PolyASignals;

                // Create a clone of PolyASignal feature feature.
                PolyASignal cloneMinus10Signal = polyASignalList[0].Clone();
                Assert.AreEqual(polyASignalList.Count.ToString((IFormatProvider)null), featureCount);
                Assert.AreEqual(cloneMinus10Signal.GeneSymbol, geneSymbol);
                Assert.IsFalse(string.IsNullOrEmpty(cloneMinus10Signal.DatabaseCrossReference.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(polyASignalList[0].Allele));
                Assert.IsFalse(string.IsNullOrEmpty(polyASignalList[0].Citation.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(polyASignalList[0].Experiment.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(polyASignalList[0].GenomicMapPosition.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(polyASignalList[0].GeneSynonym.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(polyASignalList[0].Inference.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(polyASignalList[0].Label));
                Assert.AreEqual(locBuilder.GetLocationString(
                    metadata.Features.PolyASignals[0].Location), polyALocation);
                Assert.IsFalse(string.IsNullOrEmpty(polyASignalList[0].Note.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(polyASignalList[0].OldLocusTag.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(polyASignalList[0].LocusTag.ToString()));

                // Create a new PolyA signal and validate the same.
                PolyASignal polyASignal = new PolyASignal(polyALocation);
                PolyASignal polyASignalWithILoc = new PolyASignal(
                    metadata.Features.Minus10Signals[0].Location);

                // Set qualifiers and validate them.
                polyASignal.GeneSymbol = geneSymbol;
                polyASignalWithILoc.GeneSymbol = geneSymbol;
                Assert.AreEqual(polyASignal.GeneSymbol, geneSymbol);
                Assert.AreEqual(polyASignalWithILoc.GeneSymbol, geneSymbol);

                // Log VSTest GUI.
                ApplicationLog.WriteLine(
                    "GenBank Features P1: Successfully validated the GenBank Features");
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the PolyASignal feature '{0}'",
                    metadata.Features.PolyASignals[0].Location));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the PolyASignal feature '{0}'",
                    polyASignalList.Count.ToString((IFormatProvider)null)));
            }
        }

        /// <summary>
        /// Validate GenBank Terminator feature Qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of GenBank Terminator feature Qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGenBankTerminatorFeatureQualifiers()
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankTerminatorNode, Constants.FilePathNode);
            string terminatorLocation = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankTerminatorNode, Constants.Location);
            string featureCount = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankTerminatorNode, Constants.QualifierCount);
            string geneSymbol = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankTerminatorNode, Constants.GeneSymbol);

            // Parse a GenBank file.
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                IEnumerable<ISequence> seqList = parserObj.Parse();
                LocationBuilder locBuilder = new LocationBuilder();

                GenBankMetadata metadata =
                    (GenBankMetadata)seqList.ElementAt(0).Metadata[Constants.GenBank];
                List<Terminator> terminatorList = metadata.Features.Terminators;

                // Create a clone of Terminator feature feature.
                Terminator cloneTerminator = terminatorList[0].Clone();
                Assert.AreEqual(terminatorList.Count.ToString((IFormatProvider)null), featureCount);
                Assert.AreEqual(cloneTerminator.GeneSymbol, geneSymbol);
                Assert.IsFalse(string.IsNullOrEmpty(cloneTerminator.DatabaseCrossReference.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(terminatorList[0].Allele));
                Assert.IsFalse(string.IsNullOrEmpty(terminatorList[0].Citation.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(terminatorList[0].Experiment.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(terminatorList[0].GenomicMapPosition.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(terminatorList[0].GeneSynonym.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(terminatorList[0].Inference.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(terminatorList[0].Label.ToString()));
                Assert.AreEqual(locBuilder.GetLocationString(
                    metadata.Features.Terminators[0].Location), terminatorLocation);
                Assert.IsFalse(string.IsNullOrEmpty(terminatorList[0].Note.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(terminatorList[0].OldLocusTag.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(terminatorList[0].LocusTag.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(terminatorList[0].StandardName.ToString()));

                // Create a new Terminator signal and validate the same.
                Terminator terminator = new Terminator(terminatorLocation);
                Terminator terminatorWithILoc = new Terminator(
                    metadata.Features.Terminators[0].Location);

                // Set qualifiers and validate them.
                terminator.GeneSymbol = geneSymbol;
                terminatorWithILoc.GeneSymbol = geneSymbol;
                Assert.AreEqual(terminator.GeneSymbol, geneSymbol);
                Assert.AreEqual(terminatorWithILoc.GeneSymbol, geneSymbol);

                // Log VSTest GUI.
                ApplicationLog.WriteLine(
                    "GenBank Features P1: Successfully validated the GenBank Features");
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the Terminator feature '{0}'",
                    metadata.Features.PolyASignals[0].Location));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the Terminator feature '{0}'",
                    terminatorList.Count.ToString((IFormatProvider)null)));
            }
        }

        /// <summary>
        /// Validate GenBank Misc Signal feature Qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of GenBank Misc Signal feature Qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGenBankMiscSignalFeatureQualifiers()
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankMiscSignalNode, Constants.FilePathNode);
            string miscSignalLocation = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankMiscSignalNode, Constants.Location);
            string featureCount = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankMiscSignalNode, Constants.QualifierCount);
            string geneSymbol = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankMiscSignalNode, Constants.GeneSymbol);
            string function = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankMiscSignalNode, Constants.FunctionNode);
            string dbReferenceNode = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankMiscSignalNode, Constants.DbReferenceNode);

            // Parse a GenBank file.            
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                IEnumerable<ISequence> seqList = parserObj.Parse();
                LocationBuilder locBuilder = new LocationBuilder();

                // Validate Minus35Signal feature all qualifiers.
                GenBankMetadata metadata =
                    (GenBankMetadata)seqList.ElementAt(0).Metadata[Constants.GenBank];
                List<MiscSignal> miscSignalList = metadata.Features.MiscSignals;

                // Create a clone of MiscSignal feature feature.
                MiscSignal cloneMiscSignal = miscSignalList[0].Clone();
                Assert.AreEqual(miscSignalList.Count.ToString((IFormatProvider)null), featureCount);
                Assert.AreEqual(cloneMiscSignal.GeneSymbol, geneSymbol);
                Assert.AreEqual(cloneMiscSignal.DatabaseCrossReference[0], dbReferenceNode);
                Assert.IsTrue(string.IsNullOrEmpty(miscSignalList[0].Allele.ToString((IFormatProvider)null)));
                Assert.IsFalse(string.IsNullOrEmpty(miscSignalList[0].Citation.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(miscSignalList[0].Experiment.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(miscSignalList[0].GenomicMapPosition.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(miscSignalList[0].GeneSynonym.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(miscSignalList[0].Inference.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(miscSignalList[0].Label.ToString()));
                Assert.AreEqual(locBuilder.GetLocationString(
                    metadata.Features.MiscSignals[0].Location), miscSignalLocation);
                Assert.IsFalse(string.IsNullOrEmpty(miscSignalList[0].Note.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(miscSignalList[0].OldLocusTag.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(miscSignalList[0].LocusTag.ToString()));
                Assert.AreEqual(miscSignalList[0].Function[0], function);
                Assert.IsTrue(string.IsNullOrEmpty(miscSignalList[0].Operon.ToString((IFormatProvider)null)));

                // Create a new MiscSignal signal and validate the same.
                MiscSignal miscSignal = new MiscSignal(miscSignalLocation);
                MiscSignal miscSignalWithIloc = new MiscSignal(
                    metadata.Features.MiscSignals[0].Location);

                // Set qualifiers and validate them.
                miscSignal.GeneSymbol = geneSymbol;
                miscSignalWithIloc.GeneSymbol = geneSymbol;
                Assert.AreEqual(miscSignal.GeneSymbol, geneSymbol);
                Assert.AreEqual(miscSignalWithIloc.GeneSymbol, geneSymbol);

                // Log VSTest GUI.
                ApplicationLog.WriteLine(
                    "GenBank Features P1: Successfully validated the GenBank Features");
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the Terminator feature '{0}'",
                    metadata.Features.MiscSignals[0].Location));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the Terminator feature '{0}'",
                    miscSignalList.Count.ToString((IFormatProvider)null)));
            }
        }

        /// <summary>
        /// Validate GenBank DisplacementLoop feature Qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of GenBank DisplacementLoop feature Qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGenBankDLoopFeatureQualifiers()
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankDLoopNode, Constants.FilePathNode);
            string dLoopLocation = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankDLoopNode, Constants.Location);
            string featureCount = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankDLoopNode, Constants.QualifierCount);
            string geneSymbol = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankDLoopNode, Constants.GeneSymbol);

            // Parse a GenBank file.
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                IEnumerable<ISequence> seqList = parserObj.Parse();
                LocationBuilder locBuilder = new LocationBuilder();

                GenBankMetadata metadata =
                    (GenBankMetadata)seqList.ElementAt(0).Metadata[Constants.GenBank];
                List<DisplacementLoop> dLoopList = metadata.Features.DisplacementLoops;

                // Create a clone of DLoop feature feature.
                DisplacementLoop cloneDLoop = dLoopList[0].Clone();
                Assert.AreEqual(dLoopList.Count.ToString((IFormatProvider)null), featureCount);
                Assert.AreEqual(cloneDLoop.GeneSymbol, geneSymbol);
                Assert.IsFalse(string.IsNullOrEmpty(cloneDLoop.DatabaseCrossReference.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(dLoopList[0].Allele.ToString((IFormatProvider)null)));
                Assert.IsFalse(string.IsNullOrEmpty(dLoopList[0].Citation.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(dLoopList[0].Experiment.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(dLoopList[0].GenomicMapPosition.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(dLoopList[0].GeneSynonym.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(dLoopList[0].Inference.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(dLoopList[0].Label.ToString()));
                Assert.AreEqual(locBuilder.GetLocationString(
                    metadata.Features.DisplacementLoops[0].Location), dLoopLocation);
                Assert.IsFalse(string.IsNullOrEmpty(dLoopList[0].Note.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(dLoopList[0].OldLocusTag.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(dLoopList[0].LocusTag.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(dLoopList[0].OldLocusTag.ToString()));

                // Create a new DLoop signal and validate the same.
                DisplacementLoop dLoop = new DisplacementLoop(dLoopLocation);
                DisplacementLoop dLoopWithIloc = new DisplacementLoop(
                    metadata.Features.DisplacementLoops[0].Location);

                // Set qualifiers and validate them.
                dLoop.GeneSymbol = geneSymbol;
                dLoopWithIloc.GeneSymbol = geneSymbol;
                Assert.AreEqual(dLoop.GeneSymbol, geneSymbol);
                Assert.AreEqual(dLoopWithIloc.GeneSymbol, geneSymbol);

                // Log VSTest GUI.
                ApplicationLog.WriteLine(
                    "GenBank Features P1: Successfully validated the GenBank Features");
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the DisplacementLoop feature '{0}'",
                    metadata.Features.DisplacementLoops[0].Location));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the DisplacementLoop feature '{0}'",
                    dLoopList.Count.ToString((IFormatProvider)null)));
            }
        }

        /// <summary>
        /// Validate GenBank Intervening DNA feature Qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of GenBank Intervening feature Qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGenBankInterveningDNAFeatureQualifiers()
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankInterveningDNA, Constants.FilePathNode);
            string iDNALocation = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankInterveningDNA, Constants.Location);
            string featureCount = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankInterveningDNA, Constants.QualifierCount);

            // Parse a GenBank file.
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                IEnumerable<ISequence> seqList = parserObj.Parse();
                LocationBuilder locBuilder = new LocationBuilder();

                GenBankMetadata metadata =
                    (GenBankMetadata)seqList.ElementAt(0).Metadata[Constants.GenBank];
                List<InterveningDna> iDNAList = metadata.Features.InterveningDNAs;

                // Create a clone copy and validate.
                InterveningDna iDNAClone = iDNAList[0].Clone();
                Assert.AreEqual(iDNAList.Count.ToString((IFormatProvider)null), featureCount);
                Assert.IsTrue(string.IsNullOrEmpty(iDNAClone.GeneSymbol.ToString((IFormatProvider)null)));
                Assert.IsFalse(string.IsNullOrEmpty(iDNAClone.DatabaseCrossReference.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(iDNAClone.Allele));
                Assert.IsFalse(string.IsNullOrEmpty(iDNAList[0].Citation.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(iDNAList[0].Experiment.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(iDNAList[0].GenomicMapPosition.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(iDNAList[0].GeneSynonym.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(iDNAList[0].Inference.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(iDNAList[0].Label.ToString()));
                Assert.AreEqual(locBuilder.GetLocationString(
                    metadata.Features.InterveningDNAs[0].Location), iDNALocation);
                Assert.IsFalse(string.IsNullOrEmpty(iDNAList[0].Note.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(iDNAList[0].OldLocusTag.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(iDNAList[0].LocusTag.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(iDNAList[0].OldLocusTag.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(iDNAList[0].Function.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(iDNAList[0].Number.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(iDNAList[0].StandardName.ToString()));

                // Create a new Intervening DNA signal and validate the same.
                InterveningDna iDNA = new InterveningDna(iDNALocation);
                InterveningDna iDNAWithIloc = new InterveningDna(
                    metadata.Features.DisplacementLoops[0].Location);

                // Set qualifiers and validate them.
                iDNA.GeneSymbol = iDNALocation;
                iDNAWithIloc.GeneSymbol = iDNALocation;
                Assert.AreEqual(iDNA.GeneSymbol, iDNALocation);
                Assert.AreEqual(iDNAWithIloc.GeneSymbol, iDNALocation);

                // Log VSTest GUI.
                ApplicationLog.WriteLine(
                    "GenBank Features P1: Successfully validated the GenBank Features");
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the iDNA feature '{0}'",
                    metadata.Features.InterveningDNAs[0].Location));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the iDNA feature '{0}'",
                    iDNAList.Count.ToString((IFormatProvider)null)));
            }
        }

        /// <summary>
        /// Validate GenBank Misc Recombination feature Qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of GenBank Misc Recombination feature Qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGenBankMiscRecombinationFeatureQualifiers()
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.ProteinGenBankMiscRecombinationQualifiersNode,
                Constants.FilePathNode);
            string miscCombinationCount = utilityObj.xmlUtil.GetTextValue(
                Constants.ProteinGenBankMiscRecombinationQualifiersNode,
                Constants.FeaturesCount);
            string miscCombinationLocation = utilityObj.xmlUtil.GetTextValue(
                Constants.ProteinGenBankMiscRecombinationQualifiersNode,
                Constants.Location);

            // Parse a GenBank file.
            ISequenceParser parserObj = new GenBankParser(filePath);
            IEnumerable<ISequence> seqList = parserObj.Parse();
            LocationBuilder locBuilder = new LocationBuilder();

            GenBankMetadata metadata =
                (GenBankMetadata)seqList.ElementAt(0).Metadata[Constants.GenBank];
            List<MiscRecombination> miscRecombinationList =
                metadata.Features.MiscRecombinations;

            Assert.AreEqual(miscRecombinationList.Count.ToString((IFormatProvider)null),
                miscCombinationCount);
            Assert.AreEqual(locBuilder.GetLocationString(
                metadata.Features.MiscRecombinations[0].Location),
                miscCombinationLocation);
            Assert.IsTrue(string.IsNullOrEmpty(miscRecombinationList[0].GeneSymbol));
            Assert.IsFalse(string.IsNullOrEmpty(miscRecombinationList[0].DatabaseCrossReference.ToString()));
            Assert.IsTrue(string.IsNullOrEmpty(miscRecombinationList[0].Allele));
            Assert.IsFalse(string.IsNullOrEmpty(miscRecombinationList[0].Citation.ToString()));
            Assert.IsFalse(string.IsNullOrEmpty(miscRecombinationList[0].Experiment.ToString()));
            Assert.IsFalse(string.IsNullOrEmpty(miscRecombinationList[0].GeneSynonym.ToString()));
            Assert.IsTrue(string.IsNullOrEmpty(miscRecombinationList[0].GenomicMapPosition));
            Assert.IsFalse(string.IsNullOrEmpty(miscRecombinationList[0].Inference.ToString()));
            Assert.IsTrue(string.IsNullOrEmpty(miscRecombinationList[0].Label));
            Assert.IsFalse(string.IsNullOrEmpty(miscRecombinationList[0].OldLocusTag.ToString()));
            Assert.IsTrue(string.IsNullOrEmpty(miscRecombinationList[0].StandardName));
            Assert.IsFalse(string.IsNullOrEmpty(miscRecombinationList[0].LocusTag.ToString()));
            Assert.IsFalse(string.IsNullOrEmpty(miscRecombinationList[0].OldLocusTag.ToString()));

            // Log VSTest GUI.
            ApplicationLog.WriteLine(
                "GenBank Features P1: Successfully validated the GenBank Features");
            Console.WriteLine(string.Format((IFormatProvider)null,
                "GenBank Features P1: Successfully validated the MiscRecombination feature '{0}'",
                miscRecombinationList[0].Note));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "GenBank Features P1: Successfully validated the MiscRecombination feature '{0}'",
                miscRecombinationList.Count.ToString((IFormatProvider)null)));
        }

        /// <summary>
        /// Validate GenBank Misc RNA feature Qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of GenBank Misc RNA feature Qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGenBankMiscRNAFeatureQualifiers()
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.ProteinGenBankMiscRNAQualifiersNode,
                Constants.FilePathNode);
            string miscRnaCount = utilityObj.xmlUtil.GetTextValue(
                Constants.ProteinGenBankMiscRNAQualifiersNode,
                Constants.FeaturesCount);
            string miscRnaLocation = utilityObj.xmlUtil.GetTextValue(
                Constants.ProteinGenBankMiscRNAQualifiersNode,
                Constants.Location);

            // Parse a GenBank file.            
            ISequenceParser parserObj = new GenBankParser(filePath);
            IEnumerable<ISequence> seqList = parserObj.Parse();
            LocationBuilder locBuilder = new LocationBuilder();

            // Validate MiscRNA feature all qualifiers.
            GenBankMetadata metadata =
                (GenBankMetadata)seqList.ElementAt(0).Metadata[Constants.GenBank];
            List<MiscRna> miscRnaList = metadata.Features.MiscRNAs;

            // Create a clone of MiscRNA feature and validate
            MiscRna cloneMiscRna = miscRnaList[0].Clone();
            Assert.AreEqual(miscRnaList.Count.ToString((IFormatProvider)null), miscRnaCount);
            Assert.AreEqual(locBuilder.GetLocationString(
                metadata.Features.MiscRNAs[0].Location), miscRnaLocation);
            Assert.IsTrue(string.IsNullOrEmpty(cloneMiscRna.GeneSymbol));
            Assert.IsFalse(string.IsNullOrEmpty(cloneMiscRna.DatabaseCrossReference.ToString()));
            Assert.IsTrue(string.IsNullOrEmpty(miscRnaList[0].Allele));
            Assert.IsFalse(string.IsNullOrEmpty(miscRnaList[0].Citation.ToString()));
            Assert.IsFalse(string.IsNullOrEmpty(miscRnaList[0].Experiment.ToString()));
            Assert.IsFalse(string.IsNullOrEmpty(miscRnaList[0].GeneSynonym.ToString()));
            Assert.IsTrue(string.IsNullOrEmpty(miscRnaList[0].GenomicMapPosition));
            Assert.IsTrue(string.IsNullOrEmpty(miscRnaList[0].Label));
            Assert.IsFalse(string.IsNullOrEmpty(miscRnaList[0].OldLocusTag.ToString()));
            Assert.IsTrue(string.IsNullOrEmpty(miscRnaList[0].StandardName));
            Assert.IsFalse(string.IsNullOrEmpty(miscRnaList[0].LocusTag.ToString()));
            Assert.IsFalse(miscRnaList[0].Pseudo);
            Assert.IsFalse(string.IsNullOrEmpty(miscRnaList[0].Function.ToString()));
            Assert.IsTrue(string.IsNullOrEmpty(miscRnaList[0].Operon));

            // Log VSTest GUI.
            ApplicationLog.WriteLine(
                "GenBank Features P1: Successfully validated the GenBank Features");
            Console.WriteLine(string.Format((IFormatProvider)null,
                "GenBank Features P1: Successfully validated the MiscRNA feature '{0}'",
                miscRnaList[0].Note));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "GenBank Features P1: Successfully validated the MiscRNA feature '{0}'",
                miscRnaList.Count.ToString((IFormatProvider)null)));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "GenBank Features P1: Successfully validated the MiscRNA feature '{0}'",
                miscRnaList[0].DatabaseCrossReference));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "GenBank Features P1: Successfully validated the MiscRNA feature '{0}'",
                miscRnaList[0].Inference));
        }

        /// <summary>
        /// Validate GenBank Ribosomal RNA feature Qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of GenBank Ribosomal RNA feature Qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGenBankRibosomalRNAFeatureQualifiers()
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankrRNAQualifiersNode, Constants.FilePathNode);
            string rRnaCount = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankrRNAQualifiersNode, Constants.FeaturesCount);
            string rRnaLocation = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankrRNAQualifiersNode, Constants.Location);

            // Parse a GenBank file.
            ISequenceParser parserObj = new GenBankParser(filePath);
            IEnumerable<ISequence> seqList = parserObj.Parse();
            LocationBuilder locBuilder = new LocationBuilder();

            // Validate RibosomalRNA feature all qualifiers.
            GenBankMetadata metadata =
                (GenBankMetadata)seqList.ElementAt(0).Metadata[Constants.GenBank];
            List<RibosomalRna> ribosomalRnaList =
                metadata.Features.RibosomalRNAs;

            Assert.AreEqual(ribosomalRnaList.Count.ToString((IFormatProvider)null), rRnaCount);
            Assert.AreEqual(locBuilder.GetLocationString(
                metadata.Features.RibosomalRNAs[0].Location), rRnaLocation);
            Assert.IsTrue(string.IsNullOrEmpty(ribosomalRnaList[0].GeneSymbol));
            Assert.IsFalse(string.IsNullOrEmpty(ribosomalRnaList[0].DatabaseCrossReference.ToString()));
            Assert.IsTrue(string.IsNullOrEmpty(ribosomalRnaList[0].Allele));
            Assert.IsFalse(string.IsNullOrEmpty(ribosomalRnaList[0].Citation.ToString()));
            Assert.IsFalse(string.IsNullOrEmpty(ribosomalRnaList[0].Experiment.ToString()));
            Assert.IsFalse(string.IsNullOrEmpty(ribosomalRnaList[0].GeneSynonym.ToString()));
            Assert.IsTrue(string.IsNullOrEmpty(ribosomalRnaList[0].GenomicMapPosition));
            Assert.IsTrue(string.IsNullOrEmpty(ribosomalRnaList[0].Label));
            Assert.IsFalse(string.IsNullOrEmpty(ribosomalRnaList[0].OldLocusTag.ToString()));
            Assert.IsTrue(string.IsNullOrEmpty(ribosomalRnaList[0].StandardName));
            Assert.IsFalse(string.IsNullOrEmpty(ribosomalRnaList[0].LocusTag.ToString()));
            Assert.IsFalse(string.IsNullOrEmpty(ribosomalRnaList[0].OldLocusTag.ToString()));

            // Log VSTest GUI.
            ApplicationLog.WriteLine(
                "GenBank Features P1: Successfully validated the GenBank Features");
            Console.WriteLine(string.Format((IFormatProvider)null,
                "GenBank Features P1: Successfully validated the rRNA feature '{0}'",
                ribosomalRnaList[0].Note));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "GenBank Features P1: Successfully validated the rRNA feature '{0}'",
                ribosomalRnaList.Count.ToString((IFormatProvider)null)));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "GenBank Features P1: Successfully validated the rRNA feature '{0}'",
                ribosomalRnaList[0].DatabaseCrossReference));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "GenBank Features P1: Successfully validated the rRNA feature '{0}'",
                ribosomalRnaList[0].Inference));
        }

        /// <summary>
        /// Validate GenBank Repeat Origin feature Qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of GenBank Repeat Origin feature Qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGenBankRepeatOriginFeatureQualifiers()
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankrRNAQualifiersNode, Constants.FilePathNode);
            string rOriginCount = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankrRNAQualifiersNode, Constants.ROriginCount);
            string rOriginLocation = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankrRNAQualifiersNode, Constants.ROriginLocation);

            // Parse a GenBank file.
            ISequenceParser parserObj = new GenBankParser(filePath);
            IEnumerable<ISequence> seqList = parserObj.Parse();
            LocationBuilder locBuilder = new LocationBuilder();

            // Validate Repeat Origin feature all qualifiers.
            GenBankMetadata metadata =
                (GenBankMetadata)seqList.ElementAt(0).Metadata[Constants.GenBank];
            List<ReplicationOrigin> repeatOriginList =
                metadata.Features.ReplicationOrigins;

            Assert.AreEqual(repeatOriginList.Count.ToString((IFormatProvider)null), rOriginCount);
            Assert.AreEqual(locBuilder.GetLocationString(
               metadata.Features.ReplicationOrigins[0].Location), rOriginLocation);
            Assert.IsTrue(string.IsNullOrEmpty(repeatOriginList[0].GeneSymbol));
            Assert.IsFalse(string.IsNullOrEmpty(repeatOriginList[0].DatabaseCrossReference.ToString()));
            Assert.IsTrue(string.IsNullOrEmpty(repeatOriginList[0].Allele));
            Assert.IsFalse(string.IsNullOrEmpty(repeatOriginList[0].Citation.ToString()));
            Assert.IsFalse(string.IsNullOrEmpty(repeatOriginList[0].Experiment.ToString()));
            Assert.IsFalse(string.IsNullOrEmpty(repeatOriginList[0].GeneSynonym.ToString()));
            Assert.IsTrue(string.IsNullOrEmpty(repeatOriginList[0].GenomicMapPosition));
            Assert.IsTrue(string.IsNullOrEmpty(repeatOriginList[0].Label));
            Assert.IsFalse(string.IsNullOrEmpty(repeatOriginList[0].OldLocusTag.ToString()));
            Assert.IsTrue(string.IsNullOrEmpty(repeatOriginList[0].StandardName));
            Assert.IsFalse(string.IsNullOrEmpty(repeatOriginList[0].LocusTag.ToString()));

            // Log VSTest GUI.
            ApplicationLog.WriteLine(
                "GenBank Features P1: Successfully validated the GenBank Features");
            Console.WriteLine(string.Format((IFormatProvider)null,
                "GenBank Features P1: Successfully validated the Repeat Origin feature '{0}'",
                repeatOriginList[0].Note));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "GenBank Features P1: Successfully validated the Repeat Origin '{0}'",
                repeatOriginList.Count.ToString((IFormatProvider)null)));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "GenBank Features P1: Successfully validated the Repeat Origin '{0}'",
                repeatOriginList[0].DatabaseCrossReference));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "GenBank Features P1: Successfully validated the Repeat Origin '{0}'",
                repeatOriginList[0].Inference));
        }

        /// <summary>
        /// Validate GenBank CaatSignal feature Qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of GenBank CaatSignal feature Qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGenBankCaatSignalFeatureQualifiers()
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankrRNAQualifiersNode, Constants.FilePathNode);
            string caatSignalCount = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankrRNAQualifiersNode, Constants.CaatSignalCount);
            string expectedGeneSymbol = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankrRNAQualifiersNode, Constants.CaatSignalGene);
            string expectedLocation = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankrRNAQualifiersNode, Constants.CaatSignalLocation);

            // Parse a GenBank file.
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                IEnumerable<ISequence> seqList = parserObj.Parse();
                LocationBuilder locBuilder = new LocationBuilder();

                // Validate CaatSignal feature all qualifiers.
                GenBankMetadata metadata =
                    (GenBankMetadata)seqList.ElementAt(0).Metadata[Constants.GenBank];
                List<CaatSignal> caatSignalList = metadata.Features.CAATSignals;
                Assert.AreEqual(caatSignalList.Count.ToString((IFormatProvider)null), caatSignalCount);
                Assert.AreEqual(locBuilder.GetLocationString(
                    metadata.Features.CAATSignals[0].Location), expectedLocation);
                Assert.AreEqual(caatSignalList[0].GeneSymbol, expectedGeneSymbol);
                Assert.IsFalse(string.IsNullOrEmpty(caatSignalList[0].DatabaseCrossReference.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(caatSignalList[0].Allele));
                Assert.IsFalse(string.IsNullOrEmpty(caatSignalList[0].Citation.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(caatSignalList[0].Experiment.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(caatSignalList[0].GeneSynonym.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(caatSignalList[0].GenomicMapPosition));
                Assert.IsTrue(string.IsNullOrEmpty(caatSignalList[0].Label));
                Assert.IsFalse(string.IsNullOrEmpty(caatSignalList[0].OldLocusTag.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(caatSignalList[0].LocusTag.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(caatSignalList[0].OldLocusTag.ToString()));

                // Log VSTest GUI.
                ApplicationLog.WriteLine(
                    "GenBank Features P1: Successfully validated the GenBank Features");
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the CaatSignal feature '{0}'",
                    caatSignalList[0].Note));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the CaatSignal '{0}'",
                 caatSignalList.Count.ToString((IFormatProvider)null)));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the CaatSignal '{0}'",
                    caatSignalList[0].GeneSymbol));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the CaatSignal '{0}'",
                    caatSignalList[0].Location));
            }
        }

        /// <summary>
        /// Validate GenBank TataSignal feature Qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of GenBank TataSignal feature Qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGenBankTataSignalFeatureQualifiers()
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankrRNAQualifiersNode, Constants.FilePathNode);
            string tataSignalCount = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankrRNAQualifiersNode, Constants.TataSignalCount);
            string expectedGeneSymbol = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankrRNAQualifiersNode, Constants.TataSignalGene);
            string expectedLocation = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankrRNAQualifiersNode, Constants.TataSignalLocation);

            // Parse a GenBank file.
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                IEnumerable<ISequence> seqList = parserObj.Parse();
                LocationBuilder locBuilder = new LocationBuilder();

                // Validate TataSignal feature all qualifiers.
                GenBankMetadata metadata =
                    (GenBankMetadata)seqList.ElementAt(0).Metadata[Constants.GenBank];
                List<TataSignal> tataSignalList = metadata.Features.TATASignals;
                Assert.AreEqual(tataSignalList.Count.ToString((IFormatProvider)null), tataSignalCount);
                Assert.AreEqual(locBuilder.GetLocationString(
                    metadata.Features.TATASignals[0].Location), expectedLocation);
                Assert.AreEqual(tataSignalList[0].GeneSymbol, expectedGeneSymbol);
                Assert.IsFalse(string.IsNullOrEmpty(tataSignalList[0].DatabaseCrossReference.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(tataSignalList[0].Allele));
                Assert.IsFalse(string.IsNullOrEmpty(tataSignalList[0].Citation.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(tataSignalList[0].Experiment.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(tataSignalList[0].GeneSynonym.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(tataSignalList[0].GenomicMapPosition));
                Assert.IsTrue(string.IsNullOrEmpty(tataSignalList[0].Label));
                Assert.IsFalse(string.IsNullOrEmpty(tataSignalList[0].OldLocusTag.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(tataSignalList[0].LocusTag.ToString()));

                // Log VSTest GUI.
                ApplicationLog.WriteLine(
                    "GenBank Features P1: Successfully validated the GenBank Features");
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the TataSignal feature '{0}'",
                    tataSignalList[0].Note));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the TataSignal '{0}'",
                    tataSignalList.Count.ToString((IFormatProvider)null)));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the TataSignal '{0}'",
                    tataSignalList[0].GeneSymbol));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the TataSignal '{0}'",
                    tataSignalList[0].Location));
            }
        }

        /// <summary>
        /// Validate GenBank 3'UTRs  feature Qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of GenBank 3'UTRs feature Qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGenBankThreePrimeUTRsFeatureQualifiers()
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankrRNAQualifiersNode, Constants.FilePathNode);
            string threePrimeUTRCount = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankrRNAQualifiersNode, Constants.ThreePrimeUTRCount);
            string expectedGeneSymbol = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankrRNAQualifiersNode, Constants.ThreePrimeUTRGene);

            // Parse a GenBank file.
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                IEnumerable<ISequence> seqList = parserObj.Parse();

                // Validate 3'UTRs feature all qualifiers.
                GenBankMetadata metadata =
                    (GenBankMetadata)seqList.ElementAt(0).Metadata[Constants.GenBank];
                List<ThreePrimeUtr> threeprimeUTRsList =
                    metadata.Features.ThreePrimeUTRs;
                Assert.AreEqual(threeprimeUTRsList.Count.ToString((IFormatProvider)null), threePrimeUTRCount);
                Assert.AreEqual(threeprimeUTRsList[0].GeneSymbol, expectedGeneSymbol);
                Assert.IsFalse(string.IsNullOrEmpty(threeprimeUTRsList[0].DatabaseCrossReference.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(threeprimeUTRsList[0].Allele));
                Assert.IsFalse(string.IsNullOrEmpty(threeprimeUTRsList[0].Citation.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(threeprimeUTRsList[0].Experiment.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(threeprimeUTRsList[0].GeneSynonym.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(threeprimeUTRsList[0].GenomicMapPosition));
                Assert.IsTrue(string.IsNullOrEmpty(threeprimeUTRsList[0].Label));
                Assert.IsFalse(string.IsNullOrEmpty(threeprimeUTRsList[0].OldLocusTag.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(threeprimeUTRsList[0].LocusTag.ToString()));

                // Log VSTest GUI.
                ApplicationLog.WriteLine(
                    "GenBank Features P1: Successfully validated the GenBank Features");
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the 3'UTRs '{0}'",
                    threeprimeUTRsList.Count.ToString((IFormatProvider)null)));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the 3'UTRs '{0}'",
                    threeprimeUTRsList[0].GeneSymbol));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the 3'UTRs '{0}'",
                    threeprimeUTRsList[0].Location));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the 3'UTRs '{0}'",
                    threeprimeUTRsList[0].Note));
            }
        }

        /// <summary>
        /// Validate GenBank 5'UTRs  feature Qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of GenBank 5'UTRs feature Qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGenBankFivePrimeUTRsFeatureQualifiers()
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankrRNAQualifiersNode, Constants.FilePathNode);
            string fivePrimeUTRCount = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankrRNAQualifiersNode, Constants.ThreePrimeUTRCount);
            string expectedGeneSymbol = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankrRNAQualifiersNode, Constants.FivePrimeUTRGene);
            string expectedLocation = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankrRNAQualifiersNode, Constants.FivePrimeUTRLocation);

            // Parse a GenBank file.
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                IEnumerable<ISequence> seqList = parserObj.Parse();
                LocationBuilder locBuilder = new LocationBuilder();

                // Validate 5'UTRs feature all qualifiers.
                GenBankMetadata metadata =
                    (GenBankMetadata)seqList.ElementAt(0).Metadata[Constants.GenBank];
                List<FivePrimeUtr> fivePrimeUTRsList =
                    metadata.Features.FivePrimeUTRs;
                Assert.AreEqual(fivePrimeUTRsList.Count.ToString((IFormatProvider)null), fivePrimeUTRCount);
                Assert.AreEqual(fivePrimeUTRsList[0].GeneSymbol, expectedGeneSymbol);
                Assert.AreEqual(locBuilder.GetLocationString(
                    metadata.Features.FivePrimeUTRs[0].Location), expectedLocation);
                Assert.IsFalse(string.IsNullOrEmpty(fivePrimeUTRsList[0].DatabaseCrossReference.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(fivePrimeUTRsList[0].Allele));
                Assert.IsFalse(string.IsNullOrEmpty(fivePrimeUTRsList[0].Citation.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(fivePrimeUTRsList[0].Experiment.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(fivePrimeUTRsList[0].GeneSynonym.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(fivePrimeUTRsList[0].GenomicMapPosition));
                Assert.IsTrue(string.IsNullOrEmpty(fivePrimeUTRsList[0].Label));
                Assert.IsFalse(string.IsNullOrEmpty(fivePrimeUTRsList[0].OldLocusTag.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(fivePrimeUTRsList[0].LocusTag.ToString()));

                // Log VSTest GUI.
                ApplicationLog.WriteLine(
                    "GenBank Features P1: Successfully validated the GenBank Features");
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the 5'UTRs '{0}'",
                    fivePrimeUTRsList.Count.ToString((IFormatProvider)null)));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the 5'UTRs '{0}'",
                    fivePrimeUTRsList[0].GeneSymbol));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the 5'UTRs '{0}'",
                    fivePrimeUTRsList[0].Location));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the 5'UTRs '{0}'",
                    fivePrimeUTRsList[0].Note));
            }
        }

        /// <summary>
        /// Validate GenBank SignalPeptide feature Qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of GenBank SignalPeptide feature Qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGenBankSignalPeptideFeatureQualifiers()
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankrRNAQualifiersNode,
                Constants.FilePathNode);
            string signalPeptideCount = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankrRNAQualifiersNode,
                Constants.ThreePrimeUTRCount);
            string expectedLocation = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankrRNAQualifiersNode,
                Constants.SignalPeptideLocation);

            // Parse a GenBank file.
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                IEnumerable<ISequence> seqList = parserObj.Parse();
                LocationBuilder locBuilder = new LocationBuilder();

                // Validate SignalPeptide feature all qualifiers.
                GenBankMetadata metadata =
                    (GenBankMetadata)seqList.ElementAt(0).Metadata[Constants.GenBank];
                List<SignalPeptide> signalPeptideQualifiersList =
                    metadata.Features.SignalPeptides;
                Assert.AreEqual(signalPeptideQualifiersList.Count.ToString((IFormatProvider)null),
                    signalPeptideCount);
                Assert.IsTrue(string.IsNullOrEmpty(signalPeptideQualifiersList[0].GeneSymbol));
                Assert.AreEqual(locBuilder.GetLocationString(
                     metadata.Features.SignalPeptides[0].Location),
                     expectedLocation);
                Assert.IsFalse(string.IsNullOrEmpty(signalPeptideQualifiersList[0].DatabaseCrossReference.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(signalPeptideQualifiersList[0].Allele));
                Assert.IsFalse(string.IsNullOrEmpty(signalPeptideQualifiersList[0].Citation.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(signalPeptideQualifiersList[0].Experiment.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(signalPeptideQualifiersList[0].GeneSynonym.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(signalPeptideQualifiersList[0].GenomicMapPosition));
                Assert.IsTrue(string.IsNullOrEmpty(signalPeptideQualifiersList[0].Label));
                Assert.IsFalse(string.IsNullOrEmpty(signalPeptideQualifiersList[0].OldLocusTag.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(signalPeptideQualifiersList[0].LocusTag.ToString()));

                // Log VSTest GUI.
                ApplicationLog.WriteLine(
                    "GenBank Features P1: Successfully validated the GenBank Features");
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the SignalPeptide '{0}'",
                    signalPeptideQualifiersList.Count.ToString((IFormatProvider)null)));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the SignalPeptide '{0}'",
                    signalPeptideQualifiersList[0].GeneSymbol));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the SignalPeptide '{0}'",
                    signalPeptideQualifiersList[0].Location));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the SignalPeptide '{0}'",
                    signalPeptideQualifiersList[0].Note));
            }
        }

        /// <summary>
        /// Validate GenBank RepeatRegion feature Qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of GenBank RepeatRegion feature Qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGenBankRepeatRegionFeatureQualifiers()
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankRepeatRegionQualifiersNode,
                Constants.FilePathNode);
            string repeatRegionCount = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankRepeatRegionQualifiersNode,
                Constants.ThreePrimeUTRCount);
            string expectedLocation = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankRepeatRegionQualifiersNode,
                Constants.RepeatRegionLocation);

            // Parse a GenBank file.
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                IEnumerable<ISequence> seqList = parserObj.Parse();
                LocationBuilder locBuilder = new LocationBuilder();

                // Validate RepeatRegion feature all qualifiers.
                GenBankMetadata metadata =
                    (GenBankMetadata)seqList.ElementAt(0).Metadata[Constants.GenBank];
                List<RepeatRegion> repeatRegionsList =
                    metadata.Features.RepeatRegions;
                Assert.AreEqual(repeatRegionsList.Count.ToString((IFormatProvider)null),
                    repeatRegionCount);
                Assert.IsTrue(string.IsNullOrEmpty(repeatRegionsList[0].GeneSymbol));
                Assert.AreEqual(locBuilder.GetLocationString(
                      metadata.Features.RepeatRegions[0].Location),
                      expectedLocation);
                Assert.IsFalse(string.IsNullOrEmpty(repeatRegionsList[0].DatabaseCrossReference.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(repeatRegionsList[0].Note.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(repeatRegionsList[0].Allele));
                Assert.IsFalse(string.IsNullOrEmpty(repeatRegionsList[0].Citation.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(repeatRegionsList[0].Experiment.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(repeatRegionsList[0].GeneSynonym.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(repeatRegionsList[0].GenomicMapPosition));
                Assert.IsTrue(string.IsNullOrEmpty(repeatRegionsList[0].Label));
                Assert.IsFalse(string.IsNullOrEmpty(repeatRegionsList[0].OldLocusTag.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(repeatRegionsList[0].LocusTag.ToString()));

                // Log VSTest GUI.
                ApplicationLog.WriteLine(
                    "GenBank Features P1: Successfully validated the GenBank Features");
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the RepeatRegion '{0}'",
                    repeatRegionsList.Count.ToString((IFormatProvider)null)));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the RepeatRegion '{0}'",
                    repeatRegionsList[0].GeneSymbol));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the RepeatRegion '{0}'",
                    repeatRegionsList[0].Location));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the RepeatRegion '{0}'",
                    repeatRegionsList[0].Note));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the RepeatRegion '{0}'",
                    repeatRegionsList[0].MobileElement));
            }
        }

        /// <summary>
        /// Validate GenBank Location resolver subsequence of Repeat region feature.
        /// Input : GenBank file.
        /// Output : Validation of GenBank feature sub sequence.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGenBankSubSequence()
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankRepeatRegionQualifiersNode,
                Constants.FilePathNode);
            string expectedSubSequence = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankRepeatRegionQualifiersNode,
                Constants.ExpectedFeatureSubSequence);

            // Parse a GenBank file.
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                ISequence sequence = parserObj.Parse().FirstOrDefault();
                ILocationResolver locResolver = new LocationResolver();

                // Get repeatregion subsequence.
                GenBankMetadata metadata =
                    (GenBankMetadata)sequence.Metadata[Constants.GenBank];
                ISequence subSeq = locResolver.GetSubSequence(
                    metadata.Features.RepeatRegions[0].Location, sequence);
                string sequenceString = new string(subSeq.Select(a => (char)a).ToArray());

                // Validate repeat region subsequence.
                Assert.AreEqual(sequenceString, expectedSubSequence);

                // Log VSTest GUI.
                ApplicationLog.WriteLine(
                    "GenBank Features P1: Successfully validated the GenBank subSequence");
                Console.WriteLine(string.Concat(
                    "Successfully validated the GenBank subSequence ",
                    sequenceString));
            }
        }

        /// <summary>
        /// Validate GenBank IsInStart,IsInEnd and IsInRange 
        /// methods of location resolver.
        /// Input : GenBank file.
        /// Output : Validation of GenBank feature sub sequence.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGenBankLocationStartAndEndRange()
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.GenBankRepeatRegionQualifiersNode,
                Constants.FilePathNode);
            bool startLocResult = false;
            bool endLocResult = false;
            bool rangeLocResult = false;

            // Parse a GenBank file.
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                ISequence sequence = parserObj.Parse().FirstOrDefault();
                ILocationResolver locResolver = new LocationResolver();

                // Validate Start,End and Range of Gene feature location.
                GenBankMetadata metadata =
                    (GenBankMetadata)sequence.Metadata[Constants.GenBank];
                startLocResult =
                    locResolver.IsInStart(metadata.Features.Genes[0].Location, 289);
                endLocResult =
                    locResolver.IsInEnd(metadata.Features.Genes[0].Location, 1647);
                rangeLocResult =
                    locResolver.IsInRange(metadata.Features.Genes[0].Location, 300);

                Assert.IsTrue(startLocResult);
                Assert.IsTrue(endLocResult);
                Assert.IsTrue(rangeLocResult);

                // Log VSTest GUI.
                ApplicationLog.WriteLine(
                    "GenBank Features P1: Successfully validated the location resolver Gene feature start End and IsInRange methods");
                Console.WriteLine(string.Concat(
                    "Successfully validated the location resolver Gene feature start",
                    startLocResult));
            }
        }

        /// <summary>
        /// Validate LocationRange creation.
        /// Input : GenBank file.
        /// Output : Validation of created location range.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateLocationRangeWithAccession()
        {
            // Get Values from XML node.
            string acessionNumber = utilityObj.xmlUtil.GetTextValue(
                Constants.LocationRangesNode, Constants.Accession);
            string startLoc = utilityObj.xmlUtil.GetTextValue(
                Constants.LocationRangesNode, Constants.LoocationStartNode);
            string endLoc = utilityObj.xmlUtil.GetTextValue(
                Constants.LocationRangesNode, Constants.LoocationEndNode);

            // Create a Location Range.
            LocationRange locRangeObj = new LocationRange(acessionNumber,
                Convert.ToInt32(startLoc, (IFormatProvider)null), Convert.ToInt32(endLoc, (IFormatProvider)null));

            // Validate created location Range.
            Assert.AreEqual(acessionNumber, locRangeObj.Accession.ToString((IFormatProvider)null));
            Assert.AreEqual(startLoc, locRangeObj.StartPosition.ToString((IFormatProvider)null));
            Assert.AreEqual(endLoc, locRangeObj.EndPosition.ToString((IFormatProvider)null));

            // Log VSTest GUI.
            ApplicationLog.WriteLine(
                "GenBank Features P1: Successfully validated the creation of location range");
            Console.WriteLine(string.Concat(
                "Successfully validated the start location",
                startLoc));

        }

        /// <summary>
        /// Validate LocationRange creation with empty accession ID.
        /// Input : GenBank file.
        /// Output : Validation of created location range.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateLocationRanges()
        {
            // Get Values from XML node.
            string startLoc = utilityObj.xmlUtil.GetTextValue(
                Constants.LocationRangesNode, Constants.LoocationStartNode);
            string endLoc = utilityObj.xmlUtil.GetTextValue(
                Constants.LocationRangesNode, Constants.LoocationEndNode);

            // Create a Location Range.
            LocationRange locRangeObj = new LocationRange(Convert.ToInt32(startLoc, (IFormatProvider)null),
               Convert.ToInt32(endLoc, (IFormatProvider)null));

            // Validate created location Range.
            Assert.AreEqual(startLoc, locRangeObj.StartPosition.ToString((IFormatProvider)null));
            Assert.AreEqual(endLoc, locRangeObj.EndPosition.ToString((IFormatProvider)null));

            // Log VSTest GUI.
            ApplicationLog.WriteLine(
                "GenBank Features P1: Successfully validated the creation of location range");
            Console.WriteLine(string.Concat(
                "Successfully validated the start location",
                startLoc));

        }

        /// <summary>
        /// Validate Misc Structure qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of Misc Structure qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateMiscStructureQualifiers()
        {
            ValidateGeneralGenBankFeatureQualifiers(
                Constants.GenBankMiscStructureNode, FeatureGroup.MiscStructure);
        }

        /// <summary>
        /// Validate GenBank RibosomeBindingSite qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of RibosomeBindingSite qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateRibosomeBindingSiteQualifiers()
        {
            ValidateGeneralGenBankFeatureQualifiers(
                Constants.GenBankRibosomeSiteBindingNode, FeatureGroup.RibosomeBindingSite);
        }

        /// <summary>
        /// Validate TransitPeptide feature qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of TransitPeptide qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateTransitPeptideQualifiers()
        {
            ValidateGeneralGenBankFeatureQualifiers(
                Constants.GenBankTransitPeptideNode, FeatureGroup.TrnsitPeptide);
        }

        /// <summary>
        /// Validate Stem Loop feature qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of Stem Loop qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateStemLoopQualifiers()
        {
            ValidateGeneralGenBankFeatureQualifiers(
                Constants.GenBankStemLoopNode, FeatureGroup.StemLoop);
        }

        /// <summary>
        /// Validate Modified base feature qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of Modified base qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateModifiedBaseQualifiers()
        {
            ValidateGeneralGenBankFeatureQualifiers(
                Constants.GenBankModifiedBaseNode, FeatureGroup.ModifiedBase);
        }

        /// <summary>
        /// Validate Precursor RNA feature qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of Precursor RNA qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePrecursorRNAQualifiers()
        {
            ValidateGeneralGenBankFeatureQualifiers(
                Constants.GenBankPrecursorRNANode, FeatureGroup.PrecursorRNA);
        }

        /// <summary>
        /// Validate Poly Site feature qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of Poly Site qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidatePolySiteQualifiers()
        {
            ValidateGeneralGenBankFeatureQualifiers(
                Constants.GenBankPolySiteNode, FeatureGroup.PolySite);
        }

        /// <summary>
        /// Validate Misc Binding feature qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of Misc Binding qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateMiscBindingQualifiers()
        {
            ValidateGeneralGenBankFeatureQualifiers(
                Constants.GenBankMiscBindingNode,
                FeatureGroup.MiscBinding);
        }

        /// <summary>
        /// Validate Enhancer feature qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of Enhancer qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateEnhancerQualifiers()
        {
            ValidateGeneralGenBankFeatureQualifiers(
                Constants.GenBankEnhancerNode, FeatureGroup.Enhancer);
        }

        /// <summary>
        /// Validate GC_Signal feature qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of GC_Signal qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGCSignalQualifiers()
        {
            ValidateGeneralGenBankFeatureQualifiers(
                Constants.GenBankGCSignalNode, FeatureGroup.GCSignal);
        }

        /// <summary>
        /// Validate Long Terminal Repeat feature qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of Long Terminal Repeat qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateLTRFeatureQualifiers()
        {
            ValidateGeneralGenBankFeatureQualifiers(
                Constants.GenBankLongTerminalRepeatNode, FeatureGroup.LTR);
        }

        /// <summary>
        /// Validate Operon region feature qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of Operon region qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateOperonFeatureQualifiers()
        {
            ValidateGeneralGenBankFeatureQualifiers(
                Constants.GenBankOperonFeatureNode, FeatureGroup.Operon);
        }

        /// <summary>
        /// Validate Unsure Sequence region feature qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of Unsure Sequence region qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateUnsureSeqRegionFeatureQualifiers()
        {
            ValidateGeneralGenBankFeatureQualifiers(
                Constants.GenBankUnsureSequenceRegionNode,
                FeatureGroup.UnsureSequenceRegion);
        }

        /// <summary>
        /// Validate NonCoding RNA feature qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of NonCoding RNA qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateNonCodingRNAFeatureQualifiers()
        {
            ValidateGeneralGenBankFeatureQualifiers(
                Constants.GenBankNonCodingRNANode,
                FeatureGroup.NonCodingRNA);
        }

        /// <summary>
        /// Validate CDS feature qualifiers.
        /// Input : GenBank file.
        /// Output : Validation of CDS qualifiers.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateCDSFeatureQualifiers()
        {
            ValidateGeneralGenBankFeatureQualifiers(
                Constants.GenBankCDSNode, FeatureGroup.CDS);
        }

        /// <summary>
        /// Validate Standard Feature qualifier names.
        /// Input : GenBank file.
        /// Output : Validation of Standard Qaulifier names.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateStandardQualifierNames()
        {
            // Get Values from XML node.
            string expectedAlleleQualifier = utilityObj.xmlUtil.GetTextValue(
                Constants.StandardQualifierNamesNode, Constants.AlleleQualifier);
            string expectedGeneSymbolQualifier = utilityObj.xmlUtil.GetTextValue(
                Constants.StandardQualifierNamesNode, Constants.GeneQualifier);
            string expectedDbReferenceQualifier = utilityObj.xmlUtil.GetTextValue(
                Constants.StandardQualifierNamesNode, Constants.DBReferenceQualifier);
            string allQualifiersCount = utilityObj.xmlUtil.GetTextValue(
                Constants.StandardQualifierNamesNode, Constants.QualifierCount);

            // Validate GenBank feature standard qualifier names.
            Assert.AreEqual(StandardQualifierNames.Allele,
                expectedAlleleQualifier);
            Assert.AreEqual(StandardQualifierNames.GeneSymbol,
                expectedGeneSymbolQualifier);
            Assert.AreEqual(StandardQualifierNames.DatabaseCrossReference,
                expectedDbReferenceQualifier);
            Assert.AreEqual(StandardQualifierNames.All.Count.ToString((IFormatProvider)null),
                allQualifiersCount);

            //Log to VSTest GUI.
            Console.WriteLine(string.Format((IFormatProvider)null,
                "validated the standard qualifier name '{0}'",
                StandardQualifierNames.Allele.ToString((IFormatProvider)null)));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "validated the standard qualifier name '{0}'",
                StandardQualifierNames.All.Count));
        }

        /// <summary>
        /// Validate subsequence from GenBank sequence with location using 
        /// compliment operator.
        /// Input : GenBank sequence,location.
        /// Output : Validation of expected sub sequence.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateSubSequenceWithComplimentOperator()
        {
            ValidateGenBankLocationResolver(
                Constants.LocationWithComplementOperator,
                FeatureOperator.Complement, true);
        }

        /// <summary>
        /// Validate subsequence from GenBank sequence with location using 
        /// join operator.
        /// Input : GenBank sequence,location.
        /// Output : Validation of expected sub sequence.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateSubSequenceWithJoinOperator()
        {
            ValidateGenBankLocationResolver(
                Constants.LocationWithJoinOperatorNode,
                FeatureOperator.Join, true);
        }

        /// <summary>
        /// Validate subsequence from GenBank sequence with location using 
        /// order operator.
        /// Input : GenBank sequence,location.
        /// Output : Validation of expected sub sequence.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateSubSequenceWithOrderOperator()
        {
            ValidateGenBankLocationResolver(
                Constants.LocationWithJoinOperatorNode,
                FeatureOperator.Order, true);
        }

        /// <summary>
        /// Validate subsequence from GenBank sequence with location using 
        /// dot operator.
        /// Input : GenBank sequence,location.
        /// Output : Validation of expected sub sequence.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateSubSequenceWithDotOperator()
        {
            ValidateGenBankLocationResolver(
                Constants.LocationWithDotOperatorNode,
                FeatureOperator.Complement, false);
        }

        /// <summary>
        /// Validate subsequence from GenBank sequence with location using 
        /// compliment operator with sub location.
        /// Input : GenBank sequence,location.
        /// Output : Validation of expected sub sequence.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateSubSequenceWithSubLocationComplimentOperator()
        {
            ValidateGenBankLocationResolver(
                Constants.LocationWithOutComplementOperatorNode,
                FeatureOperator.Complement, false);
        }

        /// <summary>
        /// Validate subsequence from GenBank sequence with location using 
        /// join operator with sub location.
        /// Input : GenBank sequence,location.
        /// Output : Validation of expected sub sequence.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateSubSequenceWithSubLocationJoinOperator()
        {
            ValidateGenBankLocationResolver(
                Constants.LocationWithOutJoinOperatorNode,
                FeatureOperator.Join, false);
        }

        /// <summary>
        /// Validate subsequence from GenBank sequence with location using 
        /// order operator with sub location.
        /// Input : GenBank sequence,location.
        /// Output : Validation of expected sub sequence.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateSubSequenceWithSubLocationOrderOperator()
        {
            ValidateGenBankLocationResolver(
                Constants.LocationWithOutOrderOperatorNode,
                FeatureOperator.Order, false);
        }

        /// <summary>
        /// Validate GenBank location EndData.
        /// Input : GenBank sequence,location.
        /// Output : Validation of location end data.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGenBankLocationEndData()
        {
            ValidateLocationEndData(Constants.LocationWithEndDataNode);
        }

        /// <summary>
        /// Validate GenBank location EndData with "^" operator.
        /// Input : GenBank sequence,location.
        /// Output : Validation of location end data.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGenBankLocationEndDataWithOperator()
        {
            ValidateLocationEndData(Constants.LocationWithEndDataUsingOperatorNode);
        }

        /// <summary>
        /// Validate compare GenBank locations.
        /// Input : Two instances of GenBank locations.
        /// Output : Validate compare two instance of the locations object.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateCompareGenBankLocationsObject()
        {
            // Get Values from XML node.
            string locationFirstInput = utilityObj.xmlUtil.GetTextValue(
                Constants.CompareLocationsNode, Constants.Location1Node);
            string locationSecondInput = utilityObj.xmlUtil.GetTextValue(
                Constants.CompareLocationsNode, Constants.Location2Node);
            string locationThirdInput = utilityObj.xmlUtil.GetTextValue(
                Constants.CompareLocationsNode, Constants.Location3Node);

            // Create two location instance.
            ILocationBuilder locBuilder = new LocationBuilder();
            ILocation loc1 = locBuilder.GetLocation(locationFirstInput);
            object loc2 = locBuilder.GetLocation(locationSecondInput);
            object loc3 = locBuilder.GetLocation(locationThirdInput);

            // Compare first and second location instances.
            Assert.AreEqual(0, loc1.CompareTo(loc2));

            // Compare first and third location which are not identical.
            Assert.AreEqual(-1, loc1.CompareTo(loc3));

            // Compare first and null location.
            Assert.AreEqual(1, loc1.CompareTo(null));

            ApplicationLog.WriteLine(
                "GenBank Features P1: Successfully validated the GenBank Features");
            Console.WriteLine(string.Format((IFormatProvider)null,
                "GenBank Features P1: Successfully validated the Gene feature"));

        }

        /// <summary>
        /// Validate leaf location of GenBank locations.
        /// Input : GenBank File
        /// Output : Validation of GenBank leaf locations.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGenBankLeafLocations()
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                Constants.RNAGenBankFeaturesNode, Constants.FilePathNode);
            string expectedLeafLocation = utilityObj.xmlUtil.GetTextValue(
                Constants.RNAGenBankFeaturesNode,
                Constants.LeafLocationCountNode);

            // Parse a GenBank file.
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                IEnumerable<ISequence> seqList = parserObj.Parse();

                // Validate Minus35Signal feature all qualifiers.
                GenBankMetadata metadata =
                    (GenBankMetadata)seqList.ElementAt(0).Metadata[Constants.GenBank];

                List<CodingSequence> cdsList = metadata.Features.CodingSequences;

                ILocation newLoc = cdsList[0].Location;
                List<ILocation> leafsList = newLoc.GetLeafLocations();

                Assert.AreEqual(expectedLeafLocation, leafsList.Count.ToString((IFormatProvider)null));
                ApplicationLog.WriteLine(
                    "GenBank Features P1: Successfully validated the GenBank Features");
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the Gene feature"));
            }
        }

        /// <summary>
        /// Validate GenBank locations positions.
        /// Input : GenBank File
        /// Output : Validation of GenBank location positions.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        [TestCategory("Priority1")]
        public void ValidateGenBankLocationPositions()
        {
            // Get Values from XML node.
            string location = utilityObj.xmlUtil.GetTextValue(
                Constants.LocationWithEndDataUsingOperatorNode,
                Constants.Location);
            string expectedEndData = utilityObj.xmlUtil.GetTextValue(
                Constants.LocationWithEndDataUsingOperatorNode,
                Constants.EndData);
            string expectedStartData = utilityObj.xmlUtil.GetTextValue(
                Constants.LocationWithEndDataUsingOperatorNode,
                Constants.StartData);
            string position = utilityObj.xmlUtil.GetTextValue(
                Constants.LocationWithEndDataUsingOperatorNode,
                Constants.Position);

            bool result = false;

            // Build a location.
            LocationResolver locResolver = new LocationResolver();
            ILocationBuilder locBuilder = new LocationBuilder();

            Location loc = (Location)locBuilder.GetLocation(location);
            loc.EndData = expectedEndData;
            loc.StartData = expectedStartData;

            // Validate whether mentioned end data is present in the location
            // or not.
            result = locResolver.IsInEnd(loc, Int32.Parse(position, (IFormatProvider)null));
            Assert.IsTrue(result);

            // Validate whether mentioned start data is present in the location
            // or not.
            result = locResolver.IsInStart(loc, Int32.Parse(position, (IFormatProvider)null));
            Assert.IsTrue(result);

            // Validate whether mentioned data is present in the location
            // or not.
            result = locResolver.IsInRange(loc, Int32.Parse(position, (IFormatProvider)null));
            Assert.IsTrue(result);

            // Log to VSTest GUI.
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "GenBankFeatures P1 : Expected sequence is verified"));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "GenBankFeatures P1 : Expected sequence is verified"));

        }

        #endregion GenBank P1 TestCases

        #region Supporting Methods

        /// <summary>
        /// Validate GenBank Gene features.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        void ValidateGenBankGeneFeatureQualifiers(string nodeName)
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string GenesCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GeneCount);
            string GenesDBCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.CDSCodonStart);
            string geneSymbol = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GeneFeatureGeneSymbol);

            // Parse a GenBank file.
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                IEnumerable<ISequence> seqList = parserObj.Parse();

                // Validate Minus35Signal feature all qualifiers.
                GenBankMetadata metadata =
                    (GenBankMetadata)seqList.ElementAt(0).Metadata[Constants.GenBank];
                List<Gene> genesList = metadata.Features.Genes;
                Assert.AreEqual(genesList.Count.ToString((IFormatProvider)null), GenesCount);
                Assert.AreEqual(genesList[0].GeneSymbol.ToString((IFormatProvider)null),
                    geneSymbol);
                Assert.AreEqual(genesList[0].DatabaseCrossReference.Count,
                    Convert.ToInt32(GenesDBCount, (IFormatProvider)null));
                Assert.IsTrue(string.IsNullOrEmpty(genesList[0].Allele));
                Assert.IsFalse(string.IsNullOrEmpty(genesList[0].Citation.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(genesList[0].Experiment.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(genesList[0].Function.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(genesList[0].GeneSynonym.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(genesList[0].GenomicMapPosition.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(genesList[0].Inference.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(genesList[0].Label.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(genesList[0].Note.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(genesList[0].Operon.ToString((IFormatProvider)null)));
                Assert.IsFalse(string.IsNullOrEmpty(genesList[0].OldLocusTag.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(genesList[0].Phenotype.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(genesList[0].Product.ToString()));
                Assert.IsFalse(genesList[0].Pseudo);
                Assert.IsTrue(string.IsNullOrEmpty(genesList[0].StandardName.ToString()));
                Assert.IsFalse(genesList[0].TransSplicing);

                // Log VSTest GUI.
                ApplicationLog.WriteLine(
                    "GenBank Features P1: Successfully validated the GenBank Features");
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the Gene feature '{0}'",
                    genesList[0].GeneSymbol.ToString((IFormatProvider)null)));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the Gene feature '{0}'",
                    genesList.Count.ToString((IFormatProvider)null)));
            }
        }

        /// <summary>
        /// Validate GenBank tRNA features.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        void ValidateGenBanktRNAFeatureQualifiers(string nodeName)
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string tRNAsCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.tRNACount);
            string tRNAGeneSymbol = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.TRNAGeneSymbol);
            string tRNAComplement = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.TRNAComplement);
            string tRNADBCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.CDSCodonStart);

            // Parse a GenBank file.
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                IEnumerable<ISequence> seqList = parserObj.Parse();
                LocationBuilder locBuilder = new LocationBuilder();

                GenBankMetadata metadata =
                    (GenBankMetadata)seqList.ElementAt(0).Metadata[Constants.GenBank];
                List<TransferRna> tRANsList =
                    metadata.Features.TransferRNAs;
                Assert.AreEqual(tRANsList.Count.ToString((IFormatProvider)null),
                    tRNAsCount);
                Assert.AreEqual(tRANsList[0].GeneSymbol.ToString((IFormatProvider)null),
                    tRNAGeneSymbol);
                Assert.AreEqual(tRANsList[0].DatabaseCrossReference.Count,
                    Convert.ToInt32(tRNADBCount, (IFormatProvider)null));
                Assert.IsTrue(string.IsNullOrEmpty(tRANsList[0].Allele));
                Assert.IsFalse(string.IsNullOrEmpty(tRANsList[0].Citation.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(tRANsList[0].Experiment.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(tRANsList[0].Function.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(tRANsList[0].GeneSynonym.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(tRANsList[0].GenomicMapPosition.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(tRANsList[0].Inference.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(tRANsList[0].Label.ToString()));
                Assert.AreEqual(locBuilder.GetLocationString(
                    metadata.Features.TransferRNAs[0].Location),
                    tRNAComplement);
                Assert.IsFalse(string.IsNullOrEmpty(tRANsList[0].Note.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(tRANsList[0].OldLocusTag.ToString()));
                Assert.IsFalse(tRANsList[0].Pseudo);
                Assert.IsTrue(string.IsNullOrEmpty(tRANsList[0].StandardName.ToString()));
                Assert.IsFalse(tRANsList[0].TransSplicing);

                // Log VSTest GUI.
                ApplicationLog.WriteLine(
                    "GenBank Features P1: Successfully validated the GenBank Features");
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the tRNA feature '{0}'",
                    tRANsList[0].GeneSymbol.ToString((IFormatProvider)null)));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the tRNA feature '{0}'",
                    tRANsList.Count.ToString((IFormatProvider)null)));
            }
        }

        /// <summary>
        /// Validate GenBank mRNA features.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        void ValidateGenBankmRNAFeatureQualifiers(string nodeName)
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string mRNAGeneSymbol = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.MRNAGeneSymbol);
            string mRNAComplement = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.MRNAComplement);
            string mRNADBCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.CDSCodonStart);
            string mRNAStart = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.MRNAComplementStart);
            string mRNAStdName = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.StandardNameNode);
            string mRNAAllele = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.CDSCodonStart);
            string mRNAOperon = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.AlleleNode);

            // Parse a GenBank file.
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                IEnumerable<ISequence> seqList = parserObj.Parse();

                // Validate tRNA feature all qualifiers.
                GenBankMetadata metadata =
                    (GenBankMetadata)seqList.ElementAt(0).Metadata[Constants.GenBank];
                List<MessengerRna> mRANsList =
                    metadata.Features.MessengerRNAs;

                // Create a copy of mRNA list
                MessengerRna mRNAClone = mRANsList[0].Clone();
                Assert.AreEqual(mRANsList[0].GeneSymbol.ToString((IFormatProvider)null),
                    mRNAGeneSymbol);
                Assert.AreEqual(mRANsList[0].DatabaseCrossReference.Count,
                    Convert.ToInt32(mRNADBCount, (IFormatProvider)null));
                Assert.IsTrue(string.IsNullOrEmpty(mRNAClone.Allele));
                Assert.IsFalse(string.IsNullOrEmpty(mRANsList[0].Citation.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(mRANsList[0].Experiment.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(mRANsList[0].Function.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(mRANsList[0].GeneSynonym.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(mRANsList[0].GenomicMapPosition.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(mRANsList[0].Inference.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(mRANsList[0].Label.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(mRANsList[0].LocusTag.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(mRANsList[0].Operon.ToString((IFormatProvider)null)));
                Assert.IsFalse(string.IsNullOrEmpty(mRANsList[0].Product.ToString()));
                Assert.AreEqual(mRANsList[0].Location.Operator.ToString(),
                    mRNAComplement);
                Assert.IsNull(mRANsList[0].Location.Separator);
                Assert.AreEqual(mRANsList[0].Location.LocationStart,
                    Convert.ToInt32(mRNAStart, (IFormatProvider)null));
                Assert.IsNull(mRANsList[0].Location.StartData);
                Assert.IsNull(mRANsList[0].Location.EndData);
                Assert.IsFalse(string.IsNullOrEmpty(mRANsList[0].OldLocusTag.ToString()));
                Assert.IsFalse(mRANsList[0].Pseudo);
                Assert.IsTrue(string.IsNullOrEmpty(mRANsList[0].StandardName.ToString()));
                Assert.IsFalse(mRANsList[0].TransSplicing);
                Assert.IsFalse(string.IsNullOrEmpty(mRANsList[0].LocusTag.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(mRANsList[0].Operon.ToString((IFormatProvider)null)));
                Assert.IsFalse(string.IsNullOrEmpty(mRANsList[0].Product.ToString()));

                // Create a new mRNA feature using constructor.
                MessengerRna mRNA = new MessengerRna(
                    metadata.Features.MessengerRNAs[0].Location);

                // Set and validate qualifiers.
                mRNA.GeneSymbol = mRNAGeneSymbol;
                mRNA.Allele = mRNAAllele;
                mRNA.Operon = mRNAOperon;
                mRNA.StandardName = mRNAStdName;

                // Validate properties.
                Assert.AreEqual(mRNA.GeneSymbol, mRNAGeneSymbol);
                Assert.AreEqual(mRNA.Allele, mRNAAllele);
                Assert.AreEqual(mRNA.Operon, mRNAOperon);
                Assert.AreEqual(mRNA.StandardName, mRNAStdName);

                // Log VSTest GUI.
                ApplicationLog.WriteLine(
                "GenBank Features P1: Successfully validated the GenBank Features");
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated GeneSymbol'{0}'",
                    mRANsList[0].GeneSymbol.ToString((IFormatProvider)null)));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated mRNA count'{0}'",
                    mRANsList.Count.ToString((IFormatProvider)null)));
            }
        }

        /// <summary>
        /// Validate GenBank features for medium size sequences.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        /// <param name="methodName">DNA,RNA or Protein method</param>
        void ValidateGenBankFeatures(string nodeName, string methodName)
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string mRNAFeatureCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.mRNACount);
            string exonFeatureCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExonCount);
            string intronFeatureCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.IntronCount);
            string cdsFeatureCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.CDSCount);
            string allFeaturesCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GenBankFeaturesCount);
            string expectedCDSKey = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.CDSKey);
            string expectedIntronKey = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.IntronKey);
            string expectedExonKey = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExonKey);
            string mRNAKey = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.mRNAKey);
            string sourceKeyName = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.SourceKey);

            // Parse a file.
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                IEnumerable<ISequence> sequenceList = parserObj.Parse();

                // GenBank metadata.
                GenBankMetadata metadata = new GenBankMetadata();
                if (1 == sequenceList.Count())
                {
                    metadata =
                        sequenceList.ElementAt(0).Metadata[Constants.GenBank] as GenBankMetadata;
                }
                else
                {
                    metadata =
                        sequenceList.ElementAt(1).Metadata[Constants.GenBank] as GenBankMetadata;
                }

                // Validate GenBank Features.
                Assert.AreEqual(metadata.Features.All.Count,
                    Convert.ToInt32(allFeaturesCount, (IFormatProvider)null));
                Assert.AreEqual(metadata.Features.CodingSequences.Count,
                    Convert.ToInt32(cdsFeatureCount, (IFormatProvider)null));
                Assert.AreEqual(metadata.Features.Exons.Count,
                    Convert.ToInt32(exonFeatureCount, (IFormatProvider)null));
                Assert.AreEqual(metadata.Features.Introns.Count,
                    Convert.ToInt32(intronFeatureCount, (IFormatProvider)null));
                Assert.AreEqual(metadata.Features.MessengerRNAs.Count,
                    Convert.ToInt32(mRNAFeatureCount, (IFormatProvider)null));
                Assert.AreEqual(metadata.Features.Attenuators.Count, 0);
                Assert.AreEqual(metadata.Features.CAATSignals.Count, 0);
                Assert.AreEqual(metadata.Features.DisplacementLoops.Count, 0);
                Assert.AreEqual(metadata.Features.Enhancers.Count, 0);

                // Validate GenBank feature list.
                if ((0 == string.Compare(methodName, "DNA",
                    CultureInfo.CurrentCulture, CompareOptions.IgnoreCase))
                    || (0 == string.Compare(methodName, "RNA",
                    CultureInfo.CurrentCulture, CompareOptions.IgnoreCase)))
                {
                    IList<FeatureItem> featureList = metadata.Features.All;
                    Assert.AreEqual(featureList[0].Key.ToString((IFormatProvider)null), sourceKeyName);
                    Assert.AreEqual(featureList[1].Key.ToString((IFormatProvider)null), expectedCDSKey);
                    Assert.AreEqual(featureList[2].Key.ToString((IFormatProvider)null), expectedCDSKey);
                    Assert.AreEqual(featureList[10].Key.ToString((IFormatProvider)null), mRNAKey);
                    Assert.AreEqual(featureList[12].Key.ToString((IFormatProvider)null), expectedExonKey);
                    Assert.AreEqual(featureList[18].Key.ToString((IFormatProvider)null), expectedIntronKey);
                    ApplicationLog.WriteLine(
                    "GenBank Features P1: Successfully validated the GenBank Features");
                    Console.WriteLine(string.Format((IFormatProvider)null,
                        "GenBank Features P1: Successfully validated the CDS feature '{0}'",
                        featureList[2].Key.ToString((IFormatProvider)null)));
                    Console.WriteLine(string.Format((IFormatProvider)null,
                        "GenBank Features P1: Successfully validated the Intron feature '{0}'",
                        featureList[10].Key.ToString((IFormatProvider)null)));
                }
                else
                    if ((0 == string.Compare(methodName, "Protein", CultureInfo.CurrentCulture, 
                        CompareOptions.IgnoreCase)))
                    {
                        IList<FeatureItem> featureList = metadata.Features.All;
                        Assert.AreEqual(featureList[10].Key.ToString((IFormatProvider)null), expectedIntronKey);
                        Assert.AreEqual(featureList[18].Key.ToString((IFormatProvider)null), expectedExonKey);
                        ApplicationLog.WriteLine(
                        "GenBank Features P1: Successfully validated the GenBank Features");
                        Console.WriteLine(string.Format((IFormatProvider)null,
                            "GenBank Features P1: Successfully validated the Intron feature '{0}'",
                            featureList[10].Key.ToString((IFormatProvider)null)));
                        Console.WriteLine(string.Format((IFormatProvider)null,
                            "GenBank Features P1: Successfully validated the Exon feature '{0}'",
                            featureList[1].Key.ToString((IFormatProvider)null)));
                    }
            }
        }

        /// <summary>
        /// Validate GenBank features for medium size sequences.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        void ValidateCloneGenBankFeatures(string nodeName)
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string mRNAFeatureCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.mRNACount);
            string exonFeatureCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExonCount);
            string intronFeatureCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.IntronCount);
            string cdsFeatureCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.CDSCount);
            string allFeaturesCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GenBankFeaturesCount);

            // Parse a file.
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                IEnumerable<ISequence> sequenceList = parserObj.Parse();

                GenBankMetadata metadata =
                    sequenceList.ElementAt(0).Metadata[Constants.GenBank] as GenBankMetadata;

                // Validate GenBank Features before Cloning.
                Assert.AreEqual(metadata.Features.All.Count,
                    Convert.ToInt32(allFeaturesCount, (IFormatProvider)null));
                Assert.AreEqual(metadata.Features.CodingSequences.Count,
                    Convert.ToInt32(cdsFeatureCount, (IFormatProvider)null));
                Assert.AreEqual(metadata.Features.Exons.Count,
                    Convert.ToInt32(exonFeatureCount, (IFormatProvider)null));
                Assert.AreEqual(metadata.Features.Introns.Count,
                    Convert.ToInt32(intronFeatureCount, (IFormatProvider)null));
                Assert.AreEqual(metadata.Features.MessengerRNAs.Count,
                    Convert.ToInt32(mRNAFeatureCount, (IFormatProvider)null));
                Assert.AreEqual(metadata.Features.Attenuators.Count, 0);
                Assert.AreEqual(metadata.Features.CAATSignals.Count, 0);
                Assert.AreEqual(metadata.Features.DisplacementLoops.Count, 0);
                Assert.AreEqual(metadata.Features.Enhancers.Count, 0);

                // Clone GenBank Features.
                GenBankMetadata CloneGenBankMetadat = metadata.Clone();

                // Validate cloned GenBank Metadata.
                Assert.AreEqual(CloneGenBankMetadat.Features.All.Count,
                    Convert.ToInt32(allFeaturesCount, (IFormatProvider)null));
                Assert.AreEqual(CloneGenBankMetadat.Features.CodingSequences.Count,
                    Convert.ToInt32(cdsFeatureCount, (IFormatProvider)null));
                Assert.AreEqual(CloneGenBankMetadat.Features.Exons.Count,
                    Convert.ToInt32(exonFeatureCount, (IFormatProvider)null));
                Assert.AreEqual(CloneGenBankMetadat.Features.Introns.Count,
                    Convert.ToInt32(intronFeatureCount, (IFormatProvider)null));
                Assert.AreEqual(CloneGenBankMetadat.Features.MessengerRNAs.Count,
                    Convert.ToInt32(mRNAFeatureCount, (IFormatProvider)null));
                Assert.AreEqual(CloneGenBankMetadat.Features.Attenuators.Count, 0);
                Assert.AreEqual(CloneGenBankMetadat.Features.CAATSignals.Count, 0);
                Assert.AreEqual(CloneGenBankMetadat.Features.DisplacementLoops.Count, 0);
                Assert.AreEqual(CloneGenBankMetadat.Features.Enhancers.Count, 0);

                // Log to GUI VSTest.
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the Intron feature '{0}'",
                    CloneGenBankMetadat.Features.Introns.Count));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the Exons feature '{0}'",
                    CloneGenBankMetadat.Features.Exons.Count));
            }
        }

        /// <summary>
        /// Validate GenBank standard features key for medium size sequences..
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        /// <param name="methodName">DNA,RNA or Protein method</param>
        void ValidateGenBankStandardFeatures(string nodeName,
            string methodName)
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string expectedCondingSeqCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.CDSCount);
            string expectedCDSKey = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.CDSKey);
            string expectedIntronKey = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.IntronKey);
            string mRNAKey = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.mRNAKey);
            string allFeaturesCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.StandardFeaturesCount);

            // Parse a file.
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                IEnumerable<ISequence> seq = parserObj.Parse();

                GenBankMetadata metadata =
                    seq.ElementAt(0).Metadata[Constants.GenBank] as GenBankMetadata;

                if ((0 == string.Compare(methodName, "DNA",
                     CultureInfo.CurrentCulture,CompareOptions.IgnoreCase))
                  || (0 == string.Compare(methodName, "RNA",
                    CultureInfo.CurrentCulture, CompareOptions.IgnoreCase)))
                {
                    Assert.AreEqual(StandardFeatureKeys.CodingSequence.ToString((IFormatProvider)null),
                        expectedCDSKey);
                    Assert.AreEqual(StandardFeatureKeys.Intron.ToString((IFormatProvider)null),
                        expectedIntronKey);
                    Assert.AreEqual(StandardFeatureKeys.MessengerRna.ToString((IFormatProvider)null),
                        mRNAKey);
                    Assert.AreEqual(StandardFeatureKeys.All.Count.ToString((IFormatProvider)null),
                        allFeaturesCount);

                    //Log to VSTest GUI.
                    Console.WriteLine(string.Format((IFormatProvider)null,
                        "GenBank Features P1: Successfully validated the standard feature key '{0}'",
                        StandardFeatureKeys.Intron.ToString((IFormatProvider)null)));
                    Console.WriteLine(string.Format((IFormatProvider)null,
                        "GenBank Features P1: Successfully validated the standard feature key '{0}'",
                        StandardFeatureKeys.MessengerRna.ToString((IFormatProvider)null)));
                }
                else
                {
                    Assert.AreEqual(metadata.Features.CodingSequences.Count.ToString((IFormatProvider)null),
                        expectedCondingSeqCount);
                    Assert.AreEqual(StandardFeatureKeys.CodingSequence.ToString((IFormatProvider)null),
                        expectedCDSKey);
                    Assert.AreEqual(StandardFeatureKeys.All.Count.ToString((IFormatProvider)null),
                        allFeaturesCount);

                    //Log to VSTest GUI.
                    Console.WriteLine(string.Format((IFormatProvider)null,
                        "GenBank Features P1: Successfully validated the standard feature key '{0}'",
                        StandardFeatureKeys.CodingSequence.ToString((IFormatProvider)null)));
                }
            }
        }


        /// <summary>
        /// Validate GenBank features with specified range.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        void ValidateGetFeatures(string nodeName)
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string expectedFirstRangeStartPoint = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FirstRangeStartPoint);
            string expectedSecondRangeStartPoint = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.SecondRangeStartPoint);
            string expectedFirstRangeEndPoint = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FirstRangeEndPoint);
            string expectedSecondRangeEndPoint = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.SecondRangeEndPoint);
            string expectedCountWithinSecondRange = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.SecondRangeCount);
            string expectedCountWithinFirstRange = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FirstRangeCount);
            string expectedQualifierName = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.CDSKey);
            string expectedQualifiers = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.QualifiersCount);
            string firstFeaturesCount = string.Empty;
            string secodFeaturesCount = string.Empty;

            // Parse a GenBank file.
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                ISequence seq = parserObj.Parse().FirstOrDefault();

                GenBankMetadata metadata =
                    seq.Metadata[Constants.GenBank] as GenBankMetadata;

                // Validate GetFeature within specified range.
                List<FeatureItem> features =
                    metadata.GetFeatures(Convert.ToInt32(
                    expectedFirstRangeStartPoint, (IFormatProvider)null), Convert.ToInt32(
                    expectedFirstRangeEndPoint, (IFormatProvider)null));

                firstFeaturesCount = metadata.GetFeatures(Convert.ToInt32(
                expectedFirstRangeStartPoint, (IFormatProvider)null), Convert.ToInt32(
                expectedFirstRangeEndPoint, (IFormatProvider)null)).Count.ToString((IFormatProvider)null);
                secodFeaturesCount = metadata.GetFeatures(Convert.ToInt32(
                expectedSecondRangeStartPoint, (IFormatProvider)null), Convert.ToInt32(
                expectedSecondRangeEndPoint, (IFormatProvider)null)).Count.ToString((IFormatProvider)null);

                // Validate GenBank features count within specified range.
                Assert.AreEqual(firstFeaturesCount, expectedCountWithinFirstRange);
                Assert.AreEqual(secodFeaturesCount, expectedCountWithinSecondRange);
                Assert.AreEqual(features.Count.ToString((IFormatProvider)null), firstFeaturesCount);
                Assert.AreEqual(features[1].Qualifiers.Count.ToString((IFormatProvider)null),
                    expectedQualifiers);
                Assert.AreEqual(features[1].Key.ToString((IFormatProvider)null), expectedQualifierName);

                // Log VSTest GUI.
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the Get GenBank features '{0}'",
                    firstFeaturesCount));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the Get GenBank features '{0}'",
                    secodFeaturesCount));
            }
        }

        /// <summary>
        /// Validate GenBank Citation referenced present in GenBank Metadata.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        /// <param name="featureName">Feature Name</param>
        void ValidateCitationReferenced(string nodeName,
            FeatureGroup featureName)
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string expectedCitationReferenced = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.citationReferencedCount);
            string expectedmRNACitationReferenced = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.citationReferencedCount);
            string expectedExonACitationReferenced = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExonCitationReferencedCount);
            string expectedIntronCitationReferenced = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.IntronCitationReferencedCount);
            string expectedpromoterCitationReferenced = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.PromotersCitationReferencedCount);

            // Parse a GenBank file.
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                ISequence seq = parserObj.Parse().FirstOrDefault();

                GenBankMetadata metadata =
                    seq.Metadata[Constants.GenBank] as GenBankMetadata;

                List<CitationReference> citationReferenceList;

                // Get a list citationReferenced present in GenBank file.
                switch (featureName)
                {
                    case FeatureGroup.CDS:
                        FeatureItem cds =
                            metadata.Features.CodingSequences[0];
                        citationReferenceList =
                            metadata.GetCitationsReferredInFeature(cds);

                        // Validate citation referenced present in CDS features.
                        Assert.AreEqual(citationReferenceList.Count.ToString((IFormatProvider)null),
                            expectedCitationReferenced);

                        //Log VSTest GUI.
                        Console.WriteLine(string.Format((IFormatProvider)null,
                            "GenBank Features P1: Successfully validated citation referenced '{0}'",
                            citationReferenceList.Count.ToString((IFormatProvider)null)));
                        break;
                    case FeatureGroup.mRNA:
                        FeatureItem mRNA = metadata.Features.MessengerRNAs[0];
                        citationReferenceList = metadata.GetCitationsReferredInFeature(mRNA);

                        // Validate citation referenced present in mRNA features.
                        Assert.AreEqual(citationReferenceList.Count.ToString((IFormatProvider)null), expectedmRNACitationReferenced);

                        //Log VSTest GUI.
                        Console.WriteLine(string.Format((IFormatProvider)null,
                            "GenBank Features P1: Successfully validated citation referenced '{0}'",
                            citationReferenceList.Count.ToString((IFormatProvider)null)));
                        break;
                    case FeatureGroup.Exon:
                        FeatureItem exon = metadata.Features.Exons[0];
                        citationReferenceList =
                            metadata.GetCitationsReferredInFeature(exon);

                        // Validate citation referenced present in Exons features.
                        Assert.AreEqual(citationReferenceList.Count.ToString((IFormatProvider)null),
                            expectedExonACitationReferenced);

                        //Log VSTest GUI.
                        Console.WriteLine(string.Format((IFormatProvider)null,
                            "GenBank Features P1: Successfully validated citation referenced '{0}'",
                            citationReferenceList.Count.ToString((IFormatProvider)null)));
                        break;
                    case FeatureGroup.Intron:
                        FeatureItem introns = metadata.Features.Introns[0];
                        citationReferenceList =
                            metadata.GetCitationsReferredInFeature(introns);

                        // Validate citation referenced present in Introns features.
                        Assert.AreEqual(citationReferenceList.Count.ToString((IFormatProvider)null),
                            expectedIntronCitationReferenced);

                        //Log VSTest GUI.
                        Console.WriteLine(string.Format((IFormatProvider)null,
                            "GenBank Features P1: Successfully validated citation referenced '{0}'",
                            citationReferenceList.Count.ToString((IFormatProvider)null)));
                        break;
                    case FeatureGroup.Promoter:
                        FeatureItem promoter = metadata.Features.Promoters[0];
                        citationReferenceList =
                            metadata.GetCitationsReferredInFeature(promoter);

                        // Validate citation referenced present in Promoters features.
                        Assert.AreEqual(citationReferenceList.Count.ToString((IFormatProvider)null),
                            expectedpromoterCitationReferenced);

                        //Log VSTest GUI.
                        Console.WriteLine(string.Format((IFormatProvider)null,
                            "GenBank Features P1: Successfully validated citation referenced '{0}'",
                            citationReferenceList.Count.ToString((IFormatProvider)null)));
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Validate GenBank miscFeatures features.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        void ValidateGenBankMiscFeatureQualifiers(string nodeName)
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string miscFeatureCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.MiscFeatureCount);
            string location = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Location);

            // Parse a GenBank file.
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                IEnumerable<ISequence> seqList = parserObj.Parse();

                // Validate Misc feature all qualifiers.
                GenBankMetadata metadata =
                    (GenBankMetadata)seqList.ElementAt(0).Metadata[Constants.GenBank];
                List<MiscFeature> miscFeatureList = metadata.Features.MiscFeatures;
                LocationBuilder locBuilder = new LocationBuilder();

                // Create copy of misc feature and validate all qualifiers
                MiscFeature cloneMiscFeatureList =
                    miscFeatureList[0].Clone();
                Assert.AreEqual(miscFeatureList.Count.ToString((IFormatProvider)null),
                    miscFeatureCount);
                Assert.IsTrue(string.IsNullOrEmpty(cloneMiscFeatureList.Allele));
                Assert.IsFalse(string.IsNullOrEmpty(cloneMiscFeatureList.Citation.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(cloneMiscFeatureList.Experiment.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(cloneMiscFeatureList.Function.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(miscFeatureList[0].GeneSynonym.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(miscFeatureList[0].GenomicMapPosition.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(miscFeatureList[0].Inference.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(miscFeatureList[0].Label.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(miscFeatureList[0].OldLocusTag.ToString()));
                Assert.IsFalse(miscFeatureList[0].Pseudo);
                Assert.IsTrue(string.IsNullOrEmpty(miscFeatureList[0].StandardName.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(miscFeatureList[0].Product.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(miscFeatureList[0].Number.ToString()));
                Assert.AreEqual(locBuilder.GetLocationString(
                    metadata.Features.MiscFeatures[0].Location), location);

                // Log VSTest GUI.
                ApplicationLog.WriteLine(
                    "GenBank Features P1: Successfully validated the GenBank Features");
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the misc feature '{0}'",
                    miscFeatureList[0].GeneSymbol.ToString((IFormatProvider)null)));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the misc feature '{0}'",
                    miscFeatureList.Count.ToString((IFormatProvider)null)));
            }
        }

        /// <summary>
        /// Validate GenBank Exon feature qualifiers.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        void ValidateGenBankExonFeatureQualifiers(string nodeName)
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string expectedExonFeatureCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExonCount);
            string expectedExonGeneSymbol = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExonGeneSymbol);
            string expectedExonNumber = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExonNumber);

            // Parse a GenBank file.
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                IEnumerable<ISequence> seqList = parserObj.Parse();

                // Validate Misc feature all qualifiers.
                GenBankMetadata metadata =
                    (GenBankMetadata)seqList.ElementAt(0).Metadata[Constants.GenBank];
                List<Exon> exonFeatureList = metadata.Features.Exons;
                Assert.AreEqual(exonFeatureList.Count.ToString((IFormatProvider)null),
                    expectedExonFeatureCount);
                Assert.AreEqual(exonFeatureList[0].GeneSymbol,
                    expectedExonGeneSymbol);
                Assert.AreEqual(exonFeatureList[0].Number,
                    expectedExonNumber);
                Assert.IsTrue(string.IsNullOrEmpty(exonFeatureList[0].Allele));
                Assert.IsFalse(string.IsNullOrEmpty(exonFeatureList[0].Citation.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(exonFeatureList[0].Experiment.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(exonFeatureList[0].Function.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(exonFeatureList[0].GeneSynonym.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(exonFeatureList[0].GenomicMapPosition.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(exonFeatureList[0].Inference.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(exonFeatureList[0].Label.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(exonFeatureList[0].OldLocusTag.ToString()));
                Assert.IsFalse(exonFeatureList[0].Pseudo);
                Assert.IsTrue(string.IsNullOrEmpty(exonFeatureList[0].StandardName.ToString()));

                // Log VSTest GUI.
                ApplicationLog.WriteLine(
                    "GenBank Features P1: Successfully validated the GenBank Features");
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the Exon feature '{0}'",
                    exonFeatureList[0].GeneSymbol.ToString((IFormatProvider)null)));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the Exon feature '{0}'",
                    exonFeatureList.Count.ToString((IFormatProvider)null)));
            }
        }

        /// <summary>
        /// Validate GenBank Intron feature qualifiers.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        void ValidateGenBankIntronFeatureQualifiers(string nodeName)
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string expectedIntronGeneSymbol = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.IntronGeneSymbol);
            string expectedIntronComplement = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.IntronComplement);
            string expectedIntronNumber = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.IntronNumber);

            // Parse a GenBank file.
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                IEnumerable<ISequence> seqList = parserObj.Parse();

                // Validate Misc feature all qualifiers.
                GenBankMetadata metadata =
                    (GenBankMetadata)seqList.ElementAt(0).Metadata[Constants.GenBank];
                List<Intron> intronFeatureList =
                    metadata.Features.Introns;
                Assert.AreEqual(intronFeatureList[0].GeneSymbol,
                    expectedIntronGeneSymbol);
                Assert.AreEqual(intronFeatureList[0].Location.Operator.ToString(),
                    expectedIntronComplement);
                Assert.AreEqual(intronFeatureList[0].Number,
                    expectedIntronNumber);
                Assert.IsTrue(string.IsNullOrEmpty(intronFeatureList[0].Allele));
                Assert.IsFalse(string.IsNullOrEmpty(intronFeatureList[0].Citation.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(intronFeatureList[0].Experiment.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(intronFeatureList[0].Function.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(intronFeatureList[0].GeneSynonym.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(intronFeatureList[0].GenomicMapPosition.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(intronFeatureList[0].Inference.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(intronFeatureList[0].Label.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(intronFeatureList[0].OldLocusTag.ToString()));
                Assert.IsFalse(intronFeatureList[0].Pseudo);
                Assert.IsTrue(string.IsNullOrEmpty(intronFeatureList[0].StandardName.ToString()));

                // Log VSTest GUI.
                ApplicationLog.WriteLine(
                    "GenBank Features P1: Successfully validated the GenBank Features");
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the Intron feature '{0}'",
                    intronFeatureList[0].GeneSymbol.ToString((IFormatProvider)null)));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the Intron feature '{0}'",
                    intronFeatureList.Count.ToString((IFormatProvider)null)));
            }
        }

        /// <summary>
        /// Validate GenBank Promoter feature qualifiers.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        void ValidateGenBankPromoterFeatureQualifiers(string nodeName)
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string expectedPromoterComplement = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.PromoterComplement);
            string expectedPromoterCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.PromoterCount);

            // Parse a GenBank file.            
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                IEnumerable<ISequence> seqList = parserObj.Parse();
                LocationBuilder locBuilder = new LocationBuilder();

                // Validate Misc feature all qualifiers.
                GenBankMetadata metadata =
                    (GenBankMetadata)seqList.ElementAt(0).Metadata[Constants.GenBank];
                List<Promoter> promotersFeatureList =
                    metadata.Features.Promoters;
                Assert.AreEqual(locBuilder.GetLocationString(
                    metadata.Features.Promoters[0].Location),
                    expectedPromoterComplement);
                Assert.AreEqual(promotersFeatureList.Count.ToString((IFormatProvider)null),
                    expectedPromoterCount);
                Assert.IsTrue(string.IsNullOrEmpty(promotersFeatureList[0].GeneSymbol));
                Assert.IsTrue(string.IsNullOrEmpty(promotersFeatureList[0].Allele));
                Assert.IsFalse(string.IsNullOrEmpty(promotersFeatureList[0].Citation.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(promotersFeatureList[0].Experiment.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(promotersFeatureList[0].Function.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(promotersFeatureList[0].GeneSynonym.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(promotersFeatureList[0].GenomicMapPosition));
                Assert.IsFalse(string.IsNullOrEmpty(promotersFeatureList[0].Inference.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(promotersFeatureList[0].Label));
                Assert.IsFalse(string.IsNullOrEmpty(promotersFeatureList[0].OldLocusTag.ToString()));
                Assert.IsFalse(promotersFeatureList[0].Pseudo);
                Assert.IsTrue(string.IsNullOrEmpty(promotersFeatureList[0].StandardName));

                // Log VSTest GUI.
                ApplicationLog.WriteLine(
                    "GenBank Features P1: Successfully validated the GenBank Features");
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the Promoter feature '{0}'",
                    promotersFeatureList[0].GeneSymbol.ToString((IFormatProvider)null)));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the Promoter feature '{0}'",
                    promotersFeatureList.Count.ToString((IFormatProvider)null)));
            }
        }

        /// <summary>
        /// Validate GenBank Variation feature qualifiers.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        void ValidateGenBankVariationFeatureQualifiers(string nodeName)
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string expectedVariationCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.VarationCount);
            string expectedVariationReplace = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.VariationReplace);

            // Parse a GenBank file.
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                IEnumerable<ISequence> seqList = parserObj.Parse();

                // Validate Misc feature all qualifiers.
                GenBankMetadata metadata =
                    (GenBankMetadata)seqList.ElementAt(0).Metadata[Constants.GenBank];
                List<Variation> variationFeatureList =
                    metadata.Features.Variations;
                Assert.AreEqual(variationFeatureList.Count.ToString((IFormatProvider)null),
                    expectedVariationCount);
                Assert.AreEqual(variationFeatureList[0].Replace,
                    expectedVariationReplace);
                Assert.IsTrue(string.IsNullOrEmpty(variationFeatureList[0].GeneSymbol));
                Assert.IsTrue(string.IsNullOrEmpty(variationFeatureList[0].Allele));
                Assert.IsFalse(string.IsNullOrEmpty(variationFeatureList[0].Citation.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(variationFeatureList[0].Experiment.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(variationFeatureList[0].GeneSynonym.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(variationFeatureList[0].GenomicMapPosition));
                Assert.IsFalse(string.IsNullOrEmpty(variationFeatureList[0].Inference.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(variationFeatureList[0].Label));
                Assert.IsFalse(string.IsNullOrEmpty(variationFeatureList[0].OldLocusTag.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(variationFeatureList[0].StandardName));

                // Log VSTest GUI.
                ApplicationLog.WriteLine(
                    "GenBank Features P1: Successfully validated the GenBank Features");
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the Variation feature '{0}'",
                    variationFeatureList[0].Replace.ToString((IFormatProvider)null)));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the Variation feature '{0}'",
                    variationFeatureList.Count.ToString((IFormatProvider)null)));
            }
        }

        /// <summary>
        /// Validate GenBank Misc difference feature qualifiers.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        void ValidateGenBankMiscDiffFeatureQualifiers(string nodeName)
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string expectedMiscDiffCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.MiscDiffCount);
            string expectedGeneSymbol = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GeneSymbol);

            // Parse a GenBank file.
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                IEnumerable<ISequence> seqList = parserObj.Parse();

                // Validate Protein feature all qualifiers.
                GenBankMetadata metadata =
                    (GenBankMetadata)seqList.ElementAt(1).Metadata[Constants.GenBank];
                List<MiscDifference> miscDifferenceFeatureList =
                    metadata.Features.MiscDifferences;
                Assert.AreEqual(miscDifferenceFeatureList.Count.ToString((IFormatProvider)null),
                    expectedMiscDiffCount);
                Assert.AreEqual(miscDifferenceFeatureList[0].GeneSymbol,
                    expectedGeneSymbol);
                Assert.IsTrue(string.IsNullOrEmpty(miscDifferenceFeatureList[0].Allele));
                Assert.IsFalse(string.IsNullOrEmpty(miscDifferenceFeatureList[0].Citation.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(miscDifferenceFeatureList[0].Experiment.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(miscDifferenceFeatureList[0].GeneSynonym.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(miscDifferenceFeatureList[0].GenomicMapPosition));
                Assert.IsFalse(string.IsNullOrEmpty(miscDifferenceFeatureList[0].Inference.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(miscDifferenceFeatureList[0].Label));
                Assert.IsFalse(string.IsNullOrEmpty(miscDifferenceFeatureList[0].OldLocusTag.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(miscDifferenceFeatureList[0].StandardName));
                Assert.IsTrue(string.IsNullOrEmpty(miscDifferenceFeatureList[0].Replace));
                Assert.IsFalse(string.IsNullOrEmpty(miscDifferenceFeatureList[0].Phenotype.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(miscDifferenceFeatureList[0].OldLocusTag.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(miscDifferenceFeatureList[0].LocusTag.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(miscDifferenceFeatureList[0].Compare.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(miscDifferenceFeatureList[0].DatabaseCrossReference.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(miscDifferenceFeatureList[0].ClonedFrom));


                // Create a new MiscDiff feature using constructor.
                MiscDifference miscDiffWithLoc = new MiscDifference(
                    metadata.Features.MiscDifferences[0].Location);

                // Set and validate qualifiers.
                miscDiffWithLoc.GeneSymbol = expectedGeneSymbol;
                Assert.AreEqual(miscDiffWithLoc.GeneSymbol,
                    expectedGeneSymbol);

                // Log VSTest GUI.
                ApplicationLog.WriteLine(
                    "GenBank Features P1: Successfully validated the GenBank Features");
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the Misc Difference feature '{0}'",
                    miscDifferenceFeatureList[0].GeneSymbol.ToString((IFormatProvider)null)));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the Misc Difference feature '{0}'",
                    miscDifferenceFeatureList.Count.ToString((IFormatProvider)null)));
            }
        }

        /// <summary>
        /// Validate GenBank Protein binding feature qualifiers.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        void ValidateGenBankProteinBindingFeatureQualifiers(string nodeName)
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
            nodeName, Constants.FilePathNode);
            string expectedProteinBindingCount =
                utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ProteinBindingCount);

            // Parse a GenBank file.
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                IEnumerable<ISequence> seqList = parserObj.Parse();

                // Validate ProteinBinding feature all qualifiers.
                GenBankMetadata metadata =
                    (GenBankMetadata)seqList.ElementAt(1).Metadata[Constants.GenBank];
                List<ProteinBindingSite> proteinBindingFeatureList =
                    metadata.Features.ProteinBindingSites;
                Assert.AreEqual(proteinBindingFeatureList.Count.ToString((IFormatProvider)null),
                    expectedProteinBindingCount);
                Assert.IsTrue(string.IsNullOrEmpty(proteinBindingFeatureList[0].GeneSymbol));
                Assert.IsTrue(string.IsNullOrEmpty(proteinBindingFeatureList[0].Allele));
                Assert.IsFalse(string.IsNullOrEmpty(proteinBindingFeatureList[0].Citation.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(proteinBindingFeatureList[0].Experiment.ToString()));
                Assert.IsFalse(string.IsNullOrEmpty(proteinBindingFeatureList[0].GeneSynonym.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(proteinBindingFeatureList[0].GenomicMapPosition));
                Assert.IsFalse(string.IsNullOrEmpty(proteinBindingFeatureList[0].Inference.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(proteinBindingFeatureList[0].Label));
                Assert.IsFalse(string.IsNullOrEmpty(proteinBindingFeatureList[0].OldLocusTag.ToString()));
                Assert.IsTrue(string.IsNullOrEmpty(proteinBindingFeatureList[0].StandardName));

                // Log VSTest GUI.
                ApplicationLog.WriteLine(
                    "GenBank Features P1: Successfully validated the GenBank Features");
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the Protein Binding feature '{0}'",
                    proteinBindingFeatureList[0].BoundMoiety.ToString()));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1: Successfully validated the Protein Binding feature '{0}'",
                    proteinBindingFeatureList.Count.ToString((IFormatProvider)null)));
            }
        }

        /// <summary>
        /// Validate GenBank Features clonning.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        /// <param name="featureName">Name of the GenBank feature</param>
        void ValidateGenBankFeaturesClonning(string nodeName, FeatureGroup featureName)
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);
            string expectedExonFeatureCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExonCount);
            string expectedExonGeneSymbol = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExonGeneSymbol);
            string expectedExonNumber = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExonNumber);
            string expectedMiscDiffCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.MiscQualifiersCount);
            string expectedGeneSymbol = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GeneSymbol);
            string expectedIntronGeneSymbol = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.IntronGeneSymbol);
            string expectedIntronNumber = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.IntronNumber);
            string expectedVariationReplace = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.VariationReplace);

            // Parse a GenBank file.
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                ISequence seq = parserObj.Parse().FirstOrDefault();

                GenBankMetadata metadata =
                    seq.Metadata[Constants.GenBank] as GenBankMetadata;

                // Validate cloned GenBank feature.
                switch (featureName)
                {
                    case FeatureGroup.Exon:
                        List<Exon> exonFeatureList = metadata.Features.Exons;

                        // Validate Exon feature before clonning.
                        Assert.AreEqual(exonFeatureList.Count.ToString((IFormatProvider)null),
                            expectedExonFeatureCount);
                        Assert.AreEqual(exonFeatureList[0].GeneSymbol,
                            expectedExonGeneSymbol);
                        Assert.AreEqual(exonFeatureList[0].Number,
                            expectedExonNumber);

                        // Clone Exon feature.
                        Exon clonedExons = exonFeatureList[0].Clone();

                        // Validate Exon feature after clonning.
                        Assert.AreEqual(clonedExons.GeneSymbol,
                            expectedExonGeneSymbol);
                        Assert.AreEqual(clonedExons.Number,
                            expectedExonNumber);
                        Console.WriteLine(string.Format((IFormatProvider)null,
                            "GenBank Features P1: Successfully validated the Exons Qualifiers '{0}'",
                            clonedExons.Location.ToString()));
                        break;
                    case FeatureGroup.miscDifference:
                        // Validate Misc Difference feature before clonning.
                        List<MiscDifference> miscDifferenceFeatureList =
                            metadata.Features.MiscDifferences;
                        Assert.AreEqual(miscDifferenceFeatureList.Count.ToString((IFormatProvider)null),
                            expectedMiscDiffCount);
                        Assert.AreEqual(miscDifferenceFeatureList[0].GeneSymbol,
                            expectedGeneSymbol);

                        // Clone Misc Difference feature 
                        MiscDifference clonedMiscDifferences =
                            miscDifferenceFeatureList[0].Clone();

                        // Validate Misc Difference feature  after clonning.
                        Assert.AreEqual(clonedMiscDifferences.GeneSymbol,
                            expectedGeneSymbol);
                        Console.WriteLine(string.Format((IFormatProvider)null,
                            "GenBank Features P1: Successfully validated the Misc Difference Qualifiers '{0}'",
                            clonedMiscDifferences.GeneSymbol));
                        break;
                    case FeatureGroup.Intron:
                        // Validate Intron feature before clonning.
                        List<Intron> intronFeatureList = metadata.Features.Introns;
                        Assert.AreEqual(intronFeatureList[0].GeneSymbol,
                            expectedIntronGeneSymbol);
                        Assert.AreEqual(intronFeatureList[0].Number,
                            expectedIntronNumber);

                        // Clone Intron feature.
                        Intron clonedIntrons = intronFeatureList[0].Clone();

                        // Validate Intron feature after clonning.
                        Assert.AreEqual(clonedIntrons.GeneSymbol,
                            expectedIntronGeneSymbol);
                        Assert.AreEqual(clonedIntrons.Number,
                            expectedIntronNumber);
                        Console.WriteLine(string.Format((IFormatProvider)null,
                            "GenBank Features P1: Successfully validated the Introns '{0}'",
                            clonedIntrons.Location.ToString()));
                        break;
                    case FeatureGroup.variation:
                        // Validate Variation feature before clonning.
                        List<Variation> variationFeatureList =
                            metadata.Features.Variations;
                        Assert.AreEqual(variationFeatureList[0].Replace,
                            expectedVariationReplace);

                        // Clone Variation feature.
                        Variation clonedVariations =
                            variationFeatureList[0].Clone();

                        // Validate Intron feature after clonning.
                        Assert.AreEqual(clonedVariations.Replace,
                            expectedVariationReplace);
                        Console.WriteLine(string.Format((IFormatProvider)null,
                            "GenBank Features P1: Successfully validated the Variations '{0}'",
                            clonedVariations.Replace));
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Validate General GenBank Features 
        /// </summary>
        /// <param name="nodeName">xml node name for different feature.</param>
        /// <param name="featureName">Name of the GenBank feature</param>
        void ValidateGeneralGenBankFeatureQualifiers(string nodeName, FeatureGroup featureName)
        {
            // Get Values from XML node.
            string filePath = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FilePathNode);

            // Parse a GenBank file.
            using (ISequenceParser parserObj = new GenBankParser(filePath))
            {
                IEnumerable<ISequence> seqList = parserObj.Parse();

                // Validate ProteinBinding feature all qualifiers.
                GenBankMetadata metadata =
                    (GenBankMetadata)seqList.ElementAt(0).Metadata[Constants.GenBank];
                switch (featureName)
                {
                    case FeatureGroup.MiscStructure:
                        ValidateGenBankMiscStructureFeature(nodeName,
                            metadata);
                        break;
                    case FeatureGroup.TrnsitPeptide:
                        ValidateGenBankTrnsitPeptideFeature(nodeName,
                            metadata);
                        break;
                    case FeatureGroup.StemLoop:
                        ValidateGenBankStemLoopFeature(nodeName, metadata);
                        break;
                    case FeatureGroup.ModifiedBase:
                        ValidateGenBankModifiedBaseFeature(nodeName, metadata);
                        break;
                    case FeatureGroup.PrecursorRNA:
                        ValidateGenBankPrecursorRNAFeature(nodeName, metadata);
                        break;
                    case FeatureGroup.PolySite:
                        ValidateGenBankPolySiteFeature(nodeName, metadata);
                        break;
                    case FeatureGroup.MiscBinding:
                        ValidateGenBankMiscBindingFeature(nodeName, metadata);
                        break;
                    case FeatureGroup.Enhancer:
                        ValidateGenBankEnhancerFeature(nodeName, metadata);
                        break;
                    case FeatureGroup.GCSignal:
                        ValidateGenBankGCSignalFeature(nodeName, metadata);
                        break;
                    case FeatureGroup.LTR:
                        ValidateGenBankLTRFeature(nodeName, metadata);
                        break;
                    case FeatureGroup.Operon:
                        ValidateGenBankOperon(nodeName, metadata);
                        break;
                    case FeatureGroup.UnsureSequenceRegion:
                        ValidateGenBankUnsureSequenceRegion(nodeName,
                            metadata);
                        break;
                    case FeatureGroup.NonCodingRNA:
                        ValidateGenBankNonCodingRNA(nodeName, metadata);
                        break;
                    case FeatureGroup.CDS:
                        ValidateGenBankCDSFeatures(nodeName, metadata);
                        break;
                    case FeatureGroup.RibosomeBindingSite:
                        ValidateGenBankRibosomeBindingSite(nodeName, metadata);
                        break;
                    default:
                        break;
                }

                // Log VSTest GUI.
                ApplicationLog.WriteLine(
                    "GenBank Features P1: Successfully validated the GenBank Features");
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "GenBank Features P1'{0}'",
                    metadata.Features.GCSignals.Count));
            }
        }

        /// <summary>
        /// Validate MiscStructure features
        /// </summary>
        /// <param name="nodeName">XML node name</param>
        /// <param name="genMetadata">GenBank Metadata</param>
        void ValidateGenBankMiscStructureFeature(string nodeName,
            GenBankMetadata genMetadata)
        {
            // Get Values from XML node.            
            string expectedLocation = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Location);
            string expectedAllele = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.AlleleNode);
            string featureCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.QualifierCount);
            string expectedDbReference = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.DbReferenceNode);
            string geneSymbol = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GeneSymbol);
            string expectedCitation = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.CitationNode);
            string expectedExperiment = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExperimentNode);
            string expectedFunction = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FunctionNode);
            string expectedGeneSynonym = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GeneSynonymNode);
            string expectedInference = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.InferenceNode);
            string expectedLabel = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.LabelNode);
            string expectedLocusTag = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.LocusTagNode);
            string expectedNote = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Note);
            string expectedOldLocusTag = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.OldLocusTagNode);
            string expectedMap = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GenbankMapNode);

            List<MiscStructure> miscStrFeatureList =
                        genMetadata.Features.MiscStructures;
            LocationBuilder locBuilder = new LocationBuilder();

            // Create a copy of Misc structure.
            MiscStructure cloneMiscStr = miscStrFeatureList[0].Clone();

            // Validate MiscStructure qualifiers.
            Assert.AreEqual(miscStrFeatureList.Count.ToString((IFormatProvider)null), featureCount);
            Assert.IsFalse(string.IsNullOrEmpty(cloneMiscStr.GeneSymbol));
            Assert.AreEqual(cloneMiscStr.DatabaseCrossReference[0],
                expectedDbReference);
            Assert.AreEqual(miscStrFeatureList[0].Allele,
                expectedAllele);
            Assert.AreEqual(miscStrFeatureList[0].Citation[0],
                expectedCitation);
            Assert.AreEqual(miscStrFeatureList[0].Experiment[0],
                expectedExperiment);
            Assert.AreEqual(miscStrFeatureList[0].GenomicMapPosition,
                expectedMap);
            Assert.AreEqual(miscStrFeatureList[0].GeneSynonym[0],
                expectedGeneSynonym);
            Assert.AreEqual(miscStrFeatureList[0].Inference[0],
                expectedInference);
            Assert.AreEqual(miscStrFeatureList[0].Label,
                expectedLabel);
            Assert.AreEqual(locBuilder.GetLocationString(
                genMetadata.Features.MiscStructures[0].Location),
                expectedLocation);
            Assert.AreEqual(miscStrFeatureList[0].Note[0],
                expectedNote);
            Assert.AreEqual(miscStrFeatureList[0].OldLocusTag[0],
                expectedOldLocusTag);
            Assert.AreEqual(miscStrFeatureList[0].LocusTag[0],
                expectedLocusTag);
            Assert.AreEqual(miscStrFeatureList[0].Function[0],
                expectedFunction);
            Assert.IsTrue(string.IsNullOrEmpty(miscStrFeatureList[0].StandardName));

            // Create a new MiscStructure and validate the same.
            MiscStructure miscStructure = new MiscStructure(expectedLocation);
            MiscStructure miscStructureWithILoc = new MiscStructure(
                genMetadata.Features.TransitPeptides[0].Location);

            // Set qualifiers and validate them.
            miscStructure.Allele = expectedAllele;
            miscStructure.GeneSymbol = geneSymbol;
            miscStructureWithILoc.GenomicMapPosition = expectedMap;
            Assert.AreEqual(miscStructure.GeneSymbol, geneSymbol);
            Assert.AreEqual(miscStructure.Allele, expectedAllele);
            Assert.AreEqual(miscStructureWithILoc.GenomicMapPosition,
                expectedMap);
        }

        /// <summary>
        /// Validate TrnsitPeptide features
        /// </summary>
        /// <param name="nodeName">XML node name</param>
        /// <param name="genMetadata">GenBank Metadata</param>
        void ValidateGenBankTrnsitPeptideFeature(string nodeName,
            GenBankMetadata genMetadata)
        {
            // Get Values from XML node.           
            string expectedLocation = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Location);
            string expectedAllele = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.AlleleNode);
            string featureCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.QualifierCount);
            string expectedDbReference = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.DbReferenceNode);
            string geneSymbol = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GeneSymbol);
            string expectedCitation = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.CitationNode);
            string expectedExperiment = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExperimentNode);
            string expectedFunction = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FunctionNode);
            string expectedGeneSynonym = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GeneSynonymNode);
            string expectedInference = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.InferenceNode);
            string expectedLabel = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.LabelNode);
            string expectedLocusTag = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.LocusTagNode);
            string expectedNote = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Note);
            string expectedOldLocusTag = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.OldLocusTagNode);
            string expectedMap = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GenbankMapNode);

            List<TransitPeptide> tansitPeptideFeatureList =
                        genMetadata.Features.TransitPeptides;
            LocationBuilder locBuilder = new LocationBuilder();

            // Create a copy of transit peptide features.
            TransitPeptide cloneTransit = tansitPeptideFeatureList[0].Clone();

            // Validate transit peptide qualifiers.
            Assert.AreEqual(tansitPeptideFeatureList.Count.ToString((IFormatProvider)null), featureCount);
            Assert.AreEqual(cloneTransit.GeneSymbol, geneSymbol);
            Assert.AreEqual(cloneTransit.DatabaseCrossReference[0],
                expectedDbReference);
            Assert.AreEqual(tansitPeptideFeatureList[0].Allele,
                expectedAllele);
            Assert.AreEqual(tansitPeptideFeatureList[0].Citation[0],
                expectedCitation);
            Assert.AreEqual(tansitPeptideFeatureList[0].Experiment[0],
                expectedExperiment);
            Assert.AreEqual(tansitPeptideFeatureList[0].GenomicMapPosition,
                expectedMap);
            Assert.AreEqual(tansitPeptideFeatureList[0].GeneSynonym[0],
                expectedGeneSynonym);
            Assert.AreEqual(tansitPeptideFeatureList[0].Inference[0],
                expectedInference);
            Assert.AreEqual(tansitPeptideFeatureList[0].Label,
                expectedLabel);
            Assert.AreEqual(locBuilder.GetLocationString(
                genMetadata.Features.TransitPeptides[0].Location),
                expectedLocation);
            Assert.AreEqual(tansitPeptideFeatureList[0].Note[0],
                expectedNote);
            Assert.AreEqual(tansitPeptideFeatureList[0].OldLocusTag[0],
                expectedOldLocusTag);
            Assert.AreEqual(tansitPeptideFeatureList[0].LocusTag[0],
                expectedLocusTag);
            Assert.AreEqual(tansitPeptideFeatureList[0].Function[0],
                expectedFunction);

            // Create a new TransitPeptide and validate the same.
            TransitPeptide tPeptide = new TransitPeptide(expectedLocation);
            TransitPeptide tPeptideWithILoc = new TransitPeptide(
                genMetadata.Features.TransitPeptides[0].Location);

            // Set qualifiers and validate them.
            tPeptide.Allele = expectedAllele;
            tPeptide.GeneSymbol = geneSymbol;
            tPeptideWithILoc.GenomicMapPosition = expectedMap;
            Assert.AreEqual(tPeptide.GeneSymbol, geneSymbol);
            Assert.AreEqual(tPeptide.Allele, expectedAllele);
            Assert.AreEqual(tPeptideWithILoc.GenomicMapPosition,
                expectedMap);
        }

        /// <summary>
        /// Validate StemLoop features
        /// </summary>
        /// <param name="nodeName">XML node name</param>
        /// <param name="genMetadata">GenBank Metadata</param>
        void ValidateGenBankStemLoopFeature(string nodeName,
            GenBankMetadata genMetadata)
        {
            // Get Values from XML node.
            string expectedLocation = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Location);
            string expectedAllele = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.AlleleNode);
            string featureCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.QualifierCount);
            string expectedDbReference = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.DbReferenceNode);
            string geneSymbol = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GeneSymbol);
            string expectedCitation = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.CitationNode);
            string expectedExperiment = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExperimentNode);
            string expectedFunction = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FunctionNode);
            string expectedGeneSynonym = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GeneSynonymNode);
            string expectedInference = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.InferenceNode);
            string expectedLabel = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.LabelNode);
            string expectedLocusTag = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.LocusTagNode);
            string expectedNote = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Note);
            string expectedOldLocusTag = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.OldLocusTagNode);
            string expectedMap = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GenbankMapNode);

            List<StemLoop> sLoopFeatureList = genMetadata.Features.StemLoops;
            LocationBuilder locBuilder = new LocationBuilder();

            // Create a copy of StemLoop feature.
            StemLoop cloneSLoop = sLoopFeatureList[0].Clone();

            // Validate transit peptide qualifiers.
            Assert.AreEqual(sLoopFeatureList.Count.ToString((IFormatProvider)null),
                featureCount);
            Assert.AreEqual(cloneSLoop.GeneSymbol, geneSymbol);
            Assert.AreEqual(cloneSLoop.DatabaseCrossReference[0],
                expectedDbReference);
            Assert.AreEqual(sLoopFeatureList[0].Allele,
                expectedAllele);
            Assert.AreEqual(sLoopFeatureList[0].Citation[0],
                expectedCitation);
            Assert.AreEqual(sLoopFeatureList[0].Experiment[0],
                expectedExperiment);
            Assert.AreEqual(sLoopFeatureList[0].GenomicMapPosition,
                expectedMap);
            Assert.AreEqual(sLoopFeatureList[0].GeneSynonym[0],
                expectedGeneSynonym);
            Assert.AreEqual(sLoopFeatureList[0].Inference[0],
                expectedInference);
            Assert.AreEqual(sLoopFeatureList[0].Label,
                expectedLabel);
            Assert.AreEqual(locBuilder.GetLocationString(
                genMetadata.Features.StemLoops[0].Location),
                expectedLocation);
            Assert.AreEqual(sLoopFeatureList[0].Note[0],
                expectedNote);
            Assert.AreEqual(sLoopFeatureList[0].OldLocusTag[0],
                expectedOldLocusTag);
            Assert.AreEqual(sLoopFeatureList[0].LocusTag[0],
                expectedLocusTag);
            Assert.AreEqual(sLoopFeatureList[0].Function[0],
                expectedFunction);
            Assert.IsTrue(string.IsNullOrEmpty(sLoopFeatureList[0].Operon));
            Assert.IsTrue(string.IsNullOrEmpty(sLoopFeatureList[0].StandardName));

            // Create a new StemLoop and validate the same.
            StemLoop stemLoop = new StemLoop(expectedLocation);
            StemLoop stemLoopWithILoc = new StemLoop(
                genMetadata.Features.StemLoops[0].Location);

            // Set qualifiers and validate them.
            stemLoop.Allele = expectedAllele;
            stemLoop.GeneSymbol = geneSymbol;
            stemLoopWithILoc.GenomicMapPosition = expectedMap;
            Assert.AreEqual(stemLoop.GeneSymbol, geneSymbol);
            Assert.AreEqual(stemLoop.Allele, expectedAllele);
            Assert.AreEqual(stemLoopWithILoc.GenomicMapPosition,
                expectedMap);
        }

        /// <summary>
        /// Validate ModifiedBase features
        /// </summary>
        /// <param name="nodeName">XML node name</param>
        /// <param name="genMetadata">GenBank Metadata</param>
        void ValidateGenBankModifiedBaseFeature(string nodeName,
            GenBankMetadata genMetadata)
        {
            // Get Values from XML node.           
            string expectedLocation = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Location);
            string expectedAllele = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.AlleleNode);
            string featureCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.QualifierCount);
            string expectedDbReference = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.DbReferenceNode);
            string geneSymbol = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GeneSymbol);
            string expectedCitation = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.CitationNode);
            string expectedExperiment = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExperimentNode);
            string expectedGeneSynonym = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GeneSynonymNode);
            string expectedInference = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.InferenceNode);
            string expectedLabel = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.LabelNode);
            string expectedLocusTag = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.LocusTagNode);
            string expectedNote = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Note);
            string expectedOldLocusTag = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.OldLocusTagNode);
            string expectedMap = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GenbankMapNode);

            List<ModifiedBase> modifiedBaseFeatureList =
                       genMetadata.Features.ModifiedBases;
            LocationBuilder locBuilder = new LocationBuilder();

            // Create a copy of Modified base feature.
            ModifiedBase cloneModifiedBase = modifiedBaseFeatureList[0].Clone();

            // Validate Modified Base qualifiers.
            Assert.AreEqual(modifiedBaseFeatureList.Count.ToString((IFormatProvider)null),
                featureCount);
            Assert.AreEqual(cloneModifiedBase.GeneSymbol,
                geneSymbol);
            Assert.AreEqual(cloneModifiedBase.DatabaseCrossReference[0],
                expectedDbReference);
            Assert.AreEqual(modifiedBaseFeatureList[0].Allele,
                expectedAllele);
            Assert.AreEqual(modifiedBaseFeatureList[0].Citation[0],
                expectedCitation);
            Assert.AreEqual(modifiedBaseFeatureList[0].Experiment[0],
                expectedExperiment);
            Assert.AreEqual(modifiedBaseFeatureList[0].GenomicMapPosition,
                expectedMap);
            Assert.AreEqual(modifiedBaseFeatureList[0].GeneSynonym[0],
                expectedGeneSynonym);
            Assert.AreEqual(modifiedBaseFeatureList[0].Inference[0],
                expectedInference);
            Assert.AreEqual(modifiedBaseFeatureList[0].Label,
                expectedLabel);
            Assert.AreEqual(locBuilder.GetLocationString(
                genMetadata.Features.ModifiedBases[0].Location),
                expectedLocation);
            Assert.AreEqual(modifiedBaseFeatureList[0].Note[0],
                expectedNote);
            Assert.AreEqual(modifiedBaseFeatureList[0].OldLocusTag[0],
                expectedOldLocusTag);
            Assert.AreEqual(modifiedBaseFeatureList[0].LocusTag[0],
                expectedLocusTag);
            Assert.IsFalse(string.IsNullOrEmpty(modifiedBaseFeatureList[0].ModifiedNucleotideBase.ToString()));

            // Create a new ModifiedBase and validate the same.
            ModifiedBase modifiedBase = new ModifiedBase(expectedLocation);
            ModifiedBase modifiedBaseWithILoc = new ModifiedBase(
                genMetadata.Features.ModifiedBases[0].Location);

            // Set qualifiers and validate them.
            modifiedBase.Allele = expectedAllele;
            modifiedBase.GeneSymbol = geneSymbol;
            modifiedBaseWithILoc.GenomicMapPosition = expectedMap;
            Assert.AreEqual(modifiedBase.GeneSymbol, geneSymbol);
            Assert.AreEqual(modifiedBase.Allele, expectedAllele);
            Assert.AreEqual(modifiedBaseWithILoc.GenomicMapPosition,
                expectedMap);
        }

        /// <summary>
        /// Validate PrecursorRNA features
        /// </summary>
        /// <param name="nodeName">XML node name</param>
        /// <param name="genMetadata">GenBank Metadata</param>
        void ValidateGenBankPrecursorRNAFeature(string nodeName,
            GenBankMetadata genMetadata)
        {
            // Get Values from XML node.           
            string expectedLocation = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Location);
            string expectedAllele = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.AlleleNode);
            string featureCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.QualifierCount);
            string expectedDbReference = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.DbReferenceNode);
            string geneSymbol = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GeneSymbol);
            string expectedCitation = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.CitationNode);
            string expectedExperiment = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExperimentNode);
            string expectedFunction = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FunctionNode);
            string expectedGeneSynonym = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GeneSynonymNode);
            string expectedInference = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.InferenceNode);
            string expectedLabel = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.LabelNode);
            string expectedLocusTag = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.LocusTagNode);
            string expectedNote = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Note);
            string expectedOldLocusTag = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.OldLocusTagNode);
            string expectedMap = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GenbankMapNode);

            List<PrecursorRna> precursorRNAFeatureList =
                        genMetadata.Features.PrecursorRNAs;
            LocationBuilder locBuilder = new LocationBuilder();

            // Create a copy of Precursor RNA feature.
            PrecursorRna clonePrecursorRNA =
                precursorRNAFeatureList[0].Clone();

            // Validate Precursor RNA qualifiers.
            Assert.AreEqual(precursorRNAFeatureList.Count.ToString((IFormatProvider)null),
                featureCount);
            Assert.AreEqual(clonePrecursorRNA.GeneSymbol,
                geneSymbol);
            Assert.AreEqual(clonePrecursorRNA.DatabaseCrossReference[0],
                expectedDbReference);
            Assert.AreEqual(precursorRNAFeatureList[0].Allele,
                expectedAllele);
            Assert.AreEqual(precursorRNAFeatureList[0].Citation[0],
                expectedCitation);
            Assert.AreEqual(precursorRNAFeatureList[0].Experiment[0],
                expectedExperiment);
            Assert.AreEqual(precursorRNAFeatureList[0].GenomicMapPosition,
                expectedMap);
            Assert.AreEqual(precursorRNAFeatureList[0].GeneSynonym[0],
                expectedGeneSynonym);
            Assert.AreEqual(precursorRNAFeatureList[0].Inference[0],
                expectedInference);
            Assert.AreEqual(precursorRNAFeatureList[0].Label, expectedLabel);
            Assert.AreEqual(locBuilder.GetLocationString(
                genMetadata.Features.PrecursorRNAs[0].Location),
                expectedLocation);
            Assert.AreEqual(precursorRNAFeatureList[0].Note[0],
                expectedNote);
            Assert.AreEqual(precursorRNAFeatureList[0].OldLocusTag[0],
                expectedOldLocusTag);
            Assert.AreEqual(precursorRNAFeatureList[0].LocusTag[0],
                expectedLocusTag);
            Assert.AreEqual(precursorRNAFeatureList[0].Function[0],
                expectedFunction);
            Assert.IsTrue(string.IsNullOrEmpty(precursorRNAFeatureList[0].StandardName));
            Assert.IsFalse(string.IsNullOrEmpty(precursorRNAFeatureList[0].Product.ToString()));
            Assert.IsTrue(string.IsNullOrEmpty(precursorRNAFeatureList[0].Operon));
            Assert.IsFalse(precursorRNAFeatureList[0].TransSplicing);

            // Create a new Precursor RNA and validate the same.
            PrecursorRna precursorRNA = new PrecursorRna(expectedLocation);
            PrecursorRna precursorRNAWithILoc = new PrecursorRna(
                genMetadata.Features.PrecursorRNAs[0].Location);

            // Set qualifiers and validate them.
            precursorRNA.Allele = expectedAllele;
            precursorRNA.GeneSymbol = geneSymbol;
            precursorRNAWithILoc.GenomicMapPosition = expectedMap;
            Assert.AreEqual(precursorRNA.GeneSymbol, geneSymbol);
            Assert.AreEqual(precursorRNA.Allele, expectedAllele);
            Assert.AreEqual(precursorRNAWithILoc.GenomicMapPosition, expectedMap);
        }

        /// <summary>
        /// Validate PolySite features
        /// </summary>
        /// <param name="nodeName">XML node name</param>
        /// <param name="genMetadata">GenBank Metadata</param>
        void ValidateGenBankPolySiteFeature(string nodeName,
            GenBankMetadata genMetadata)
        {
            // Get Values from XML node.            
            string expectedLocation = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Location);
            string expectedAllele = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.AlleleNode);
            string featureCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.QualifierCount);
            string expectedDbReference = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.DbReferenceNode);
            string geneSymbol = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GeneSymbol);
            string expectedCitation = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.CitationNode);
            string expectedExperiment = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExperimentNode);
            string expectedGeneSynonym = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GeneSynonymNode);
            string expectedInference = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.InferenceNode);
            string expectedLabel = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.LabelNode);
            string expectedLocusTag = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.LocusTagNode);
            string expectedNote = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Note);
            string expectedOldLocusTag = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.OldLocusTagNode);
            string expectedMap = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GenbankMapNode);

            List<PolyASite> polySiteFeatureList = genMetadata.Features.PolyASites;
            LocationBuilder locBuilder = new LocationBuilder();

            // Create a copy of Poly site feature.
            PolyASite clonePolySite = polySiteFeatureList[0].Clone();

            // Validate Poly site qualifiers.
            Assert.AreEqual(polySiteFeatureList.Count.ToString((IFormatProvider)null),
                featureCount);
            Assert.AreEqual(clonePolySite.GeneSymbol, geneSymbol);
            Assert.AreEqual(clonePolySite.DatabaseCrossReference[0],
                expectedDbReference);
            Assert.AreEqual(polySiteFeatureList[0].Allele,
                expectedAllele);
            Assert.AreEqual(polySiteFeatureList[0].Citation[0],
                expectedCitation);
            Assert.AreEqual(polySiteFeatureList[0].Experiment[0],
                expectedExperiment);
            Assert.AreEqual(polySiteFeatureList[0].GenomicMapPosition,
                expectedMap);
            Assert.AreEqual(polySiteFeatureList[0].GeneSynonym[0],
                expectedGeneSynonym);
            Assert.AreEqual(polySiteFeatureList[0].Inference[0],
                expectedInference);
            Assert.AreEqual(polySiteFeatureList[0].Label,
                expectedLabel);
            Assert.AreEqual(locBuilder.GetLocationString(
                genMetadata.Features.PolyASites[0].Location),
                expectedLocation);
            Assert.AreEqual(polySiteFeatureList[0].Note[0],
                expectedNote);
            Assert.AreEqual(polySiteFeatureList[0].OldLocusTag[0],
                expectedOldLocusTag);
            Assert.AreEqual(polySiteFeatureList[0].LocusTag[0],
                expectedLocusTag);

            // Create a new PolySite and validate the same.
            PolyASite polySite = new PolyASite(expectedLocation);
            PolyASite polySiteWithILoc = new PolyASite(
                genMetadata.Features.PolyASites[0].Location);

            // Set qualifiers and validate them.
            polySite.Allele = expectedAllele;
            polySite.GeneSymbol = geneSymbol;
            polySiteWithILoc.GenomicMapPosition = expectedMap;
            Assert.AreEqual(polySite.GeneSymbol, geneSymbol);
            Assert.AreEqual(polySite.Allele, expectedAllele);
            Assert.AreEqual(polySiteWithILoc.GenomicMapPosition, expectedMap);
        }

        /// <summary>
        /// Validate MiscBinding features
        /// </summary>
        /// <param name="nodeName">XML node name</param>
        /// <param name="genMetadata">GenBank Metadata</param>
        void ValidateGenBankMiscBindingFeature(string nodeName,
            GenBankMetadata genMetadata)
        {
            // Get Values from XML node.           
            string expectedLocation = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Location);
            string expectedAllele = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.AlleleNode);
            string featureCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.QualifierCount);
            string expectedDbReference = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.DbReferenceNode);
            string geneSymbol = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GeneSymbol);
            string expectedCitation = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.CitationNode);
            string expectedExperiment = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExperimentNode);
            string expectedGeneSynonym = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GeneSynonymNode);
            string expectedInference = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.InferenceNode);
            string expectedLabel = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.LabelNode);
            string expectedLocusTag = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.LocusTagNode);
            string expectedNote = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Note);
            string expectedOldLocusTag = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.OldLocusTagNode);
            string expectedMap = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GenbankMapNode);

            List<MiscBinding> miscBindingFeatureList = genMetadata.Features.MiscBindings;
            LocationBuilder locBuilder = new LocationBuilder();

            // Create a copy of Misc Binding feature.
            MiscBinding cloneMiscBinding = miscBindingFeatureList[0].Clone();

            // Validate Misc Binding qualifiers.
            Assert.AreEqual(miscBindingFeatureList.Count.ToString((IFormatProvider)null),
                featureCount);
            Assert.AreEqual(cloneMiscBinding.GeneSymbol,
                geneSymbol);
            Assert.AreEqual(cloneMiscBinding.DatabaseCrossReference[0],
                expectedDbReference);
            Assert.AreEqual(miscBindingFeatureList[0].Allele,
                expectedAllele);
            Assert.AreEqual(miscBindingFeatureList[0].Citation[0],
                expectedCitation);
            Assert.AreEqual(miscBindingFeatureList[0].Experiment[0],
                expectedExperiment);
            Assert.AreEqual(miscBindingFeatureList[0].GenomicMapPosition,
                expectedMap);
            Assert.AreEqual(miscBindingFeatureList[0].GeneSynonym[0],
                expectedGeneSynonym);
            Assert.AreEqual(miscBindingFeatureList[0].Inference[0],
                expectedInference);
            Assert.AreEqual(miscBindingFeatureList[0].Label,
                expectedLabel);
            Assert.AreEqual(locBuilder.GetLocationString(
                genMetadata.Features.MiscBindings[0].Location),
                expectedLocation);
            Assert.AreEqual(miscBindingFeatureList[0].Note[0],
                expectedNote);
            Assert.AreEqual(miscBindingFeatureList[0].OldLocusTag[0],
                expectedOldLocusTag);
            Assert.AreEqual(miscBindingFeatureList[0].LocusTag[0],
                expectedLocusTag);

            // Create a new MiscBinding and validate the same.
            MiscBinding miscBinding = new MiscBinding(expectedLocation);
            MiscBinding miscBindingWithILoc = new MiscBinding(
                genMetadata.Features.MiscBindings[0].Location);

            // Set qualifiers and validate them.
            miscBinding.Allele = expectedAllele;
            miscBinding.GeneSymbol = geneSymbol;
            miscBindingWithILoc.GenomicMapPosition = expectedMap;
            Assert.AreEqual(miscBinding.GeneSymbol, geneSymbol);
            Assert.AreEqual(miscBinding.Allele, expectedAllele);
            Assert.AreEqual(miscBindingWithILoc.GenomicMapPosition,
                expectedMap);
        }

        /// <summary>
        /// Validate GenBank Enhancer features
        /// </summary>
        /// <param name="nodeName">XML node name</param>
        /// <param name="genMetadata">GenBank Metadata</param>
        void ValidateGenBankEnhancerFeature(string nodeName,
            GenBankMetadata genMetadata)
        {
            // Get Values from XML node.            
            string expectedLocation = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Location);
            string expectedAllele = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.AlleleNode);
            string featureCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.QualifierCount);
            string expectedDbReference = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.DbReferenceNode);
            string geneSymbol = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GeneSymbol);
            string expectedCitation = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.CitationNode);
            string expectedExperiment = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExperimentNode);
            string expectedGeneSynonym = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GeneSynonymNode);
            string expectedInference = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.InferenceNode);
            string expectedLabel = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.LabelNode);
            string expectedLocusTag = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.LocusTagNode);
            string expectedNote = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Note);
            string expectedOldLocusTag = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.OldLocusTagNode);
            string expectedMap = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GenbankMapNode);

            List<Enhancer> enhancerFeatureList = genMetadata.Features.Enhancers;

            // Create a copy of Enhancer feature.
            Enhancer cloneEnhancer = enhancerFeatureList[0].Clone();
            LocationBuilder locBuilder = new LocationBuilder();

            // Validate Enhancer qualifiers.
            Assert.AreEqual(enhancerFeatureList.Count.ToString((IFormatProvider)null),
                featureCount);
            Assert.AreEqual(cloneEnhancer.GeneSymbol,
                geneSymbol);
            Assert.AreEqual(cloneEnhancer.DatabaseCrossReference[0],
                expectedDbReference);
            Assert.AreEqual(enhancerFeatureList[0].Allele,
                expectedAllele);
            Assert.AreEqual(enhancerFeatureList[0].Citation[0],
                expectedCitation);
            Assert.AreEqual(enhancerFeatureList[0].Experiment[0],
                expectedExperiment);
            Assert.AreEqual(enhancerFeatureList[0].GenomicMapPosition,
                expectedMap);
            Assert.AreEqual(enhancerFeatureList[0].GeneSynonym[0],
                expectedGeneSynonym);
            Assert.AreEqual(enhancerFeatureList[0].Inference[0],
                expectedInference);
            Assert.AreEqual(enhancerFeatureList[0].Label,
                expectedLabel);
            Assert.AreEqual(locBuilder.GetLocationString(
                genMetadata.Features.Enhancers[0].Location),
                expectedLocation);
            Assert.AreEqual(enhancerFeatureList[0].Note[0],
                expectedNote);
            Assert.AreEqual(enhancerFeatureList[0].OldLocusTag[0],
                expectedOldLocusTag);
            Assert.AreEqual(enhancerFeatureList[0].LocusTag[0],
                expectedLocusTag);
            Assert.IsTrue(string.IsNullOrEmpty(enhancerFeatureList[0].StandardName));

            // Create a new Enhancer and validate the same.
            Enhancer enhancer = new Enhancer(expectedLocation);
            GcSingal enhancerWithILoc = new GcSingal(
                genMetadata.Features.Enhancers[0].Location);

            // Set qualifiers and validate them.
            enhancer.Allele = expectedAllele;
            enhancer.GeneSymbol = geneSymbol;
            enhancerWithILoc.GenomicMapPosition = expectedMap;
            Assert.AreEqual(enhancer.GeneSymbol, geneSymbol);
            Assert.AreEqual(enhancer.Allele, expectedAllele);
            Assert.AreEqual(enhancerWithILoc.GenomicMapPosition, expectedMap);

        }

        /// <summary>
        /// Validate GenBank GCSignal features
        /// </summary>
        /// <param name="nodeName">XML node name</param>
        /// <param name="genMetadata">GenBank Metadata</param>
        void ValidateGenBankGCSignalFeature(string nodeName,
            GenBankMetadata genMetadata)
        {
            // Get Values from XML node.            
            string expectedLocation = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Location);
            string expectedAllele = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.AlleleNode);
            string featureCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.QualifierCount);
            string expectedDbReference = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.DbReferenceNode);
            string geneSymbol = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GeneSymbol);
            string expectedCitation = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.CitationNode);
            string expectedExperiment = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExperimentNode);
            string expectedGeneSynonym = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GeneSynonymNode);
            string expectedInference = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.InferenceNode);
            string expectedLabel = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.LabelNode);
            string expectedLocusTag = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.LocusTagNode);
            string expectedNote = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Note);
            string expectedOldLocusTag = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.OldLocusTagNode);
            string expectedMap = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GenbankMapNode);

            List<GcSingal> gcSignalFeatureList = genMetadata.Features.GCSignals;
            LocationBuilder locBuilder = new LocationBuilder();

            // Create a copy of GC_Signal feature.
            GcSingal cloneGCSignal = gcSignalFeatureList[0].Clone();

            // Validate GC_Signal qualifiers.
            Assert.AreEqual(gcSignalFeatureList.Count.ToString((IFormatProvider)null),
                featureCount);
            Assert.AreEqual(cloneGCSignal.GeneSymbol,
                geneSymbol);
            Assert.AreEqual(cloneGCSignal.DatabaseCrossReference[0],
                expectedDbReference);
            Assert.AreEqual(gcSignalFeatureList[0].Allele,
                expectedAllele);
            Assert.AreEqual(gcSignalFeatureList[0].Citation[0],
                expectedCitation);
            Assert.AreEqual(gcSignalFeatureList[0].Experiment[0],
                expectedExperiment);
            Assert.AreEqual(gcSignalFeatureList[0].GenomicMapPosition,
                expectedMap);
            Assert.AreEqual(gcSignalFeatureList[0].GeneSynonym[0],
                expectedGeneSynonym);
            Assert.AreEqual(gcSignalFeatureList[0].Inference[0],
                expectedInference);
            Assert.AreEqual(gcSignalFeatureList[0].Label,
                expectedLabel);
            Assert.AreEqual(locBuilder.GetLocationString(
                genMetadata.Features.GCSignals[0].Location),
                expectedLocation);
            Assert.AreEqual(gcSignalFeatureList[0].Note[0],
                expectedNote);
            Assert.AreEqual(gcSignalFeatureList[0].OldLocusTag[0],
                expectedOldLocusTag);
            Assert.AreEqual(gcSignalFeatureList[0].LocusTag[0],
                expectedLocusTag);

            // Create a new GCSignal and validate the same.
            GcSingal gcSignal = new GcSingal(expectedLocation);
            GcSingal gcSignalWithILoc = new GcSingal(
                genMetadata.Features.GCSignals[0].Location);

            // Set qualifiers and validate them.
            gcSignal.Allele = expectedAllele;
            gcSignal.GeneSymbol = geneSymbol;
            gcSignalWithILoc.GenomicMapPosition = expectedMap;
            Assert.AreEqual(gcSignal.GeneSymbol, geneSymbol);
            Assert.AreEqual(gcSignal.Allele, expectedAllele);
            Assert.AreEqual(gcSignalWithILoc.GenomicMapPosition,
                expectedMap);
        }

        /// <summary>
        /// Validate GenBank LTR features
        /// </summary>
        /// <param name="nodeName">XML node name</param>
        /// <param name="genMetadata">GenBank Metadata</param>
        void ValidateGenBankLTRFeature(string nodeName,
            GenBankMetadata genMetadata)
        {
            // Get Values from XML node.           
            string expectedLocation = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Location);
            string expectedAllele = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.AlleleNode);
            string featureCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.QualifierCount);
            string expectedDbReference = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.DbReferenceNode);
            string geneSymbol = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GeneSymbol);
            string expectedCitation = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.CitationNode);
            string expectedExperiment = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExperimentNode);
            string expectedFunction = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FunctionNode);
            string expectedGeneSynonym = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GeneSynonymNode);
            string expectedInference = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.InferenceNode);
            string expectedLabel = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.LabelNode);
            string expectedLocusTag = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.LocusTagNode);
            string expectedNote = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Note);
            string expectedOldLocusTag = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.OldLocusTagNode);
            string expectedMap = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GenbankMapNode);

            List<LongTerminalRepeat> LTRFeatureList =
                        genMetadata.Features.LongTerminalRepeats;
            LocationBuilder locBuilder = new LocationBuilder();

            // Create a copy of Long Terminal Repeat feature.
            LongTerminalRepeat cloneLTR = LTRFeatureList[0].Clone();

            // Validate Long Terminal Repeat qualifiers.
            Assert.AreEqual(LTRFeatureList.Count.ToString((IFormatProvider)null),
                featureCount);
            Assert.AreEqual(cloneLTR.GeneSymbol, geneSymbol);
            Assert.AreEqual(cloneLTR.DatabaseCrossReference[0],
                expectedDbReference);
            Assert.AreEqual(LTRFeatureList[0].Allele,
                expectedAllele);
            Assert.AreEqual(LTRFeatureList[0].Citation[0],
                expectedCitation);
            Assert.AreEqual(LTRFeatureList[0].Experiment[0],
                expectedExperiment);
            Assert.AreEqual(LTRFeatureList[0].GenomicMapPosition,
                expectedMap);
            Assert.AreEqual(LTRFeatureList[0].GeneSynonym[0],
                expectedGeneSynonym);
            Assert.AreEqual(LTRFeatureList[0].Inference[0],
                expectedInference);
            Assert.AreEqual(LTRFeatureList[0].Label,
                expectedLabel);
            Assert.AreEqual(locBuilder.GetLocationString(
                genMetadata.Features.LongTerminalRepeats[0].Location),
                expectedLocation);
            Assert.AreEqual(LTRFeatureList[0].Note[0],
                expectedNote);
            Assert.AreEqual(LTRFeatureList[0].OldLocusTag[0],
                expectedOldLocusTag);
            Assert.AreEqual(LTRFeatureList[0].LocusTag[0],
                expectedLocusTag);
            Assert.AreEqual(LTRFeatureList[0].Function[0],
                expectedFunction);
            Assert.IsTrue(string.IsNullOrEmpty(LTRFeatureList[0].StandardName));

            // Create a new LTR and validate.
            LongTerminalRepeat ltr =
                new LongTerminalRepeat(expectedLocation);
            LongTerminalRepeat ltrWithILoc = new LongTerminalRepeat(
                genMetadata.Features.LongTerminalRepeats[0].Location);

            // Set qualifiers and validate them.
            ltr.Allele = expectedAllele;
            ltr.GeneSymbol = geneSymbol;
            ltrWithILoc.GenomicMapPosition = expectedMap;
            Assert.AreEqual(ltr.GeneSymbol, geneSymbol);
            Assert.AreEqual(ltr.Allele, expectedAllele);
            Assert.AreEqual(ltrWithILoc.GenomicMapPosition,
                expectedMap);
        }

        /// <summary>
        /// Validate GenBank Operon features
        /// </summary>
        /// <param name="nodeName">XML node name</param>
        /// <param name="genMetadata">GenBank Metadata</param>
        void ValidateGenBankOperon(string nodeName,
            GenBankMetadata genMetadata)
        {
            // Get Values from XML node.            
            string expectedLocation = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Location);
            string expectedAllele = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.AlleleNode);
            string featureCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.QualifierCount);
            string expectedDbReference = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.DbReferenceNode);
            string expectedCitation = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.CitationNode);
            string expectedExperiment = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExperimentNode);
            string expectedInference = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.InferenceNode);
            string expectedLabel = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.LabelNode);
            string expectedNote = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Note);
            string expectedMap = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GenbankMapNode);

            List<OperonRegion> operonFeatureList =
                       genMetadata.Features.OperonRegions;
            LocationBuilder locBuilder = new LocationBuilder();

            // Create a copy of Long Terminal Repeat feature.
            OperonRegion cloneOperon = operonFeatureList[0].Clone();

            // Validate Operon region qualifiers.
            Assert.AreEqual(operonFeatureList.Count.ToString((IFormatProvider)null),
                featureCount);
            Assert.AreEqual(cloneOperon.DatabaseCrossReference[0],
                expectedDbReference);
            Assert.AreEqual(operonFeatureList[0].Allele,
                expectedAllele);
            Assert.AreEqual(operonFeatureList[0].Citation[0],
                expectedCitation);
            Assert.AreEqual(operonFeatureList[0].Experiment[0],
                expectedExperiment);
            Assert.AreEqual(operonFeatureList[0].GenomicMapPosition,
                expectedMap);
            Assert.AreEqual(operonFeatureList[0].Inference[0],
                expectedInference);
            Assert.AreEqual(operonFeatureList[0].Label,
                expectedLabel);
            Assert.AreEqual(locBuilder.GetLocationString(
                genMetadata.Features.OperonRegions[0].Location),
                expectedLocation);
            Assert.AreEqual(operonFeatureList[0].Note[0],
                expectedNote);
            Assert.IsFalse(string.IsNullOrEmpty(operonFeatureList[0].Function.ToString()));
            Assert.AreEqual(operonFeatureList[0].GenomicMapPosition,
                expectedMap);
            Assert.IsTrue(string.IsNullOrEmpty(operonFeatureList[0].Operon));
            Assert.IsFalse(string.IsNullOrEmpty(operonFeatureList[0].Phenotype.ToString()));
            Assert.IsTrue(string.IsNullOrEmpty(operonFeatureList[0].StandardName));
            Assert.IsFalse(operonFeatureList[0].Pseudo);

            // Create a new Operon feature using constructor.
            OperonRegion operonRegion =
                new OperonRegion(expectedLocation);
            OperonRegion operonRegionWithLoc = new OperonRegion(
                genMetadata.Features.OperonRegions[0].Location);

            // Set and validate qualifiers.
            operonRegion.Allele = expectedAllele;
            operonRegionWithLoc.GenomicMapPosition = expectedMap;
            Assert.AreEqual(operonRegionWithLoc.GenomicMapPosition,
                expectedMap);
            Assert.AreEqual(operonRegion.Allele, expectedAllele);
        }

        /// <summary>
        /// Validate GenBank UnsureSequenceRegion features
        /// </summary>
        /// <param name="nodeName">XML node name</param>
        /// <param name="genMetadata">GenBank Metadata</param>
        void ValidateGenBankUnsureSequenceRegion(string nodeName,
            GenBankMetadata genMetadata)
        {
            // Get Values from XML node.           
            string expectedLocation = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Location);
            string expectedAllele = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.AlleleNode);
            string featureCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.QualifierCount);
            string expectedDbReference = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.DbReferenceNode);
            string geneSymbol = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GeneSymbol);
            string expectedCitation = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.CitationNode);
            string expectedExperiment = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExperimentNode);
            string expectedInference = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.InferenceNode);
            string expectedLabel = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.LabelNode);
            string expectedNote = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Note);
            string expectedMap = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GenbankMapNode);

            List<UnsureSequenceRegion> unsureSeqRegionFeatureList =
                        genMetadata.Features.UnsureSequenceRegions;

            // Create a copy of Unsure Seq Region feature.
            UnsureSequenceRegion cloneUnSureSeqRegion =
                unsureSeqRegionFeatureList[0].Clone();
            LocationBuilder locBuilder = new LocationBuilder();

            // Validate Unsure Seq Region qualifiers.
            Assert.AreEqual(unsureSeqRegionFeatureList.Count.ToString((IFormatProvider)null)
                , featureCount);
            Assert.AreEqual(cloneUnSureSeqRegion.DatabaseCrossReference[0],
                expectedDbReference);
            Assert.AreEqual(cloneUnSureSeqRegion.GeneSymbol,
                geneSymbol);
            Assert.AreEqual(unsureSeqRegionFeatureList[0].Allele,
                expectedAllele);
            Assert.AreEqual(unsureSeqRegionFeatureList[0].Citation[0],
                expectedCitation);
            Assert.AreEqual(unsureSeqRegionFeatureList[0].Experiment[0],
                expectedExperiment);
            Assert.AreEqual(unsureSeqRegionFeatureList[0].GenomicMapPosition,
                expectedMap);
            Assert.AreEqual(unsureSeqRegionFeatureList[0].Inference[0],
                expectedInference);
            Assert.AreEqual(unsureSeqRegionFeatureList[0].Label,
                expectedLabel);
            Assert.AreEqual(locBuilder.GetLocationString(
                genMetadata.Features.UnsureSequenceRegions[0].Location),
                expectedLocation);
            Assert.AreEqual(unsureSeqRegionFeatureList[0].Note[0],
                expectedNote);
            Assert.AreEqual(unsureSeqRegionFeatureList[0].GenomicMapPosition,
                expectedMap);
            Assert.IsFalse(string.IsNullOrEmpty(unsureSeqRegionFeatureList[0].Compare.ToString()));
            Assert.IsTrue(string.IsNullOrEmpty(unsureSeqRegionFeatureList[0].Replace));

            // Create a new Unsure feature using constructor.
            UnsureSequenceRegion unsureRegion =
                new UnsureSequenceRegion(expectedLocation);
            UnsureSequenceRegion unsureRegionWithLoc =
                new UnsureSequenceRegion(
                genMetadata.Features.UnsureSequenceRegions[0].Location);

            // Set and validate qualifiers.
            unsureRegion.Allele = expectedAllele;
            unsureRegionWithLoc.GeneSymbol = geneSymbol;
            unsureRegionWithLoc.GenomicMapPosition = expectedMap;
            Assert.AreEqual(unsureRegionWithLoc.GenomicMapPosition,
                expectedMap);
            Assert.AreEqual(unsureRegion.Allele, expectedAllele);
            Assert.AreEqual(unsureRegionWithLoc.GeneSymbol,
                geneSymbol);
        }

        /// <summary>
        /// Validate GenBank RibosomeBindingSite features
        /// </summary>
        /// <param name="nodeName">XML node name</param>
        /// <param name="genMetadata">GenBank Metadata</param>
        void ValidateGenBankRibosomeBindingSite(string nodeName,
            GenBankMetadata genMetadata)
        {
            // Get Values from XML node.           
            string expectedLocation = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Location);
            string expectedAllele = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.AlleleNode);
            string featureCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.QualifierCount);
            string expectedDbReference = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.DbReferenceNode);
            string geneSymbol = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GeneSymbol);
            string expectedCitation = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.CitationNode);
            string expectedExperiment = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExperimentNode);
            string expectedInference = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.InferenceNode);
            string expectedLabel = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.LabelNode);
            string expectedNote = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Note);
            string expectedMap = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GenbankMapNode);

            List<RibosomeBindingSite> ribosomeSite =
                        genMetadata.Features.RibosomeBindingSites;

            // Create a copy of RibosomeBindigSite  Region feature.
            RibosomeBindingSite cloneRibosomeSite =
                ribosomeSite[0].Clone();
            LocationBuilder locBuilder = new LocationBuilder();

            // Validate RibosomeBindigSite qualifiers.
            Assert.AreEqual(ribosomeSite.Count.ToString((IFormatProvider)null)
                , featureCount);
            Assert.AreEqual(cloneRibosomeSite.DatabaseCrossReference[0],
                expectedDbReference);
            Assert.AreEqual(cloneRibosomeSite.GeneSymbol,
                geneSymbol);
            Assert.AreEqual(ribosomeSite[0].Allele,
                expectedAllele);
            Assert.AreEqual(ribosomeSite[0].Citation[0],
                expectedCitation);
            Assert.AreEqual(ribosomeSite[0].Experiment[0],
                expectedExperiment);
            Assert.AreEqual(ribosomeSite[0].Inference[0],
                expectedInference);
            Assert.AreEqual(ribosomeSite[0].Label,
                expectedLabel);
            Assert.AreEqual(locBuilder.GetLocationString(
                genMetadata.Features.RibosomeBindingSites[0].Location),
                expectedLocation);
            Assert.AreEqual(ribosomeSite[0].Note[0],
                expectedNote);
            Assert.AreEqual(ribosomeSite[0].GenomicMapPosition,
                expectedMap);
            Assert.IsNotNull(ribosomeSite[0].OldLocusTag[0]);
            Assert.IsNotNull(ribosomeSite[0].LocusTag[0]);
            Assert.IsNotNull(ribosomeSite[0].StandardName);

            // Create a new RibosomeBindingSite feature using constructor.
            RibosomeBindingSite ribosomeBindingSite =
                new RibosomeBindingSite(expectedLocation);
            RibosomeBindingSite ribosomeBindingSiteLoc =
                new RibosomeBindingSite(
                genMetadata.Features.RibosomeBindingSites[0].Location);

            // Set and validate qualifiers.
            ribosomeBindingSite.Allele = expectedAllele;
            ribosomeBindingSiteLoc.GeneSymbol = geneSymbol;
            ribosomeBindingSiteLoc.GenomicMapPosition = expectedMap;
            Assert.AreEqual(ribosomeBindingSiteLoc.GenomicMapPosition,
                expectedMap);
            Assert.AreEqual(ribosomeBindingSite.Allele, expectedAllele);
            Assert.AreEqual(ribosomeBindingSiteLoc.GeneSymbol,
                geneSymbol);
        }

        /// <summary>
        /// Validate GenBank Non Coding RNA features
        /// </summary>
        /// <param name="nodeName">XML node name</param>
        /// <param name="genMetadata">GenBank Metadata</param>
        void ValidateGenBankNonCodingRNA(string nodeName,
            GenBankMetadata genMetadata)
        {
            // Get Values from XML node.           
            string expectedLocation = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Location);
            string featureCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.QualifierCount);
            string expectedLabel = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.LabelNode);
            string expectedNonCodingRnaClass = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.NonCodingRnaClassNode);

            List<NonCodingRna> nonCodingRNAFeatureList =
                        genMetadata.Features.NonCodingRNAs;
            LocationBuilder locBuilder = new LocationBuilder();

            // Create a copy of Non coding RNA feature.
            NonCodingRna cloneNonCodingRNA =
                nonCodingRNAFeatureList[0].Clone();

            // Validate Non Coding RNA Region qualifiers.
            Assert.AreEqual(nonCodingRNAFeatureList.Count.ToString((IFormatProvider)null),
                featureCount);
            Assert.AreEqual(nonCodingRNAFeatureList[0].NonCodingRnaClass,
                expectedNonCodingRnaClass);
            Assert.AreEqual(cloneNonCodingRNA.Label,
                expectedLabel);
            Assert.AreEqual(locBuilder.GetLocationString(
                genMetadata.Features.NonCodingRNAs[0].Location),
                expectedLocation);

            // Create a non Coding RNA and validate the same.
            NonCodingRna nRNA =
                new NonCodingRna(genMetadata.Features.NonCodingRNAs[0].Location);
            NonCodingRna nRNAWithLocation =
                new NonCodingRna(expectedLocation);

            // Set properties 
            nRNA.NonCodingRnaClass = expectedNonCodingRnaClass;
            nRNAWithLocation.NonCodingRnaClass = expectedNonCodingRnaClass;

            // Validate created nRNA.
            Assert.AreEqual(nRNA.NonCodingRnaClass,
                expectedNonCodingRnaClass);
            Assert.AreEqual(nRNAWithLocation.NonCodingRnaClass,
                expectedNonCodingRnaClass);
        }

        /// <summary>
        /// Validate GenBank CDS features
        /// </summary>
        /// <param name="nodeName">XML node name</param>
        /// <param name="genMetadata">GenBank Metadata</param>
        void ValidateGenBankCDSFeatures(string nodeName,
            GenBankMetadata genMetadata)
        {
            // Get Values from XML node.            
            string expectedLocation = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Location);
            string expectedAllele = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.AlleleNode);
            string featureCount = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.QualifierCount);
            string expectedDbReference = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.DbReferenceNode);
            string geneSymbol = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GeneSymbol);
            string expectedCitation = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.CitationNode);
            string expectedExperiment = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExperimentNode);
            string expectedInference = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.InferenceNode);
            string expectedLabel = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.LabelNode);
            string expectedNote = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Note);
            string expectedMap = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GenbankMapNode);
            string expectedTranslation = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.GenbankTranslationNode);
            string expectedCodonStart = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.CodonStartNode);

            List<CodingSequence> codingSequenceFeatureList =
                        genMetadata.Features.CodingSequences;
            LocationBuilder locBuilder = new LocationBuilder();

            // Create a copy of Coding Seq Region feature.
            CodingSequence cloneCDS = codingSequenceFeatureList[0].Clone();

            // Validate Unsure Seq Region qualifiers.
            Assert.AreEqual(codingSequenceFeatureList.Count.ToString((IFormatProvider)null),
                featureCount);
            Assert.AreEqual(cloneCDS.DatabaseCrossReference[0],
                expectedDbReference);
            Assert.AreEqual(cloneCDS.GeneSymbol, geneSymbol);
            Assert.AreEqual(codingSequenceFeatureList[0].Allele,
                expectedAllele);
            Assert.AreEqual(codingSequenceFeatureList[0].Citation[0],
                expectedCitation);
            Assert.AreEqual(codingSequenceFeatureList[0].Experiment[0],
                expectedExperiment);
            Assert.AreEqual(codingSequenceFeatureList[0].GenomicMapPosition,
                expectedMap);
            Assert.AreEqual(codingSequenceFeatureList[0].Inference[0],
                expectedInference);
            Assert.AreEqual(codingSequenceFeatureList[0].Label,
                expectedLabel);
            Assert.AreEqual(locBuilder.GetLocationString(
                genMetadata.Features.CodingSequences[0].Location),
                expectedLocation);
            Assert.AreEqual(codingSequenceFeatureList[0].Note[0],
                expectedNote);
            Assert.AreEqual(codingSequenceFeatureList[0].GenomicMapPosition,
                expectedMap);
            Assert.AreEqual(codingSequenceFeatureList[0].CodonStart[0],
                expectedCodonStart);
            Assert.AreEqual(codingSequenceFeatureList[0].Translation,
                expectedTranslation);
            Assert.IsFalse(string.IsNullOrEmpty(codingSequenceFeatureList[0].Codon.ToString()));
            Assert.IsTrue(string.IsNullOrEmpty(codingSequenceFeatureList[0].EnzymeCommissionNumber));
            Assert.IsTrue(string.IsNullOrEmpty(codingSequenceFeatureList[0].Number));
            Assert.IsTrue(string.IsNullOrEmpty(codingSequenceFeatureList[0].Operon));
            Assert.IsFalse(codingSequenceFeatureList[0].Pseudo);
            Assert.IsFalse(codingSequenceFeatureList[0].RibosomalSlippage);
            Assert.IsTrue(string.IsNullOrEmpty(codingSequenceFeatureList[0].StandardName));
            Assert.IsFalse(string.IsNullOrEmpty(codingSequenceFeatureList[0].TranslationalExcept.ToString()));
            Assert.IsTrue(string.IsNullOrEmpty(codingSequenceFeatureList[0].TranslationTable));
            Assert.IsFalse(codingSequenceFeatureList[0].TransSplicing);
            Assert.IsTrue(string.IsNullOrEmpty(codingSequenceFeatureList[0].Exception));

            // Create a new CDS feature using constructor.
            CodingSequence cds = new CodingSequence(expectedLocation);
            CodingSequence cdsWithLoc = new CodingSequence(
                genMetadata.Features.CodingSequences[0].Location);
            Sequence seq = cds.GetTranslation();
            Assert.IsNotNull(seq);

            // Set and validate qualifiers.
            cds.Allele = expectedAllele;
            cdsWithLoc.GeneSymbol = geneSymbol;
            cdsWithLoc.GenomicMapPosition = expectedMap;
            Assert.AreEqual(cdsWithLoc.GenomicMapPosition, expectedMap);
            Assert.AreEqual(cds.Allele, expectedAllele);
            Assert.AreEqual(cdsWithLoc.GeneSymbol, geneSymbol);
        }


        /// <summary>
        /// Validate Location builder and location resolver.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        /// <param name="FeatureOperator">Name of the operator used in a location</param>
        /// <param name="isOperator">True if location resolver validation with 
        /// operator</param>
        void ValidateGenBankLocationResolver(string nodeName,
            FeatureOperator operatorName, bool isOperator)
        {
            // Get Values from XML node.
            string sequence = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectedSequence);
            string location = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Location);
            string alphabet = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.AlphabetNameNode);
            string expectedSeq = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpSequenceWithOperator);

            // Create a sequence object.
            ISequence seqObj = new Sequence(Utility.GetAlphabet(alphabet),
                sequence);
            ISequence expectedSeqWithLoc;

            // Build a location.
            ILocationBuilder locBuilder = new LocationBuilder();
            ILocation loc = locBuilder.GetLocation(location);

            if (isOperator)
            {
                switch (operatorName)
                {
                    case FeatureOperator.Complement:
                        loc.Operator = LocationOperator.Complement;
                        break;
                    case FeatureOperator.Join:
                        loc.Operator = LocationOperator.Join;
                        break;
                    case FeatureOperator.Order:
                        loc.Operator = LocationOperator.Order;
                        break;
                    default:
                        break;
                }
            }

            // Get sequence using location of the sequence with operator.
            expectedSeqWithLoc = loc.GetSubSequence(seqObj);

            string sequenceString = new string(expectedSeqWithLoc.Select(a => (char)a).ToArray());
            Assert.AreEqual(expectedSeq, sequenceString);

            // Log to VSTest GUI.
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "GenBankFeatures P1 : Expected sequence is verfied '{0}'.",
               sequenceString));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "GenBankFeatures P1 : Expected sequence is verfied '{0}'.",
               sequenceString));
        }

        /// <summary>
        /// Validate location resolver end data.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        void ValidateLocationEndData(string nodeName)
        {
            // Get Values from XML node.
            string location = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Location);
            string expectedEndData = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.EndData);
            string position = utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.Position);

            bool result = false;

            // Build a location.
            LocationResolver locResolver = new LocationResolver();
            ILocationBuilder locBuilder = new LocationBuilder();
            ILocation loc = locBuilder.GetLocation(location);
            loc.EndData = expectedEndData;

            // Validate whether mentioned end data is present in the location
            // or not.
            result = locResolver.IsInEnd(loc, Int32.Parse(position, (IFormatProvider)null));
            Assert.IsTrue(result);

            // Log to VSTest GUI.
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "GenBankFeatures P1 : Expected sequence is verified"));
            Console.WriteLine(string.Format((IFormatProvider)null,
                "GenBankFeatures P1 : Expected sequence is verified"));
        }

        #endregion Supporting Methods
    }
}
