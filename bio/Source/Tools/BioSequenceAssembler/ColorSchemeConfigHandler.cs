namespace SequenceAssembler
{
    #region -- Using Directives --

    using System.Collections.Generic;
    using System.Configuration;
    using System.Xml;

    #endregion -- Using Directives --

    /// <summary>
    /// ColorSchemeConfigHandler reads the "Colors" section of the app.config, parses it
    /// and returns a list of ColorSchemeInfo class which holds the data of all color schemes.
    /// </summary>
    public class ColorSchemeConfigHandler : IConfigurationSectionHandler
    {
        #region -- Private Members --

        /// <summary>
        /// Represents ColorScheme tag in app.config
        /// </summary>
        private const string COLORSCHEME = "ColorScheme";

        /// <summary>
        /// Represents Name attribute in app.config
        /// </summary>
        private const string NAME = "Name";

        /// <summary>
        /// Represents Default tag in app.config
        /// </summary>
        private const string DEFAULT = "Default";

        /// <summary>
        /// Represents Color tag in app.config
        /// </summary>
        private const string COLOR = "Color";

        /// <summary>
        /// Represents Symbol attribute in app.config
        /// </summary>
        private const string SYMBOL = "Symbol";

        /// <summary>
        /// Represents Char attribute in app.config
        /// </summary>
        private const string CHAR = "Char";

        /// <summary>
        /// Holds a list of all the color schemes mentioned in the app.config file.
        /// </summary>
        private List<ColorSchemeInfo> colorschemes;

        #endregion -- Private Members --

        #region -- Constructor --

        /// <summary>
        /// Initializes a new instance of the ColorSchemeConfigHandler class.
        /// </summary>
        public ColorSchemeConfigHandler()
        {
            this.colorschemes = new List<ColorSchemeInfo>();
        }

        #endregion -- Constructor --

        #region -- Public Methods -- 

        /// <summary>
        /// This method parses the "Colors" section of the app.config value.
        /// It creates ColorSchemeInfo objects and populates these objects
        /// with the values mentioned in app.config.
        /// </summary>
        /// <param name="parent">Parent object.</param>
        /// <param name="configContext"> Configuration context object.</param>
        /// <param name="section">Colors Section XML node.</param>
        /// <returns>list of all the color schemes mentioned in the app.config file.</returns>
        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            XmlNodeList colorSchemeNodes = section.SelectNodes(COLORSCHEME);

            foreach (XmlNode colorSchemeNode in colorSchemeNodes)
            {
                ColorSchemeInfo info = new ColorSchemeInfo();
                info.Name = ReadAttribute(colorSchemeNode, NAME);

                string defaultColor = ReadAttribute(colorSchemeNode.SelectSingleNode(DEFAULT), COLOR);
                info.ColorMapping.Add(DEFAULT, defaultColor);

                XmlNodeList symbolNodes = colorSchemeNode.SelectNodes(SYMBOL);

                foreach (XmlNode symbolNode in symbolNodes)
                {
                    string alphabet = ReadAttribute(symbolNode, CHAR);
                    string color = ReadAttribute(symbolNode, COLOR);

                    if (!info.ColorMapping.ContainsKey(alphabet))
                    {
                        info.ColorMapping.Add(alphabet, color);
                    }
                }

                this.colorschemes.Add(info);
            }

            return this.colorschemes;
        }

        #endregion -- Public Methods --

        #region -- Private Methods --

        /// <summary>
        /// This method reads a value of a particular attribute
        /// of a given node.
        /// </summary>
        /// <param name="node">The node which holds the attribute.</param>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <returns>The value of the attribute.</returns>
        private static string ReadAttribute(XmlNode node, string attributeName)
        {
            string attributeValue = string.Empty;
            if (node != null && node.Attributes != null)
            {
               XmlNode atributeNode = node.Attributes.GetNamedItem(attributeName);
               if (atributeNode != null)
               {
                   attributeValue = atributeNode.Value;
               }
            }

            return attributeValue;
        }

        #endregion -- Private Methods --
    }
}
