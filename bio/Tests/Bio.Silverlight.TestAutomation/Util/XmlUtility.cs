/****************************************************************************
 * XmlUtility.cs
 * 
 *   This file contains the all the xml related functions.
 * 
***************************************************************************/

using System.Xml;
using System;

namespace Bio.TestAutomation.Util
{

    /// <summary>
    /// This class contains the all the xml related functions.
    /// </summary>
    public class XmlUtility
    {
        XmlDocument m_xmlDoc;

        /// <summary>
        /// Constructor which sets the config file.
        /// </summary>
        /// <param name="xmlFilePath">Config file path.</param>
        public XmlUtility(string xmlFilePath)
        {
            m_xmlDoc = new XmlDocument();
            m_xmlDoc.Load(xmlFilePath);
        }

        /// <summary>
        /// Returns the Text value for the nodes specified from the configuration file.
        /// </summary>
        /// <param name="parentNode">Name of the Parent node.</param>
        /// <param name="nodeName">Name of the node, from which the text value to be read.</param>
        /// <returns>Text with in the node.</returns>
        internal string GetTextValue(string parentNode, string nodeName)
        {
            XmlNode actualNode = GetNode(parentNode, nodeName);
            return actualNode.InnerText;
        }

        /// <summary>
        /// Returns the contents of the file for the Path specified.
        /// </summary>
        /// <param name="parentNode">Name of the Parent node.</param>
        /// <param name="nodeName">Name of the node, from which the file path to be read.</param>
        /// <returns>Contents of file for the path specified.</returns>
        internal string GetFileTextValue(string parentNode, string nodeName)
        {
            XmlNode actualNode = GetNode(parentNode, nodeName);
            string textValue = Utility.GetFileContent(actualNode.InnerText);
            return textValue;
        }

        /// <summary>
        /// Returns the node from the configuration file.
        /// </summary>
        /// <param name="parentNode">Name of the Parent node</param>
        /// <param name="nodeName">Name of the node, which needs to be returned</param>
        /// <returns>Xml node.</returns>
        internal XmlNode GetNode(string parentNode, string nodeName)
        {
            XmlNode lst = null;

            lst = m_xmlDoc.ChildNodes[1];
            string xPath = string.Concat("/AutomationTest/", parentNode, "/", nodeName);

            XmlNode childNode = lst.SelectSingleNode(xPath);

            if (null == childNode)
                throw new XmlException(string.Format((IFormatProvider)null,
                    "Could not find the Xpath '{0}'", xPath));

            return childNode;
        }

        /// <summary>
        /// Returns the Text values for the nodes specified from the configuration file.
        /// </summary>
        /// <param name="parentNode">Name of the Parent node.</param>
        /// <param name="nodeName">Name of the node, from which the text values to be read.</param>
        /// <returns>Text values with in the node.</returns>
        internal string[] GetTextValues(string parentNode, string nodeName)
        {
            XmlNode actualNode = GetNode(parentNode, nodeName);

            XmlNodeList chldNodes = actualNode.ChildNodes;
            string[] values = new string[actualNode.ChildNodes.Count];

            int i = 0;
            foreach (XmlNode singleNode in chldNodes)
            {
                values[i] = singleNode.InnerText;
                i++;
            }

            return values;
        }
    }
}
