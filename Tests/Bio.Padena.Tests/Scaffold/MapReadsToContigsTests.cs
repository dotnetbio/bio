using System.Collections.Generic;
using System.Linq;
using Bio.Algorithms.Assembly.Padena;
using Bio.Algorithms.Assembly.Padena.Scaffold;
using Bio.Util.Logging;
using NUnit.Framework;

namespace Bio.Padena.Tests.Scaffold
{
    /// <summary>
    /// Maps Reads to contigs
    /// </summary>
    [TestFixture]
    public class MapReadsToContigsTests : ParallelDeNovoAssembler
    {
        static MapReadsToContigsTests()
        {
            Trace.Set(Trace.SeqWarnings);
        }

        /// <summary>
        /// Test for read contig alignment.
        /// </summary>
        [Test]
        public void MapReadToContig1()
        {
            IList<ISequence> contigs = new List<ISequence>();
            IList<ISequence> reads = new List<ISequence>();
            Sequence seq = new Sequence(Alphabets.DNA, "TCTGATAAGG".Select(a => (byte)a).ToArray());
            seq.ID = "1";
            contigs.Add(seq);
            Sequence read = new Sequence(Alphabets.DNA, "CTGATAAGG".Select(a => (byte)a).ToArray());
            read.ID = "2";
            reads.Add(read);
            const int kmerLength = 6;
            ReadContigMapper mapper = new ReadContigMapper();
            ReadContigMap map = mapper.Map(contigs, reads, kmerLength);
            Assert.AreEqual(map.Count, reads.Count);
            Dictionary<ISequence, IList<ReadMap>> alignment = map[reads[0].ID];
            IList<ReadMap> readMap = alignment[contigs[0]];
            Assert.AreEqual(readMap[0].Length, 9);
            Assert.AreEqual(readMap[0].StartPositionOfContig, 1);
            Assert.AreEqual(readMap[0].StartPositionOfRead, 0);
            Assert.AreEqual(readMap[0].ReadOverlap, ContigReadOverlapType.FullOverlap);
        }

        /// <summary>
        /// Test for read contig alignment.
        /// </summary>
        [Test]
        public void MapReadToContig2()
        {
            IList<ISequence> contigs = new List<ISequence>();
            IList<ISequence> reads = new List<ISequence>();
            Sequence seq = new Sequence(Alphabets.DNA, "TCTGATAAGG".Select(a => (byte)a).ToArray());
            seq.ID = "1";
            contigs.Add(seq);
            Sequence read = new Sequence(Alphabets.DNA, "CCTTATCAG".Select(a => (byte)a).ToArray());
            read.ID = "2";
            reads.Add(read);
            const int kmerLength = 6;
            ReadContigMapper mapper = new ReadContigMapper();
            ReadContigMap map = mapper.Map(contigs, reads, kmerLength);
            Assert.AreEqual(map.Count, reads.Count);
            Dictionary<ISequence, IList<ReadMap>> alignment = map[reads[0].ID];
            IList<ReadMap> readMap = alignment[contigs[0]];
            Assert.AreEqual(readMap[0].Length, 9);
            Assert.AreEqual(readMap[0].StartPositionOfContig, 1);
            Assert.AreEqual(readMap[0].StartPositionOfRead, 0);
            Assert.AreEqual(readMap[0].ReadOverlap, ContigReadOverlapType.FullOverlap);
        }

