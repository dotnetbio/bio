/****************************************************************************
 * PhylogeneticTreeBvtTestCases.cs
 * 
 *   This file contains the PhylogeneticTree - Parsers and Formatters Bvt test cases.
 * 
***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Bio;
using Bio.IO.Newick;
using Bio.Phylogenetics;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;
using NUnit.Framework;
using System.Linq;

namespace Bio.TestAutomation.IO.Newick
{
    /// <summary>
    /// Phylogenetic Tree Bvt parser and formatter Test case implementation.
    /// </summary>
    [TestFixture]
    public class PhylogeneticTreeBvtTestCases
    {
        #region Enums

        /// <summary>
        /// Additional Parameters which are used for different test cases 
        /// based on which the test cases are executed.
        /// </summary>
        enum AdditionalParameters
        {
            Stream,
            StringBuilder,
            Default
        };

        /// <summary>
        /// Formatter Parameters which are used for different test cases 
        /// based on which the test cases are executed.
        /// </summary>
        enum FormatterParameters
        {
            Object,
            Stream,
            FormatString,
            Default
        };

        #endregion Enums

        #region Global Variables

        Utility _utilityObj = new Utility(@"TestUtils\PhylogeneticTestsConfig.xml");

        #endregion Global Variables

        #region PhylogeneticTree Parser Bvt Test cases

        /// <summary>
        /// Parse a valid Phylogenetic File and validate the Node Name and distance.
        /// Input : Phylogenetic Tree
        /// Validation : Root Branch Count, Node Name and Edge Distance.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void PhylogeneticTreeBvtParserValidateOneLineParse()
        {
            PhylogeneticTreeParserGeneralTests(Constants.OneLinePhyloTreeNodeName,
                AdditionalParameters.Default);
        }

        /// <summary>
        /// Parse a valid small sizePhylogenetic File and validate the Node Name and distance.
        /// Input : Phylogenetic Tree
        /// Validation : Root Branch Count, Node Name and Edge Distance.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void PhylogeneticTreeBvtParserValidateSmallSizeParse()
        {
            PhylogeneticTreeParserGeneralTests(Constants.SmallSizePhyloTreeNodeName,
                AdditionalParameters.Default);
        }

        /// <summary>
        /// Parse a valid Phylogenetic File using TextReader and validate the Node Name and distance.
        /// Input : Phylogenetic Tree
        /// Validation : Root Branch Count, Node Name and Edge Distance.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void PhylogeneticTreeBvtParserValidateParseTextReader()
        {
            PhylogeneticTreeParserGeneralTests(Constants.OneLinePhyloTreeNodeName,
                AdditionalParameters.Stream);
        }

        /// <summary>
        /// Parse a valid Phylogenetic File contain '\n', '\r' using TextReader
        /// and validate the Node Name and distance.
        /// Input : Phylogenetic Tree
        /// Validation : Root Branch Count, Node Name and Edge Distance.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void PhylogeneticTreeBvtParserValidateWithSpecialChar()
        {
            PhylogeneticTreeParserGeneralTests(
                Constants.SpecialCharSmallSizePhyloTreeNode,
                AdditionalParameters.Stream);
        }


        /// <summary>
        /// Parse a new extended file format tree and verify that it 
        /// Input : Phylogenetic Tree
        /// Validation : Root Branch Count, Node Name and Edge Distance.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void PhylogeneticTreeBvtParserValidateNewickExtended()
        {
            NewickParser parser = new NewickParser();
            {
                using (var reader = File.OpenRead(@"TestUtils\NewickExtended.nhx"))
                {
                    Tree rootTree = parser.Parse(reader);

                    //Verify metadata at root
                    Assert.AreEqual("40",rootTree.Root.MetaData["N"]);
                    //Verify name at root
                    Assert.AreEqual("Euteleostomi",rootTree.Root.Name);
                    //now verify it also worked for a somewhat arbitrary internal node
                    var internalNode = rootTree.Root.Children.Keys.First().Children.Keys.First();
                    Assert.AreEqual("Tetrapoda", internalNode.Name);
                    Assert.AreEqual("0.00044378",internalNode.MetaData["PVAL"]);
                    Assert.AreEqual(8,internalNode.MetaData.Count);
                }
                
            }
        }

        /// <summary>
        /// Parse a valid Phylogenetic File using StringBuilder and validate the Node Name and distance.
        /// Input : Phylogenetic Tree
        /// Validation : Root Branch Count, Node Name and Edge Distance.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void PhylogeneticTreeBvtParserValidateParseStringBuilder()
        {
            PhylogeneticTreeParserGeneralTests(Constants.OneLinePhyloTreeNodeName,
                AdditionalParameters.StringBuilder);
        }

        #endregion PhylogeneticTree Parser BVT Test cases

        #region Phylogenetic Formatter Bvt Test cases

        /// <summary>
        /// Format a valid Phylogenetic tree and validate the Node Name and distance.
        /// Input : Phylogenetic Tree Object
        /// Validation : Root Branch Count, Node Name and Edge Distance.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void PhylogeneticTreeBvtFormatterValidateObject()
        {
            PhylogeneticTreeFormatterGeneralTests(Constants.OneLinePhyloTreeObjectNodeName,
                FormatterParameters.Object);
        }

        /// <summary>
        /// Format a valid Phylogenetic tree which is parsed from a file 
        /// and validate the Node Name and distance.
        /// Input : Phylogenetic Tree, one line
        /// Validation : Root Branch Count, Node Name and Edge Distance.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void PhylogeneticTreeBvtFormatterValidateOneLine()
        {
            PhylogeneticTreeFormatterGeneralTests(Constants.OneLinePhyloTreeNodeName,
                FormatterParameters.Default);
        }

        /// <summary>
        /// Format a valid Phylogenetic tree which is parsed from a small size file 
        /// and validate the Node Name and distance.
        /// Input : Phylogenetic Tree, one line
        /// Validation : Root Branch Count, Node Name and Edge Distance.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void PhylogeneticTreeBvtFormatterValidateSmallSize()
        {
            PhylogeneticTreeFormatterGeneralTests(Constants.SmallSizePhyloTreeNodeName,
                FormatterParameters.Default);
        }

        /// <summary>
        /// Format a valid Phylogenetic tree using text writer which is 
        /// parsed from a small size file and validate the Node Name and distance.
        /// Input : Phylogenetic Tree, one line
        /// Validation : Root Branch Count, Node Name and Edge Distance.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void PhylogeneticTreeBvtFormatterValidateTextWriter()
        {
            PhylogeneticTreeFormatterGeneralTests(Constants.SmallSizePhyloTreeNodeName,
                FormatterParameters.Stream);
        }

        /// <summary>
        /// Format a valid Phylogenetic tree using file name which is parsed from 
        /// a small size file and validate the Node Name and distance.
        /// Input : Phylogenetic Tree, one line
        /// Validation : Root Branch Count, Node Name and Edge Distance.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void PhylogeneticTreeBvtFormatterValidateFileName()
        {
            PhylogeneticTreeFormatterGeneralTests(Constants.SmallSizePhyloTreeNodeName,
                FormatterParameters.Default);
        }

        /// <summary>
        /// FormatString a valid Phylogenetic tree using file name which is parsed from 
        /// a small size file and validate the Node Name and distance.
        /// Input : Phylogenetic Tree, one line
        /// Validation : Root Branch Count, Node Name and Edge Distance.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void PhylogeneticTreeBvtFormatterValidateFormatString()
        {
            PhylogeneticTreeFormatterGeneralTests(Constants.SmallSizePhyloTreeNodeName,
                FormatterParameters.FormatString);
        }

        /// <summary>
        /// Check for internal nodes with names. This is new for 1.1
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestNamesOnInternalNodes()
        {
            const string filename = @"TestUtils\\positives.newick";
            var parser = new NewickParser();
            var tree = parser.Parse(filename);
            Assert.AreEqual(3, tree.Root.Children.Count);
        }

        #endregion Phylogenetic Formatter Bvt Test cases

        #region Supported Methods

        /// <summary>
        /// Phylogenetic Tree Parsers General Tests
        /// </summary>
        /// <param name="nodeName">Xml node Name.</param>
        /// <param name="addParam">Additional parameter</param>
        void PhylogeneticTreeParserGeneralTests(string nodeName, AdditionalParameters addParam)
        {
            // Gets the expected sequence from the Xml
            string filePath = _utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.FilePathNode);

            Assert.IsTrue(File.Exists(filePath));

            NewickParser parser = new NewickParser();
            {
                Tree rootTree = null;

                switch (addParam)
                {
                    case AdditionalParameters.Stream:
                        using (var reader = File.OpenRead(filePath))
                        {
                            rootTree = parser.Parse(reader);
                        }
                        break;
                    case AdditionalParameters.StringBuilder:
                        using (StreamReader reader = File.OpenText(filePath))
                        {
                            StringBuilder strBuilderObj = new StringBuilder(reader.ReadToEnd());
                            rootTree = parser.Parse(strBuilderObj);
                        }
                        break;
                    default:
                        rootTree = parser.Parse(filePath);
                        break;
                }

                Node rootNode = rootTree.Root;

                string rootBranchCount = _utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.RootBranchCountNode);

                // Validate the root branch count
                Assert.AreEqual(rootBranchCount,
                    rootNode.Children.Count.ToString((IFormatProvider)null));

                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "Phylogenetic Tree Parser BVT: Number of Root Branches found are '{0}'.",
                    rootNode.Children.Count.ToString((IFormatProvider)null)));

                List<string> leavesName = new List<string>();
                List<double> leavesDistance = new List<double>();

                // Gets all the nodes and edges
                GetAllNodesAndEdges(rootNode, ref leavesName, ref leavesDistance);

                string[] expectedLeavesName = _utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.NodeNamesNode).Split(',');

                string[] expectedLeavesDistance = _utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.EdgeDistancesNode).Split(',');

                for (int i = 0; i < expectedLeavesName.Length; i++)
                {
                    Assert.AreEqual(expectedLeavesName[i], leavesName[i]);
                }

                for (int i = 0; i < expectedLeavesDistance.Length; i++)
                {
                    Assert.AreEqual(expectedLeavesDistance[i],
                        leavesDistance[i].ToString((IFormatProvider)null));
                }

            }
            ApplicationLog.WriteLine(
                "Phylogenetic Tree Parser BVT: The Node Names and Distance are as expected.");
        }

        /// <summary>
        /// Phylogenetic Tree Formatter General Tests
        /// </summary>
        /// <param name="nodeName">Xml node Name.</param>
        /// <param name="formatParam">Additional parameter</param>
        void PhylogeneticTreeFormatterGeneralTests(string nodeName,
            FormatterParameters formatParam)
        {
            // Gets the expected sequence from the Xml
            string filePath = string.Empty;

            NewickParser parser = new NewickParser();
            {
                Tree rootTree;

                switch (formatParam)
                {
                    case FormatterParameters.Object:
                        rootTree = GetFormattedObject();
                        break;
                    default:
                        filePath = _utilityObj.xmlUtil.GetTextValue(nodeName,
                         Constants.FilePathNode);
                        Assert.IsTrue(File.Exists(filePath));

                        // Logs information to the log file
                        ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                            "Phylogenetic Tree Parser BVT: File Exists in the Path '{0}'.",
                            filePath));

                        rootTree = parser.Parse(filePath);
                        break;
                }

                string outputFilepath = _utilityObj.xmlUtil.GetTextValue(nodeName,
                         Constants.OutputFilePathNode);

                NewickFormatter format = new NewickFormatter();
                switch (formatParam)
                {
                    case FormatterParameters.Stream:
                        using (var writer = File.Create(outputFilepath))
                        {
                            format.Format(writer, rootTree);
                        }
                        break;
                    case FormatterParameters.FormatString:
                        // Validate format String
                        var formatString = format.FormatString(rootTree);

                        string expectedFormatString =
                            _utilityObj.xmlUtil.GetTextValue(nodeName,
                            Constants.FormatStringNode);

                        Assert.AreEqual(expectedFormatString, formatString);

                        ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                            "Phylogenetic Tree Parser BVT: Format string '{0}' is as expected.",
                            formatString));
                        break;
                    default:
                        format.Format(rootTree, outputFilepath);
                        break;
                }

                // Validate only if not a Format String
                if (FormatterParameters.FormatString != formatParam)
                {
                    // Re-parse the created file and validate the tree.
                    NewickParser newparserObj = new NewickParser();
                    {
                        Tree newrootTreeObj = null;

                        Assert.IsTrue(File.Exists(outputFilepath));

                        // Logs information to the log file
                        ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                            "Phylogenetic Tree Parser BVT: New File Exists in the Path '{0}'.",
                            outputFilepath));

                        newrootTreeObj = newparserObj.Parse(outputFilepath);

                        Node rootNode = newrootTreeObj.Root;

                        string rootBranchCount = _utilityObj.xmlUtil.GetTextValue(nodeName,
                           Constants.RootBranchCountNode);

                        // Validate the root branch count
                        Assert.AreEqual(rootBranchCount,
                            rootNode.Children.Count.ToString((IFormatProvider)null));

                        ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                            "Phylogenetic Tree Parser BVT: Number of Root Branches found are '{0}'.",
                            rootNode.Children.Count.ToString((IFormatProvider)null)));

                        List<string> leavesName = new List<string>();
                        List<double> leavesDistance = new List<double>();

                        // Gets all the leaves in the root node list
                        GetAllNodesAndEdges(rootNode, ref leavesName, ref leavesDistance);

                        string[] expectedLeavesName = _utilityObj.xmlUtil.GetTextValue(nodeName,
                           Constants.NodeNamesNode).Split(',');

                        string[] expectedLeavesDistance = _utilityObj.xmlUtil.GetTextValue(nodeName,
                           Constants.EdgeDistancesNode).Split(',');

                        for (int i = 0; i < expectedLeavesName.Length; i++)
                        {
                            Assert.AreEqual(expectedLeavesName[i], leavesName[i]);
                        }

                        for (int i = 0; i < expectedLeavesDistance.Length; i++)
                        {
                            Assert.AreEqual(expectedLeavesDistance[i],
                                leavesDistance[i].ToString((IFormatProvider)null));
                        }
                    }

                    ApplicationLog.WriteLine(
                        "Phylogenetic Tree Parser BVT: The Node Names and Distance are as expected.");
                }

                if (File.Exists(outputFilepath))
                    File.Delete(outputFilepath);
            }
        }

        /// <summary>
        /// Gets the leaves for the entire phylogenetic tree.
        /// </summary>
        /// <param name="nodeList">Phylogenetic tree node list.</param>
        /// <param name="nodeName">Node names</param>
        /// <param name="distance">edge distances</param>
        void GetAllNodesAndEdges(Node rootNode,
              ref List<string> nodeName, ref List<double> distance)
        {
            IList<Edge> edges = rootNode.Edges;

            if (null != edges)
            {
                // Get all the edges distances
                foreach (Edge ed in edges)
                {
                    distance.Add(ed.Distance);
                }
            }

            // Gets all the nodes
            IList<Node> nodes = rootNode.Nodes;

            if (null != nodes)
            {
                foreach (Node nd in nodes)
                {
                    if (nd.IsLeaf)
                    {
                        nodeName.Add(nd.Name);
                    }
                    else
                    {
                        // Get the nodes and edges recursively until 
                        // all the nodes and edges are retrieved.
                        GetAllNodesAndEdges(nd, ref nodeName, ref distance);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the formatted object which is used for formatting
        /// </summary>
        /// <returns>Formatter tree.</returns>
        private static Tree GetFormattedObject()
        {
            // Tree with three nodes are created
            Node nd1 = new Node();
            nd1.Name = "a";
            Edge ed1 = new Edge();
            ed1.Distance = 0.1;

            Node nd2 = new Node();
            nd2.Name = "b";
            Edge ed2 = new Edge();
            ed2.Distance = 0.2;

            Node nd3 = new Node();
            nd3.Name = "c";
            Edge ed3 = new Edge();
            ed3.Distance = 0.3;

            Node nd4 = new Node();
            nd4.Children.Add(nd1, ed1);
            Edge ed4 = new Edge();
            ed4.Distance = 0.4;

            Node nd5 = new Node();
            nd5.Children.Add(nd2, ed2);
            nd5.Children.Add(nd3, ed3);
            Edge ed5 = new Edge();
            ed5.Distance = 0.5;

            Node ndRoot = new Node();
            ndRoot.Children.Add(nd4, ed4);
            ndRoot.Children.Add(nd5, ed5);

            // All the Node and Edges are combined to form a tree
            Tree baseTree = new Tree();
            baseTree.Root = ndRoot;

            return baseTree;
        }

        #endregion Supported Methods
    }
}
