using System.Collections.Generic;
using System.Linq;
using Bio.Algorithms.Assembly.Padena.Scaffold;
using Bio.Algorithms.Assembly.Padena.Scaffold.ContigOverlapGraph;
using Bio.Algorithms.Kmer;
using Bio.Util.Logging;
using NUnit.Framework;

namespace Bio.Padena.Tests.Scaffold
{
    /// <summary>
    /// Removes containing paths and merge overlapping paths.
    /// </summary>
    [TestFixture]
    public class PathPurgerTests
    {
        /// <summary>
        /// Initializes static member of the PathPurgerTests class.
        /// </summary>
        static PathPurgerTests()
        {
            Trace.Set(Trace.SeqWarnings);
        }

        /// <summary>
        /// Containing paths.
        /// </summary>
        [Test]
        public void PathPurger1()
        {
            const int KmerLength = 7;
            ISequence sequence = new Sequence(Alphabets.DNA, "GATTCAAGGGCTGGGGG");
            IList<ISequence> contigsSequence = SequenceToKmerBuilder.GetKmerSequences(sequence, KmerLength).ToList();
            ContigGraph graph = new ContigGraph();
            graph.BuildContigGraph(contigsSequence, KmerLength);
            List<Node> contigs = graph.Nodes.ToList();
            IList<ScaffoldPath> paths =
                new List<ScaffoldPath>();
            ScaffoldPath path = new ScaffoldPath();
            foreach (Node node in contigs)
            {
                path.Add(new KeyValuePair<Node, Edge>(node, null));
            }

            paths.Add(path);
            path = new ScaffoldPath();
            foreach (Node node in contigs.GetRange(2, 5))
            {
                path.Add(new KeyValuePair<Node, Edge>(node, null));
            }

            paths.Add(path);
            path = new ScaffoldPath();
            foreach (Node node in contigs.GetRange(3, 5))
            {
                path.Add(new KeyValuePair<Node, Edge>(node, null));
            }

            paths.Add(path);
            path = new ScaffoldPath();
            foreach (Node node in contigs.GetRange(6, 5))
            {
                path.Add(new KeyValuePair<Node, Edge>(node, null));
            }

            paths.Add(path);
            path = new ScaffoldPath();
            foreach (Node node in contigs.GetRange(0, 11))
            {
                path.Add(new KeyValuePair<Node, Edge>(node, null));
            }

            paths.Add(path);
            path = new ScaffoldPath();
            foreach (Node node in contigs.GetRange(7, 4))
            {
                path.Add(new KeyValuePair<Node, Edge>(node, null));
            }

            paths.Add(path);
            path = new ScaffoldPath();
            foreach (Node node in contigs.GetRange(11, 0))
            {
                path.Add(new KeyValuePair<Node, Edge>(node, null));
            }

            paths.Add(path);
            path = new ScaffoldPath();
            foreach (Node node in contigs.GetRange(2, 9))
            {
                path.Add(new KeyValuePair<Node, Edge>(node, null));
            }

            paths.Add(path);
            path = new ScaffoldPath();
            foreach (Node node in contigs.GetRange(1, 10))
            {
                path.Add(new KeyValuePair<Node, Edge>(node, null));
            }

            paths.Add(path);
            PathPurger assembler = new PathPurger();
            assembler.PurgePath(paths);
            Assert.AreEqual(paths.Count, 1);
            Assert.IsTrue(Compare(paths.First(), contigs));
        }