        /// <summary>
        /// Test for Contig Read mapping using single contig.
        /// </summary>
        [Test]
        public void MapReadsWithSingleContigRightTraversal()
        {
            const int kmerLength = 6;

            IList<ISequence> readSeqs = new List<ISequence>();
            Sequence read = new Sequence(Alphabets.DNA, "GATGCCTC".Select(a => (byte)a).ToArray());
            read.ID = "0";
            readSeqs.Add(read);
            read = new Sequence(Alphabets.DNA, "CCTCCTAT".Select(a => (byte)a).ToArray());
            read.ID = "1";
            readSeqs.Add(read);
            read = new Sequence(Alphabets.DNA, "TCCTATC".Select(a => (byte)a).ToArray());
            read.ID = "2";
            readSeqs.Add(read);
            read = new Sequence(Alphabets.DNA, "GCCTCCTAT".Select(a => (byte)a).ToArray());
            read.ID = "3";
            readSeqs.Add(read);
            read = new Sequence(Alphabets.DNA, "TGCCTCCT".Select(a => (byte)a).ToArray());
            read.ID = "4";
            readSeqs.Add(read);

            IList<ISequence> contigs = new List<ISequence> { new Sequence(Alphabets.DNA, "GATGCCTCCTATC".Select(a => (byte)a).ToArray()) };

            ReadContigMapper mapper = new ReadContigMapper();
            ReadContigMap maps = mapper.Map(contigs, readSeqs, kmerLength);

            Assert.AreEqual(maps.Count, readSeqs.Count);
            Dictionary<ISequence, IList<ReadMap>> map = maps[readSeqs[0].ID];

            IList<ReadMap> readMap = map[contigs[0]];
            Assert.AreEqual(readMap[0].Length, 8);
            Assert.AreEqual(readMap[0].StartPositionOfContig, 0);
            Assert.AreEqual(readMap[0].StartPositionOfRead, 0);
            Assert.AreEqual(readMap[0].ReadOverlap, ContigReadOverlapType.FullOverlap);

            map = maps[readSeqs[1].ID];
            readMap = map[contigs[0]];
            Assert.AreEqual(readMap[0].Length, 8);
            Assert.AreEqual(readMap[0].StartPositionOfContig, 4);
            Assert.AreEqual(readMap[0].StartPositionOfRead, 0);
            Assert.AreEqual(readMap[0].ReadOverlap, ContigReadOverlapType.FullOverlap);

            map = maps[readSeqs[2].ID];
            readMap = map[contigs[0]];
            Assert.AreEqual(readMap[0].Length, 7);
            Assert.AreEqual(readMap[0].StartPositionOfContig, 6);
            Assert.AreEqual(readMap[0].StartPositionOfRead, 0);
            Assert.AreEqual(readMap[0].ReadOverlap, ContigReadOverlapType.FullOverlap);

            map = maps[readSeqs[3].ID];
            readMap = map[contigs[0]];
            Assert.AreEqual(readMap[0].Length, 9);
            Assert.AreEqual(readMap[0].StartPositionOfContig, 3);
            Assert.AreEqual(readMap[0].StartPositionOfRead, 0);
            Assert.AreEqual(readMap[0].ReadOverlap, ContigReadOverlapType.FullOverlap);

            map = maps[readSeqs[4].ID];
            readMap = map[contigs[0]];
            Assert.AreEqual(readMap[0].Length, 8);
            Assert.AreEqual(readMap[0].StartPositionOfContig, 2);
            Assert.AreEqual(readMap[0].StartPositionOfRead, 0);
            Assert.AreEqual(readMap[0].ReadOverlap, ContigReadOverlapType.FullOverlap);
        }

