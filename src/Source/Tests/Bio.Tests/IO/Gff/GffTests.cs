﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bio;
using Bio.IO;
using Bio.IO.Gff;
using Bio.Util.Logging;
using NUnit.Framework;
using Bio.Tests.Properties;

namespace Bio.Tests.IO.Gff
{
    /// <summary>
    /// Unit tests for the GffParser and GffFormatter.
    /// </summary>
    [TestFixture]
    public class GffTests
    {

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static GffTests()
        {
            Assert.IsTrue(File.Exists(_singleSeqGffFilename));
            Assert.IsTrue(File.Exists(_multipleSeqGffFilename));
        }

        #region Fields

        private static readonly string _today = DateTime.Today.ToString("yyyy-MM-dd", (IFormatProvider)null);
        private static readonly string _gffDataPath = System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, "TestUtils", "GFF");
        private static readonly string _singleSeqGffFilename = System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, "TestUtils", "GFF", "random.GFF");
        private static readonly string TempGFFFileName = System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, "tempGFF.GFF");
        private static readonly string _multipleSeqGffFilename = System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, "TestUtils", "GFF", "Tachibana2005.gff");
        private static readonly string _singleSeqGffFileExpectedOutput =
@"##gff-version 2
##date " + _today + @"
NC_001133.7	RefSeq	source	1	230208	.	+	.	organism=Saccharomyces cerevisiae;mol_type=genomic DNA;strain=S288C;db_xref=taxon:4932;chromosome=I
NC_001133.7	RefSeq	repeat_region	1	801	.	-	.	ID=NC_001133.7:TEL01L:unknown_transcript_1;Parent=NC_001133.7:TEL01L;note=Telomeric region on the left arm of Chromosome I%3B composed of an X element core sequence%2C X element combinatorial repeats%2C and a short terminal stretch of telomeric repeats;inference=non-experimental evidence%2C no additional details recorded;rpt_family=Telomeric Region;db_xref=SGD:S000028862
NC_001133.7	RefSeq	repeat_region	1	62	.	-	.	ID=NC_001133.7:TEL01L-TR:unknown_transcript_1;Parent=NC_001133.7:TEL01L-TR;note=Terminal stretch of telomeric repeats on the left arm of Chromosome I;inference=non-experimental evidence%2C no additional details recorded;rpt_family=Telomeric Repeat;db_xref=SGD:S000028864
NC_001133.7	RefSeq	repeat_region	63	336	.	-	.	ID=NC_001133.7:TEL01L-XR:unknown_transcript_1;Parent=NC_001133.7:TEL01L-XR;note=Telomeric X element combinatorial Repeat region on the left arm of Chromosome I%3B contains repeats of the D%2C C%2C B and A types%2C as well as Tbf1p binding sites%3B formerly called SubTelomeric Repeats;inference=non-experimental evidence%2C no additional details recorded;rpt_family=X element Combinatorial Repeats;db_xref=SGD:S000028866
NC_001133.7	RefSeq	repeat_region	337	801	.	-	.	ID=NC_001133.7:TEL01L-XC:unknown_transcript_1;Parent=NC_001133.7:TEL01L-XC;note=Telomeric X element Core sequence on the left arm of Chromosome I%3B contains an ARS consensus sequence%2C an Abf1p binding site consensus sequence and two small overlapping ORFs %28YAL068W-A and YAL069W%29;inference=non-experimental evidence%2C no additional details recorded;rpt_family=X element Core sequence;db_xref=SGD:S000028865
NC_001133.7	RefSeq	rep_origin	650	1791	.	+	.	note=ARS102;inference=non-experimental evidence%2C no additional details recorded;db_xref=SGD:S000121252
NC_001133.7	RefSeq	gene	1807	2169	.	-	.	ID=NC_001133.7:PAU8;locus_tag=YAL068C;db_xref=GeneID:851229
NC_001133.7	RefSeq	CDS	1810	2169	.	-	0	ID=NC_001133.7:PAU8:unknown_transcript_1;Parent=NC_001133.7:PAU8;locus_tag=YAL068C;experiment=experimental evidence%2C no additional details recorded;product=Pau8p;protein_id=NP_009332.1;db_xref=SGD:S000002142;db_xref=GI:6319249;db_xref=GeneID:851229;exon_number=1
NC_001133.7	RefSeq	start_codon	2167	2169	.	-	0	ID=NC_001133.7:PAU8:unknown_transcript_1;Parent=NC_001133.7:PAU8;locus_tag=YAL068C;experiment=experimental evidence%2C no additional details recorded;product=Pau8p;protein_id=NP_009332.1;db_xref=SGD:S000002142;db_xref=GI:6319249;db_xref=GeneID:851229;exon_number=1
NC_001133.7	RefSeq	stop_codon	1807	1809	.	-	0	ID=NC_001133.7:PAU8:unknown_transcript_1;Parent=NC_001133.7:PAU8;locus_tag=YAL068C;experiment=experimental evidence%2C no additional details recorded;product=Pau8p;protein_id=NP_009332.1;db_xref=SGD:S000002142;db_xref=GI:6319249;db_xref=GeneID:851229;exon_number=1
NC_001133.7	RefSeq	gene	2480	2707	.	+	.	locus_tag=YAL067W-A;db_xref=GeneID:1466426
NC_001133.7	RefSeq	CDS	2480	2704	.	+	0	locus_tag=YAL067W-A;note=Yal067w-ap;inference=non-experimental evidence%2C no additional details recorded;product=Putative protein of unknown function%3B identified by gene-trapping%2C microarray-based expression analysis%2C and genome-wide homology searching;protein_id=NP_878038.1;db_xref=SGD:S000028593;db_xref=GI:33438754;db_xref=GeneID:1466426;exon_number=1
NC_001133.7	RefSeq	start_codon	2480	2482	.	+	0	locus_tag=YAL067W-A;note=Yal067w-ap;inference=non-experimental evidence%2C no additional details recorded;product=Putative protein of unknown function%3B identified by gene-trapping%2C microarray-based expression analysis%2C and genome-wide homology searching;protein_id=NP_878038.1;db_xref=SGD:S000028593;db_xref=GI:33438754;db_xref=GeneID:1466426;exon_number=1
NC_001133.7	RefSeq	stop_codon	2705	2707	.	+	0	locus_tag=YAL067W-A;note=Yal067w-ap;inference=non-experimental evidence%2C no additional details recorded;product=Putative protein of unknown function%3B identified by gene-trapping%2C microarray-based expression analysis%2C and genome-wide homology searching;protein_id=NP_878038.1;db_xref=SGD:S000028593;db_xref=GI:33438754;db_xref=GeneID:1466426;exon_number=1
NC_001133.7	RefSeq	gene	7236	9017	.	-	.	ID=NC_001133.7:SEO1;locus_tag=YAL067C;db_xref=GeneID:851230
NC_001133.7	RefSeq	CDS	7239	9017	.	-	0	ID=NC_001133.7:SEO1:unknown_transcript_1;Parent=NC_001133.7:SEO1;locus_tag=YAL067C;note=Seo1p;experiment=experimental evidence%2C no additional details recorded;product=Putative permease%2C member of the allantoate transporter subfamily of the major facilitator superfamily%3B mutation confers resistance to ethionine sulfoxide;protein_id=NP_009333.1;db_xref=SGD:S000000062;db_xref=GI:6319250;db_xref=GeneID:851230;exon_number=1
NC_001133.7	RefSeq	start_codon	9015	9017	.	-	0	ID=NC_001133.7:SEO1:unknown_transcript_1;Parent=NC_001133.7:SEO1;locus_tag=YAL067C;note=Seo1p;experiment=experimental evidence%2C no additional details recorded;product=Putative permease%2C member of the allantoate transporter subfamily of the major facilitator superfamily%3B mutation confers resistance to ethionine sulfoxide;protein_id=NP_009333.1;db_xref=SGD:S000000062;db_xref=GI:6319250;db_xref=GeneID:851230;exon_number=1
NC_001133.7	RefSeq	stop_codon	7236	7238	.	-	0	ID=NC_001133.7:SEO1:unknown_transcript_1;Parent=NC_001133.7:SEO1;locus_tag=YAL067C;note=Seo1p;experiment=experimental evidence%2C no additional details recorded;product=Putative permease%2C member of the allantoate transporter subfamily of the major facilitator superfamily%3B mutation confers resistance to ethionine sulfoxide;protein_id=NP_009333.1;db_xref=SGD:S000000062;db_xref=GI:6319250;db_xref=GeneID:851230;exon_number=1
NC_001133.7	RefSeq	rep_origin	7998	8548	.	+	.	note=ARS103;inference=non-experimental evidence%2C no additional details recorded;db_xref=SGD:S000121253
";

        private static readonly string _multipleSeqGffFileExpectedOutput =