        /// <summary>
        /// Overlapping paths.
        /// </summary>
        [Test]
        public void PathPurger2()
        {
            const int KmerLength = 7;
            ISequence sequence = new Sequence(Alphabets.DNA, "GATTCAAGGGCTGGGGG");
            IList<ISequence> contigsSequence = SequenceToKmerBuilder.GetKmerSequences(sequence, KmerLength).ToList();
            ContigGraph graph = new ContigGraph();
            graph.BuildContigGraph(contigsSequence, KmerLength);
            List<Node> contigs = graph.Nodes.ToList();
            IList<ScaffoldPath> paths =
                new List<ScaffoldPath>();
            ScaffoldPath path = new ScaffoldPath();

            foreach (Node node in contigs.GetRange(0, 2))
            {
                path.Add(new KeyValuePair<Node, Edge>(node, null));
            }

            paths.Add(path);
            path = new ScaffoldPath();
            foreach (Node node in contigs.GetRange(3, 5))
            {
                path.Add(new KeyValuePair<Node, Edge>(node, null));
            }

            paths.Add(path);
            path = new ScaffoldPath();
            foreach (Node node in contigs.GetRange(6, 2))
            {
                path.Add(new KeyValuePair<Node, Edge>(node, null));
            }

            paths.Add(path);
            path = new ScaffoldPath();
            foreach (Node node in contigs.GetRange(3, 3))
            {
                path.Add(new KeyValuePair<Node, Edge>(node, null));
            }

            paths.Add(path);
            path = new ScaffoldPath();
            foreach (Node node in contigs.GetRange(7, 2))
            {
                path.Add(new KeyValuePair<Node, Edge>(node, null));
            }

            paths.Add(path);
            path = new ScaffoldPath();
            foreach (Node node in contigs.GetRange(8, 2))
            {
                path.Add(new KeyValuePair<Node, Edge>(node, null));
            }

            paths.Add(path);
            path = new ScaffoldPath();
            foreach (Node node in contigs.GetRange(2, 2))
            {
                path.Add(new KeyValuePair<Node, Edge>(node, null));
            }

            paths.Add(path);
            path = new ScaffoldPath();
            foreach (Node node in contigs.GetRange(1, 2))
            {
                path.Add(new KeyValuePair<Node, Edge>(node, null));
            }

            paths.Add(path);
            path = new ScaffoldPath();
            foreach (Node node in contigs.GetRange(8, 3))
            {
                path.Add(new KeyValuePair<Node, Edge>(node, null));
            }

            paths.Add(path);
            PathPurger assembler = new PathPurger();
            assembler.PurgePath(paths);
            Assert.AreEqual(paths.Count, 1);
            Assert.IsTrue(Compare(paths.First(), contigs));
        }

        /// <summary>
        /// Containing and Overlapping paths.
        /// </summary>
        [Test]
        public void PathPurger3()
        {
            const int KmerLength = 7;
            ISequence sequence = new Sequence(Alphabets.DNA, "GATTCAAGGGCTGGGGG");
            IList<ISequence> contigsSequence = SequenceToKmerBuilder.GetKmerSequences(sequence, KmerLength).ToList();
            ContigGraph graph = new ContigGraph();
            graph.BuildContigGraph(contigsSequence, KmerLength);
            List<Node> contigs = graph.Nodes.ToList();
            IList<ScaffoldPath> paths =
                new List<ScaffoldPath>();
            ScaffoldPath path = new ScaffoldPath();
            foreach (Node node in ((List<Node>)contigs).GetRange(0, 2))
            {
                path.Add(new KeyValuePair<Node, Edge>(node, null));
            }

            paths.Add(path);
            path = new ScaffoldPath();
            foreach (Node node in contigs.GetRange(1, 2))
            {
                path.Add(new KeyValuePair<Node, Edge>(node, null));
            }

            paths.Add(path);
            path = new ScaffoldPath();
            foreach (Node node in contigs.GetRange(8, 2))
            {
                path.Add(new KeyValuePair<Node, Edge>(node, null));
            }

            paths.Add(path);
            path = new ScaffoldPath();
            foreach (Node node in contigs.GetRange(7, 2))
            {
                path.Add(new KeyValuePair<Node, Edge>(node, null));
            }

            paths.Add(path);
            path = new ScaffoldPath();
            foreach (Node node in contigs.GetRange(7, 4))
            {
                path.Add(new KeyValuePair<Node, Edge>(node, null));
            }

            paths.Add(path);
            path = new ScaffoldPath();
            foreach (Node node in contigs.GetRange(11, 0))
            {
                path.Add(new KeyValuePair<Node, Edge>(node, null));
            }

            paths.Add(path);
            path = new ScaffoldPath();
            foreach (Node node in contigs.GetRange(2, 9))
            {
                path.Add(new KeyValuePair<Node, Edge>(node, null));
            }

            paths.Add(path);
            path = new ScaffoldPath();
            foreach (Node node in contigs.GetRange(1, 10))
            {
                path.Add(new KeyValuePair<Node, Edge>(node, null));
            }

            paths.Add(path);
            path = new ScaffoldPath();
            foreach (Node node in contigs)
            {
                path.Add(new KeyValuePair<Node, Edge>(node, null));
            }

            paths.Add(path);

            PathPurger assembler = new PathPurger();
            assembler.PurgePath(paths);
            Assert.AreEqual(paths.Count, 1);
            Assert.IsTrue(Compare(paths.First(), contigs));
        }

        /// <summary>
        /// Comapre expected result with actual result
        /// </summary>
        /// <param name="path">Scaffold paths</param>
        /// <param name="contig">Contigs</param>
        /// <returns>Returns true or false.</returns>
        private static bool Compare(ScaffoldPath path, IList<Node> contig)
        {
            if (path.Count == contig.Count)
            {
                for (int index = 0; index < contig.Count; index++)
                {
                    if (path[index].Key != contig[index])
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
