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
using System.Windows;

namespace Bio.TestAutomation.Util{

    /// <summary>
    /// This class contains the all the xml related functions.
    /// </summary>
    public class XmlUtility
    {
        XDocument m_xmlDoc;

        /// <summary>
        /// Constructor which sets the config file.
        /// </summary>
        /// <param name="xmlFilePath">Config file path.</param>
        public XmlUtility(string xmlFilePath)
        {         
           string path = Directory.GetCurrentDirectory() + "\\" + xmlFilePath;

           using(StreamReader pathStream=new StreamReader(path))
           {                   
             m_xmlDoc = XDocument.Load(pathStream);           
           }
        }

        /// <summary>
        /// Returns the Text value for the nodes specified from the configuration file.
        /// </summary>
        /// <param name="parentNode">Name of the Parent node.</param>
        /// <param name="nodeName">Name of the node, from which the text value to be read.</param>
        /// <returns>Text with in the node.</returns>
        internal string GetTextValue(string parentNode, string nodeName)
        {
            XElement actualNode = GetNode(parentNode, nodeName);
            return actualNode.Value.ToString();
        }

        /// <summary>
        /// Returns the contents of the file for the Path specified.
        /// </summary>
        /// <param name="parentNode">Name of the Parent node.</param>
        /// <param name="nodeName">Name of the node, from which the file path to be read.</param>
        /// <returns>Contents of file for the path specified.</returns>
        internal string GetFileTextValue(string parentNode, string nodeName)
        {
            XElement actualNode = GetNode(parentNode, nodeName);
            string textValue = Utility.GetFileContent(actualNode.Value.ToString());
            return textValue;
        }

        /// <summary>
        /// Returns the node from the configuration file.
        /// </summary>
        /// <param name="parentNode">Name of the Parent node</param>
        /// <param name="nodeName">Name of the node, which needs to be returned</param>
        /// <returns>Xml node.</returns>
        internal XElement GetNode(string parentNode, string nodeName)
        {
            XNode firstNode = null;

            //Get the first node from the Xml doc.
            firstNode = m_xmlDoc.FirstNode;                       

            XElement automationNode = (XElement)firstNode;

            //Get the parent node.
            IEnumerable<XElement> parent=automationNode.Descendants(parentNode);

            if (parent == null)
                throw new XmlException(string.Format((IFormatProvider)null,
                    "Could not find '{0}'", parentNode));

            //Get the child nodes.
            IEnumerable<XElement> childNode=parent.ElementAt(0).Descendants(nodeName);

            if (childNode == null)
                throw new XmlException(string.Format((IFormatProvider)null,
                    "Could not find '{0}'", childNode));
                                   
            return childNode.ElementAt(0);
        }

        /// <summary>
        /// Returns the Text values for the nodes specified from the configuration file.
        /// </summary>
        /// <param name="parentNode">Name of the Parent node.</param>
        /// <param name="nodeName">Name of the node, from which the text values to be read.</param>
        /// <returns>Text values with in the node.</returns>
        internal string[] GetTextValues(string parentNode, string nodeName)
        {
            XNode firstNode = null;
            int index = 0, countOfChildNodes;

            //Get the first node from the Xml doc.
            firstNode = m_xmlDoc.FirstNode;

            XElement automationNode = (XElement)firstNode;
            
            //Get the parent node.
            IEnumerable<XElement> parent = automationNode.Descendants(parentNode);

            if (parent == null)
                throw new XmlException(string.Format((IFormatProvider)null,
                    "Could not find '{0}'", parentNode));

            //Get the child nodes.
            IEnumerable<XElement> nodesChild = parent.ElementAt(0).Descendants(nodeName);

            if (nodesChild == null)
                throw new XmlException(string.Format((IFormatProvider)null,
                    "Could not find '{0}'", nodeName));

            //Get the node with nodeName.
            IEnumerable<XNode> nodes = nodesChild.ElementAt(0).Nodes();

            countOfChildNodes = nodes.Count();
            string[] values = new string[countOfChildNodes];

            //If the node does not have child nodes then return the Inner text of the node.            
            if (countOfChildNodes == 1 && nodes.ElementAt(0).GetType().ToString() != "System.Xml.Linq.XElement")
            {
                values[0] = nodes.ElementAt(0).ToString();
            }
            else
            {
                //Get all the values from the elements in the node from XPath.
                foreach (XElement node in nodes)
                {
                    values[index] = node.Value.ToString();
                    index++;
                }
            }

            return values;
        }
    }
}
