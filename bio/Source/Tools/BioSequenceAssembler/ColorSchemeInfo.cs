namespace SequenceAssembler
{
    #region -- Using Directives --

    using System.Collections;

    #endregion -- Using Directives --

    /// <summary>
    /// ColorSchemeInfo holds information about a particular color scheme
    /// mentioned in the app.config. The information includes Name, Alphabet
    /// and the alphabet v\s color mapping.
    /// </summary>
    public class ColorSchemeInfo
    {
        #region -- Private Members --

        /// <summary>
        /// Holds the alphabet v\s color mapping for a particular color scheme.
        /// </summary>
        private Hashtable colorMapping;

        #endregion -- Private Members --

        #region -- Constructor --

        /// <summary>
        /// Initializes a new instance of the ColorSchemeInfo class.
        /// </summary>
        public ColorSchemeInfo()
        {
            this.colorMapping = new Hashtable();
        }

        #endregion -- Constructor --

        #region -- Properties --

        /// <summary>
        /// Gets or sets the name of a particular color scheme.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the alphabet v\s color mapping for a particular color scheme.
        /// </summary>
        public Hashtable ColorMapping
        {
            get
            {
                return this.colorMapping;
            }
        }

        #endregion -- Properties --
    }
}
