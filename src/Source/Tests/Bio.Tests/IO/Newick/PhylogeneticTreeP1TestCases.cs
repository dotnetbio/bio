/****************************************************************************
 * PhylogeneticTreeP1TestCases.cs
 * 
 *   This file contains the PhylogeneticTree - Parsers and Formatters P1 test cases.
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

namespace Bio.TestAutomation.IO.Newick
{
    /// <summary>
    /// Phylogenetic Tree P1 parser and formatter Test case implementation.
    /// </summary>
    [TestFixture]
    public class PhylogeneticTreeP1TestCases
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
            ObjectSingleNode,
            ParseTextReader,
            ParseStringBuilder,
            Default
        };

        #endregion Enums

        #region Global Variables

        Utility _utilityObj = new Utility(@"TestUtils\PhylogeneticTestsConfig.xml");

        #endregion Global Variables

        #region PhylogeneticTree Parser P1 Test cases

        /// <summary>
        /// Parse a valid Phylogenetic File with multiple nodes
        /// and validate the Node Name and distance.
        /// Input : Phylogenetic Tree
        /// Validation : Root Branch Count, Node Name and Edge Distance.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PhylogeneticTreeP1ParserValidateMultipleNode()
        {
            PhylogeneticTreeParserGeneralTests(Constants.SmallSizePhyloTreeNodeName,
                AdditionalParameters.Default);
        }

        /// <summary>
        /// Parse a valid small sizePhylogenetic File and validate the Node Name and distance.
        /// Input : Phylogenetic Tree
        /// Validation : Root Branch Count, Node Name and Edge Distance.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PhylogeneticTreeP1ParserValidateOneNodeAndNode()
        {
            PhylogeneticTreeParserGeneralTests(Constants.OneNodePhyloTreeNodeName,
                AdditionalParameters.Default);
        }

        /// <summary>
        /// Parse a validate all properties.
        /// Input : Phylogenetic Tree
        /// Validation : All properties.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PhylogeneticTreeP1ParserValidateAllProperties()
        {
            NewickParser parser = new NewickParser();
            {
                string name = parser.Name;
                string fileTypes = parser.SupportedFileTypes;
                string description = parser.Description;
                Assert.AreEqual(Constants.ParserName, name);
                Assert.AreEqual(Constants.ParserFileTypes, fileTypes);
                Assert.AreEqual(Constants.ParserDescription, description);
            }

            ApplicationLog.WriteLine(
                "Phylogenetic Tree Parser P1: Validated all properties in Parser class.");
        }

        /// <summary>
        /// Parse a valid Phylogenetic File which has Node distance greater than one
        /// and validate the Node Name and distance.
        /// Input : Phylogenetic Tree
        /// Validation : Root Branch Count, Node Name and Edge Distance.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PhylogeneticTreeP1ParserValidateDistanceGreaterThanOne()
        {
            PhylogeneticTreeParserGeneralTests(Constants.OneLineGreaterThanOnePhyloTreeNodeName,
                AdditionalParameters.Default);
        }

        /// <summary>
        /// Parse a valid Phylogenetic File with decimal distances
        /// and validate the Node Name and distance.
        /// Input : Phylogenetic Tree
        /// Validation : Root Branch Count, Node Name and Edge Distance.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PhylogeneticTreeP1ParserValidateDecimalDistance()
        {
            PhylogeneticTreeParserGeneralTests(Constants.SmallSizePhyloTreeNodeName,
                AdditionalParameters.Default);
        }

        #endregion PhylogeneticTree Parser P1 Test cases

        #region Phylogenetic Formatter P1 Test cases

        /// <summary>
        /// Parse a validate all properties.
        /// Input : Phylogenetic Tree
        /// Validation : All properties.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PhylogeneticTreeP1FormatterValidateAllProperties()
        {
            NewickFormatter formatter = new NewickFormatter();

            string name = formatter.Name;
            string fileTypes = formatter.SupportedFileTypes;
            string description = formatter.Description;
            Assert.AreEqual(Constants.ParserName, name);
            Assert.AreEqual(Constants.ParserFileTypes, fileTypes);
            Assert.AreEqual(Constants.FormatDescription, description);
            ApplicationLog.WriteLine(
                "Phylogenetic Tree Formatter P1: Validated all properties in Formatter class.");
        }

        /// <summary>
        /// Format a valid Phylogenetic tree object and validate the Node Name and distance.
        /// Input : Phylogenetic Tree Object
        /// Validation : Root Branch Count, Node Name and Edge Distance.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PhylogeneticTreeP1FormatterValidateObjectSingleNode()
        {
            PhylogeneticTreeFormatterGeneralTests(Constants.OneNodePhyloTreeObjectNodeName,
                FormatterParameters.ObjectSingleNode);
        }

        /// <summary>
        /// Format a valid Phylogenetic tree and validate the Node Name and distance.
        /// Input : Phylogenetic Tree Object
        /// Validation : Root Branch Count, Node Name and Edge Distance.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PhylogeneticTreeP1FormatterValidateObjectMultipleNode()
        {
            PhylogeneticTreeFormatterGeneralTests(Constants.OneLinePhyloTreeObjectNodeName,
                FormatterParameters.Object);
        }

        /// <summary>
        /// FormatString a valid Phylogenetic tree using file name which is parsed from 
        /// a small size file and validate the Node Name and distance.
        /// Input : Phylogenetic Tree, one line
        /// Validation : Root Branch Count, Node Name and Edge Distance.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PhylogeneticTreeP1FormatterValidateFormatStringMultipleNodes()
        {
            PhylogeneticTreeFormatterGeneralTests(Constants.SmallSizePhyloTreeNodeName,
                FormatterParameters.FormatString);
        }

        /// <summary>
        /// FormatString a valid Phylogenetic tree using file name which is parsed from 
        /// a small size file and validate the Node Name and distance.
        /// Input : Phylogenetic Tree, one line
        /// Validation : Root Branch Count, Node Name and Edge Distance.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PhylogeneticTreeP1FormatterValidateFormatStringSingleNode()
        {
            PhylogeneticTreeFormatterGeneralTests(Constants.OneNodePhyloTreeNodeName,
                FormatterParameters.FormatString);
        }

        /// <summary>
        /// Format a valid Phylogenetic tree using Parse Text Reader
        /// and validate the Node Name and distance.
        /// Input : Phylogenetic Tree Object
        /// Validation : Root Branch Count, Node Name and Edge Distance.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PhylogeneticTreeP1FormatterValidateParseTextReader()
        {
            PhylogeneticTreeFormatterGeneralTests(Constants.SmallSizePhyloTreeNodeName,
                FormatterParameters.ParseTextReader);
        }

        /// <summary>
        /// Format a valid Phylogenetic tree using Parse String Builder
        /// and validate the Node Name and distance.
        /// Input : Phylogenetic Tree Object
        /// Validation : Root Branch Count, Node Name and Edge Distance.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PhylogeneticTreeP1FormatterValidateParseStringBuilder()
        {
            PhylogeneticTreeFormatterGeneralTests(Constants.SmallSizePhyloTreeNodeName,
                FormatterParameters.ParseStringBuilder);
        }

        /// <summary>
        /// Format a valid Phylogenetic tree using Parse file-name
        /// and validate the Node Name and distance.
        /// Input : Phylogenetic Tree Object
        /// Validation : Root Branch Count, Node Name and Edge Distance.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PhylogeneticTreeP1FormatterValidateParseFileName()
        {
            PhylogeneticTreeFormatterGeneralTests(Constants.SmallSizePhyloTreeNodeName,
                FormatterParameters.Default);
        }

        /// <summary>
        /// Format a valid Phylogenetic tree using Parse file-name
        /// with distance greater than one and validate the Node Name and distance.
        /// Input : Phylogenetic Tree Object
        /// Validation : Root Branch Count, Node Name and Edge Distance.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PhylogeneticTreeP1FormatterValidateDistanceGreaterThanOne()
        {
            PhylogeneticTreeFormatterGeneralTests(Constants.OneLineGreaterThanOnePhyloTreeNodeName,
                FormatterParameters.Default);
        }

        /// <summary>
        /// Format a valid Phylogenetic tree using Parse file-name
        /// with distance in decimal and validate the Node Name and distance.
        /// Input : Phylogenetic Tree Object
        /// Validation : Root Branch Count, Node Name and Edge Distance.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void PhylogeneticTreeP1FormatterValidateDistanceInDecimal()
        {
            PhylogeneticTreeFormatterGeneralTests(Constants.SmallSizePhyloTreeNodeName,
                FormatterParameters.Default);
        }

        #endregion Phylogenetic Formatter P1 Test cases

        #region Supported Methods

        /// <summary>
        /// Phylogenetic Tree Parsers General Tests
        /// </summary>
        /// <param name="nodeName">Xml node Name.</param>
        /// <param name="addParam">Additional parameter</param>
        void PhylogeneticTreeParserGeneralTests(string nodeName,
            AdditionalParameters addParam)
        {
            // Gets the expected sequence from the Xml
            string filePath = _utilityObj.xmlUtil.GetTextValue(nodeName,
                Constants.FilePathNode);

            Assert.IsTrue(File.Exists(filePath));

            // Logs information to the log file
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Phylogenetic Tree Parser P1: File Exists in the Path '{0}'.", filePath));

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
                            StringBuilder strBuilderObj = new StringBuilder(
                                reader.ReadToEnd());
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
                    "Phylogenetic Tree Parser P1: Number of Root Branches found are '{0}'.",
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
                "Phylogenetic Tree Parser P1: The Node Names and Distance are as expected.");
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
                Tree rootTree = null;

                switch (formatParam)
                {
                    case FormatterParameters.Object:
                        rootTree = GetFormattedObject(false);
                        break;
                    case FormatterParameters.ObjectSingleNode:
                        rootTree = GetFormattedObject(true);
                        break;
                    case FormatterParameters.ParseTextReader:
                        filePath = _utilityObj.xmlUtil.GetTextValue(nodeName,
                            Constants.FilePathNode);
                        Assert.IsTrue(File.Exists(filePath));

                        // Logs information to the log file
                        ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                            "Phylogenetic Tree Formatter P1: File Exists in the Path '{0}'.",
                            filePath));
                        using (var reader = File.OpenRead(filePath))
                        {
                            rootTree = parser.Parse(reader);
                        }
                        break;
                    case FormatterParameters.ParseStringBuilder:
                        filePath = _utilityObj.xmlUtil.GetTextValue(nodeName,
                            Constants.FilePathNode);
                        Assert.IsTrue(File.Exists(filePath));

                        // Logs information to the log file
                        ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                            "Phylogenetic Tree Formatter P1: File Exists in the Path '{0}'.",
                            filePath));

                        using (StreamReader reader = File.OpenText(filePath))
                        {
                            StringBuilder strBuilderObj =
                                new StringBuilder(reader.ReadToEnd());
                            rootTree = parser.Parse(strBuilderObj);
                        }
                        break;
                    default:
                        filePath = _utilityObj.xmlUtil.GetTextValue(nodeName,
                            Constants.FilePathNode);
                        Assert.IsTrue(File.Exists(filePath));

                        // Logs information to the log file
                        ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                            "Phylogenetic Tree Formatter P1: File Exists in the Path '{0}'.",
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
                        string formatString = format.FormatString(rootTree);

                        string expectedFormatString = _utilityObj.xmlUtil.GetTextValue(nodeName,
                            Constants.FormatStringNode);

                        Assert.AreEqual(expectedFormatString, formatString);

                        ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                            "Phylogenetic Tree Formatter P1: Format string '{0}' is as expected.",
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
                            "Phylogenetic Tree Formatter P1: New File Exists in the Path '{0}'.",
                            outputFilepath));

                        newrootTreeObj = newparserObj.Parse(outputFilepath);

                        Node rootNode = newrootTreeObj.Root;

                        string rootBranchCount = _utilityObj.xmlUtil.GetTextValue(nodeName,
                            Constants.RootBranchCountNode);

                        // Validate the root branch count
                        Assert.AreEqual(rootBranchCount,
                            rootNode.Children.Count.ToString((IFormatProvider)null));

                        ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                            "Phylogenetic Tree Formatter P1: Number of Root Branches found are '{0}'.",
                            rootNode.Children.Count.ToString((IFormatProvider)null)));

                        List<string> leavesName = new List<string>();
                        List<double> leavesDistance = new List<double>();

                        // Gets all the leaves in the root node list
                        GetAllNodesAndEdges(rootNode, ref leavesName,
                            ref leavesDistance);

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
                            Assert.AreEqual(expectedLeavesDistance[i], leavesDistance[i].ToString((IFormatProvider)null));
                        }

                        ApplicationLog.WriteLine(
                            "Phylogenetic Tree Parser P1: The Node Names and Distance are as expected.");
                    }

                    if (File.Exists(outputFilepath))
                        File.Delete(outputFilepath);
                }
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
        /// <param name="isSingleNode">Is Single Node?</param>
        /// <returns>Formatter tree.</returns>
        private static Tree GetFormattedObject(bool isSingleNode)
        {
            Tree baseTree = new Tree();

            // Tree with three nodes are created
            Node nd1 = new Node();
            nd1.Name = "a";
            Edge ed1 = new Edge();
            ed1.Distance = 0.1;

            if (isSingleNode)
            {
                Node singleNode = new Node();
                singleNode.Children.Add(nd1, ed1);
                baseTree.Root = singleNode;
            }
            else
            {
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
                baseTree.Root = ndRoot;
            }

            return baseTree;
        }

        #endregion Supported Methods
    }
}
