using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Bio;
using Bio.Algorithms.Alignment;
using Bio.IO;
using Bio.IO.ClustalW;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.Tests.IO.ClustalW
{
    /// <summary>
    /// ClustalW format parser.
    /// </summary>
    [TestClass]
    public class ClustalWTests
    {
        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static ClustalWTests()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("Bio.Tests.log");
            }
        }

        /// <summary>
        /// Parse sample FASTA file 186972391.fasta and verify that it is read correctly.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ClustalWParse()
        {
            string filepath = @"TestUtils\ClustalW\AlignmentData.aln";
            Assert.IsTrue(File.Exists(filepath));

            IList<Dictionary<string, string>> expectedOutput = new List<Dictionary<string, string>>();

            Dictionary<string, string> expectedAlignment = new Dictionary<string, string>();
            expectedAlignment["CYS1_DICDI"] = "-----MKVILLFVLAVFTVFVSS---------------RGIPPEEQ------------SQ"
                    + "FLEFQDKFNKKY-SHEEYLERFEIFKSNLGKIEELNLIAINHKADTKFGVNKFADLSSDE"
                    + "FKNYYLNNKEAIFTDDLPVADYLDDEFINSIPTAFDWRTRG-AVTPVKNQGQCGSCWSFS"
                    + "TTGNVEGQHFISQNKLVSLSEQNLVDCDHECMEYEGEEACDEGCNGGLQPNAYNYIIKNG"
                    + "GIQTESSYPYTAETGTQCNFNSANIGAKISNFTMIP-KNETVMAGYIVSTGPLAIAADAV"
                    + "E-WQFYIGGVF-DIPCN--PNSLDHGILIVGYSAKNTIFRKNMPYWIVKNSWGADWGEQG"
                    + "YIYLRRGKNTCGVSNFVSTSII--";

            expectedAlignment["ALEU_HORVU"] = "MAHARVLLLALAVLATAAVAVASSSSFADSNPIRPVTDRAASTLESAVLGALGRTRHALR"
                    + "FARFAVRYGKSYESAAEVRRRFRIFSESLEEVRSTN----RKGLPYRLGINRFSDMSWEE"
                    + "FQATRL-GAAQTCSATLAGNHLMRDA--AALPETKDWREDG-IVSPVKNQAHCGSCWTFS"
                    + "TTGALEAAYTQATGKNISLSEQQLVDCAGGFNNF--------GCNGGLPSQAFEYIKYNG"
                    + "GIDTEESYPYKGVNGV-CHYKAENAAVQVLDSVNITLNAEDELKNAVGLVRPVSVAFQVI"
                    + "DGFRQYKSGVYTSDHCGTTPDDVNHAVLAVGYGVENGV-----PYWLIKNSWGADWGDNG"
                    + "YFKMEMGKNMCAIATCASYPVVAA";

            expectedAlignment["CATH_HUMAN"] = "------MWATLPLLCAGAWLLGV--------PVCGAAELSVNSLEK------------FH"
                    + "FKSWMSKHRKTY-STEEYHHRLQTFASNWRKINAHN----NGNHTFKMALNQFSDMSFAE"
                    + "IKHKYLWSEPQNCSAT--KSNYLRGT--GPYPPSVDWRKKGNFVSPVKNQGACGSCWTFS"
                    + "TTGALESAIAIATGKMLSLAEQQLVDCAQDFNNY--------GCQGGLPSQAFEYILYNK"
                    + "GIMGEDTYPYQGKDGY-CKFQPGKAIGFVKDVANITIYDEEAMVEAVALYNPVSFAFEVT"
                    + "QDFMMYRTGIYSSTSCHKTPDKVNHAVLAVGYGEKNGI-----PYWIVKNSWGPQWGMNG"
                    + "YFLIERGKNMCGLAACASYPIPLV";

            expectedOutput.Add(expectedAlignment);

            IList<ISequenceAlignment> actualOutput = null;
            ISequenceAlignmentParser parser = new ClustalWParser();

            using (StreamReader reader = File.OpenText(filepath))
            {
                actualOutput = parser.Parse(reader);
            }

            CompareOutput(actualOutput, expectedOutput);
        }

        /// <summary>
        /// Parse sample FASTA file 186972391.fasta and verify that it is read correctly.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ClustalWParseOne()
        {
            string filepath = @"TestUtils\ClustalW\AlignmentData.aln";
            Assert.IsTrue(File.Exists(filepath));

            IList<Dictionary<string, string>> expectedOutput = new List<Dictionary<string, string>>();

            Dictionary<string, string> expectedAlignment = new Dictionary<string, string>();
            expectedAlignment["CYS1_DICDI"] = "-----MKVILLFVLAVFTVFVSS---------------RGIPPEEQ------------SQ"
                    + "FLEFQDKFNKKY-SHEEYLERFEIFKSNLGKIEELNLIAINHKADTKFGVNKFADLSSDE"
                    + "FKNYYLNNKEAIFTDDLPVADYLDDEFINSIPTAFDWRTRG-AVTPVKNQGQCGSCWSFS"
                    + "TTGNVEGQHFISQNKLVSLSEQNLVDCDHECMEYEGEEACDEGCNGGLQPNAYNYIIKNG"
                    + "GIQTESSYPYTAETGTQCNFNSANIGAKISNFTMIP-KNETVMAGYIVSTGPLAIAADAV"
                    + "E-WQFYIGGVF-DIPCN--PNSLDHGILIVGYSAKNTIFRKNMPYWIVKNSWGADWGEQG"
                    + "YIYLRRGKNTCGVSNFVSTSII--";

            expectedAlignment["ALEU_HORVU"] = "MAHARVLLLALAVLATAAVAVASSSSFADSNPIRPVTDRAASTLESAVLGALGRTRHALR"
                    + "FARFAVRYGKSYESAAEVRRRFRIFSESLEEVRSTN----RKGLPYRLGINRFSDMSWEE"
                    + "FQATRL-GAAQTCSATLAGNHLMRDA--AALPETKDWREDG-IVSPVKNQAHCGSCWTFS"
                    + "TTGALEAAYTQATGKNISLSEQQLVDCAGGFNNF--------GCNGGLPSQAFEYIKYNG"
                    + "GIDTEESYPYKGVNGV-CHYKAENAAVQVLDSVNITLNAEDELKNAVGLVRPVSVAFQVI"
                    + "DGFRQYKSGVYTSDHCGTTPDDVNHAVLAVGYGVENGV-----PYWLIKNSWGADWGDNG"
                    + "YFKMEMGKNMCAIATCASYPVVAA";

            expectedAlignment["CATH_HUMAN"] = "------MWATLPLLCAGAWLLGV--------PVCGAAELSVNSLEK------------FH"
                    + "FKSWMSKHRKTY-STEEYHHRLQTFASNWRKINAHN----NGNHTFKMALNQFSDMSFAE"
                    + "IKHKYLWSEPQNCSAT--KSNYLRGT--GPYPPSVDWRKKGNFVSPVKNQGACGSCWTFS"
                    + "TTGALESAIAIATGKMLSLAEQQLVDCAQDFNNY--------GCQGGLPSQAFEYILYNK"
                    + "GIMGEDTYPYQGKDGY-CKFQPGKAIGFVKDVANITIYDEEAMVEAVALYNPVSFAFEVT"
                    + "QDFMMYRTGIYSSTSCHKTPDKVNHAVLAVGYGEKNGI-----PYWIVKNSWGPQWGMNG"
                    + "YFLIERGKNMCGLAACASYPIPLV";

            expectedOutput.Add(expectedAlignment);

            List<ISequenceAlignment> actualOutput = new List<ISequenceAlignment>();
            ISequenceAlignment actualAlignment = null;
            ISequenceAlignmentParser parser = new ClustalWParser();

            using (StreamReader reader = File.OpenText(filepath))
            {
                actualAlignment = parser.ParseOne(reader);
            }
            actualOutput.Add(actualAlignment);
            CompareOutput(actualOutput, expectedOutput);
        }

        /// <summary>
        /// Compare the actual output to expected output
        /// </summary>
        /// <param name="actualOutput">Actual output</param>
        /// <param name="expectedOutput">Expected output</param>
        /// <returns></returns>
        private static bool CompareOutput(
                IList<ISequenceAlignment> actualOutput,
                IList<Dictionary<string, string>> expectedOutput)
        {
            if (actualOutput.Count != expectedOutput.Count)
            {
                return false;
            }

            int alignmentIndex = 0;
            foreach (ISequenceAlignment alignment in actualOutput)
            {
                Dictionary<string, string> expcetedAlignment = expectedOutput[alignmentIndex];

                foreach (Sequence actualSequence in alignment.AlignedSequences[0].Sequences)
                {
                    if (0 != string.Compare(new string(actualSequence.Select(a => (char)a).ToArray()),
                            expcetedAlignment[actualSequence.ID],
                            true, CultureInfo.CurrentCulture))
                    {
                        return false;
                    }
                }

                alignmentIndex++;
            }

            return true;
        }
    }
}