        /// <summary>
        /// Test for Contig Read mapping using single contig generated by 
        /// left traversal of graph.
        /// </summary>
        [Test]
        public void MapReadsWithSingleContigLeftTraversal()
        {
            const int kmerLength = 6;
            IList<ISequence> readSeqs = new List<ISequence>();
            Sequence read = new Sequence(Alphabets.DNA, "ATGCCTC".Select(a => (byte)a).ToArray());
            read.ID = "0";
            readSeqs.Add(read);
            read = new Sequence(Alphabets.DNA, "CCTCCTAT".Select(a => (byte)a).ToArray());
            read.ID = "1";
            readSeqs.Add(read);
            read = new Sequence(Alphabets.DNA, "TCCTATC".Select(a => (byte)a).ToArray());
            read.ID = "2";
            readSeqs.Add(read);
            read = new Sequence(Alphabets.DNA, "TGCCTCCT".Select(a => (byte)a).ToArray());
            read.ID = "3";
            readSeqs.Add(read);
            read = new Sequence(Alphabets.DNA, "ATCTTAGC".Select(a => (byte)a).ToArray());
            read.ID = "4";
            readSeqs.Add(read);
            read = new Sequence(Alphabets.DNA, "CTATCTTAG".Select(a => (byte)a).ToArray());
            read.ID = "5";
            readSeqs.Add(read);
            read = new Sequence(Alphabets.DNA, "CTTAGCG".Select(a => (byte)a).ToArray());
            read.ID = "6";
            readSeqs.Add(read);
            read = new Sequence(Alphabets.DNA, "GCCTCCTAT".Select(a => (byte)a).ToArray());
            read.ID = "7";
            readSeqs.Add(read);

            IList<ISequence> contigs = new List<ISequence> { 
                new Sequence(Alphabets.DNA, "ATGCCTCCTATCTTAGCG".Select(a => (byte)a).ToArray()) };
            ReadContigMapper mapper = new ReadContigMapper();
            ReadContigMap maps = mapper.Map(contigs, readSeqs, kmerLength);
            Assert.AreEqual(maps.Count, readSeqs.Count);

            Dictionary<ISequence, IList<ReadMap>> map = maps[readSeqs[0].ID];

            IList<ReadMap> readMap = map[contigs[0]];
            Assert.AreEqual(readMap[0].Length, 7);
            Assert.AreEqual(readMap[0].StartPositionOfContig, 0);
            Assert.AreEqual(readMap[0].StartPositionOfRead, 0);
            Assert.AreEqual(readMap[0].ReadOverlap, ContigReadOverlapType.FullOverlap);

            map = maps[readSeqs[1].ID];
            readMap = map[contigs[0]];
            Assert.AreEqual(readMap[0].Length, 8);
            Assert.AreEqual(readMap[0].StartPositionOfContig, 3);
            Assert.AreEqual(readMap[0].StartPositionOfRead, 0);
            Assert.AreEqual(readMap[0].ReadOverlap, ContigReadOverlapType.FullOverlap);

            map = maps[readSeqs[2].ID];
            readMap = map[contigs[0]];
            Assert.AreEqual(readMap[0].Length, 7);
            Assert.AreEqual(readMap[0].StartPositionOfContig, 5);
            Assert.AreEqual(readMap[0].StartPositionOfRead, 0);
            Assert.AreEqual(readMap[0].ReadOverlap, ContigReadOverlapType.FullOverlap);

            map = maps[readSeqs[3].ID];
            readMap = map[contigs[0]];
            Assert.AreEqual(readMap[0].Length, 8);
            Assert.AreEqual(readMap[0].StartPositionOfContig, 1);
            Assert.AreEqual(readMap[0].StartPositionOfRead, 0);
            Assert.AreEqual(readMap[0].ReadOverlap, ContigReadOverlapType.FullOverlap);

            map = maps[readSeqs[4].ID];
            readMap = map[contigs[0]];
            Assert.AreEqual(readMap[0].Length, 8);
            Assert.AreEqual(readMap[0].StartPositionOfContig, 9);
            Assert.AreEqual(readMap[0].StartPositionOfRead, 0);
            Assert.AreEqual(readMap[0].ReadOverlap, ContigReadOverlapType.FullOverlap);

            map = maps[readSeqs[5].ID];
            readMap = map[contigs[0]];
            Assert.AreEqual(readMap[0].StartPositionOfContig, 7);
            Assert.AreEqual(readMap[0].StartPositionOfRead, 0);
            Assert.AreEqual(readMap[0].ReadOverlap, ContigReadOverlapType.FullOverlap);

            map = maps[readSeqs[6].ID];
            readMap = map[contigs[0]];
            Assert.AreEqual(readMap[0].Length, 7);
            Assert.AreEqual(readMap[0].StartPositionOfContig, 11);
            Assert.AreEqual(readMap[0].StartPositionOfRead, 0);
            Assert.AreEqual(readMap[0].ReadOverlap, ContigReadOverlapType.FullOverlap);

            map = maps[readSeqs[7].ID];
            readMap = map[contigs[0]];
            Assert.AreEqual(readMap[0].Length, 9);
            Assert.AreEqual(readMap[0].StartPositionOfContig, 2);
            Assert.AreEqual(readMap[0].StartPositionOfRead, 0);
            Assert.AreEqual(readMap[0].ReadOverlap, ContigReadOverlapType.FullOverlap);
        }