@"##gff-version 2
##source-version NCBI C++
##date " + _today + @"
##type DNA NC_001133.7
##DNA NC_001133.7
##acggctcggattggcgctggatgatagatcagacgac
##acggctcggattggcgctggatgatagatcagacgac
##end-DNA
##RNA NC_001143.7
##guuaacu
##end-RNA
##sequence-region NC_001133.7 1000 1000000
NC_001133.7	RefSeq	source	1	230208	.	+	.	organism=Saccharomyces cerevisiae;mol_type=genomic DNA;strain=S288C;db_xref=taxon:4932;chromosome=I
NC_001133.7	RefSeq	repeat_region	1	801	.	-	.	ID=NC_001133.7:TEL01L:unknown_transcript_1;Parent=NC_001133.7:TEL01L;note=Telomeric region on the left arm of Chromosome I%3B composed of an X element core sequence%2C X element combinatorial repeats%2C and a short terminal stretch of telomeric repeats;inference=non-experimental evidence%2C no additional details recorded;rpt_family=Telomeric Region;db_xref=SGD:S000028862
NC_001133.7	RefSeq	repeat_region	1	62	.	-	.	ID=NC_001133.7:TEL01L-TR:unknown_transcript_1;Parent=NC_001133.7:TEL01L-TR;note=Terminal stretch of telomeric repeats on the left arm of Chromosome I;inference=non-experimental evidence%2C no additional details recorded;rpt_family=Telomeric Repeat;db_xref=SGD:S000028864
NC_001133.7	RefSeq	repeat_region	63	336	.	-	.	ID=NC_001133.7:TEL01L-XR:unknown_transcript_1;Parent=NC_001133.7:TEL01L-XR;note=Telomeric X element combinatorial Repeat region on the left arm of Chromosome I%3B contains repeats of the D%2C C%2C B and A types%2C as well as Tbf1p binding sites%3B formerly called SubTelomeric Repeats;inference=non-experimental evidence%2C no additional details recorded;rpt_family=X element Combinatorial Repeats;db_xref=SGD:S000028866
NC_001133.7	RefSeq	repeat_region	337	801	.	-	.	ID=NC_001133.7:TEL01L-XC:unknown_transcript_1;Parent=NC_001133.7:TEL01L-XC;note=Telomeric X element Core sequence on the left arm of Chromosome I%3B contains an ARS consensus sequence%2C an Abf1p binding site consensus sequence and two small overlapping ORFs %28YAL068W-A and YAL069W%29;inference=non-experimental evidence%2C no additional details recorded;rpt_family=X element Core sequence;db_xref=SGD:S000028865
NC_001133.7	RefSeq	rep_origin	650	1791	.	+	.	note=ARS102;inference=non-experimental evidence%2C no additional details recorded;db_xref=SGD:S000121252
NC_001133.7	RefSeq	gene	1807	2169	.	-	.	ID=NC_001133.7:PAU8;locus_tag=YAL068C;db_xref=GeneID:851229
NC_001133.7	RefSeq	CDS	1810	2169	.	-	0	ID=NC_001133.7:PAU8:unknown_transcript_1;Parent=NC_001133.7:PAU8;locus_tag=YAL068C;experiment=experimental evidence%2C no additional details recorded;product=Pau8p;protein_id=NP_009332.1;db_xref=SGD:S000002142;db_xref=GI:6319249;db_xref=GeneID:851229;exon_number=1
NC_001133.7	RefSeq	start_codon	2167	2169	.	-	0	ID=NC_001133.7:PAU8:unknown_transcript_1;Parent=NC_001133.7:PAU8;locus_tag=YAL068C;experiment=experimental evidence%2C no additional details recorded;product=Pau8p;protein_id=NP_009332.1;db_xref=SGD:S000002142;db_xref=GI:6319249;db_xref=GeneID:851229;exon_number=1
NC_001133.7	RefSeq	stop_codon	1807	1809	.	-	0	ID=NC_001133.7:PAU8:unknown_transcript_1;Parent=NC_001133.7:PAU8;locus_tag=YAL068C;experiment=experimental evidence%2C no additional details recorded;product=Pau8p;protein_id=NP_009332.1;db_xref=SGD:S000002142;db_xref=GI:6319249;db_xref=GeneID:851229;exon_number=1
NC_001133.7	RefSeq	gene	2480	2707	.	+	.	locus_tag=YAL067W-A;db_xref=GeneID:1466426
NC_001133.7	RefSeq	CDS	2480	2704	.	+	0	locus_tag=YAL067W-A;note=Yal067w-ap;inference=non-experimental evidence%2C no additional details recorded;product=Putative protein of unknown function%3B identified by gene-trapping%2C microarray-based expression analysis%2C and genome-wide homology searching;protein_id=NP_878038.1;db_xref=SGD:S000028593;db_xref=GI:33438754;db_xref=GeneID:1466426;exon_number=1
NC_001133.7	RefSeq	start_codon	2480	2482	.	+	0	locus_tag=YAL067W-A;note=Yal067w-ap;inference=non-experimental evidence%2C no additional details recorded;product=Putative protein of unknown function%3B identified by gene-trapping%2C microarray-based expression analysis%2C and genome-wide homology searching;protein_id=NP_878038.1;db_xref=SGD:S000028593;db_xref=GI:33438754;db_xref=GeneID:1466426;exon_number=1
NC_001133.7	RefSeq	stop_codon	2705	2707	.	+	0	locus_tag=YAL067W-A;note=Yal067w-ap;inference=non-experimental evidence%2C no additional details recorded;product=Putative protein of unknown function%3B identified by gene-trapping%2C microarray-based expression analysis%2C and genome-wide homology searching;protein_id=NP_878038.1;db_xref=SGD:S000028593;db_xref=GI:33438754;db_xref=GeneID:1466426;exon_number=1
NC_001133.7	RefSeq	gene	7236	9017	.	-	.	ID=NC_001133.7:SEO1;locus_tag=YAL067C;db_xref=GeneID:851230
NC_001133.7	RefSeq	CDS	7239	9017	.	-	0	ID=NC_001133.7:SEO1:unknown_transcript_1;Parent=NC_001133.7:SEO1;locus_tag=YAL067C;note=Seo1p;experiment=experimental evidence%2C no additional details recorded;product=Putative permease%2C member of the allantoate transporter subfamily of the major facilitator superfamily%3B mutation confers resistance to ethionine sulfoxide;protein_id=NP_009333.1;db_xref=SGD:S000000062;db_xref=GI:6319250;db_xref=GeneID:851230;exon_number=1
NC_001133.7	RefSeq	start_codon	9015	9017	.	-	0	ID=NC_001133.7:SEO1:unknown_transcript_1;Parent=NC_001133.7:SEO1;locus_tag=YAL067C;note=Seo1p;experiment=experimental evidence%2C no additional details recorded;product=Putative permease%2C member of the allantoate transporter subfamily of the major facilitator superfamily%3B mutation confers resistance to ethionine sulfoxide;protein_id=NP_009333.1;db_xref=SGD:S000000062;db_xref=GI:6319250;db_xref=GeneID:851230;exon_number=1
NC_001133.7	RefSeq	stop_codon	7236	7238	.	-	0	ID=NC_001133.7:SEO1:unknown_transcript_1;Parent=NC_001133.7:SEO1;locus_tag=YAL067C;note=Seo1p;experiment=experimental evidence%2C no additional details recorded;product=Putative permease%2C member of the allantoate transporter subfamily of the major facilitator superfamily%3B mutation confers resistance to ethionine sulfoxide;protein_id=NP_009333.1;db_xref=SGD:S000000062;db_xref=GI:6319250;db_xref=GeneID:851230;exon_number=1
NC_001133.7	RefSeq	rep_origin	7998	8548	.	+	.	note=ARS103;inference=non-experimental evidence%2C no additional details recorded;db_xref=SGD:S000121253
NC_001133.7	RefSeq	gene	11566	11952	.	-	.	locus_tag=YAL065C;db_xref=GeneID:851232
NC_001133.7	RefSeq	CDS	11569	11952	.	-	0	locus_tag=YAL065C;note=Yal065cp;inference=non-experimental evidence%2C no additional details recorded;product=Putative protein of unknown function%3B has homology to FLO1%3B possible pseudogene;protein_id=NP_009335.1;db_xref=SGD:S000001817;db_xref=GI:6319252;db_xref=GeneID:851232;exon_number=1
NC_001133.7	RefSeq	start_codon	11950	11952	.	-	0	locus_tag=YAL065C;note=Yal065cp;inference=non-experimental evidence%2C no additional details recorded;product=Putative protein of unknown function%3B has homology to FLO1%3B possible pseudogene;protein_id=NP_009335.1;db_xref=SGD:S000001817;db_xref=GI:6319252;db_xref=GeneID:851232;exon_number=1
NC_001133.7	RefSeq	stop_codon	11566	11568	.	-	0	locus_tag=YAL065C;note=Yal065cp;inference=non-experimental evidence%2C no additional details recorded;product=Putative protein of unknown function%3B has homology to FLO1%3B possible pseudogene;protein_id=NP_009335.1;db_xref=SGD:S000001817;db_xref=GI:6319252;db_xref=GeneID:851232;exon_number=1
NC_001133.7	RefSeq	gene	12047	12427	.	+	.	locus_tag=YAL064W-B;db_xref=GeneID:851233
NC_001133.7	RefSeq	CDS	12047	12424	.	+	0	locus_tag=YAL064W-B;note=Yal064w-bp;inference=non-experimental evidence%2C no additional details recorded;product=Fungal-specific protein of unknown function;protein_id=NP_009336.1;db_xref=SGD:S000002141;db_xref=GI:6319253;db_xref=GeneID:851233;exon_number=1
NC_001133.7	RefSeq	start_codon	12047	12049	.	+	0	locus_tag=YAL064W-B;note=Yal064w-bp;inference=non-experimental evidence%2C no additional details recorded;product=Fungal-specific protein of unknown function;protein_id=NP_009336.1;db_xref=SGD:S000002141;db_xref=GI:6319253;db_xref=GeneID:851233;exon_number=1
NC_001133.7	RefSeq	stop_codon	12425	12427	.	+	0	locus_tag=YAL064W-B;note=Yal064w-bp;inference=non-experimental evidence%2C no additional details recorded;product=Fungal-specific protein of unknown function;protein_id=NP_009336.1;db_xref=SGD:S000002141;db_xref=GI:6319253;db_xref=GeneID:851233;exon_number=1
NC_001133.7	RefSeq	gene	13364	13744	.	-	.	locus_tag=YAL064C-A;gene_synonym=YAL065C-A;db_xref=GeneID:851234
NC_001133.7	RefSeq	CDS	13367	13744	.	-	0	locus_tag=YAL064C-A;gene_synonym=YAL065C-A;note=Yal064c-ap;inference=non-experimental evidence%2C no additional details recorded;product=Putative protein of unknown function%3B YAL064C-A is not an essential gene;protein_id=NP_058136.1;db_xref=SGD:S000002140;db_xref=GI:7839146;db_xref=GeneID:851234;exon_number=1
NC_001133.7	RefSeq	start_codon	13742	13744	.	-	0	locus_tag=YAL064C-A;gene_synonym=YAL065C-A;note=Yal064c-ap;inference=non-experimental evidence%2C no additional details recorded;product=Putative protein of unknown function%3B YAL064C-A is not an essential gene;protein_id=NP_058136.1;db_xref=SGD:S000002140;db_xref=GI:7839146;db_xref=GeneID:851234;exon_number=1
NC_001133.7	RefSeq	stop_codon	13364	13366	.	-	0	locus_tag=YAL064C-A;gene_synonym=YAL065C-A;note=Yal064c-ap;inference=non-experimental evidence%2C no additional details recorded;product=Putative protein of unknown function%3B YAL064C-A is not an essential gene;protein_id=NP_058136.1;db_xref=SGD:S000002140;db_xref=GI:7839146;db_xref=GeneID:851234;exon_number=1
NC_001133.7	RefSeq	gene	21526	21852	.	+	.	locus_tag=YAL064W;db_xref=GeneID:851235
NC_001133.7	RefSeq	CDS	21526	21849	.	+	0	locus_tag=YAL064W;note=Yal064wp;inference=non-experimental evidence%2C no additional details recorded;product=Protein of unknown function%3B may interact with ribosomes%2C based on co-purification experiments;protein_id=NP_009337.1;db_xref=SGD:S000000060;db_xref=GI:6319254;db_xref=GeneID:851235;exon_number=1
NC_001133.7	RefSeq	start_codon	21526	21528	.	+	0	locus_tag=YAL064W;note=Yal064wp;inference=non-experimental evidence%2C no additional details recorded;product=Protein of unknown function%3B may interact with ribosomes%2C based on co-purification experiments;protein_id=NP_009337.1;db_xref=SGD:S000000060;db_xref=GI:6319254;db_xref=GeneID:851235;exon_number=1
NC_001133.7	RefSeq	stop_codon	21850	21852	.	+	0	locus_tag=YAL064W;note=Yal064wp;inference=non-experimental evidence%2C no additional details recorded;product=Protein of unknown function%3B may interact with ribosomes%2C based on co-purification experiments;protein_id=NP_009337.1;db_xref=SGD:S000000060;db_xref=GI:6319254;db_xref=GeneID:851235;exon_number=1
NC_001133.7	RefSeq	LTR	22232	22554	.	+	.	note=YALWDELTA1;experiment=experimental evidence%2C no additional details recorded;db_xref=SGD:S000006787
NC_001133.7	RefSeq	gene	22397	22687	.	-	.	locus_tag=YAL063C-A;db_xref=GeneID:1466427
NC_001133.7	RefSeq	CDS	22400	22687	.	-	0	locus_tag=YAL063C-A;note=Yal063c-ap;inference=non-experimental evidence%2C no additional details recorded;product=Putative protein of unknown function%3B identified by expression profiling and mass spectrometry;protein_id=NP_878039.1;db_xref=SGD:S000028813;db_xref=GI:33438755;db_xref=GeneID:1466427;exon_number=1
NC_001133.7	RefSeq	start_codon	22685	22687	.	-	0	locus_tag=YAL063C-A;note=Yal063c-ap;inference=non-experimental evidence%2C no additional details recorded;product=Putative protein of unknown function%3B identified by expression profiling and mass spectrometry;protein_id=NP_878039.1;db_xref=SGD:S000028813;db_xref=GI:33438755;db_xref=GeneID:1466427;exon_number=1
NC_001133.7	RefSeq	stop_codon	22397	22399	.	-	0	locus_tag=YAL063C-A;note=Yal063c-ap;inference=non-experimental evidence%2C no additional details recorded;product=Putative protein of unknown function%3B identified by expression profiling and mass spectrometry;protein_id=NP_878039.1;db_xref=SGD:S000028813;db_xref=GI:33438755;db_xref=GeneID:1466427;exon_number=1
NC_001133.7	RefSeq	gene	24001	27969	.	-	.	ID=NC_001133.7:FLO9;locus_tag=YAL063C;db_xref=GeneID:851236
NC_001133.7	RefSeq	CDS	24004	27969	.	-	0	ID=NC_001133.7:FLO9:unknown_transcript_1;Parent=NC_001133.7:FLO9;locus_tag=YAL063C;note=Lectin-like protein with similarity to Flo1p%2C thought to be expressed and involved in flocculation;experiment=experimental evidence%2C no additional details recorded;product=Flo9p;protein_id=NP_009338.1;db_xref=SGD:S000000059;db_xref=GI:6319255;db_xref=GeneID:851236;exon_number=1
NC_001133.7	RefSeq	start_codon	27967	27969	.	-	0	ID=NC_001133.7:FLO9:unknown_transcript_1;Parent=NC_001133.7:FLO9;locus_tag=YAL063C;note=Lectin-like protein with similarity to Flo1p%2C thought to be expressed and involved in flocculation;experiment=experimental evidence%2C no additional details recorded;product=Flo9p;protein_id=NP_009338.1;db_xref=SGD:S000000059;db_xref=GI:6319255;db_xref=GeneID:851236;exon_number=1
NC_001133.7	RefSeq	stop_codon	24001	24003	.	-	0	ID=NC_001133.7:FLO9:unknown_transcript_1;Parent=NC_001133.7:FLO9;locus_tag=YAL063C;note=Lectin-like protein with similarity to Flo1p%2C thought to be expressed and involved in flocculation;experiment=experimental evidence%2C no additional details recorded;product=Flo9p;protein_id=NP_009338.1;db_xref=SGD:S000000059;db_xref=GI:6319255;db_xref=GeneID:851236;exon_number=1
NC_001133.7	RefSeq	rep_origin	30947	31184	.	+	.	note=ARS104;inference=non-experimental evidence%2C no additional details recorded;db_xref=SGD:S000118317
NC_001133.7	RefSeq	gene	31568	32941	.	+	.	ID=NC_001133.7:GDH3;locus_tag=YAL062W;gene_synonym=FUN51;db_xref=GeneID:851237
NC_001133.7	RefSeq	CDS	31568	32938	.	+	0	ID=NC_001133.7:GDH3:unknown_transcript_1;Parent=NC_001133.7:GDH3;locus_tag=YAL062W;gene_synonym=FUN51;EC_number=1.4.1.4;note=NADP%28%2B%29-dependent glutamate dehydrogenase%2C synthesizes glutamate from ammonia and alpha-ketoglutarate%3B rate of alpha-ketoglutarate utilization differs from Gdh1p%3B expression regulated by nitrogen and carbon sources;experiment=experimental evidence%2C no additional details recorded;product=Gdh3p;protein_id=NP_009339.1;db_xref=SGD:S000000058;db_xref=GI:6319256;db_xref=GeneID:851237;exon_number=1
NC_001133.7	RefSeq	start_codon	31568	31570	.	+	0	ID=NC_001133.7:GDH3:unknown_transcript_1;Parent=NC_001133.7:GDH3;locus_tag=YAL062W;gene_synonym=FUN51;EC_number=1.4.1.4;note=NADP%28%2B%29-dependent glutamate dehydrogenase%2C synthesizes glutamate from ammonia and alpha-ketoglutarate%3B rate of alpha-ketoglutarate utilization differs from Gdh1p%3B expression regulated by nitrogen and carbon sources;experiment=experimental evidence%2C no additional details recorded;product=Gdh3p;protein_id=NP_009339.1;db_xref=SGD:S000000058;db_xref=GI:6319256;db_xref=GeneID:851237;exon_number=1
NC_001133.7	RefSeq	stop_codon	32939	32941	.	+	0	ID=NC_001133.7:GDH3:unknown_transcript_1;Parent=NC_001133.7:GDH3;locus_tag=YAL062W;gene_synonym=FUN51;EC_number=1.4.1.4;note=NADP%28%2B%29-dependent glutamate dehydrogenase%2C synthesizes glutamate from ammonia and alpha-ketoglutarate%3B rate of alpha-ketoglutarate utilization differs from Gdh1p%3B expression regulated by nitrogen and carbon sources;experiment=experimental evidence%2C no additional details recorded;product=Gdh3p;protein_id=NP_009339.1;db_xref=SGD:S000000058;db_xref=GI:6319256;db_xref=GeneID:851237;exon_number=1
NC_001133.7	RefSeq	gene	33449	34702	.	+	.	ID=NC_001133.7:BDH2;locus_tag=YAL061W;db_xref=GeneID:851238
NC_001143.7	RefSeq	source	1	666454	.	+	.	organism=Saccharomyces cerevisiae;mol_type=genomic DNA;strain=S288C;db_xref=taxon:4932;chromosome=XI
NC_001143.7	RefSeq	repeat_region	1	808	.	-	.	ID=NC_001143.7:TEL11L:unknown_transcript_1;Parent=NC_001143.7:TEL11L;note=Telomeric region on the left arm of Chromosome XI%2C composed of an X element core sequence%2C X element combinatorial repeats%2C and a short terminal stretch of telomeric repeats;inference=non-experimental evidence%2C no additional details recorded;rpt_family=Telomeric Region;db_xref=SGD:S000028906
NC_001143.7	RefSeq	repeat_region	1	67	.	-	.	ID=NC_001143.7:TEL11L-TR:unknown_transcript_1;Parent=NC_001143.7:TEL11L-TR;note=Terminal stretch of telomeric repeats on the left arm of Chromosome XI;inference=non-experimental evidence%2C no additional details recorded;rpt_family=Telomeric Repeat;db_xref=SGD:S000028907
NC_001143.7	RefSeq	repeat_region	68	346	.	-	.	ID=NC_001143.7:TEL11L-XR:unknown_transcript_1;Parent=NC_001143.7:TEL11L-XR;note=Telomeric X element combinatorial Repeat region on the left arm of Chromosome XI%3B contains repeats of the D%2C C%2C B and A types%2C as well as Tbf1p binding sites%3B formerly called SubTelomeric Repeats;inference=non-experimental evidence%2C no additional details recorded;rpt_family=X element Combinatorial Repeats;db_xref=SGD:S000028909
NC_001143.7	RefSeq	repeat_region	347	808	.	-	.	ID=NC_001143.7:TEL11L-XC:unknown_transcript_1;Parent=NC_001143.7:TEL11L-XC;note=Telomeric X element Core sequence on the left arm of Chromosome XI%3B contains an ARS consensus sequence%2C an Abf1p binding site consensus sequence%2C and ORF YKL225W;inference=non-experimental evidence%2C no additional details recorded;rpt_family=X element Core sequence;db_xref=SGD:S000028908
NC_001143.7	RefSeq	gene	1811	2182	.	-	.	ID=NC_001143.7:PAU16;locus_tag=YKL224C;db_xref=GeneID:853656
NC_001143.7	RefSeq	CDS	1814	2182	.	-	0	ID=NC_001143.7:PAU16:unknown_transcript_1;Parent=NC_001143.7:PAU16;locus_tag=YKL224C;note=Putative protein of unknown function;experiment=experimental evidence%2C no additional details recorded;product=Pau16p;protein_id=NP_012698.1;db_xref=SGD:S000001707;db_xref=GI:6322625;db_xref=GeneID:853656;exon_number=1
NC_001143.7	RefSeq	start_codon	2180	2182	.	-	0	ID=NC_001143.7:PAU16:unknown_transcript_1;Parent=NC_001143.7:PAU16;locus_tag=YKL224C;note=Putative protein of unknown function;experiment=experimental evidence%2C no additional details recorded;product=Pau16p;protein_id=NP_012698.1;db_xref=SGD:S000001707;db_xref=GI:6322625;db_xref=GeneID:853656;exon_number=1
NC_001143.7	RefSeq	stop_codon	1811	1813	.	-	0	ID=NC_001143.7:PAU16:unknown_transcript_1;Parent=NC_001143.7:PAU16;locus_tag=YKL224C;note=Putative protein of unknown function;experiment=experimental evidence%2C no additional details recorded;product=Pau16p;protein_id=NP_012698.1;db_xref=SGD:S000001707;db_xref=GI:6322625;db_xref=GeneID:853656;exon_number=1
NC_001143.7	RefSeq	gene	3504	5621	.	-	.	locus_tag=YKL222C;db_xref=GeneID:853658
NC_001143.7	RefSeq	CDS	3507	5621	.	-	0	locus_tag=YKL222C;note=Ykl222cp;inference=non-experimental evidence%2C no additional details recorded;product=Protein of unknown function that may interact with ribosomes%2C based on co-purification experiments%3B similar to transcriptional regulators from the zinc cluster %28binuclear cluster%29 family%3B null mutant is sensitive to caffeine;protein_id=NP_012700.1;db_xref=SGD:S000001705;db_xref=GI:6322627;db_xref=GeneID:853658;exon_number=1
NC_001143.7	RefSeq	start_codon	5619	5621	.	-	0	locus_tag=YKL222C;note=Ykl222cp;inference=non-experimental evidence%2C no additional details recorded;product=Protein of unknown function that may interact with ribosomes%2C based on co-purification experiments%3B similar to transcriptional regulators from the zinc cluster %28binuclear cluster%29 family%3B null mutant is sensitive to caffeine;protein_id=NP_012700.1;db_xref=SGD:S000001705;db_xref=GI:6322627;db_xref=GeneID:853658;exon_number=1
NC_001143.7	RefSeq	stop_codon	3504	3506	.	-	0	locus_tag=YKL222C;note=Ykl222cp;inference=non-experimental evidence%2C no additional details recorded;product=Protein of unknown function that may interact with ribosomes%2C based on co-purification experiments%3B similar to transcriptional regulators from the zinc cluster %28binuclear cluster%29 family%3B null mutant is sensitive to caffeine;protein_id=NP_012700.1;db_xref=SGD:S000001705;db_xref=GI:6322627;db_xref=GeneID:853658;exon_number=1
NC_001143.7	RefSeq	gene	6108	7529	.	+	.	ID=NC_001143.7:MCH2;locus_tag=YKL221W;db_xref=GeneID:853659
NC_001143.7	RefSeq	CDS	6108	7526	.	+	0	ID=NC_001143.7:MCH2:unknown_transcript_1;Parent=NC_001143.7:MCH2;locus_tag=YKL221W;note=Protein with similarity to mammalian monocarboxylate permeases%2C which are involved in transport of monocarboxylic acids across the plasma membrane%3B mutant is not deficient in monocarboxylate transport;experiment=experimental evidence%2C no additional details recorded;product=Mch2p;protein_id=NP_012701.1;db_xref=SGD:S000001704;db_xref=GI:6322628;db_xref=GeneID:853659;exon_number=1
NC_001143.7	RefSeq	start_codon	6108	6110	.	+	0	ID=NC_001143.7:MCH2:unknown_transcript_1;Parent=NC_001143.7:MCH2;locus_tag=YKL221W;note=Protein with similarity to mammalian monocarboxylate permeases%2C which are involved in transport of monocarboxylic acids across the plasma membrane%3B mutant is not deficient in monocarboxylate transport;experiment=experimental evidence%2C no additional details recorded;product=Mch2p;protein_id=NP_012701.1;db_xref=SGD:S000001704;db_xref=GI:6322628;db_xref=GeneID:853659;exon_number=1
NC_001143.7	RefSeq	stop_codon	7527	7529	.	+	0	ID=NC_001143.7:MCH2:unknown_transcript_1;Parent=NC_001143.7:MCH2;locus_tag=YKL221W;note=Protein with similarity to mammalian monocarboxylate permeases%2C which are involved in transport of monocarboxylic acids across the plasma membrane%3B mutant is not deficient in monocarboxylate transport;experiment=experimental evidence%2C no additional details recorded;product=Mch2p;protein_id=NP_012701.1;db_xref=SGD:S000001704;db_xref=GI:6322628;db_xref=GeneID:853659;exon_number=1
NC_001143.7	RefSeq	gene	9092	11227	.	-	.	ID=NC_001143.7:FRE2;locus_tag=YKL220C;db_xref=GeneID:853660
NC_001143.7	RefSeq	CDS	9095	11227	.	-	0	ID=NC_001143.7:FRE2:unknown_transcript_1;Parent=NC_001143.7:FRE2;locus_tag=YKL220C;note=Fre2p;experiment=experimental evidence%2C no additional details recorded;product=Ferric reductase and cupric reductase%2C reduces siderophore-bound iron and oxidized copper prior to uptake by transporters%3B expression induced by low iron levels but not by low copper levels;protein_id=NP_012702.1;db_xref=SGD:S000001703;db_xref=GI:6322629;db_xref=GeneID:853660;exon_number=1
NC_001143.7	RefSeq	start_codon	11225	11227	.	-	0	ID=NC_001143.7:FRE2:unknown_transcript_1;Parent=NC_001143.7:FRE2;locus_tag=YKL220C;note=Fre2p;experiment=experimental evidence%2C no additional details recorded;product=Ferric reductase and cupric reductase%2C reduces siderophore-bound iron and oxidized copper prior to uptake by transporters%3B expression induced by low iron levels but not by low copper levels;protein_id=NP_012702.1;db_xref=SGD:S000001703;db_xref=GI:6322629;db_xref=GeneID:853660;exon_number=1
NC_001143.7	RefSeq	stop_codon	9092	9094	.	-	0	ID=NC_001143.7:FRE2:unknown_transcript_1;Parent=NC_001143.7:FRE2;locus_tag=YKL220C;note=Fre2p;experiment=experimental evidence%2C no additional details recorded;product=Ferric reductase and cupric reductase%2C reduces siderophore-bound iron and oxidized copper prior to uptake by transporters%3B expression induced by low iron levels but not by low copper levels;protein_id=NP_012702.1;db_xref=SGD:S000001703;db_xref=GI:6322629;db_xref=GeneID:853660;exon_number=1
NC_001143.7	RefSeq	gene	14485	15708	.	+	.	ID=NC_001143.7:COS9;locus_tag=YKL219W;db_xref=GeneID:853661
NC_001143.7	RefSeq	CDS	14485	15705	.	+	0	ID=NC_001143.7:COS9:unknown_transcript_1;Parent=NC_001143.7:COS9;locus_tag=YKL219W;note=Protein of unknown function%2C member of the DUP380 subfamily of conserved%2C often subtelomerically-encoded proteins;experiment=experimental evidence%2C no additional details recorded;product=Cos9p;protein_id=NP_012703.1;db_xref=SGD:S000001702;db_xref=GI:6322630;db_xref=GeneID:853661;exon_number=1
NC_001143.7	RefSeq	start_codon	14485	14487	.	+	0	ID=NC_001143.7:COS9:unknown_transcript_1;Parent=NC_001143.7:COS9;locus_tag=YKL219W;note=Protein of unknown function%2C member of the DUP380 subfamily of conserved%2C often subtelomerically-encoded proteins;experiment=experimental evidence%2C no additional details recorded;product=Cos9p;protein_id=NP_012703.1;db_xref=SGD:S000001702;db_xref=GI:6322630;db_xref=GeneID:853661;exon_number=1
NC_001143.7	RefSeq	stop_codon	15706	15708	.	+	0	ID=NC_001143.7:COS9:unknown_transcript_1;Parent=NC_001143.7:COS9;locus_tag=YKL219W;note=Protein of unknown function%2C member of the DUP380 subfamily of conserved%2C often subtelomerically-encoded proteins;experiment=experimental evidence%2C no additional details recorded;product=Cos9p;protein_id=NP_012703.1;db_xref=SGD:S000001702;db_xref=GI:6322630;db_xref=GeneID:853661;exon_number=1
NC_001143.7	RefSeq	gene	17359	18339	.	-	.	ID=NC_001143.7:SRY1;locus_tag=YKL218C;db_xref=GeneID:853662
NC_001143.7	RefSeq	CDS	17362	18339	.	-	0	ID=NC_001143.7:SRY1:unknown_transcript_1;Parent=NC_001143.7:SRY1;locus_tag=YKL218C;note=Sry1p;experiment=experimental evidence%2C no additional details recorded;product=3-hydroxyaspartate dehydratase%2C deaminates L-threo-3-hydroxyaspartate to form oxaloacetate and ammonia%3B required for survival in the presence of hydroxyaspartate;protein_id=NP_012704.1;db_xref=SGD:S000001701;db_xref=GI:6322631;db_xref=GeneID:853662;exon_number=1
NC_001143.7	RefSeq	start_codon	18337	18339	.	-	0	ID=NC_001143.7:SRY1:unknown_transcript_1;Parent=NC_001143.7:SRY1;locus_tag=YKL218C;note=Sry1p;experiment=experimental evidence%2C no additional details recorded;product=3-hydroxyaspartate dehydratase%2C deaminates L-threo-3-hydroxyaspartate to form oxaloacetate and ammonia%3B required for survival in the presence of hydroxyaspartate;protein_id=NP_012704.1;db_xref=SGD:S000001701;db_xref=GI:6322631;db_xref=GeneID:853662;exon_number=1
NC_001143.7	RefSeq	stop_codon	17359	17361	.	-	0	ID=NC_001143.7:SRY1:unknown_transcript_1;Parent=NC_001143.7:SRY1;locus_tag=YKL218C;note=Sry1p;experiment=experimental evidence%2C no additional details recorded;product=3-hydroxyaspartate dehydratase%2C deaminates L-threo-3-hydroxyaspartate to form oxaloacetate and ammonia%3B required for survival in the presence of hydroxyaspartate;protein_id=NP_012704.1;db_xref=SGD:S000001701;db_xref=GI:6322631;db_xref=GeneID:853662;exon_number=1
NC_001143.7	RefSeq	gene	22234	24084	.	+	.	ID=NC_001143.7:JEN1;locus_tag=YKL217W;db_xref=GeneID:853663
NC_001143.7	RefSeq	CDS	22234	24081	.	+	0	ID=NC_001143.7:JEN1:unknown_transcript_1;Parent=NC_001143.7:JEN1;locus_tag=YKL217W;note=Lactate transporter%2C required for uptake of lactate and pyruvate%3B phosphorylated%3B expression is derepressed by transcriptional activator Cat8p during respiratory growth%2C and repressed in the presence of glucose%2C fructose%2C and mannose;experiment=experimental evidence%2C no additional details recorded;product=Jen1p;protein_id=NP_012705.1;db_xref=SGD:S000001700;db_xref=GI:6322632;db_xref=GeneID:853663;exon_number=1
NC_001143.7	RefSeq	start_codon	22234	22236	.	+	0	ID=NC_001143.7:JEN1:unknown_transcript_1;Parent=NC_001143.7:JEN1;locus_tag=YKL217W;note=Lactate transporter%2C required for uptake of lactate and pyruvate%3B phosphorylated%3B expression is derepressed by transcriptional activator Cat8p during respiratory growth%2C and repressed in the presence of glucose%2C fructose%2C and mannose;experiment=experimental evidence%2C no additional details recorded;product=Jen1p;protein_id=NP_012705.1;db_xref=SGD:S000001700;db_xref=GI:6322632;db_xref=GeneID:853663;exon_number=1
NC_001143.7	RefSeq	stop_codon	24082	24084	.	+	0	ID=NC_001143.7:JEN1:unknown_transcript_1;Parent=NC_001143.7:JEN1;locus_tag=YKL217W;note=Lactate transporter%2C required for uptake of lactate and pyruvate%3B phosphorylated%3B expression is derepressed by transcriptional activator Cat8p during respiratory growth%2C and repressed in the presence of glucose%2C fructose%2C and mannose;experiment=experimental evidence%2C no additional details recorded;product=Jen1p;protein_id=NP_012705.1;db_xref=SGD:S000001700;db_xref=GI:6322632;db_xref=GeneID:853663;exon_number=1
NC_001143.7	RefSeq	gene	25216	26160	.	+	.	ID=NC_001143.7:URA1;locus_tag=YKL216W;db_xref=GeneID:853664
NC_001143.7	RefSeq	CDS	25216	26157	.	+	0	ID=NC_001143.7:URA1:unknown_transcript_1;Parent=NC_001143.7:URA1;locus_tag=YKL216W;EC_number=1.3.3.1;note=Ura1p;experiment=experimental evidence%2C no additional details recorded;product=Dihydroorotate dehydrogenase%2C catalyzes the fourth enzymatic step in the de novo biosynthesis of pyrimidines%2C converting dihydroorotic acid into orotic acid;protein_id=NP_012706.1;db_xref=SGD:S000001699;db_xref=GI:6322633;db_xref=GeneID:853664;exon_number=1
NC_001143.7	RefSeq	start_codon	25216	25218	.	+	0	ID=NC_001143.7:URA1:unknown_transcript_1;Parent=NC_001143.7:URA1;locus_tag=YKL216W;EC_number=1.3.3.1;note=Ura1p;experiment=experimental evidence%2C no additional details recorded;product=Dihydroorotate dehydrogenase%2C catalyzes the fourth enzymatic step in the de novo biosynthesis of pyrimidines%2C converting dihydroorotic acid into orotic acid;protein_id=NP_012706.1;db_xref=SGD:S000001699;db_xref=GI:6322633;db_xref=GeneID:853664;exon_number=1
NC_001143.7	RefSeq	stop_codon	26158	26160	.	+	0	ID=NC_001143.7:URA1:unknown_transcript_1;Parent=NC_001143.7:URA1;locus_tag=YKL216W;EC_number=1.3.3.1;note=Ura1p;experiment=experimental evidence%2C no additional details recorded;product=Dihydroorotate dehydrogenase%2C catalyzes the fourth enzymatic step in the de novo biosynthesis of pyrimidines%2C converting dihydroorotic acid into orotic acid;protein_id=NP_012706.1;db_xref=SGD:S000001699;db_xref=GI:6322633;db_xref=GeneID:853664;exon_number=1
NC_001143.7	RefSeq	gene	26828	30688	.	-	.	locus_tag=YKL215C;db_xref=GeneID:853665
NC_001143.7	RefSeq	CDS	26831	30688	.	-	0	locus_tag=YKL215C;note=Ykl215cp;inference=non-experimental evidence%2C no additional details recorded;product=Putative protein of unknown function%3B green fluorescent protein %28GFP%29-fusion protein localizes to the cytoplasm;protein_id=NP_012707.1;db_xref=SGD:S000001698;db_xref=GI:6322634;db_xref=GeneID:853665;exon_number=1
NC_001143.7	RefSeq	start_codon	30686	30688	.	-	0	locus_tag=YKL215C;note=Ykl215cp;inference=non-experimental evidence%2C no additional details recorded;product=Putative protein of unknown function%3B green fluorescent protein %28GFP%29-fusion protein localizes to the cytoplasm;protein_id=NP_012707.1;db_xref=SGD:S000001698;db_xref=GI:6322634;db_xref=GeneID:853665;exon_number=1
NC_001143.7	RefSeq	stop_codon	26828	26830	.	-	0	locus_tag=YKL215C;note=Ykl215cp;inference=non-experimental evidence%2C no additional details recorded;product=Putative protein of unknown function%3B green fluorescent protein %28GFP%29-fusion protein localizes to the cytoplasm;protein_id=NP_012707.1;db_xref=SGD:S000001698;db_xref=GI:6322634;db_xref=GeneID:853665;exon_number=1
NC_001143.7	RefSeq	gene	31083	31694	.	-	.	ID=NC_001143.7:YRA2;locus_tag=YKL214C;db_xref=GeneID:853666
NC_001143.7	RefSeq	CDS	31086	31694	.	-	0	ID=NC_001143.7:YRA2:unknown_transcript_1;Parent=NC_001143.7:YRA2;locus_tag=YKL214C;note=Yra2p;experiment=experimental evidence%2C no additional details recorded;product=Member of the REF %28RNA and export factor binding proteins%29 family%3B when overexpressed%2C can substitute for the function of Yra1p in export of poly%28A%29%2B mRNA from the nucleus;protein_id=NP_012708.1;db_xref=SGD:S000001697;db_xref=GI:6322635;db_xref=GeneID:853666;exon_number=1
NC_001143.7	RefSeq	start_codon	31692	31694	.	-	0	ID=NC_001143.7:YRA2:unknown_transcript_1;Parent=NC_001143.7:YRA2;locus_tag=YKL214C;note=Yra2p;experiment=experimental evidence%2C no additional details recorded;product=Member of the REF %28RNA and export factor binding proteins%29 family%3B when overexpressed%2C can substitute for the function of Yra1p in export of poly%28A%29%2B mRNA from the nucleus;protein_id=NP_012708.1;db_xref=SGD:S000001697;db_xref=GI:6322635;db_xref=GeneID:853666;exon_number=1
NC_001143.7	RefSeq	stop_codon	31083	31085	.	-	0	ID=NC_001143.7:YRA2:unknown_transcript_1;Parent=NC_001143.7:YRA2;locus_tag=YKL214C;note=Yra2p;experiment=experimental evidence%2C no additional details recorded;product=Member of the REF %28RNA and export factor binding proteins%29 family%3B when overexpressed%2C can substitute for the function of Yra1p in export of poly%28A%29%2B mRNA from the nucleus;protein_id=NP_012708.1;db_xref=SGD:S000001697;db_xref=GI:6322635;db_xref=GeneID:853666;exon_number=1
NC_001143.7	RefSeq	gene	31961	34108	.	-	.	ID=NC_001143.7:DOA1;locus_tag=YKL213C;gene_synonym=ZZZ4;gene_synonym=UFD3;db_xref=GeneID:853667
NC_001143.7	RefSeq	CDS	31964	34108	.	-	0	ID=NC_001143.7:DOA1:unknown_transcript_1;Parent=NC_001143.7:DOA1;locus_tag=YKL213C;gene_synonym=ZZZ4;gene_synonym=UFD3;note=WD repeat protein required for ubiquitin-mediated protein degradation%2C forms complex with Cdc48p%2C plays a role in controlling cellular ubiquitin concentration%3B also promotes efficient NHEJ in postdiauxic%2Fstationary phase;experiment=experimental evidence%2C no additional details recorded;product=Doa1p;protein_id=NP_012709.1;db_xref=SGD:S000001696;db_xref=GI:6322636;db_xref=GeneID:853667;exon_number=1
NC_001143.7	RefSeq	start_codon	34106	34108	.	-	0	ID=NC_001143.7:DOA1:unknown_transcript_1;Parent=NC_001143.7:DOA1;locus_tag=YKL213C;gene_synonym=ZZZ4;gene_synonym=UFD3;note=WD repeat protein required for ubiquitin-mediated protein degradation%2C forms complex with Cdc48p%2C plays a role in controlling cellular ubiquitin concentration%3B also promotes efficient NHEJ in postdiauxic%2Fstationary phase;experiment=experimental evidence%2C no additional details recorded;product=Doa1p;protein_id=NP_012709.1;db_xref=SGD:S000001696;db_xref=GI:6322636;db_xref=GeneID:853667;exon_number=1
NC_001143.7	RefSeq	stop_codon	31961	31963	.	-	0	ID=NC_001143.7:DOA1:unknown_transcript_1;Parent=NC_001143.7:DOA1;locus_tag=YKL213C;gene_synonym=ZZZ4;gene_synonym=UFD3;note=WD repeat protein required for ubiquitin-mediated protein degradation%2C forms complex with Cdc48p%2C plays a role in controlling cellular ubiquitin concentration%3B also promotes efficient NHEJ in postdiauxic%2Fstationary phase;experiment=experimental evidence%2C no additional details recorded;product=Doa1p;protein_id=NP_012709.1;db_xref=SGD:S000001696;db_xref=GI:6322636;db_xref=GeneID:853667;exon_number=1
NC_001143.7	RefSeq	gene	34544	36415	.	+	.	ID=NC_001143.7:SAC1;locus_tag=YKL212W;gene_synonym=RSD1;db_xref=GeneID:853668
NC_001143.7	RefSeq	CDS	34544	36412	.	+	0	ID=NC_001143.7:SAC1:unknown_transcript_1;Parent=NC_001143.7:SAC1;locus_tag=YKL212W;gene_synonym=RSD1;note=Sac1p;experiment=experimental evidence%2C no additional details recorded;product=Phosphatidylinositol %28PI%29 phosphatase%2C involved in hydrolysis of PI 3-phosphate%2C PI 4-phosphate and PI 3%2C5-bisphosphate to PI%3B membrane protein localizes to ER and Golgi%3B involved in protein trafficking%2C secretion and inositol metabolism;protein_id=NP_012710.1;db_xref=SGD:S000001695;db_xref=GI:6322637;db_xref=GeneID:853668;exon_number=1
NC_001143.7	RefSeq	start_codon	34544	34546	.	+	0	ID=NC_001143.7:SAC1:unknown_transcript_1;Parent=NC_001143.7:SAC1;locus_tag=YKL212W;gene_synonym=RSD1;note=Sac1p;experiment=experimental evidence%2C no additional details recorded;product=Phosphatidylinositol %28PI%29 phosphatase%2C involved in hydrolysis of PI 3-phosphate%2C PI 4-phosphate and PI 3%2C5-bisphosphate to PI%3B membrane protein localizes to ER and Golgi%3B involved in protein trafficking%2C secretion and inositol metabolism;protein_id=NP_012710.1;db_xref=SGD:S000001695;db_xref=GI:6322637;db_xref=GeneID:853668;exon_number=1
NC_001143.7	RefSeq	stop_codon	36413	36415	.	+	0	ID=NC_001143.7:SAC1:unknown_transcript_1;Parent=NC_001143.7:SAC1;locus_tag=YKL212W;gene_synonym=RSD1;note=Sac1p;experiment=experimental evidence%2C no additional details recorded;product=Phosphatidylinositol %28PI%29 phosphatase%2C involved in hydrolysis of PI 3-phosphate%2C PI 4-phosphate and PI 3%2C5-bisphosphate to PI%3B membrane protein localizes to ER and Golgi%3B involved in protein trafficking%2C secretion and inositol metabolism;protein_id=NP_012710.1;db_xref=SGD:S000001695;db_xref=GI:6322637;db_xref=GeneID:853668;exon_number=1
NC_001143.7	RefSeq	gene	36700	38154	.	-	.	ID=NC_001143.7:TRP3;locus_tag=YKL211C;db_xref=GeneID:853669
NC_001143.7	RefSeq	CDS	36703	38154	.	-	0	ID=NC_001143.7:TRP3:unknown_transcript_1;Parent=NC_001143.7:TRP3;locus_tag=YKL211C;EC_number=4.1.1.48;note=Trp3p;experiment=experimental evidence%2C no additional details recorded;product=Bifunctional enzyme exhibiting both indole-3-glycerol-phosphate synthase and anthranilate synthase activities%2C forms multifunctional hetero-oligomeric anthranilate synthase:indole-3-glycerol phosphate synthase enzyme complex with Trp2p;protein_id=NP_012711.1;db_xref=SGD:S000001694;db_xref=GI:6322638;db_xref=GeneID:853669;exon_number=1
NC_001143.7	RefSeq	start_codon	38152	38154	.	-	0	ID=NC_001143.7:TRP3:unknown_transcript_1;Parent=NC_001143.7:TRP3;locus_tag=YKL211C;EC_number=4.1.1.48;note=Trp3p;experiment=experimental evidence%2C no additional details recorded;product=Bifunctional enzyme exhibiting both indole-3-glycerol-phosphate synthase and anthranilate synthase activities%2C forms multifunctional hetero-oligomeric anthranilate synthase:indole-3-glycerol phosphate synthase enzyme complex with Trp2p;protein_id=NP_012711.1;db_xref=SGD:S000001694;db_xref=GI:6322638;db_xref=GeneID:853669;exon_number=1
NC_001143.7	RefSeq	stop_codon	36700	36702	.	-	0	ID=NC_001143.7:TRP3:unknown_transcript_1;Parent=NC_001143.7:TRP3;locus_tag=YKL211C;EC_number=4.1.1.48;note=Trp3p;experiment=experimental evidence%2C no additional details recorded;product=Bifunctional enzyme exhibiting both indole-3-glycerol-phosphate synthase and anthranilate synthase activities%2C forms multifunctional hetero-oligomeric anthranilate synthase:indole-3-glycerol phosphate synthase enzyme complex with Trp2p;protein_id=NP_012711.1;db_xref=SGD:S000001694;db_xref=GI:6322638;db_xref=GeneID:853669;exon_number=1
NC_001143.7	RefSeq	exon	38812	38912	.	+	.	gbkey=ncRNA;ncRNA_class=snoRNA;product=SNR64;exon_number=1
NC_001143.7	RefSeq	gene	39164	42238	.	+	.	ID=NC_001143.7:UBA1;locus_tag=YKL210W;db_xref=GeneID:853670
NC_001143.7	RefSeq	CDS	39164	42235	.	+	0	ID=NC_001143.7:UBA1:unknown_transcript_1;Parent=NC_001143.7:UBA1;locus_tag=YKL210W;note=Ubiquitin activating enzyme %28E1%29%2C involved in ubiquitin-mediated protein degradation and essential for viability;experiment=experimental evidence%2C no additional details recorded;product=Uba1p;protein_id=NP_012712.1;db_xref=SGD:S000001693;db_xref=GI:6322639;db_xref=GeneID:853670;exon_number=1
NC_001143.7	RefSeq	start_codon	39164	39166	.	+	0	ID=NC_001143.7:UBA1:unknown_transcript_1;Parent=NC_001143.7:UBA1;locus_tag=YKL210W;note=Ubiquitin activating enzyme %28E1%29%2C involved in ubiquitin-mediated protein degradation and essential for viability;experiment=experimental evidence%2C no additional details recorded;product=Uba1p;protein_id=NP_012712.1;db_xref=SGD:S000001693;db_xref=GI:6322639;db_xref=GeneID:853670;exon_number=1
NC_001143.7	RefSeq	stop_codon	42236	42238	.	+	0	ID=NC_001143.7:UBA1:unknown_transcript_1;Parent=NC_001143.7:UBA1;locus_tag=YKL210W;note=Ubiquitin activating enzyme %28E1%29%2C involved in ubiquitin-mediated protein degradation and essential for viability;experiment=experimental evidence%2C no additional details recorded;product=Uba1p;protein_id=NP_012712.1;db_xref=SGD:S000001693;db_xref=GI:6322639;db_xref=GeneID:853670;exon_number=1
NC_001143.7	RefSeq	gene	42424	46296	.	-	.	ID=NC_001143.7:STE6;locus_tag=YKL209C;db_xref=GeneID:853671
NC_001143.7	RefSeq	CDS	42427	46296	.	-	0	ID=NC_001143.7:STE6:unknown_transcript_1;Parent=NC_001143.7:STE6;locus_tag=YKL209C;note=Ste6p;experiment=experimental evidence%2C no additional details recorded;product=Plasma membrane ATP-binding cassette %28ABC%29 transporter required for the export of a-factor%2C catalyzes ATP hydrolysis coupled to a-factor transport%3B contains 12 transmembrane domains and two ATP binding domains%3B expressed only in MATa cells;protein_id=NP_012713.1;db_xref=SGD:S000001692;db_xref=GI:6322640;db_xref=GeneID:853671;exon_number=1
NC_001143.7	RefSeq	start_codon	46294	46296	.	-	0	ID=NC_001143.7:STE6:unknown_transcript_1;Parent=NC_001143.7:STE6;locus_tag=YKL209C;note=Ste6p;experiment=experimental evidence%2C no additional details recorded;product=Plasma membrane ATP-binding cassette %28ABC%29 transporter required for the export of a-factor%2C catalyzes ATP hydrolysis coupled to a-factor transport%3B contains 12 transmembrane domains and two ATP binding domains%3B expressed only in MATa cells;protein_id=NP_012713.1;db_xref=SGD:S000001692;db_xref=GI:6322640;db_xref=GeneID:853671;exon_number=1
NC_001143.7	RefSeq	stop_codon	42424	42426	.	-	0	ID=NC_001143.7:STE6:unknown_transcript_1;Parent=NC_001143.7:STE6;locus_tag=YKL209C;note=Ste6p;experiment=experimental evidence%2C no additional details recorded;product=Plasma membrane ATP-binding cassette %28ABC%29 transporter required for the export of a-factor%2C catalyzes ATP hydrolysis coupled to a-factor transport%3B contains 12 transmembrane domains and two ATP binding domains%3B expressed only in MATa cells;protein_id=NP_012713.1;db_xref=SGD:S000001692;db_xref=GI:6322640;db_xref=GeneID:853671;exon_number=1
NC_001143.7	RefSeq	gene	46736	46807	.	-	.	ID=NC_001143.7:TRT2;locus_tag=TT%28CGU%29K;db_xref=GeneID:853672
NC_001143.7	RefSeq	exon	46736	46807	.	-	.	ID=NC_001143.7:TRT2:unknown_transcript_1;Parent=NC_001143.7:TRT2;gbkey=tRNA;locus_tag=TT%28CGU%29K;product=tRNA-Thr;experiment=experimental evidence%2C no additional details recorded;db_xref=SGD:S000006748;db_xref=GeneID:853672;exon_number=1
NC_001143.7	RefSeq	gene	47158	47973	.	+	.	ID=NC_001143.7:CBT1;locus_tag=YKL208W;gene_synonym=SOC1;db_xref=GeneID:853673
NC_001143.7	RefSeq	CDS	47158	47970	.	+	0	ID=NC_001143.7:CBT1:unknown_transcript_1;Parent=NC_001143.7:CBT1;locus_tag=YKL208W;gene_synonym=SOC1;note=Protein involved in 5%27 end processing of mitochondrial COB%2C 15S_rRNA%2C and RPM1 transcripts%3B may also have a role in 3%27 end processing of the COB pre-mRNA%3B displays genetic interaction with cell cycle-regulated kinase Dbf2p;experiment=experimental evidence%2C no additional details recorded;product=Cbt1p;protein_id=NP_012714.1;db_xref=SGD:S000001691;db_xref=GI:6322641;db_xref=GeneID:853673;exon_number=1
NC_001143.7	RefSeq	start_codon	47158	47160	.	+	0	ID=NC_001143.7:CBT1:unknown_transcript_1;Parent=NC_001143.7:CBT1;locus_tag=YKL208W;gene_synonym=SOC1;note=Protein involved in 5%27 end processing of mitochondrial COB%2C 15S_rRNA%2C and RPM1 transcripts%3B may also have a role in 3%27 end processing of the COB pre-mRNA%3B displays genetic interaction with cell cycle-regulated kinase Dbf2p;experiment=experimental evidence%2C no additional details recorded;product=Cbt1p;protein_id=NP_012714.1;db_xref=SGD:S000001691;db_xref=GI:6322641;db_xref=GeneID:853673;exon_number=1
NC_001143.7	RefSeq	stop_codon	47971	47973	.	+	0	ID=NC_001143.7:CBT1:unknown_transcript_1;Parent=NC_001143.7:CBT1;locus_tag=YKL208W;gene_synonym=SOC1;note=Protein involved in 5%27 end processing of mitochondrial COB%2C 15S_rRNA%2C and RPM1 transcripts%3B may also have a role in 3%27 end processing of the COB pre-mRNA%3B displays genetic interaction with cell cycle-regulated kinase Dbf2p;experiment=experimental evidence%2C no additional details recorded;product=Cbt1p;protein_id=NP_012714.1;db_xref=SGD:S000001691;db_xref=GI:6322641;db_xref=GeneID:853673;exon_number=1
NC_001143.7	RefSeq	gene	48195	48956	.	+	.	ID=NC_001143.7:LRC3;locus_tag=YKL207W;db_xref=GeneID:853628
NC_001143.7	RefSeq	CDS	48195	48953	.	+	0	ID=NC_001143.7:LRC3:unknown_transcript_1;Parent=NC_001143.7:LRC3;locus_tag=YKL207W;note=Putative protein of unknown function%3B non-essential gene%3B null mutant displays decreased frequency of mitochondrial genome loss %28petite formation%29 and severe growth defect in minimal glycerol media;experiment=experimental evidence%2C no additional details recorded;product=Lrc3p;protein_id=NP_012715.3;db_xref=SGD:S000001690;db_xref=GI:99030926;db_xref=GeneID:853628;exon_number=1
NC_001143.7	RefSeq	start_codon	48195	48197	.	+	0	ID=NC_001143.7:LRC3:unknown_transcript_1;Parent=NC_001143.7:LRC3;locus_tag=YKL207W;note=Putative protein of unknown function%3B non-essential gene%3B null mutant displays decreased frequency of mitochondrial genome loss %28petite formation%29 and severe growth defect in minimal glycerol media;experiment=experimental evidence%2C no additional details recorded;product=Lrc3p;protein_id=NP_012715.3;db_xref=SGD:S000001690;db_xref=GI:99030926;db_xref=GeneID:853628;exon_number=1
NC_001143.7	RefSeq	stop_codon	48954	48956	.	+	0	ID=NC_001143.7:LRC3:unknown_transcript_1;Parent=NC_001143.7:LRC3;locus_tag=YKL207W;note=Putative protein of unknown function%3B non-essential gene%3B null mutant displays decreased frequency of mitochondrial genome loss %28petite formation%29 and severe growth defect in minimal glycerol media;experiment=experimental evidence%2C no additional details recorded;product=Lrc3p;protein_id=NP_012715.3;db_xref=SGD:S000001690;db_xref=GI:99030926;db_xref=GeneID:853628;exon_number=1
NC_001143.7	RefSeq	gene	49007	49810	.	-	.	ID=NC_001143.7:ADD66;locus_tag=YKL206C;gene_synonym=POC2;gene_synonym=PBA2;db_xref=GeneID:853629
NC_001143.7	RefSeq	CDS	49010	49810	.	-	0	ID=NC_001143.7:ADD66:unknown_transcript_1;Parent=NC_001143.7:ADD66;locus_tag=YKL206C;gene_synonym=POC2;gene_synonym=PBA2;note=Protein involved in 20S proteasome assembly%3B forms a heterodimer with Pba1p that binds to proteasome precursors%3B similar to human PAC2 constituent of the PAC1-PAC2 complex involved in proteasome assembly;experiment=experimental evidence%2C no additional details recorded;product=Add66p;protein_id=NP_012716.1;db_xref=SGD:S000001689;db_xref=GI:6322643;db_xref=GeneID:853629;exon_number=1
";

        #endregion


        /// <summary>
        /// Verifies that the parser can read and format random.GFF correctly.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestGffWhenParsingOne()
        {
            // parse
            GffParser parser = new GffParser();
            ISequence seq = parser.Parse(_singleSeqGffFilename).FirstOrDefault();

            // test the non-metadata properties
            Assert.AreEqual(Alphabets.DNA, seq.Alphabet);
            Assert.AreEqual("NC_001133.7", seq.ID);

            // just test the formatting; if that's good, the parsing was good
            GffFormatter formatter = new GffFormatter();
            string actual = formatter.FormatString(seq);
            Assert.AreEqual(_singleSeqGffFileExpectedOutput.Replace("\r\n", Environment.NewLine), actual);
        }

        /// <summary>
        /// Verifies that the parser can read and format Tachibana2005.gff correctly.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestGffWhenParsingMultiple()
        {
            // parse
            ISequenceParser parser = new GffParser();
            IEnumerable<ISequence> seqList = parser.Parse(_multipleSeqGffFilename).ToList();

            // test the file-scope metadata that is tricky to parse, and will not be tested
            // implicitly by testing the formatting
            foreach (ISequence seq in seqList)
            {
                var item = seq.Metadata["SOURCE-VERSION"] as MetadataListItem<string>;

                if (item != null)
                {
                    Assert.AreEqual("NCBI", item.SubItems["source"]);
                    Assert.AreEqual("C++", item.SubItems["version"]);
                }

                Assert.AreEqual(DateTime.Parse("2009-10-27", null), seq.Metadata["date"]);
            }

            // just test the formatting; if that's good, the parsing was good
                new GffFormatter()
                .Format(seqList, TempGFFFileName);

            string actual;
            using (StreamReader reader = new StreamReader(TempGFFFileName))
            {
                actual = reader.ReadToEnd();
            }
            
            File.Delete(TempGFFFileName);
            Assert.AreEqual(_multipleSeqGffFileExpectedOutput.Replace("\r\n", Environment.NewLine), actual);
            
        }

        /// <summary>
        /// Verifies that the parser can read and format many files without exceptions.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestGffForManyFiles()
        {
            // parser and formatter will be used for all files in input dir
            GffFormatter formatter = new GffFormatter();

            // iterate through the files in input dir, parsing and formatting each; write results
            // to log file
            DirectoryInfo inputDirInfo = new DirectoryInfo(_gffDataPath);
            foreach (FileInfo fileInfo in inputDirInfo.GetFiles("*.gff"))
            {
                ApplicationLog.WriteLine("Parsing file {0}...{1}", fileInfo.FullName, Environment.NewLine);
                ISequenceParser parser = new GffParser();
                foreach (ISequence sequence in parser.Parse(fileInfo.FullName))
                {
                    // don't do anything with it; just make sure it doesn't crash
                    formatter.FormatString(sequence);
                }

                ApplicationLog.WriteLine("Parse completed successfully." + Environment.NewLine);
            }
        }

        /// <summary>
        /// Tests the name, description, and file extension property of 
        /// Gff formatter and parser.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void GffProperties()
        {
            GffParser parser = new GffParser();
            Assert.AreEqual(parser.Name, Resource.GFF_NAME);
            Assert.AreEqual(parser.Description, Resource.GFFPARSER_DESCRIPTION);
            Assert.AreEqual(parser.SupportedFileTypes, Resource.GFF_FILEEXTENSION);

            GffFormatter formatter = new GffFormatter();
            Assert.AreEqual(formatter.Name, Resource.GFF_NAME);
            Assert.AreEqual(formatter.Description, Resource.GFFFORMATTER_DESCRIPTION);
            Assert.AreEqual(formatter.SupportedFileTypes, Resource.GFF_FILEEXTENSION);
        }
    }
}
