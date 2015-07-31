using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Bio;
using Bio.IO;
using Bio.IO.GenBank;
using Bio.Util.Logging;
using NUnit.Framework;
using Bio.Tests.Properties;

namespace Bio.Tests.IO.GenBank
{
    /// <summary>
    /// Unit tests for the GenBankParser and GenBankFormatter.
    /// </summary>
    [TestFixture]
    public class GenBankTests
    {

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static GenBankTests()
        {
            Assert.IsTrue(File.Exists(_singleProteinSeqGenBankFilename));
            Assert.IsTrue(File.Exists(_multipleSeqGenBankFilename));
        }

        #region Fields
        private const string _genBankFile_LocusTokenParserTest = @"TestUtils\GenBank\LocusTokenParserTest.gb";
        private const string _genBankFile_ParseOriginShifted = @"TestUtils\GenBank\ParseOriginShifted.gb";
        private const string _genBankFile_ParseVersionEmpty = @"TestUtils\GenBank\ParseVersionEmptyTest.gb";
        private const string _genBankFile_EmptyOrganismClassificationTest = @"TestUtils\GenBank\EmptyOrganismClassificationTest.gb";
        private const string _genBankFile_WithTPAPrimaryData = @"TestUtils\GenBank\BK000016-tpa.gbk";
        private const string    _genBankFile_WithMultipleDBLines = @"TestUtils\GenBank\seq1.gbk";
        private const string _genBankFile_WithREFSEQPrimaryData = @"TestUtils\GenBank\NM_001747.gb";
        private const string _genBankDataPath = @"TestUtils\GenBank";
        private const string TempGenBankFileName = "tempGenBank.gbk";
        private const string _singleProteinSeqGenBankFilename = @"TestUtils\GenBank\U49845.gbk";
        private const string _singleDnaSeqGenBankFilename = @"TestUtils\GenBank\D12555.gbk";
        private const string _multipleSeqGenBankFilename = @"TestUtils\GenBank\U49845+D12555.gbk";
        private const string _singleProteinSeqGenBankFileExpectedOutput =
@"LOCUS       SCU49845     5028 bp    DNA             PLN       21-JUN-1999
DEFINITION  Saccharomyces cerevisiae TCP1-beta gene, partial cds, and Axl2p
            (AXL2) and Rev7p (REV7) genes, complete cds.
ACCESSION   U49845
VERSION     U49845.1  GI:1293613
KEYWORDS    .
SOURCE      Saccharomyces cerevisiae (baker's yeast)
  ORGANISM  Saccharomyces cerevisiae
            Eukaryota; Fungi; Ascomycota; Saccharomycotina; Saccharomycetes;
            Saccharomycetales; Saccharomycetaceae; Saccharomyces.
REFERENCE   1  (bases 1 to 5028)
  AUTHORS   Torpey,L.E., Gibbs,P.E., Nelson,J. and Lawrence,C.W.
  TITLE     Cloning and sequence of REV7, a gene whose function is required for
            DNA damage-induced mutagenesis in Saccharomyces cerevisiae
  JOURNAL   Yeast 10 (11), 1503-1509 (1994)
  PUBMED    7871890
REFERENCE   2  (bases 1 to 5028)
  AUTHORS   Roemer,T., Madden,K., Chang,J. and Snyder,M.
  TITLE     Selection of axial growth sites in yeast requires Axl2p, a novel
            plasma membrane glycoprotein
  JOURNAL   Genes Dev. 10 (7), 777-793 (1996)
  PUBMED    8846915
REFERENCE   3  (bases 1 to 5028)
  AUTHORS   Roemer,T.
  TITLE     Direct Submission
  JOURNAL   Submitted (22-FEB-1996) Terry Roemer, Biology, Yale University, New
            Haven, CT, USA
FEATURES             Location/Qualifiers
     source          1..5028
                     /organism=""Saccharomyces cerevisiae""
                     /db_xref=""taxon:4932""
                     /chromosome=""IX""
                     /map=""9""
     CDS             <1..206
                     /codon_start=3
                     /product=""TCP1-beta""
                     /protein_id=""AAA98665.1""
                     /db_xref=""GI:1293614""
                     /translation=""SSIYNGISTSGLDLNNGTIADMRQLGIVESYKLKRAVVSSASEA
                     AEVLLRVDNIIRARPRTANRQHM""
     gene            687..3158
                     /gene=""AXL2""
     CDS             687..3158
                     /gene=""AXL2""
                     /note=""plasma membrane glycoprotein""
                     /codon_start=1
                     /function=""required for axial budding pattern of S.
                     cerevisiae""
                     /product=""Axl2p""
                     /protein_id=""AAA98666.1""
                     /db_xref=""GI:1293615""
                     /translation=""MTQLQISLLLTATISLLHLVVATPYEAYPIGKQYPPVARVNESF
                     TFQISNDTYKSSVDKTAQITYNCFDLPSWLSFDSSSRTFSGEPSSDLLSDANTTLYFN
                     VILEGTDSADSTSLNNTYQFVVTNRPSISLSSDFNLLALLKNYGYTNGKNALKLDPNE
                     VFNVTFDRSMFTNEESIVSYYGRSQLYNAPLPNWLFFDSGELKFTGTAPVINSAIAPE
                     TSYSFVIIATDIEGFSAVEVEFELVIGAHQLTTSIQNSLIINVTDTGNVSYDLPLNYV
                     YLDDDPISSDKLGSINLLDAPDWVALDNATISGSVPDELLGKNSNPANFSVSIYDTYG
                     DVIYFNFEVVSTTDLFAISSLPNINATRGEWFSYYFLPSQFTDYVNTNVSLEFTNSSQ
                     DHDWVKFQSSNLTLAGEVPKNFDKLSLGLKANQGSQSQELYFNIIGMDSKITHSNHSA
                     NATSTRSSHHSTSTSSYTSSTYTAKISSTSAAATSSAPAALPAANKTSSHNKKAVAIA
                     CGVAIPLGVILVALICFLIFWRRRRENPDDENLPHAISGPDLNNPANKPNQENATPLN
                     NPFDDDASSYDDTSIARRLAALNTLKLDNHSATESDISSVDEKRDSLSGMNTYNDQFQ
                     SQSKEELLAKPPVQPPESPFFDPQNRSSSVYMDSEPAVNKSWRYTGNLSPVSDIVRDS
                     YGSQKTVDTEKLFDLEAPEKEKRTSRDVTMSSLDPWNSNISPSPVRKSVTPSPYNVTK
                     HRNRHLQNIQDSQSGKNGITPTTMSTSSSDDFVPVKDGENFCWVHSMEPDRRPSKKRL
                     VDFSNKSNVNVGQVKDIHGRIPEML""
     gene            complement(3300..4037)
                     /gene=""REV7""
     CDS             complement(3300..4037)
                     /gene=""REV7""
                     /codon_start=1
                     /product=""Rev7p""
                     /protein_id=""AAA98667.1""
                     /db_xref=""GI:1293616""
                     /translation=""MNRWVEKWLRVYLKCYINLILFYRNVYPPQSFDYTTYQSFNLPQ
                     FVPINRHPALIDYIEELILDVLSKLTHVYRFSICIINKKNDLCIEKYVLDFSELQHVD
                     KDDQIITETEVFDEFRSSLNSLIMHLEKLPKVNDDTITFEAVINAIELELGHKLDRNR
                     RVDSLEEKAEIERDSNWVKCQEDENLPDNNGFQPPKIKLTSLVGSDVGPLIIHQFSEK
                     LISGDDKILNGVYSQYEEGESIFGSLF""
ORIGIN
        1 gatcctccat atacaacggt atctccacct caggtttaga tctcaacaac ggaaccattg
       61 ccgacatgag acagttaggt atcgtcgaga gttacaagct aaaacgagca gtagtcagct
      121 ctgcatctga agccgctgaa gttctactaa gggtggataa catcatccgt gcaagaccaa
      181 gaaccgccaa tagacaacat atgtaacata tttaggatat acctcgaaaa taataaaccg
      241 ccacactgtc attattataa ttagaaacag aacgcaaaaa ttatccacta tataattcaa
      301 agacgcgaaa aaaaaagaac aacgcgtcat agaacttttg gcaattcgcg tcacaaataa
      361 attttggcaa cttatgtttc ctcttcgagc agtactcgag ccctgtctca agaatgtaat
      421 aatacccatc gtaggtatgg ttaaagatag catctccaca acctcaaagc tccttgccga
      481 gagtcgccct cctttgtcga gtaattttca cttttcatat gagaacttat tttcttattc
      541 tttactctca catcctgtag tgattgacac tgcaacagcc accatcacta gaagaacaga
      601 acaattactt aatagaaaaa ttatatcttc ctcgaaacga tttcctgctt ccaacatcta
      661 cgtatatcaa gaagcattca cttaccatga cacagcttca gatttcatta ttgctgacag
      721 ctactatatc actactccat ctagtagtgg ccacgcccta tgaggcatat cctatcggaa
      781 aacaataccc cccagtggca agagtcaatg aatcgtttac atttcaaatt tccaatgata
      841 cctataaatc gtctgtagac aagacagctc aaataacata caattgcttc gacttaccga
      901 gctggctttc gtttgactct agttctagaa cgttctcagg tgaaccttct tctgacttac
      961 tatctgatgc gaacaccacg ttgtatttca atgtaatact cgagggtacg gactctgccg
     1021 acagcacgtc tttgaacaat acataccaat ttgttgttac aaaccgtcca tccatctcgc
     1081 tatcgtcaga tttcaatcta ttggcgttgt taaaaaacta tggttatact aacggcaaaa
     1141 acgctctgaa actagatcct aatgaagtct tcaacgtgac ttttgaccgt tcaatgttca
     1201 ctaacgaaga atccattgtg tcgtattacg gacgttctca gttgtataat gcgccgttac
     1261 ccaattggct gttcttcgat tctggcgagt tgaagtttac tgggacggca ccggtgataa
     1321 actcggcgat tgctccagaa acaagctaca gttttgtcat catcgctaca gacattgaag
     1381 gattttctgc cgttgaggta gaattcgaat tagtcatcgg ggctcaccag ttaactacct
     1441 ctattcaaaa tagtttgata atcaacgtta ctgacacagg taacgtttca tatgacttac
     1501 ctctaaacta tgtttatctc gatgacgatc ctatttcttc tgataaattg ggttctataa
     1561 acttattgga tgctccagac tgggtggcat tagataatgc taccatttcc gggtctgtcc
     1621 cagatgaatt actcggtaag aactccaatc ctgccaattt ttctgtgtcc atttatgata
     1681 cttatggtga tgtgatttat ttcaacttcg aagttgtctc cacaacggat ttgtttgcca
     1741 ttagttctct tcccaatatt aacgctacaa ggggtgaatg gttctcctac tattttttgc
     1801 cttctcagtt tacagactac gtgaatacaa acgtttcatt agagtttact aattcaagcc
     1861 aagaccatga ctgggtgaaa ttccaatcat ctaatttaac attagctgga gaagtgccca
     1921 agaatttcga caagctttca ttaggtttga aagcgaacca aggttcacaa tctcaagagc
     1981 tatattttaa catcattggc atggattcaa agataactca ctcaaaccac agtgcgaatg
     2041 caacgtccac aagaagttct caccactcca cctcaacaag ttcttacaca tcttctactt
     2101 acactgcaaa aatttcttct acctccgctg ctgctacttc ttctgctcca gcagcgctgc
     2161 cagcagccaa taaaacttca tctcacaata aaaaagcagt agcaattgcg tgcggtgttg
     2221 ctatcccatt aggcgttatc ctagtagctc tcatttgctt cctaatattc tggagacgca
     2281 gaagggaaaa tccagacgat gaaaacttac cgcatgctat tagtggacct gatttgaata
     2341 atcctgcaaa taaaccaaat caagaaaacg ctacaccttt gaacaacccc tttgatgatg
     2401 atgcttcctc gtacgatgat acttcaatag caagaagatt ggctgctttg aacactttga
     2461 aattggataa ccactctgcc actgaatctg atatttccag cgtggatgaa aagagagatt
     2521 ctctatcagg tatgaataca tacaatgatc agttccaatc ccaaagtaaa gaagaattat
     2581 tagcaaaacc cccagtacag cctccagaga gcccgttctt tgacccacag aataggtctt
     2641 cttctgtgta tatggatagt gaaccagcag taaataaatc ctggcgatat actggcaacc
     2701 tgtcaccagt ctctgatatt gtcagagaca gttacggatc acaaaaaact gttgatacag
     2761 aaaaactttt cgatttagaa gcaccagaga aggaaaaacg tacgtcaagg gatgtcacta
     2821 tgtcttcact ggacccttgg aacagcaata ttagcccttc tcccgtaaga aaatcagtaa
     2881 caccatcacc atataacgta acgaagcatc gtaaccgcca cttacaaaat attcaagact
     2941 ctcaaagcgg taaaaacgga atcactccca caacaatgtc aacttcatct tctgacgatt
     3001 ttgttccggt taaagatggt gaaaattttt gctgggtcca tagcatggaa ccagacagaa
     3061 gaccaagtaa gaaaaggtta gtagattttt caaataagag taatgtcaat gttggtcaag
     3121 ttaaggacat tcacggacgc atcccagaaa tgctgtgatt atacgcaacg atattttgct
     3181 taattttatt ttcctgtttt attttttatt agtggtttac agatacccta tattttattt
     3241 agtttttata cttagagaca tttaatttta attccattct tcaaatttca tttttgcact
     3301 taaaacaaag atccaaaaat gctctcgccc tcttcatatt gagaatacac tccattcaaa
     3361 attttgtcgt caccgctgat taatttttca ctaaactgat gaataatcaa aggccccacg
     3421 tcagaaccga ctaaagaagt gagttttatt ttaggaggtt gaaaaccatt attgtctggt
     3481 aaattttcat cttcttgaca tttaacccag tttgaatccc tttcaatttc tgctttttcc
     3541 tccaaactat cgaccctcct gtttctgtcc aacttatgtc ctagttccaa ttcgatcgca
     3601 ttaataactg cttcaaatgt tattgtgtca tcgttgactt taggtaattt ctccaaatgc
     3661 ataatcaaac tatttaagga agatcggaat tcgtcgaaca cttcagtttc cgtaatgatc
     3721 tgatcgtctt tatccacatg ttgtaattca ctaaaatcta aaacgtattt ttcaatgcat
     3781 aaatcgttct ttttattaat aatgcagatg gaaaatctgt aaacgtgcgt taatttagaa
     3841 agaacatcca gtataagttc ttctatatag tcaattaaag caggatgcct attaatggga
     3901 acgaactgcg gcaagttgaa tgactggtaa gtagtgtagt cgaatgactg aggtgggtat
     3961 acatttctat aaaataaaat caaattaatg tagcatttta agtataccct cagccacttc
     4021 tctacccatc tattcataaa gctgacgcaa cgattactat tttttttttc ttcttggatc
     4081 tcagtcgtcg caaaaacgta taccttcttt ttccgacctt ttttttagct ttctggaaaa
     4141 gtttatatta gttaaacagg gtctagtctt agtgtgaaag ctagtggttt cgattgactg
     4201 atattaagaa agtggaaatt aaattagtag tgtagacgta tatgcatatg tatttctcgc
     4261 ctgtttatgt ttctacgtac ttttgattta tagcaagggg aaaagaaata catactattt
     4321 tttggtaaag gtgaaagcat aatgtaaaag ctagaataaa atggacgaaa taaagagagg
     4381 cttagttcat cttttttcca aaaagcaccc aatgataata actaaaatga aaaggatttg
     4441 ccatctgtca gcaacatcag ttgtgtgagc aataataaaa tcatcacctc cgttgccttt
     4501 agcgcgtttg tcgtttgtat cttccgtaat tttagtctta tcaatgggaa tcataaattt
     4561 tccaatgaat tagcaatttc gtccaattct ttttgagctt cttcatattt gctttggaat
     4621 tcttcgcact tcttttccca ttcatctctt tcttcttcca aagcaacgat ccttctaccc
     4681 atttgctcag agttcaaatc ggcctctttc agtttatcca ttgcttcctt cagtttggct
     4741 tcactgtctt ctagctgttg ttctagatcc tggtttttct tggtgtagtt ctcattatta
     4801 gatctcaagt tattggagtc ttcagccaat tgctttgtat cagacaattg actctctaac
     4861 ttctccactt cactgtcgag ttgctcgttt ttagcggaca aagatttaat ctcgttttct
     4921 ttttcagtgt tagattgctc taattctttg agctgttctc tcagctcctc atatttttct
     4981 tgccatgact cagattctaa ttttaagcta ttcaatttct ctttgatc
//
";
        private const string _singleDnaSeqGenBankFileExpectedOutput =
@"LOCUS       MUSBC05                  105 bp    DNA     linear   ROD 18-DEC-2007
DEFINITION  Mus spretus gene for beta-casein, 3'UTR.
ACCESSION   D12555
VERSION     D12555.1  GI:303649
KEYWORDS    .
SOURCE      Mus spretus (western wild mouse)
  ORGANISM  Mus spretus
            Eukaryota; Metazoa; Chordata; Craniata; Vertebrata; Euteleostomi;
            Mammalia; Eutheria; Euarchontoglires; Glires; Rodentia;
            Sciurognathi; Muroidea; Muridae; Murinae; Mus.
REFERENCE   1  (bases 1 to 105)
  AUTHORS   Takahashi,N. and Ko,M.S.
  TITLE     The short 3'-end region of complementary DNAs as PCR-based
            polymorphic markers for an expression map of the mouse genome
  JOURNAL   Genomics 16 (1), 161-168 (1993)
   PUBMED   8486351
REFERENCE   2  (bases 1 to 105)
  AUTHORS   Ko,M.S.H.
  TITLE     Direct Submission
  JOURNAL   Submitted (06-JUL-1993) Contact:Minoru S.H. Ko ERATO Research
            Development Corporation of Japan (JRDC); 5-9-6 Tohkohdai, Tsukuba,
            Ibaraki 300-26, Japan
FEATURES             Location/Qualifiers
     source          1..105
                     /organism=""Mus spretus""
                     /mol_type=""genomic DNA""
                     /db_xref=""taxon:10096""
     3'UTR           <1..>105
                     /note=""beta-casein;
                     genomic DNA sequence corresponding to a part of the 3'UTR
                     of beta-caseine gene, MMBCASE (bases 7015 - 7121)""
     variation       69^70
                     /note=""A in MMBCASE""
                     /replace=""a""
     variation       102^103
                     /note=""T in MMBCASE;
                     deleted in CAST/Ei (M. m. castaneus)""
                     /replace=""t""
ORIGIN      
        1 agttatatta caggaatttt ataagtgttc aatatggagt tgaaaatgca agtcaataat
       61 gtatacaaat agtttgtgaa aaattggatt ttctattttt ttctt
//
";
        #endregion

        /// <summary>
        /// This test used to fail.  The sequence is in a GenBank format and would fail with the original parsing based on 
        /// index positions.  After fixing this to use a token based approach this parsing passes.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestGenBankLocusTokenParser()
        {
            // parse
            GenBankParser parser = new GenBankParser();
            ISequence seq = parser.Parse(_genBankFile_LocusTokenParserTest).FirstOrDefault();
            Assert.IsNotNull(seq);
        }

        /// <summary>
        /// This test used to fail.  The sequence is in a GenBank format and would fail if the classification of the 
        /// ORGANISM was empty (a line with a single dot).
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestGenBankEmptyOrganismClassification()
        {
            // parse
            GenBankParser parser = new GenBankParser();
            ISequence seq = parser.Parse(_genBankFile_EmptyOrganismClassificationTest).FirstOrDefault();
            Assert.IsNotNull(seq);
        }

        /// <summary>
        /// This test used to fail.  The sequence is in a GenBank format and would fail if the
        /// VERSION was empty (a line with a single dot). GenBank files generated by SnapGene
        /// can have the version omitted.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestGenBankParseVersionEmpty()
        {
            // parse
            GenBankParser parser = new GenBankParser();
            ISequence seq = parser.Parse(_genBankFile_ParseVersionEmpty).FirstOrDefault();
            Assert.IsNotNull(seq);
        }

        /// <summary>
        /// This test used to fail.  The sequence is in a GenBank format and would fail if the
        /// ORIGIN contents started at index 9 instead of 10. In rare occassions Vector NTI have 
        /// produced such GenBank files.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestGenBankParseOriginShifted()
        {
            // parse
            GenBankParser parser = new GenBankParser();
            ISequence seq = parser.Parse(_genBankFile_ParseOriginShifted).FirstOrDefault();
            Assert.IsNotNull(seq);
        }

        /// <summary>
        /// Verifies that the parser throws an exception when calling ParseOne on a file containing
        /// more than one sequence.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestGenBankFailureWhenParsingEmpty()
        {

            bool failed = false;

            try
            {
                ISequenceParser parser = new GenBankParser();
                parser.Parse();
                failed = true;
            }
            catch (Exception)
            {
                // all is well with the world
            }
            if (failed)
            {
                Assert.Fail("Failed to throw exception for calling ParseOne on reader containing empty string.");
            }
        }

        /// <summary>
        /// Verifies that the parser can read and format U49845.gbk correctly.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestGenBankWhenParsingOne()
        {
            // parse
            ISequenceParser parser = new GenBankParser();
            ISequence seq = parser.Parse(_singleProteinSeqGenBankFilename).FirstOrDefault();

            // test the non-metadata properties
            Assert.AreEqual(Alphabets.DNA, seq.Alphabet);
            Assert.AreEqual("SCU49845", seq.ID);

            // test the metadata that is tricky to parse, and will not be tested implicitly by
            // testing the formatting
            GenBankMetadata metadata = (GenBankMetadata)seq.Metadata["GenBank"];

            Assert.AreEqual(metadata.Locus.Strand, SequenceStrandType.None);
            Assert.AreEqual("none", metadata.Locus.StrandTopology.ToString().ToLower(CultureInfo.CurrentCulture));
            Assert.AreEqual("PLN", metadata.Locus.DivisionCode.ToString());
            Assert.AreEqual(DateTime.Parse("21-JUN-1999", (IFormatProvider)null), metadata.Locus.Date);
            Assert.AreEqual("1", metadata.Version.Version);
            Assert.AreEqual("1293613", metadata.Version.GiNumber);

            // test that we're correctly putting all types of metadata in the right places
            Assert.AreEqual(1, seq.Metadata.Count);
            IList<CitationReference> referenceList = metadata.References;
            Assert.AreEqual(3, referenceList.Count);
            IList<FeatureItem> featureList = metadata.Features.All;
            Assert.AreEqual(6, featureList.Count);
            Assert.AreEqual(4, featureList[0].Qualifiers.Count);
            Assert.AreEqual(5, featureList[1].Qualifiers.Count);
            Assert.AreEqual(1, featureList[2].Qualifiers.Count);

            // test the sequence string
            string expected = @"gatcctccatatacaacggtatctccacctcaggtttagatctcaacaacggaaccattgccgacatgagacagttaggtatcgtcgagagttacaagctaaaacgagcagtagtcagctctgcatctgaagccgctgaagttctactaagggtggataacatcatccgtgcaagaccaagaaccgccaatagacaacatatgtaacatatttaggatatacctcgaaaataataaaccgccacactgtcattattataattagaaacagaacgcaaaaattatccactatataattcaaagacgcgaaaaaaaaagaacaacgcgtcatagaacttttggcaattcgcgtcacaaataaattttggcaacttatgtttcctcttcgagcagtactcgagccctgtctcaagaatgtaataatacccatcgtaggtatggttaaagatagcatctccacaacctcaaagctccttgccgagagtcgccctcctttgtcgagtaattttcacttttcatatgagaacttattttcttattctttactctcacatcctgtagtgattgacactgcaacagccaccatcactagaagaacagaacaattacttaatagaaaaattatatcttcctcgaaacgatttcctgcttccaacatctacgtatatcaagaagcattcacttaccatgacacagcttcagatttcattattgctgacagctactatatcactactccatctagtagtggccacgccctatgaggcatatcctatcggaaaacaataccccccagtggcaagagtcaatgaatcgtttacatttcaaatttccaatgatacctataaatcgtctgtagacaagacagctcaaataacatacaattgcttcgacttaccgagctggctttcgtttgactctagttctagaacgttctcaggtgaaccttcttctgacttactatctgatgcgaacaccacgttgtatttcaatgtaatactcgagggtacggactctgccgacagcacgtctttgaacaatacataccaatttgttgttacaaaccgtccatccatctcgctatcgtcagatttcaatctattggcgttgttaaaaaactatggttatactaacggcaaaaacgctctgaaactagatcctaatgaagtcttcaacgtgacttttgaccgttcaatgttcactaacgaagaatccattgtgtcgtattacggacgttctcagttgtataatgcgccgttacccaattggctgttcttcgattctggcgagttgaagtttactgggacggcaccggtgataaactcggcgattgctccagaaacaagctacagttttgtcatcatcgctacagacattgaaggattttctgccgttgaggtagaattcgaattagtcatcggggctcaccagttaactacctctattcaaaatagtttgataatcaacgttactgacacaggtaacgtttcatatgacttacctctaaactatgtttatctcgatgacgatcctatttcttctgataaattgggttctataaacttattggatgctccagactgggtggcattagataatgctaccatttccgggtctgtcccagatgaattactcggtaagaactccaatcctgccaatttttctgtgtccatttatgatacttatggtgatgtgatttatttcaacttcgaagttgtctccacaacggatttgtttgccattagttctcttcccaatattaacgctacaaggggtgaatggttctcctactattttttgccttctcagtttacagactacgtgaatacaaacgtttcattagagtttactaattcaagccaagaccatgactgggtgaaattccaatcatctaatttaacattagctggagaagtgcccaagaatttcgacaagctttcattaggtttgaaagcgaaccaaggttcacaatctcaagagctatattttaacatcattggcatggattcaaagataactcactcaaaccacagtgcgaatgcaacgtccacaagaagttctcaccactccacctcaacaagttcttacacatcttctacttacactgcaaaaatttcttctacctccgctgctgctacttcttctgctccagcagcgctgccagcagccaataaaacttcatctcacaataaaaaagcagtagcaattgcgtgcggtgttgctatcccattaggcgttatcctagtagctctcatttgcttcctaatattctggagacgcagaagggaaaatccagacgatgaaaacttaccgcatgctattagtggacctgatttgaataatcctgcaaataaaccaaatcaagaaaacgctacacctttgaacaacccctttgatgatgatgcttcctcgtacgatgatacttcaatagcaagaagattggctgctttgaacactttgaaattggataaccactctgccactgaatctgatatttccagcgtggatgaaaagagagattctctatcaggtatgaatacatacaatgatcagttccaatcccaaagtaaagaagaattattagcaaaacccccagtacagcctccagagagcccgttctttgacccacagaataggtcttcttctgtgtatatggatagtgaaccagcagtaaataaatcctggcgatatactggcaacctgtcaccagtctctgatattgtcagagacagttacggatcacaaaaaactgttgatacagaaaaacttttcgatttagaagcaccagagaaggaaaaacgtacgtcaagggatgtcactatgtcttcactggacccttggaacagcaatattagcccttctcccgtaagaaaatcagtaacaccatcaccatataacgtaacgaagcatcgtaaccgccacttacaaaatattcaagactctcaaagcggtaaaaacggaatcactcccacaacaatgtcaacttcatcttctgacgattttgttccggttaaagatggtgaaaatttttgctgggtccatagcatggaaccagacagaagaccaagtaagaaaaggttagtagatttttcaaataagagtaatgtcaatgttggtcaagttaaggacattcacggacgcatcccagaaatgctgtgattatacgcaacgatattttgcttaattttattttcctgttttattttttattagtggtttacagataccctatattttatttagtttttatacttagagacatttaattttaattccattcttcaaatttcatttttgcacttaaaacaaagatccaaaaatgctctcgccctcttcatattgagaatacactccattcaaaattttgtcgtcaccgctgattaatttttcactaaactgatgaataatcaaaggccccacgtcagaaccgactaaagaagtgagttttattttaggaggttgaaaaccattattgtctggtaaattttcatcttcttgacatttaacccagtttgaatccctttcaatttctgctttttcctccaaactatcgaccctcctgtttctgtccaacttatgtcctagttccaattcgatcgcattaataactgcttcaaatgttattgtgtcatcgttgactttaggtaatttctccaaatgcataatcaaactatttaaggaagatcggaattcgtcgaacacttcagtttccgtaatgatctgatcgtctttatccacatgttgtaattcactaaaatctaaaacgtatttttcaatgcataaatcgttctttttattaataatgcagatggaaaatctgtaaacgtgcgttaatttagaaagaacatccagtataagttcttctatatagtcaattaaagcaggatgcctattaatgggaacgaactgcggcaagttgaatgactggtaagtagtgtagtcgaatgactgaggtgggtatacatttctataaaataaaatcaaattaatgtagcattttaagtataccctcagccacttctctacccatctattcataaagctgacgcaacgattactattttttttttcttcttggatctcagtcgtcgcaaaaacgtataccttctttttccgaccttttttttagctttctggaaaagtttatattagttaaacagggtctagtcttagtgtgaaagctagtggtttcgattgactgatattaagaaagtggaaattaaattagtagtgtagacgtatatgcatatgtatttctcgcctgtttatgtttctacgtacttttgatttatagcaaggggaaaagaaatacatactattttttggtaaaggtgaaagcataatgtaaaagctagaataaaatggacgaaataaagagaggcttagttcatcttttttccaaaaagcacccaatgataataactaaaatgaaaaggatttgccatctgtcagcaacatcagttgtgtgagcaataataaaatcatcacctccgttgcctttagcgcgtttgtcgtttgtatcttccgtaattttagtcttatcaatgggaatcataaattttccaatgaattagcaatttcgtccaattctttttgagcttcttcatatttgctttggaattcttcgcacttcttttcccattcatctctttcttcttccaaagcaacgatccttctacccatttgctcagagttcaaatcggcctctttcagtttatccattgcttccttcagtttggcttcactgtcttctagctgttgttctagatcctggtttttcttggtgtagttctcattattagatctcaagttattggagtcttcagccaattgctttgtatcagacaattgactctctaacttctccacttcactgtcgagttgctcgtttttagcggacaaagatttaatctcgttttctttttcagtgttagattgctctaattctttgagctgttctctcagctcctcatatttttcttgccatgactcagattctaattttaagctattcaatttctctttgatc";
            Assert.AreEqual(expected, new string(seq.Select(a => (char)a).ToArray()));

            // format
            ISequenceFormatter formatter = new GenBankFormatter();
            formatter.Format(seq, TempGenBankFileName);

            string actual = string.Empty;
            using (StreamReader reader = new StreamReader(TempGenBankFileName))
            {
                actual = reader.ReadToEnd();
            }
            File.Delete(TempGenBankFileName);

            // test the formatting
            Assert.AreEqual(_singleProteinSeqGenBankFileExpectedOutput.Replace(" ", "").Replace("\r\n", Environment.NewLine), actual.Replace(" ", ""));
        }

        /// <summary>
        /// Verifies that the parser can read and format U49845+D12555.gbk correctly.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestGenBankWhenParsingMultiple()
        {
            // parse
            ISequenceParser parser = new GenBankParser();
            IEnumerable<ISequence> seqList = parser.Parse(_multipleSeqGenBankFilename);

            // Just check the number of items returned and that they're not empty.  The guts
            // are tested in TestGenBankWhenParsingOne.
            Assert.AreEqual(2, seqList.Count());
            Assert.AreEqual(105, seqList.ElementAt(0).Count);
            Assert.AreEqual(5028, seqList.ElementAt(1).Count);
        }

        /// <summary>
        /// Verifies that the parser works correctly when a user designates a protein alphabet.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestGenBankWhenUserSetsProteinAlphabet()
        {
            // set correct alphabet and parse
            ISequenceParser parser = new GenBankParser();
            parser.Alphabet = Alphabets.DNA;
            ISequence seq = parser.Parse(_singleProteinSeqGenBankFilename).FirstOrDefault();
            Assert.AreEqual(Alphabets.DNA, seq.Alphabet);

            // format
            ISequenceFormatter formatter = new GenBankFormatter();

            using (formatter.Open(TempGenBankFileName))
                formatter.Format(seq);

            string actual = string.Empty;
            using (StreamReader reader = new StreamReader(TempGenBankFileName))
            {
                actual = reader.ReadToEnd();
            }

            File.Delete(TempGenBankFileName);
            // test the formatting
            Assert.AreEqual(_singleProteinSeqGenBankFileExpectedOutput.Replace(" ", "").Replace("\r\n", Environment.NewLine), actual.Replace(" ", ""));
        }

        /// <summary>
        /// Verifies that the parser works correctly when a user designates a DNA alphabet.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestGenBankWhenUserSetsDnaAlphabet()
        {
            // set correct alphabet and parse
            ISequenceParser parser = new GenBankParser();
            parser.Alphabet = Alphabets.DNA;
            ISequence seq = parser.Parse(_singleDnaSeqGenBankFilename).FirstOrDefault();
            Assert.AreEqual(Alphabets.DNA, seq.Alphabet);

            // format
            ISequenceFormatter formatter = new GenBankFormatter();
            formatter.Format(seq, TempGenBankFileName);

            string actual = string.Empty;
            using (StreamReader reader = new StreamReader(TempGenBankFileName))
            {
                actual = reader.ReadToEnd();
            }
            File.Delete(TempGenBankFileName);

            // test the formatting
            Assert.AreEqual(_singleDnaSeqGenBankFileExpectedOutput.Replace(" ", "").Replace("\r\n", Environment.NewLine), actual.Replace(" ", ""));
        }

        /// <summary>
        /// Verifies that the parser throws an exception when a user designates an incorrect
        /// alphabet.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestGenBankWhenUserSetsIncorrectAlphabet()
        {
            // parse
            ISequenceParser parser = new GenBankParser();
            parser.Alphabet = Alphabets.Protein;
            bool failed = false;
            try
            {
                var seqList = parser.Parse(_singleDnaSeqGenBankFilename);
                var x = seqList.ElementAt(0);
                failed = true;
            }
            catch (InvalidDataException)
            {
                // all is well with the world
            }
            if (failed)
            {
                Assert.Fail("Failed to throw exception for trying to create sequence using incorrect alphabet.");
            }
        }

        /// <summary>
        /// Verifies that the parser can read and format many files without exceptions.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestGenBankForManyFiles()
        {
            // parser and formatter will be used for all files in input dir

            // iterate through the files in input dir, parsing and formatting each; write results
            // to log file
            DirectoryInfo inputDirInfo = new DirectoryInfo(_genBankDataPath);
            foreach (FileInfo fileInfo in inputDirInfo.GetFiles("*.gbk"))
            {
                ApplicationLog.WriteLine("Parsing file {0}...{1}", fileInfo.FullName, Environment.NewLine);

                IEnumerable<ISequence> seqList = new GenBankParser().Parse(fileInfo.FullName);

                ISequenceFormatter formatter = new GenBankFormatter();
                using (formatter.Open(TempGenBankFileName))
                {
                    (formatter as GenBankFormatter).Format(seqList.ToList());
                }

                using (var reader = new StreamReader(TempGenBankFileName))
                {
                    string actual = reader.ReadToEnd();
                }
                
                File.Delete(TempGenBankFileName);
            }
        }

        /// <summary>
        /// Tests the name,description and file extension property of 
        /// GenBank formatter and parser.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void GenBankProperties()
        {
            ISequenceParser parser = new GenBankParser();

            Assert.AreEqual(parser.Name, Resource.GENBANK_NAME);
            Assert.AreEqual(parser.Description, Resource.GENBANKPARSER_DESCRIPTION);
            Assert.AreEqual(parser.SupportedFileTypes, Resource.GENBANK_FILEEXTENSION);

            ISequenceFormatter formatter = new GenBankFormatter();

            Assert.AreEqual(formatter.Name, Resource.GENBANK_NAME);
            Assert.AreEqual(formatter.Description, Resource.GENBANKFORMATTER_DESCRIPTION);
            Assert.AreEqual(formatter.SupportedFileTypes, Resource.GENBANK_FILEEXTENSION);
        }

        /// <summary>
        /// Tests Genbank Feature.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void GenBankFeatures()
        {
            // parse
            ISequence seq = new GenBankParser()
                .Parse(_singleProteinSeqGenBankFilename)
                .FirstOrDefault();
            Assert.IsNotNull(seq);

            GenBankMetadata metadata = seq.Metadata["GenBank"] as GenBankMetadata;
            Assert.IsNotNull(metadata);

            List<CodingSequence> CDS = metadata.Features.CodingSequences;
            Assert.AreEqual(CDS.Count, 3);
            Assert.AreEqual(CDS[0].DatabaseCrossReference.Count, 1);
            Assert.AreEqual(CDS[0].GeneSymbol, string.Empty);
            Assert.AreEqual(metadata.Features.GetFeatures("source").Count, 1);
            Assert.IsFalse(CDS[0].Pseudo);
            Assert.AreEqual(metadata.GetFeatures(1, 109).Count, 2);
            Assert.AreEqual(metadata.GetFeatures(1, 10).Count, 2);
            Assert.AreEqual(metadata.GetFeatures(10, 100).Count, 2);
            Assert.AreEqual(metadata.GetFeatures(120, 150).Count, 2);
            Assert.AreEqual(metadata.GetCitationsReferredInFeatures().Count, 0);

            ISequence seq1 = new GenBankParser()
                .Parse(_genBankDataPath + @"\NC_001284.gbk")
                .FirstOrDefault();
            Assert.IsNotNull(seq1);

            metadata = seq1.Metadata["GenBank"] as GenBankMetadata;
            Assert.IsNotNull(metadata);
            Assert.AreEqual(metadata.Features.All.Count, 743);
            Assert.AreEqual(metadata.Features.CodingSequences.Count, 117);
            Assert.AreEqual(metadata.Features.Exons.Count, 32);
            Assert.AreEqual(metadata.Features.Introns.Count, 22);
            Assert.AreEqual(metadata.Features.Genes.Count, 60);
            Assert.AreEqual(metadata.Features.MiscFeatures.Count, 455);
            Assert.AreEqual(metadata.Features.Promoters.Count, 17);
            Assert.AreEqual(metadata.Features.TransferRNAs.Count, 21);
            Assert.AreEqual(metadata.Features.All.FindAll(F => F.Key.Equals(StandardFeatureKeys.CodingSequence)).Count, 117);
            Assert.AreEqual(metadata.Features.GetFeatures(StandardFeatureKeys.CodingSequence).Count, 117);
            
            ISequence seqTemp = metadata.Features.CodingSequences[0].GetTranslation();
            byte[] tempData = new byte[seqTemp.Count];
            for (int i = 0; i < seqTemp.Count; i++)
            {
                tempData[i] = seqTemp[i];
            }
            string sequenceInString = Encoding.ASCII.GetString(tempData);
            Assert.AreEqual(metadata.Features.CodingSequences[0].Translation.Trim('"'), sequenceInString.Trim('"'));
            Assert.AreEqual(2, metadata.GetFeatures(11918, 12241).Count);
        }

        /// <summary>
        /// Tests Genbank TPA Primary header.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestParsingTPAPrimaryHeader()
        {
            // Test parsing Primary header which contains table with header.
            // TPA_SPAN         PRIMARY_IDENTIFIER PRIMARY_SPAN        COMP
            var results = new GenBankParser()
                .Parse(_genBankFile_WithTPAPrimaryData)
                .ToList();
        }

        /// <summary>
        /// Tests Genbank REFSEQ Primary header.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestParsingREFSEQPrimaryHeader()
        {
            // Test parsing Primary header which contains table with header.
            // REFSEQ_SPAN         PRIMARY_IDENTIFIER PRIMARY_SPAN        COMP
            var results = new GenBankParser()
                .Parse(_genBankFile_WithREFSEQPrimaryData)
                .ToList();
        }

         /// <summary>
        ///     New test based on GenBankFormatterValidateWrite that ensures the input and output
        ///     files are the same. Designed to test the case of multiple dblink fields.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void GenBankFormatterValidateReadAndWriteMultipleDBLinks()
        {
            // Create a Sequence with all attributes.
            // parse and update the properties instead of parsing entire file.   
            string tempFileName = Path.GetTempFileName();
            ISequenceParser parser1 = new GenBankParser();
            using (parser1.Open(_genBankFile_WithMultipleDBLines))
            {
                var orgSeq = parser1.Parse().First();
                ISequenceFormatter formatter = new GenBankFormatter();
                using (formatter.Open(tempFileName))
                {
                    formatter.Format(orgSeq);
                    formatter.Close();
                }
            }
            var same = CompareFiles(tempFileName, _genBankFile_WithMultipleDBLines);
            File.Delete(tempFileName);
            Assert.IsTrue(same);
            ApplicationLog.WriteLine("GenBank Formatter: Successful read->write loop");
        }



        /// <summary>
        /// Compare the results/output file
        /// 
        /// Checks that only differences are \r\n versus \n
        /// </summary>
        /// <param name="file1">File 1 to compare</param>
        /// <param name="file2">File 2 to compare with</param>
        /// <returns>True, if both files are the same.</returns>
        internal static bool CompareFiles(string observed, string expected)
        {
            
            FileInfo fileInfoObj1 = new FileInfo(observed);
            FileInfo fileInfoObj2 = new FileInfo(expected);

            if (fileInfoObj1.Length > fileInfoObj2.Length)
                return false;

            byte[] bytesO = File.ReadAllBytes(observed);
            byte[] bytesE = File.ReadAllBytes(expected);

            if (bytesO.Length > bytesE.Length)
                return false;
            int j = 0;
            for (int i = 0; i < bytesE.Length; i++)
            {
                if (bytesO [j] != bytesE [i]) {
                    if (i + 1 < bytesE.Length) {
                        if (bytesO [j] == '\n' &&
                            bytesE [i] == '\r' &&
                            bytesE [i + 1] == '\n') {
                            i++;
                        } else {
                            return false;
                        }
                    } else {
                        return false;
                    }
                }
                j++;
            }
            return true;
        }


    }
}
