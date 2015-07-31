/****************************************************************************
 * FastQP1TestCases.cs
 * 
 *This file contains FastQ Parsers and Formatters P1 test cases.
 * 
***************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Bio.Extensions;
using Bio.IO;
using Bio.IO.FastQ;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;
using NUnit.Framework;

namespace Bio.TestAutomation.IO.FastQ
{
    /// <summary>
    ///     FASTQ P1 parser and formatter Test cases implementation.
    /// </summary>
    [TestFixture]
    public class FastQP1TestCases
    {
        private readonly Utility utilityObj = new Utility(@"TestUtils\FastQTestsConfig.xml");

      #region FastQ P1 Test cases

        /// <summary>
        ///     Parse a valid small size FastQ file and convert the same to
        ///     sequence using Parse(file-name) method and validate with the
        ///     expected sequence.
        ///     Input : FastQ file with Sanger format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateFastQParserWithSangerUsingFastQExtensionFile()
        {
            ValidateFastQParser(Constants.SimpleSangerFastQNode);
        }

        /// <summary>
        ///     Parse a valid small size Rna FastQ file and convert the same to
        ///     sequence using Parse(file-name) method and validate with the
        ///     expected sequence.
        ///     Input : Rna FastQ file with Sanger format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateFastQParserWithRnaSanger()
        {
            ValidateFastQParser(Constants.SimpleRnaSangerFastQNode);
        }

        /// <summary>
        ///     Parse a valid small size Rna FastQ file and convert the same to
        ///     sequence using Parse(file-name) method and validate with the
        ///     expected sequence.
        ///     Input : Rna FastQ file with Illumina format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateFastQParserWithRnaIllumina()
        {
            ValidateFastQParser(Constants.SimpleRnaIlluminaFastQNode);
        }

        /// <summary>
        ///     Parse a valid small size Rna FastQ file and convert the same to
        ///     sequence using Parse(file-name) method and validate with the
        ///     expected sequence.
        ///     Input : Rna FastQ file with Solexa format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateFastQParserWithRnaSolexa()
        {
            ValidateFastQParser(Constants.SimpleRnaSolexaFastQNode);
        }

        /// <summary>
        ///     Parse a valid small size Dna FastQ file and convert the same to
        ///     sequence using Parse(file-name) method and validate with the
        ///     expected sequence.
        ///     Input : Dna FastQ file with Sanger format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateFastQParserWithDnaSanger()
        {
            ValidateFastQParser(Constants.SimpleSangerFastQNode);
        }

        /// <summary>
        ///     Parse a valid small size Dna FastQ file and convert the same to
        ///     sequence using Parse(file-name) method and validate with the
        ///     expected sequence.
        ///     Input : Rna FastQ file with Illumina format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateFastQParserWithDnaIllumina()
        {
            ValidateFastQParser(Constants.SimpleIlluminaFastQNode);
        }

        /// <summary>
        ///     Parse a valid small size Dna FastQ file and convert the same to
        ///     sequence using Parse(file-name) method and validate with the
        ///     expected sequence.
        ///     Input : Rna FastQ file with Solexa format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateFastQParserWithDnaSolexa()
        {
            ValidateFastQParser(Constants.SimpleSolexaFqFastQNode);
        }

        /// <summary>
        ///     Parse a valid small size Protein FastQ file and convert the same to
        ///     sequence using Parse(file-name) method and validate with the
        ///     expected sequence.
        ///     Input : Protein FastQ file with Sanger format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateFastQParserWithProteinSanger()
        {
            ValidateFastQParser(Constants.SimpleProteinSangerFastQNode);
        }

        /// <summary>
        ///     Parse a valid small size Protein FastQ file and convert the same to
        ///     sequence using Parse(file-name) method and validate with the
        ///     expected sequence.
        ///     Input : Protein FastQ file with Illumina format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateFastQParserWithProteinIllumina()
        {
            ValidateFastQParser(Constants.SimpleProteinIlluminaFastQNode);
        }

        /// <summary>
        ///     Parse a valid small size Protein FastQ file and convert the same to
        ///     sequence using Parse(file-name) method and validate with the
        ///     expected sequence.
        ///     Input : Protein FastQ file with Solexa format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateFastQParserWithProteinSolexa()
        {
            ValidateFastQParser(Constants.SimpleProteinSolexaFastQNode);
        }

        /// <summary>
        ///     Parse a valid Medium size Sanger FastQ file and convert the same to
        ///     sequence using Parse(file-name) method and validate with the
        ///     expected sequence.
        ///     Input : Medium size FastQ file with Sanger format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQParserValidateParseWithMediumSizeSangerDnaSequence()
        {
            ValidateFastQParser(Constants.MediumSizeDnaSangerFastQNode);
        }

        /// <summary>
        ///     Parse a valid Medium size Illumina FastQ file and convert the same to
        ///     sequence using Parse(file-name) method and validate with the
        ///     expected sequence.
        ///     Input : Medium size FastQ file with Illumina format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQParserValidateParseWithMediumSizeIlluminaDnaSequence()
        {
            ValidateFastQParser(Constants.MediumSizeDnaIlluminaFastQNode);
        }

        /// <summary>
        ///     Parse a valid Medium size Solexa FastQ file and convert the same to
        ///     sequence using Parse(file-name) method and validate with the
        ///     expected sequence.
        ///     Input : Medium size FastQ file with Solexa format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQParserValidateParseWithMediumSizeSolexaDnaSequence()
        {
            ValidateFastQParser(Constants.MediumSizeDnaSolexaFastQNode);
        }

        /// <summary>
        ///     Parse a valid Large size Sanger FastQ file and convert the same to
        ///     sequence using Parse(file-name) method and validate with the
        ///     expected sequence.
        ///     Input : Large size FastQ file with Sanger format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQParserValidateParseWithLargeSizeSangerDnaSequence()
        {
            ValidateFastQParser(Constants.LargeSizeDnaSangerFastQNode);
        }

        /// <summary>
        ///     Parse a valid Large size Illumina FastQ file and convert the same to
        ///     sequence using Parse(file-name) method and validate with the
        ///     expected sequence.
        ///     Input : Large size FastQ file with Illumina format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQParserValidateParseWithLargeSizeIlluminaDnaSequence()
        {
            ValidateFastQParser(Constants.LargeSizeDnaIlluminaFastQNode);
        }

        /// <summary>
        ///     Parse a valid Large size Solexa FastQ file and convert the same to
        ///     sequence using Parse(file-name) method and validate with the
        ///     expected sequence.
        ///     Input : Large size FastQ file with Solexa format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQParserValidateParseWithLargeSizeSolexaDnaSequence()
        {
            ValidateFastQParser(Constants.LargeSizeDnaSolexaFastQNode);
        }

        /// <summary>
        ///     Parse a valid One line sequence Illumina FastQ file and convert the same to
        ///     sequence using Parse(file-name) method and validate with the
        ///     expected sequence.
        ///     Input : One line Dna sequence Illumina FastQ file with Solexa format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQParserValidateParseWithOneLineIluminaDnaSequence()
        {
            ValidateFastQParser(Constants.SingleSequenceIlluminaFastQNode);
        }

        /// <summary>
        ///     Parse a valid One line sequence Sanger FastQ file and convert the same to
        ///     sequence using Parse(file-name) method and validate with the
        ///     expected sequence.
        ///     Input : One line Dna sequence Sanger FastQ file with Solexa format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQParserValidateParseWithOneLineSangerDnaSequence()
        {
            ValidateFastQParser(Constants.SingleSequenceSangerFastQNode);
        }

        /// <summary>
        ///     Parse a valid One line sequence Solexa FastQ file and convert the same to
        ///     sequence using Parse(file-name) method and validate with the
        ///     expected sequence.
        ///     Input : One line Dna sequence Solexa FastQ file with Solexa format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQParserValidateParseWithOneLineSolexaDnaSequence()
        {
            ValidateFastQParser(Constants.SingleSequenceSolexaFastQNode);
        }

        /// <summary>
        ///     Parse a valid Two line Medium size sequence Sanger FastQ file and
        ///     convert the same to sequence using Parse(file-name) method and
        ///     validate with the expected sequence.
        ///     Input : Two line medium size Dna sequence Sanger FastQ file with Solexa format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQParserValidateParseWithTwoLineMediumSizeSangerDnaSequence()
        {
            ValidateFastQParser(Constants.TwoLineDnaSangerFastQNode);
        }

        /// <summary>
        ///     Parse a valid Two line Medium size sequence Illumina FastQ file and
        ///     convert the same to sequence using Parse(file-name) method and
        ///     validate with the expected sequence.
        ///     Input : Two line medium size Dna sequence Sanger FastQ file with Illumina format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQParserValidateParseWithTwoLineMediumSizeIlluminaDnaSequence()
        {
            ValidateFastQParser(Constants.TwoLineDnaIlluminaFastQNode);
        }

        /// <summary>
        ///     Parse a valid Two line Medium size sequence Solexa FastQ file and
        ///     convert the same to sequence using Parse(file-name) method and
        ///     validate with the expected sequence.
        ///     Input : Two line medium size Dna sequence Solexa FastQ file with Illumina format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQParserValidateParseWithTwoLineMediumSizeSolexaDnaSequence()
        {
            ValidateFastQParser(Constants.TwoLineDnaSolexaFastQNode);
        }

        /// <summary>
        ///     Parse a valid Dna Rna multiple sequence FastQ file and
        ///     convert the same to sequence using Parse(file-name) method
        ///     and validate with the expected sequence.
        ///     Input : Multiple sequence FastQ file with Solexa format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateDnaRnaMultipleSeqFastQParserWithSolexa()
        {
            ValidateMulitpleSequenceFastQParser(Constants.MultiSeqSolexaDnaRnaNode, null);
        }

        /// <summary>
        ///     Parse a valid Rna Protein multiple sequence FastQ file and
        ///     convert the same to sequence using Parse(file-name) method
        ///     and validate with the expected sequence.
        ///     Input : Multiple sequence FastQ file with Sanger format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateRnaProteinMultipleSeqFastQParserWithSanger()
        {
            ValidateMulitpleSequenceFastQParser(Constants.MultiSeqSangerRnaProNode, null);
        }

        /// <summary>
        ///     Parse a valid Rna Protein multiple sequence FastQ file and
        ///     convert the same to sequence using Parse(file-name) method
        ///     and validate with the expected sequence.
        ///     Input : Multiple sequence FastQ file with Illumina format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateRnaProteinMultipleSeqFastQParserWithIllumina()
        {
            ValidateMulitpleSequenceFastQParser(Constants.MultiSeqIlluminaRnaProNode, null);
        }

        /// <summary>
        ///     Parse a valid Rna Protein multiple sequence FastQ file and
        ///     convert the same to sequence using Parse(file-name) method
        ///     and validate with the expected sequence.
        ///     Input : Multiple sequence FastQ file with Solexa format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateRnaProteinMultipleSeqFastQParserWithSolexa()
        {
            ValidateMulitpleSequenceFastQParser(Constants.MultiSeqSolexaRnaProNode, null);
        }

        /// <summary>
        ///     Parse a valid Dna Protein multiple sequence FastQ file and
        ///     convert the same to sequence using Parse(file-name) method
        ///     and validate with the expected sequence.
        ///     Input : Multiple sequence FastQ file with Sanger format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateDnaProteinMultipleSeqFastQParserWithSanger()
        {
            ValidateMulitpleSequenceFastQParser(Constants.MultiSeqSangerDnaProNode, null);
        }

        /// <summary>
        ///     Parse a valid Dna Protein multiple sequence FastQ file and
        ///     convert the same to sequence using Parse(file-name) method
        ///     and validate with the expected sequence.
        ///     Input : Multiple sequence FastQ file with Illumina format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateDnaProteinMultipleSeqFastQParserWithIllumina()
        {
            ValidateMulitpleSequenceFastQParser(Constants.MultiSeqIlluminaDnaProNode, null);
        }

        /// <summary>
        ///     Parse a valid Dna Protein multiple sequence FastQ file and
        ///     convert the same to sequence using Parse(file-name) method
        ///     and validate with the expected sequence.
        ///     Input : Multiple sequence FastQ file with Solexa format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateDnaProteinMultipleSeqFastQParserWithSolexa()
        {
            ValidateMulitpleSequenceFastQParser(Constants.MultiSeqSolexaDnaProNode, null);
        }

        /// <summary>
        ///     Parse a valid Dna Rna Protein sequences FastQ file and
        ///     convert the same to sequence using Parse(file-name) method
        ///     and validate with the expected sequence.
        ///     Input : Multiple sequence FastQ file with Sanger format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateDnaRnaProteinMultipleSeqFastQParserWithSanger()
        {
            ValidateMulitpleSequenceFastQParser(Constants.MultiSeqSangerDnaRnaProNode,
                                                "MultiSequenceFastQ");
        }

        /// <summary>
        ///     Parse a valid Dna Rna Protein sequences FastQ file and
        ///     convert the same to sequence using Parse(file-name) method
        ///     and validate with the expected sequence.
        ///     Input : Multiple sequence FastQ file with Illumina format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateDnaRnaProteinMultipleSeqFastQParserWithIllumina()
        {
            ValidateMulitpleSequenceFastQParser(Constants.MultiSeqIlluminaDnaRnaProNode,
                                                "MultiSequenceFastQ");
        }

        /// <summary>
        ///     Parse a valid Dna Rna Protein sequences FastQ file and
        ///     convert the same to sequence using Parse(file-name) method
        ///     and validate with the expected sequence.
        ///     Input : Multiple sequence FastQ file with Solexa format.
        ///     Output : Validation of Expected sequence, Sequence Id,Sequence Type.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateDnaRnaProteinMultipleSeqFastQParserWithSolexa()
        {
            ValidateMulitpleSequenceFastQParser(Constants.MultiSeqSolexaDnaRnaProNode,
                                                "MultiSequenceFastQ");
        }

        /// <summary>
        ///     Parse a Sanger FastQ file and Format a valid Sanger Qualitative Sequence
        ///     to FastQ file using Format(text-writer) and validate Sequence.
        ///     Input : Sanger FastQ file
        ///     Output : Validate format Sanger FastQ file to temp file.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatValidateFormatSangerFastQ()
        {
            ValidateFastQFormatter(Constants.SimpleSangerFastQNode, true);
        }

        /// <summary>
        ///     Parse a Sanger FastQ file and Format a valid Sanger Qualitative Sequence
        ///     to FastQ file using Format(text-writer) and validate Sequence.
        ///     Input :Dna Sanger FastQ file
        ///     Output : Validate format Sanger FastQ file to temp file.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatValidateFormatIlluminaFastQ()
        {
            ValidateFastQFormatter(Constants.SimpleIlluminaFastQNode, true);
        }

        /// <summary>
        ///     Parse a Sanger FastQ file and Format a valid Sanger Qualitative Sequence
        ///     to FastQ file using Format(file-name) and validate Sequence.
        ///     Input : Sanger FastQ file
        ///     Output : Validate format Sanger FastQ file to temp file.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatFastQFileWithDnaSanger()
        {
            ValidateFastQFormatByFormattingQualSeqeunce(Constants.SimpleSangerFastQNode);
        }

        /// <summary>
        ///     Parse a Illumina FastQ file and Format a valid Illumina Qualitative Sequence
        ///     to FastQ file using Format(file-name) and validate Sequence.
        ///     Input :Dna Illumina FastQ file
        ///     Output : Validate format Illumina FastQ file to temp file.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatFastQFileWithDnaIllumina()
        {
            ValidateFastQFormatByFormattingQualSeqeunce(Constants.SimpleIlluminaFastQNode);
        }

        /// <summary>
        ///     Parse a Solexa FastQ file and Format a valid Solexa Qualitative Sequence
        ///     to FastQ file using Format(file-name) and validate Sequence.
        ///     Input :Dna Solexa FastQ file
        ///     Output : Validate format Solexa FastQ file to temp file.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatFastQFileWithDnaSolexa()
        {
            ValidateFastQFormatByFormattingQualSeqeunce(Constants.SimpleSolexaFqFastQNode);
        }

        /// <summary>
        ///     Parse a Sanger FastQ file and Format a valid Sanger Qualitative Sequence
        ///     to FastQ file using Format(file-name) and validate Sequence.
        ///     Input : Rna Sanger FastQ file
        ///     Output : Validate format Sanger FastQ file to temp file.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatFastQFileWithRnaSanger()
        {
            ValidateFastQFormatByFormattingQualSeqeunce(Constants.SimpleRnaSangerFastQNode);
        }

        /// <summary>
        ///     Parse a Illumina FastQ file and Format a valid Illumina Qualitative Sequence
        ///     to FastQ file using Format(file-name) and validate Sequence.
        ///     Input : Rna Illumina FastQ file
        ///     Output : Validate format Illumina FastQ file to temp file.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatFastQFileWithRnaIllumina()
        {
            ValidateFastQFormatByFormattingQualSeqeunce(Constants.SimpleRnaIlluminaFastQNode);
        }

        /// <summary>
        ///     Parse a Solexa FastQ file and Format a valid Solexa Qualitative Sequence
        ///     to FastQ file using Format(file-name) and validate Sequence.
        ///     Input : Rna Solexa FastQ file
        ///     Output : Validate format Solexa FastQ file to temp file.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatFastQFileWithRnaSolexa()
        {
            ValidateFastQFormatByFormattingQualSeqeunce(Constants.SimpleRnaSolexaFastQNode);
        }

        /// <summary>
        ///     Parse a Sanger FastQ file and Format a valid Sanger Qualitative Sequence
        ///     to FastQ file using Format(file-name) and validate Sequence.
        ///     Input : Protein Sanger FastQ file
        ///     Output : Validate format Sanger FastQ file to temp file.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatFastQFileWithProteinSanger()
        {
            ValidateFastQFormatByFormattingQualSeqeunce(Constants.SimpleProteinSangerFastQNode);
        }

        /// <summary>
        ///     Parse a Illumina FastQ file and Format a valid Illumina Qualitative Sequence
        ///     to FastQ file using Format(file-name) and validate Sequence.
        ///     Input : Protein Illumina FastQ file
        ///     Output : Validate format Illumina FastQ file to temp file.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatFastQFileWithProteinIllumina()
        {
            ValidateFastQFormatByFormattingQualSeqeunce(Constants.SimpleProteinIlluminaFastQNode);
        }

        /// <summary>
        ///     Parse a Solexa FastQ file and Format a valid Solexa Qualitative Sequence
        ///     to FastQ file using Format(file-name) and validate Sequence.
        ///     Input : Protein Solexa FastQ file
        ///     Output : Validate format Solexa FastQ file to temp file.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatFastQFileWithProteinSolexa()
        {
            ValidateFastQFormatByFormattingQualSeqeunce(Constants.SimpleProteinSolexaFastQNode);
        }

        /// <summary>
        ///     Parse a Dna Sanger FastQ file and Format a valid Sanger Qualitative Sequence
        ///     to FastQ file using Format(text-writer) and validate Sequence.
        ///     Input :Dna Sanger FastQ file
        ///     Output : Validate format Sanger FastQ file to temp file.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatValidateFormatDnaSangerFastQUsingTextWriter()
        {
            ValidateFastQFormatter(Constants.SimpleSangerFastQNode, true);
        }

        /// <summary>
        ///     Parse a Dna Illumina FastQ file and Format a valid Sanger Qualitative Sequence
        ///     to FastQ file using Format(text-writer) and validate Sequence.
        ///     Input :Dna Illumina FastQ file
        ///     Output : Validate format Illumina FastQ file to temp file.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatValidateFormatDnaIlluminaFastQUsingTextWriter()
        {
            ValidateFastQFormatter(Constants.SimpleIlluminaFastQNode, true);
        }

        /// <summary>
        ///     Parse a Dna Solexa FastQ file and Format a valid Solexa Qualitative Sequence
        ///     to FastQ file using Format(text-writer) and validate Sequence.
        ///     Input :Dna Solexa FastQ file
        ///     Output : Validate format Solexa FastQ file to temp file.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatValidateFormatDnaSolexaFastQUsingTextWriter()
        {
            ValidateFastQFormatter(Constants.SimpleSolexaFqFastQNode, true);
        }

        /// <summary>
        ///     Parse a Rna Sanger FastQ file and Format a valid Sanger Qualitative Sequence
        ///     to FastQ file using Format(text-writer) and validate Sequence.
        ///     Input :Rna Sanger FastQ file
        ///     Output : Validate format Sanger FastQ file to temp file.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatValidateFormatRnaSangerFastQUsingTextWriter()
        {
            ValidateFastQFormatter(Constants.SimpleRnaSangerFastQNode, true);
        }

        /// <summary>
        ///     Parse a Rna Illumina FastQ file and Format a valid Sanger Qualitative Sequence
        ///     to FastQ file using Format(text-writer) and validate Sequence.
        ///     Input :Rna Illumina FastQ file
        ///     Output : Validate format Illumina FastQ file to temp file.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatValidateFormatRnaIlluminaFastQUsingTextWriter()
        {
            ValidateFastQFormatter(Constants.SimpleRnaIlluminaFastQNode, true);
        }

        /// <summary>
        ///     Parse a Rna Solexa FastQ file and Format a valid Solexa Qualitative Sequence
        ///     to FastQ file using Format(text-writer) and validate Sequence.
        ///     Input :Rna Solexa FastQ file
        ///     Output : Validate format Solexa FastQ file to temp file.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatValidateFormatRnaSolexaFastQUsingTextWriter()
        {
            ValidateFastQFormatter(Constants.SimpleRnaSolexaFastQNode, true);
        }

        /// <summary>
        ///     Parse a Protein Sanger FastQ file and Format a valid Sanger Qualitative Sequence
        ///     to FastQ file using Format(text-writer) and validate Sequence.
        ///     Input :Protein Sanger FastQ file
        ///     Output : Validate format Sanger FastQ file to temp file.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatValidateFormatProteinSangerFastQUsingTextWriter()
        {
            ValidateFastQFormatter(Constants.SimpleProteinSangerFastQNode, true);
        }

        /// <summary>
        ///     Parse a Protein Illumina FastQ file and Format a valid Sanger Qualitative Sequence
        ///     to FastQ file using Format(text-writer) and validate Sequence.
        ///     Input :Protein Illumina FastQ file
        ///     Output : Validate format Illumina FastQ file to temp file.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatValidateFormatProteinIlluminaFastQUsingTextWriter()
        {
            ValidateFastQFormatter(Constants.SimpleProteinIlluminaFastQNode, true);
        }

        /// <summary>
        ///     Parse a Protein Solexa FastQ file and Format a valid Solexa Qualitative Sequence
        ///     to FastQ file using Format(text-writer) and validate Sequence.
        ///     Input :Protein Solexa FastQ file
        ///     Output : Validate format Solexa FastQ file to temp file.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatValidateFormatProteinSolexaFastQUsingTextWriter()
        {
            ValidateFastQFormatter(Constants.SimpleProteinSolexaFastQNode, true);
        }

        /// <summary>
        ///     Format a medium size Solexa Qualitative sequence to FastQ
        ///     file.using format(Text-Writer) method and validate the same.
        ///     Input Data : Solexa Medium size sequence.
        ///     Output Data : Validation of fromatting medium size Solexa
        ///     qualitative sequence to valid FastQ file.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatMediumSizeDnaSolexaUsingTextWriter()
        {
            ValidateFastQFormatter(Constants.MediumSizeDnaSolexaFastQNode, true);
        }

        /// <summary>
        ///     Format a medium size Illumina Qualitative sequence to FastQ
        ///     file.using format(Text-Writer) method and validate the same.
        ///     Input Data : Illumina Medium size sequence.
        ///     Output Data : Validation of fromatting medium size Illumina
        ///     qualitative sequence to valid FastQ file.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatMediumSizeDnaIlluminaUsingTextWriter()
        {
            ValidateFastQFormatter(Constants.MediumSizeDnaIlluminaFastQNode, true);
        }

        /// <summary>
        ///     Format a medium size Sanger Qualitative sequence to FastQ
        ///     file.using format(Text-Writer) method and validate the same.
        ///     Input Data : Sanger Medium size sequence.
        ///     Output Data : Validation of fromatting medium size sanger
        ///     qualitative sequence to valid FastQ file.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatMediumSizeDnaSangerUsingTextWriter()
        {
            ValidateFastQFormatter(Constants.MediumSizeDnaSangerFastQNode, true);
        }

        /// <summary>
        ///     Format a medium size Solexa Qualitative sequence to FastQ
        ///     file.using format(file-name) method and validate the same.
        ///     Input Data : Solexa Medium size sequence.
        ///     Output Data : Validation of fromatting medium size Solexa
        ///     qualitative sequence to valid FastQ file.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatMediumSizeDnaSolexaUsingFile()
        {
            ValidateFastQFormatter(Constants.MediumSizeDnaSolexaFastQNode, false);
        }

        /// <summary>
        ///     Format a medium size Illumina Qualitative sequence to FastQ
        ///     file.using format(file-name) method and validate the same.
        ///     Input Data : Illumina Medium size sequence.
        ///     Output Data : Validation of fromatting medium size Illumina
        ///     qualitative sequence to valid FastQ file.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatMediumSizeDnaIlluminaUsingFile()
        {
            ValidateFastQFormatter(Constants.MediumSizeDnaIlluminaFastQNode, false);
        }

        /// <summary>
        ///     Format a medium size Sanger Qualitative sequence to FastQ
        ///     file.using format(file-name) method and validate the same.
        ///     Input Data : Sanger Medium size sequence.
        ///     Output Data : Validation of fromatting medium size sanger
        ///     qualitative sequence to valid FastQ file.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatMediumSizeDnaSangerUsingFile()
        {
            ValidateFastQFormatter(Constants.MediumSizeDnaSangerFastQNode, false);
        }

        /// <summary>
        ///     Format a Large size(>100KB) Sanger Qualitative sequence to FastQ
        ///     file.using Format() method and validate the same.
        ///     Input Data : Sanger Large size sequence.
        ///     Output Data : Validation of fromatting Large size Sanger
        ///     qualitative sequence to valid FastQ file.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatLargeSizeDnaSangerSeq()
        {
            ValidateFastQFormatByFormattingQualSeqeunce(
                Constants.LargeSizeDnaSangerFastQNode);
        }

        /// <summary>
        ///     Format a Large size(>100KB) Illumina Qualitative sequence to FastQ
        ///     file.using Format() method and validate the same.
        ///     Input Data : Illumina Large size sequence.
        ///     Output Data : Validation of fromatting Large size Illumina
        ///     qualitative sequence to valid FastQ file.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatLargeSizeDnaIlluminaSeq()
        {
            ValidateFastQFormatByFormattingQualSeqeunce(
                Constants.LargeSizeDnaIlluminaFastQNode);
        }

        /// <summary>
        ///     Format a Large size(>100KB) Solexa Qualitative sequence to FastQ
        ///     file.using Format() method and validate the same.
        ///     Input Data : Solexa Large size sequence.
        ///     Output Data : Validation of fromatting Large size Solexa
        ///     qualitative sequence to valid FastQ file.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatLargeSizeDnaSolexaSeq()
        {
            ValidateFastQFormatByFormattingQualSeqeunce(
                Constants.LargeSizeDnaSolexaFastQNode);
        }

        /// <summary>
        ///     Parse and Format a Large size(>100KB) Sanger Qualitative sequence
        ///     to FastQ file.using Format() method and validate the same.
        ///     Input Data : Sanger Large size sequence.
        ///     Output Data : Validation of fromatting Large size Sanger
        ///     qualitative sequence to valid FastQ file.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatLargeSizeDnaSangerFile()
        {
            ValidateFastQFormatter(Constants.LargeSizeDnaSangerFastQNode, false);
        }

        /// <summary>
        ///     Parse and Format a Large size(>100KB) Illumina Qualitative sequence
        ///     to FastQ file.using Format() method and validate the same.
        ///     Input Data : Illumina Large size sequence.
        ///     Output Data : Validation of fromatting Large size Illumina
        ///     qualitative sequence to valid FastQ file.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatLargeSizeDnaIlluminaFile()
        {
            ValidateFastQFormatter(Constants.LargeSizeDnaIlluminaFastQNode, false);
        }

        /// <summary>
        ///     Parse and Format a Large size(>100KB) Solexa Qualitative sequence
        ///     to FastQ file.using Format() method and validate the same.
        ///     Input Data : Solexa Large size sequence.
        ///     Output Data : Validation of fromatting Large size Solexa
        ///     qualitative sequence to valid FastQ file.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatLargeSizeDnaSolexaFile()
        {
            ValidateFastQFormatter(Constants.LargeSizeDnaSolexaFastQNode, false);
        }

        /// <summary>
        ///     Parse and Format a Sanger Qualitative Dna Rna sequence
        ///     to FastQ file.using Format() method and validate the same.
        ///     Input Data : Sanger Dna Rna multi sequence file.
        ///     Output Data : Validation of Multi sequence FastQ format.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatDnaRnaSangerFile()
        {
            ValidateMultiSeqFastQFormatter(Constants.MultiSeqSangerDnaRnaNode);
        }

        /// <summary>
        ///     Parse and Format a Illumina Qualitative Dna Rna sequence
        ///     to FastQ file.using Format() method and validate the same.
        ///     Input Data : Illumina Dna Rna multi sequence file.
        ///     Output Data : Validation of Multi sequence FastQ format.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatDnaRnaIlluminaFile()
        {
            ValidateMultiSeqFastQFormatter(Constants.MultiSeqIlluminaDnaRnaNode);
        }

        /// <summary>
        ///     Parse and Format a Solexa Qualitative Dna Rna sequence
        ///     to FastQ file.using Format() method and validate the same.
        ///     Input Data : Solexa Dna Rna multi sequence file.
        ///     Output Data : Validation of Multi sequence FastQ format.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatDnaRnaSolexaFile()
        {
            ValidateMultiSeqFastQFormatter(Constants.MultiSeqSolexaDnaRnaNode);
        }

        /// <summary>
        ///     Parse and Format a Sanger Qualitative Rna Protein sequence
        ///     to FastQ file.using Format() method and validate the same.
        ///     Input Data : Sanger Rna Protein multi sequence file.
        ///     Output Data : Validation of Multi sequence FastQ format.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatRnaProteinSangerFile()
        {
            ValidateMultiSeqFastQFormatter(Constants.MultiSeqSangerRnaProNode);
        }

        /// <summary>
        ///     Parse and Format a Illumina Qualitative Rna Protein sequence
        ///     to FastQ file.using Format() method and validate the same.
        ///     Input Data : Illumina Rna Protein multi sequence file.
        ///     Output Data : Validation of Multi sequence FastQ format.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatRnaProteinIlluminaFile()
        {
            ValidateMultiSeqFastQFormatter(Constants.MultiSeqIlluminaRnaProNode);
        }

        /// <summary>
        ///     Parse and Format a Solexa Qualitative Rna Protein sequence
        ///     to FastQ file.using Format() method and validate the same.
        ///     Input Data : Solexa Rna Protein multi sequence file.
        ///     Output Data : Validation of Multi sequence FastQ format.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatRnaProteinSolexaFile()
        {
            ValidateMultiSeqFastQFormatter(Constants.MultiSeqSolexaRnaProNode);
        }

        /// <summary>
        ///     Parse and Format a Sanger Qualitative Dna Protein sequence
        ///     to FastQ file.using Format() method and validate the same.
        ///     Input Data : Sanger Dna Protein multi sequence file.
        ///     Output Data : Validation of Multi sequence FastQ format.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatDnaProteinSangerFile()
        {
            ValidateMultiSeqFastQFormatter(Constants.MultiSeqSangerDnaProNode);
        }

        /// <summary>
        ///     Parse and Format a Illumina Qualitative Dna Protein sequence
        ///     to FastQ file.using Format() method and validate the same.
        ///     Input Data : Illumina Dna Protein multi sequence file.
        ///     Output Data : Validation of Multi sequence FastQ format.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatDnaProteinIlluminaFile()
        {
            ValidateMultiSeqFastQFormatter(Constants.MultiSeqIlluminaDnaProNode);
        }

        /// <summary>
        ///     Parse and Format a Solexa Qualitative Dna Protein sequence
        ///     to FastQ file.using Format() method and validate the same.
        ///     Input Data : Solexa Dna Protein multi sequence file.
        ///     Output Data : Validation of Multi sequence FastQ format.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatDnaProteinSolexaFile()
        {
            ValidateMultiSeqFastQFormatter(Constants.MultiSeqSolexaDnaProNode);
        }

        /// <summary>
        ///     Parse and Format a Sanger Qualitative Dna Rna Protein sequence
        ///     to FastQ file.using Format() method and validate the same.
        ///     Input Data : Sanger Dna Rna Protein multi sequence file.
        ///     Output Data : Validation of Multi sequence FastQ format.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatDnaRnaProteinSangerFile()
        {
            ValidateMultiSeqFastQFormatter(Constants.MultiSeqSangerDnaRnaProNode);
        }

        /// <summary>
        ///     Parse and Format a Illumina Qualitative Dna Rna Protein sequence
        ///     to FastQ file.using Format() method and validate the same.
        ///     Input Data : Illumina Dna Rna Protein multi sequence file.
        ///     Output Data : Validation of Multi sequence FastQ format.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatDnaRnaProteinIlluminaFile()
        {
            ValidateMultiSeqFastQFormatter(Constants.MultiSeqIlluminaDnaRnaProNode);
        }

        /// <summary>
        ///     Parse and Format a Solexa Qualitative Dna Rna Protein sequence
        ///     to FastQ file.using Format() method and validate the same.
        ///     Input Data : Solexa Dna Rna Protein multi sequence file.
        ///     Output Data : Validation of Multi sequence FastQ format.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void FastQFormatDnaRnaProteinSolexaFile()
        {
            ValidateMultiSeqFastQFormatter(Constants.MultiSeqSolexaDnaRnaProNode);
        }

        /// <summary>
        ///     Parse a valid small size Dna FastQ file and convert the same to
        ///     sequence using parse(file-name) method and validate with the
        ///     expected sequence.
        ///     Input : Dna FastQ file with Sanger format.
        ///     Output : Validation of Expected sequence.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateFastQParserBasicSequenceWithDnaSanger()
        {
            ValidateBasicSequenceParser(
                Constants.SimpleSangerFastQNode);
        }

        #endregion FastQ P1 Test cases

        #region Supporting Methods

        /// <summary>
        ///     General method to validate FastQ Parser.
        ///     <param name="nodeName">xml node name.</param>
        /// </summary>
        private void ValidateFastQParser(string nodeName)
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string expectedQualitativeSequence = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceNode);
            string expectedSequenceId = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SequenceIdNode);
            string expectedSeqCount = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SeqsCount);
            IAlphabet alphabet = Utility.GetAlphabet(utilityObj.xmlUtil.GetTextValue(nodeName, Constants.AlphabetNameNodeV2));

            var fastQParserObj = new FastQParser();
            {
                // Validate qualitative Sequence upon parsing FastQ file.
                IList<IQualitativeSequence> qualSequenceList = fastQParserObj.Parse<IQualitativeSequence>(filePath).ToList();
                Assert.AreEqual(expectedSeqCount, qualSequenceList.Count.ToString((IFormatProvider) null));
                Assert.AreEqual(expectedQualitativeSequence, qualSequenceList[0].ConvertToString());
                Assert.AreEqual(expectedSequenceId, qualSequenceList[0].ID.ToString(null));
                Assert.AreEqual(alphabet, qualSequenceList[0].Alphabet);
            }
        }

        /// <summary>
        ///     General method to validate FastQ Parser for Multiple sequence with
        ///     different alphabets.
        ///     <param name="nodeName">xml node name.</param>
        ///     <param name="triSeq">Tri Sequence</param>
        /// </summary>
        private void ValidateMulitpleSequenceFastQParser(string nodeName, string triSeq)
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string expectedFirstQualitativeSequence = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequence1Node);
            string expectedSecondQualitativeSequence = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequence2Node);
            string expectedthirdQualitativeSequence = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequence3Node);
            string expectedSequenceId = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SequenceIdNode);
            int expectedSeqCount = int.Parse(utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SeqsCount));

            // Parse a multiple sequence FastQ file.
            var fastQParserObj = new FastQParser();
            using (fastQParserObj.Open(filePath))
            {
                IList<IQualitativeSequence> qualSequenceList = fastQParserObj.Parse().ToList();

                // Validate first qualitative Sequence upon parsing FastQ file.
                Assert.AreEqual(expectedSeqCount, qualSequenceList.Count);
                Assert.AreEqual(expectedFirstQualitativeSequence, qualSequenceList[0].ConvertToString());
                Assert.AreEqual(expectedSequenceId, qualSequenceList[0].ID);

                // Validate second qualitative Sequence upon parsing FastQ file.
                Assert.AreEqual(expectedSecondQualitativeSequence, qualSequenceList[1].ConvertToString());
                Assert.AreEqual(expectedSequenceId, qualSequenceList[1].ID);

                // Validate third sequence in FastQ file if it is tri sequence FastQ file.
                if (0 == string.Compare(triSeq, "MultiSequenceFastQ", CultureInfo.CurrentCulture, CompareOptions.IgnoreCase))
                {
                    // Validate second qualitative Sequence upon parsing FastQ file.
                    Assert.AreEqual(expectedthirdQualitativeSequence, qualSequenceList[2].ConvertToString());
                    Assert.AreEqual(expectedSequenceId, qualSequenceList[2].ID);
                }

                ApplicationLog.WriteLine(string.Format("FastQ Parser P1: The FASTQ sequence '{0}' validation after Parse() is found to be as expected.",
                                                       qualSequenceList[0]));
            }
        }


        /// <summary>
        ///     General method to validate FastQ formatting
        ///     Qualitative Sequence by passing TextWriter as a parameter
        ///     <param name="nodeName">xml node name.</param>
        /// </summary>
        private void ValidateFastQFormatByFormattingQualSeqeunce(string nodeName)
        {
            // Gets the actual sequence and the alphabet from the Xml
            IAlphabet alphabet = Utility.GetAlphabet(utilityObj.xmlUtil.GetTextValue(nodeName, Constants.AlphabetNameNodeV2));
            FastQFormatType expectedFormatType = Utility.GetFastQFormatType(utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FastQFormatType));
            string qualSequence = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceNode);
            string expectedQualitativeSequence = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceNode);
            string qualityScores = "";
            int i;

            for (i = 0; i < qualSequence.Length; i++)
                qualityScores = qualityScores + "}";

            byte[] seq = Encoding.UTF8.GetBytes(qualSequence);
            byte[] qScore = Encoding.UTF8.GetBytes(qualityScores);
            string tempFileName = Path.GetTempFileName();

            // Create a Qualitative Sequence.
            var qualSeq = new QualitativeSequence(alphabet, expectedFormatType, seq, qScore);

            var formatter = new FastQFormatter();
            using (formatter.Open(tempFileName))
            {
                formatter.Format(qualSeq);
                formatter.Close();

                var fastQParserObj = new FastQParser();
                using (fastQParserObj.Open(tempFileName))
                {
                    // Read the new file and validate Sequences.
                    var seqsNew = fastQParserObj.Parse();
                    var firstSequence = seqsNew.First();

                    // Validate qualitative Sequence upon parsing FastQ file.
                    Assert.AreEqual(expectedQualitativeSequence, firstSequence.ConvertToString());
                    Assert.IsTrue(string.IsNullOrEmpty(firstSequence.ID));

                    ApplicationLog.WriteLine(string.Format("FastQ Parser P1: The FASTQ sequence '{0}' validation after Parse() is found to be as expected.", firstSequence));
                }

                File.Delete(tempFileName);
            }
        }

        /// <summary>
        ///     General method to validate FastQ Formatter by Passing Writer as parameter.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        /// <param name="writeMultipleSequences">FastQ formatter Format() method parameter</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void ValidateFastQFormatter(string nodeName, bool writeMultipleSequences)
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string expectedQualitativeSequence = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceNode);
            string expectedSequenceId = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SequenceIdNode);
            string tempFileName = Path.GetTempFileName();

            // Parse a FastQ file.
            var fastQParserObj = new FastQParser();
            using (fastQParserObj.Open(filePath))
            {
                IEnumerable<IQualitativeSequence> qualSequenceList = fastQParserObj.Parse();

                var fastQFormatter = new FastQFormatter();
                using (fastQFormatter.Open(tempFileName))
                {
                    if (writeMultipleSequences)
                    {
                        foreach (IQualitativeSequence newQualSeq in qualSequenceList)
                        {
                            fastQFormatter.Format(newQualSeq);
                        }
                    }                    
                    else
                    {
                        fastQFormatter.Format(qualSequenceList.First());
                    }
                } // temp file is closed.

                // Read the new file and validate the first Sequence.
                FastQParser fastQParserObjNew = new FastQParser();
                IQualitativeSequence firstSequence = fastQParserObjNew.ParseOne(tempFileName);

                // Validate qualitative Sequence upon parsing FastQ file.
                Assert.AreEqual(expectedQualitativeSequence, firstSequence.ConvertToString());
                Assert.AreEqual(expectedSequenceId, firstSequence.ID);

                ApplicationLog.WriteLine(string.Format("FastQ Parser P1: The FASTQ sequence '{0}' validation after Parse() is found to be as expected.", firstSequence));

                File.Delete(tempFileName);
            }
        }

        /// <summary>
        ///     General method to validate multi sequence FastQ Format.
        /// </summary>
        /// <param name="nodeName">xml node name.</param>
        private void ValidateMultiSeqFastQFormatter(string nodeName)
        {
            // Gets the expected sequence from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string expectedQualitativeSequence = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceNode);
            string expectedSequenceId = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SequenceIdNode);
            string expectedSecondQualitativeSequence = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequence2Node);
            string expectedSecondSeqID = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceId1Node);

            // Parse a FastQ file.
            var fastQParserObj = new FastQParser();
            using (fastQParserObj.Open(filePath))
            {
                var qualSequenceList = fastQParserObj.Parse().ToList();

                // Format a first Qualitative sequence
                new FastQFormatter().Format(qualSequenceList[0], Constants.FastQTempFileName);

                // Read it back
                var seqsNew = new FastQParser().Parse(Constants.FastQTempFileName).ToList();

                // Format a Second Qualitative sequence
                new FastQFormatter().Format(qualSequenceList[1], Constants.StreamWriterFastQTempFileName);
                var secondSeqsNew = new FastQParser().Parse(Constants.StreamWriterFastQTempFileName).ToList();

                // Validate Second qualitative Sequence upon parsing FastQ file.
                Assert.AreEqual(expectedSecondQualitativeSequence, secondSeqsNew[0].ConvertToString());
                Assert.AreEqual(expectedSecondSeqID, secondSeqsNew[0].ID);

                // Validate first qualitative Sequence upon parsing FastQ file.
                Assert.AreEqual(expectedQualitativeSequence, seqsNew[0].ConvertToString());
                Assert.AreEqual(expectedSequenceId, seqsNew[0].ID);

                ApplicationLog.WriteLine(string.Format("FastQ Parser P1: The FASTQ sequence '{0}' validation after Parse() is found to be as expected.", seqsNew[0]));

                File.Delete(Constants.FastQTempFileName);
                File.Delete(Constants.StreamWriterFastQTempFileName);
            }
        }

        /// <summary>
        ///     General method to validate BasicSequence Parser.
        ///     <param name="nodeName">xml node name.</param>
        /// </summary>
        private void ValidateBasicSequenceParser(string nodeName)
        {
            // Gets the expected sequence from the Xml
            string filepathOriginal = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            string expectedQualitativeSequence = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceNode);
            string expectedSequenceId = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SequenceIdNode);
            IAlphabet alphabet = Utility.GetAlphabet(utilityObj.xmlUtil.GetTextValue(nodeName, Constants.AlphabetNameNode));
            Assert.IsTrue(File.Exists(filepathOriginal));

            string tempPath = Path.GetTempFileName();

            try
            {
                ISequenceParser fastQParserObj = SequenceParsers.FindParserByFileName("temp.fq");
                
                // Read the original file
                IEnumerable<ISequence> seqsOriginal = fastQParserObj.Parse(filepathOriginal);
                Assert.IsNotNull(seqsOriginal);

                // Use the formatter to write the original sequences to a temp file               
                var formatter = new FastQFormatter();
                formatter.Format(seqsOriginal.ElementAt(0), tempPath);


                // Read the new file, then compare the sequences
                var fastQParserObjNew = new FastQParser();
                IEnumerable<IQualitativeSequence> seqsNew = fastQParserObjNew.Parse(tempPath);
                Assert.IsNotNull(seqsNew);

                // Validate qualitative Sequence upon parsing FastQ file.
                Assert.AreEqual(expectedQualitativeSequence,
                    new string(seqsOriginal.ElementAt(0).Select(a => (char) a).ToArray()));
                Assert.AreEqual(
                    seqsOriginal.ElementAt(0).ID.ToString(null),
                    expectedSequenceId);
                Assert.AreEqual(
                    seqsOriginal.ElementAt(0).Alphabet.Name,
                    alphabet.Name);

                ApplicationLog.WriteLine(string.Format("FastQ Parser P1: The FASTQ sequence '{0}' validation after Parse() is found to be as expected.",
                                                       seqsOriginal.ElementAt(0)));
            }
            finally
            {
                File.Delete(tempPath);
            }
        }

        #endregion Supporting Methods
    }
}