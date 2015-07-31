using System;
using System.Collections.Generic;

namespace Bio.IO.GenBank
{
    /// <summary>
    /// The identifier of a project (such as a Genome Sequencing Project) 
    /// to which a GenBank sequence record belongs.
    /// 
    /// This is obsolete and was removed from the GenBank flat file format 
    /// after Release 171.0 in April 2009.
    /// </summary>
    public class ProjectIdentifier
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ProjectIdentifier()
        {
            Numbers = new List<string>();
        }

        /// <summary>
        /// Private Constructor for clone method.
        /// </summary>
        /// <param name="other">ProjectIdentifier instance to clone.</param>
        private ProjectIdentifier(ProjectIdentifier other)
        {
            Name = other.Name;
            Numbers = new List<string>(other.Numbers);
        }

        #endregion Constructors

        #region Properties
        /// <summary>
        /// Name of the project.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Project numbers.
        /// </summary>
        public IList<string> Numbers { get; private set; }
        #endregion Properties
        #region Methods
        /// <summary>
        /// Creates a new ProjectIdentifier that is a copy of the current ProjectIdentifier.
        /// </summary>
        /// <returns>A new ProjectIdentifier that is a copy of this ProjectIdentifier.</returns>
        public ProjectIdentifier Clone()
        {
            return new ProjectIdentifier(this);
        }
        #endregion Methods

    }
}