        /// <summary>
        /// Test for Contig Read mapping having two contigs generated in right traversal of graph
        /// and having partial overlap of reads.
        /// </summary>
        [Test]
        public void MapReadsWithTwoContigRightTraversal()
        {
            const int kmerLength = 6;
            IList<ISequence> readSeqs = new List<ISequence>();
            Sequence read = new Sequence(Alphabets.DNA, "GATCTGATAA".Select(a => (byte)a).ToArray());
            read.ID = "0";
            readSeqs.Add(read);
            read = new Sequence(Alphabets.DNA, "ATCTGATAAG".Select(a => (byte)a).ToArray());
            read.ID = "1";
            readSeqs.Add(read);
            read = new Sequence(Alphabets.DNA, "TCTGATAAGG".Select(a => (byte)a).ToArray());
            read.ID = "2";
            readSeqs.Add(read);
            read = new Sequence(Alphabets.DNA, "TTTTTGATGG".Select(a => (byte)a).ToArray());
            read.ID = "3";
            readSeqs.Add(read);
            read = new Sequence(Alphabets.DNA, "TTTTGATGGC".Select(a => (byte)a).ToArray());
            read.ID = "4";
            readSeqs.Add(read);
            read = new Sequence(Alphabets.DNA, "TTTGATGGCA".Select(a => (byte)a).ToArray());
            read.ID = "5";
            readSeqs.Add(read);

            IList<ISequence> contigs = new List<ISequence> { 
                new Sequence(Alphabets.DNA, "GATCTGATAAGG".Select(a => (byte)a).ToArray()), 
                new Sequence(Alphabets.DNA, "TTTTTGATGGCA".Select(a => (byte)a).ToArray()) };

            ReadContigMapper mapper = new ReadContigMapper();
            ReadContigMap maps = mapper.Map(contigs, readSeqs, kmerLength);
            Assert.AreEqual(maps.Count, readSeqs.Count);

            Dictionary<ISequence, IList<ReadMap>> map = maps[readSeqs[0].ID];

            IList<ReadMap> readMap = map[contigs[0]];
            Assert.AreEqual(readMap[0].Length, 10);
            Assert.AreEqual(readMap[0].StartPositionOfContig, 0);
            Assert.AreEqual(readMap[0].StartPositionOfRead, 0);
            Assert.AreEqual(readMap[0].ReadOverlap, ContigReadOverlapType.FullOverlap);

            map = maps[readSeqs[1].ID];
            readMap = map[contigs[0]];
            Assert.AreEqual(readMap[0].Length, 10);
            Assert.AreEqual(readMap[0].StartPositionOfContig, 1);
            Assert.AreEqual(readMap[0].StartPositionOfRead, 0);
            Assert.AreEqual(readMap[0].ReadOverlap, ContigReadOverlapType.FullOverlap);

            map = maps[readSeqs[2].ID];
            readMap = map[contigs[0]];
            Assert.AreEqual(readMap[0].Length, 10);
            Assert.AreEqual(readMap[0].StartPositionOfContig, 2);
            Assert.AreEqual(readMap[0].StartPositionOfRead, 0);
            Assert.AreEqual(readMap[0].ReadOverlap, ContigReadOverlapType.FullOverlap);

            map = maps[readSeqs[3].ID];
            readMap = map[contigs[1]];
            Assert.AreEqual(readMap[0].Length, 10);
            Assert.AreEqual(readMap[0].StartPositionOfContig, 0);
            Assert.AreEqual(readMap[0].StartPositionOfRead, 0);
            Assert.AreEqual(readMap[0].ReadOverlap, ContigReadOverlapType.FullOverlap);

            map = maps[readSeqs[4].ID];
            readMap = map[contigs[1]];
            Assert.AreEqual(readMap[0].Length, 10);
            Assert.AreEqual(readMap[0].StartPositionOfContig, 1);
            Assert.AreEqual(readMap[0].StartPositionOfRead, 0);
            Assert.AreEqual(readMap[0].ReadOverlap, ContigReadOverlapType.FullOverlap);

            map = maps[readSeqs[5].ID];
            readMap = map[contigs[1]];
            Assert.AreEqual(readMap[0].Length, 10);
            Assert.AreEqual(readMap[0].StartPositionOfContig, 2);
            Assert.AreEqual(readMap[0].StartPositionOfRead, 0);
            Assert.AreEqual(readMap[0].ReadOverlap, ContigReadOverlapType.FullOverlap);
        }
    }
}
