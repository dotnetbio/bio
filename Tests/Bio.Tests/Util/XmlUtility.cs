/****************************************************************************
 * XmlUtility.cs
 * 
 *   This file contains the all the xml related functions.
 * 
***************************************************************************/

using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Bio.Tests;

namespace Bio.TestAutomation.Util
{
    /// <summary>
    /// This class contains the all the xml related functions.
    /// </summary>
    public class XmlUtility
    {
        readonly XDocument _document;

        /// <summary>
        /// Constructor which sets the config file.
        /// </summary>
        /// <param name="xmlFilePath">Config file path.</param>
        public XmlUtility(string xmlFilePath)
        {
            string path = xmlFilePath.TestDir();
            _document = XDocument.Load(path);
        }

        /// <summary>
        /// Returns the Text value for the nodes specified from the configuration file.
        /// </summary>
        /// <param name="parentNode">Name of the Parent node.</param>
        /// <param name="nodeName">Name of the node, from which the text value to be read.</param>
        /// <returns>Text with in the node.</returns>
        public string GetTextValue(string parentNode, string nodeName)
        {
            XElement actualNode = GetNode(parentNode, nodeName);
            return actualNode?.Value;
        }

        /// <summary>
        /// Returns the contents of the file for the Path specified.
        /// </summary>
        /// <param name="parentNode">Name of the Parent node.</param>
        /// <param name="nodeName">Name of the node, from which the file path to be read.</param>
        /// <returns>Contents of file for the path specified.</returns>
        public string GetFileTextValue(string parentNode, string nodeName)
        {
            XElement actualNode = GetNode(parentNode, nodeName);
            return Utility.GetFileContent(actualNode.Value);
        }

        /// <summary>
        /// Returns the node from the configuration file.
        /// </summary>
        /// <param name="parentNode">Name of the Parent node</param>
        /// <param name="nodeName">Name of the node, which needs to be returned</param>
        /// <returns>Xml node.</returns>
        public XElement GetNode(string parentNode, string nodeName)
        {
            IEnumerable<XElement> parent = _document.Root.Descendants(parentNode);
            if (parent == null)
                throw new XmlException(string.Format(null, "Could not find '{0}'", parentNode));

            //Get the child nodes.
            IEnumerable<XElement> childNode = parent.First().Descendants(nodeName);

            if (childNode == null)
                throw new XmlException(string.Format(null, "Could not find '{0}'", childNode));

            return childNode.First();
        }

        /// <summary>
        /// Returns the Text values for the nodes specified from the configuration file.
        /// </summary>
        /// <param name="parentNode">Name of the Parent node.</param>
        /// <param name="nodeName">Name of the node, from which the text values to be read.</param>
        /// <returns>Text values with in the node.</returns>
        public string[] GetTextValues(string parentNode, string nodeName)
        {
            IEnumerable<XElement> parent = _document.Root.Descendants(parentNode);
            if (parent == null)
                throw new XmlException(string.Format(null, "Could not find '{0}'", parentNode));

            //Get the child nodes.
            IEnumerable<XElement> nodesChild = parent.First().Descendants(nodeName);
            if (nodesChild == null)
                throw new XmlException(string.Format(null, "Could not find '{0}'", nodeName));

            //Get the node with nodeName.
            IList<XNode> nodes = nodesChild.First().Nodes().ToList();

            //If the node does not have child nodes then return the Inner text of the node.            
            string[] values = nodes.Count == 1 && nodes[0].GetType() != typeof (XElement)
                                  ? new[] {nodes[0].ToString()}
                                  : nodes.Cast<XElement>().Select(n => n.Value).ToArray();

            return values;
        }
    }
}
