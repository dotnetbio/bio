using System.Collections.Generic;
using System.Linq;
using Bio.Algorithms.Assembly;
using Bio.Algorithms.Assembly.Padena;
using Bio.Algorithms.Assembly.Padena.Scaffold;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.Tests
{
    /// <summary>
    /// Calculate Distance between Contigs using mate pairs.
    /// </summary>
    [TestClass]
    public class DistanceCalculationTests : ParallelDeNovoAssembler
    {
        /// <summary>
        /// Initializes static members of the DistanceCalculationTests class.
        /// </summary>
        static DistanceCalculationTests()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("Bio.tests.log");
            }
        }

        /// <summary>
        /// Calculate Distance between contigs, both contig sequences are 
        /// in forward direction.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void DistanceCalculationwithTwoContigs()
        {
            const int KmerLength = 6;
            IList<ISequence> sequences = new List<ISequence>();
            Sequence seq = new Sequence(Alphabets.DNA, "GATCTGATAA".Select(a => (byte)a).ToArray());
            seq.ID = ">gi|263191773|ref|NG_015830.1| Homo sapiens insulin receptor"
            + "substrate 1 (IRS1) on chromosome 2.X1:0.5K";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "ATCTGATAAG".Select(a => (byte)a).ToArray());
            seq.ID = ">gi|263191773|ref|NG_015830.1| Homo sapiens insulin receptor"
            + "on chromosome 2.F:0.5K";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "TCTGATAAGG".Select(a => (byte)a).ToArray());
            seq.ID = ">gi|263191773|ref | Homo sapiens ........insulin receptor"
            + "on chromosome 2.2:0.5K";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "TTTTTGATGG".Select(a => (byte)a).ToArray());
            seq.ID = ">gi|263191773|ref|NG_015830.1| Homo sapiens insulin receptor"
            + "substrate 1 (IRS1) on chromosome 2.Y1:0.5K";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "TTTTGATGGC".Select(a => (byte)a).ToArray());
            seq.ID = ">gi|263191773|ref|NG_015830.1| Homo sapiens insulin receptor"
            + "on chromosome 2.R:0.5K";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "TTTGATGGCA".Select(a => (byte)a).ToArray());
            seq.ID = ">gi|263191773|ref | Homo sapiens ........insulin receptor"
            + "on chromosome 2.1:0.5K";
            sequences.Add(seq);

            IList<ISequence> contigs = new List<ISequence> { 
                new Sequence(Alphabets.DNA, "GATCTGATAAGG".Select(a => (byte)a).ToArray()), 
                new Sequence(Alphabets.DNA, "TTTTTGATGGCA".Select(a => (byte)a).ToArray()) };
            ReadContigMapper mapper = new ReadContigMapper();
            ReadContigMap maps = mapper.Map(contigs, sequences, KmerLength);
            MatePairMapper mapPairedReads = new MatePairMapper();
            ContigMatePairs pairs = mapPairedReads.MapContigToMatePairs(sequences, maps);

            OrientationBasedMatePairFilter filter = new OrientationBasedMatePairFilter();
            ContigMatePairs contigpairedReads = filter.FilterPairedReads(pairs);
            DistanceCalculator calc = new DistanceCalculator(contigpairedReads);
            contigpairedReads = calc.CalculateDistance();
            Assert.AreEqual(contigpairedReads.Values.Count, 1);
            Assert.IsTrue(contigpairedReads.ContainsKey(contigs[0]));

            Dictionary<ISequence, IList<ValidMatePair>> map = contigpairedReads[contigs[0]];
            Assert.IsTrue(map.ContainsKey(contigs[1]));
            IList<ValidMatePair> valid = map[contigs[1]];

            Assert.AreEqual(valid.First().DistanceBetweenContigs[0], (float)478.000031);
            Assert.AreEqual(valid.First().DistanceBetweenContigs[1], (float)477.0);
            Assert.AreEqual(valid.First().StandardDeviation[0], (float)14.1421356);
            Assert.AreEqual(valid.First().StandardDeviation[1], (float)14.1421356);
            Assert.AreEqual(valid.First().Weight, 2);
        }

        /// <summary>
        /// Contig formed in forward direction and reverse complementary of contig, 
        /// but one paired read doesn't support orientation.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void DistanceCalculationWithTwoContigsReverseComplement()
        {
            const int KmerLength = 6;
            List<ISequence> sequences = new List<ISequence>();
            Sequence seq = new Sequence(Alphabets.DNA, "GATCTGATAA".Select(a => (byte)a).ToArray());
            seq.ID = ">gi|263191773|ref|NG_015830.1| Homo sapiens insulin receptor"
            + "substrate 1 (IRS1) on chromosome 2.X1:0.5K";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "ATCTGATAAG".Select(a => (byte)a).ToArray());
            seq.ID = ">gi|263191773|ref|NG_015830.1| Homo sapiens insulin receptor"
            + "on chromosome 2.F:0.5K";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "TCTGATAAGG".Select(a => (byte)a).ToArray());
            seq.ID = ">gi|263191773|ref | Homo sapiens ........insulin receptor"
            + "on chromosome 2.2:0.5K";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "CCATCAAAAA".Select(a => (byte)a).ToArray());
            seq.ID = ">gi|263191773|ref|NG_015830.1| Homo sapiens insulin receptor"
            + "substrate 1 (IRS1) on chromosome 2.Y1:0.5K";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "TTTTGATGGC".Select(a => (byte)a).ToArray());
            seq.ID = ">gi|263191773|ref|NG_015830.1| Homo sapiens insulin receptor"
            + "on chromosome 2.R:0.5K";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "TTTGATGGCA".Select(a => (byte)a).ToArray());
            seq.ID = ">gi|263191773|ref | Homo sapiens ........insulin receptor"
            + "on chromosome 2.1:0.5K";
            sequences.Add(seq);
            IList<ISequence> contigs = new List<ISequence> { 
                new Sequence(Alphabets.DNA, "GATCTGATAAGG".Select(a => (byte)a).ToArray()), 
                new Sequence(Alphabets.DNA, "TGCCATCAAAAA".Select(a => (byte)a).ToArray()) };

            ReadContigMapper mapper = new ReadContigMapper();
            ReadContigMap maps = mapper.Map(contigs, sequences, KmerLength);
            MatePairMapper mapPairedReads = new MatePairMapper();
            ContigMatePairs pairedReads = mapPairedReads.MapContigToMatePairs(sequences, maps);
            OrientationBasedMatePairFilter filter = new OrientationBasedMatePairFilter();
            ContigMatePairs contigpairedReads = filter.FilterPairedReads(pairedReads);
            DistanceCalculator calc = new DistanceCalculator(contigpairedReads);
            contigpairedReads = calc.CalculateDistance();

            Assert.AreEqual(contigpairedReads.Values.Count, 1);
            Assert.IsTrue(contigpairedReads.ContainsKey(contigs[0]));
            Dictionary<ISequence, IList<ValidMatePair>> map = contigpairedReads[contigs[0]];
            Assert.IsTrue(map.ContainsKey(contigs[1]));

            IList<ValidMatePair> valid = map[contigs[1]];

            Assert.AreEqual(valid.First().DistanceBetweenContigs[1], (float)478.000031);
            Assert.AreEqual(valid.First().DistanceBetweenContigs[0], (float)477.0);
            Assert.AreEqual(valid.First().StandardDeviation[0], (float)14.1421356);
            Assert.AreEqual(valid.First().StandardDeviation[1], (float)14.1421356);
            Assert.AreEqual(valid.First().Weight, 2);
        }

        /// <summary>
        /// Calculate Distance between contigs, both contig sequences are 
        /// in forward direction using weighted mean.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void DistanceCalculationwithTwoContigsWeightedMean()
        {
            const int KmerLength = 6;
            List<ISequence> sequences = new List<ISequence>();
            Sequence seq = new Sequence(Alphabets.DNA, "GATCTGATAA".Select(a => (byte)a).ToArray());
            seq.ID = ">gi|263191773|ref|NG_015830.1| Homo sapiens insulin receptor"
            + "substrate 1 (IRS1) on chromosome 2.x1:2K";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "ATCTGATAAG".Select(a => (byte)a).ToArray());
            seq.ID = ">gi|263191773|ref|NG_015830.1| Homo sapiens insulin receptor"
            + "on chromosome 2.f:0.5K";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "TCTGATAAGG".Select(a => (byte)a).ToArray());
            seq.ID = ">gi|263191773|ref | Homo sapiens ........insulin receptor"
            + "on chromosome 2.2:0.5K";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "TTTTTGATGG".Select(a => (byte)a).ToArray());
            seq.ID = ">gi|263191773|ref|NG_015830.1| Homo sapiens insulin receptor"
            + "substrate 1 (IRS1) on chromosome 2.y1:2K";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "TTTTGATGGC".Select(a => (byte)a).ToArray());
            seq.ID = ">gi|263191773|ref|NG_015830.1| Homo sapiens insulin receptor"
            + "on chromosome 2.r:0.5K";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "TTTGATGGCA".Select(a => (byte)a).ToArray());
            seq.ID = ">gi|263191773|ref | Homo sapiens ........insulin receptor"
            + "on chromosome 2.1:0.5K";
            sequences.Add(seq);
            IList<ISequence> contigs = new List<ISequence> { 
                new Sequence(Alphabets.DNA, "GATCTGATAAGG".Select(a => (byte)a).ToArray()), 
                new Sequence(Alphabets.DNA, "TTTTTGATGGCA".Select(a => (byte)a).ToArray()) };

            ReadContigMapper mapper = new ReadContigMapper();
            ReadContigMap maps = mapper.Map(contigs, sequences, KmerLength);

            MatePairMapper mapPairedReads = new MatePairMapper();
            ContigMatePairs pairedReads = mapPairedReads.MapContigToMatePairs(sequences, maps);
            OrientationBasedMatePairFilter filter = new OrientationBasedMatePairFilter();
            ContigMatePairs contigpairedReads = filter.FilterPairedReads(pairedReads);
            DistanceCalculator calc = new DistanceCalculator(contigpairedReads);
            contigpairedReads = calc.CalculateDistance();
            Assert.AreEqual(contigpairedReads.Values.Count, 1);
            Assert.IsTrue(contigpairedReads.ContainsKey(contigs[0]));

            Dictionary<ISequence, IList<ValidMatePair>> map = contigpairedReads[contigs[0]];
            Assert.IsTrue(map.ContainsKey(contigs[1]));
            IList<ValidMatePair> valid = map[contigs[1]];

            Assert.AreEqual(valid.First().DistanceBetweenContigs[0], (float)1228.0);
            Assert.AreEqual(valid.First().DistanceBetweenContigs[1], (float)1227.0);
            Assert.AreEqual(valid.First().StandardDeviation[0], (float)60);
            Assert.AreEqual(valid.First().StandardDeviation[1], (float)60);
            Assert.AreEqual(valid.First().Weight, 2);
        }
    }
}
