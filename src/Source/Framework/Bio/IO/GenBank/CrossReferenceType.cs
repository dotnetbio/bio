using System;
using System.ComponentModel;
namespace Bio.IO.GenBank
{
    /// <summary>
    /// A CrossReferenceType specifies whether the DBLink is 
    /// referring to project or a Trace Assembly Archive.
    /// </summary>
    public enum CrossReferenceType
    {
        /// <summary>
        /// None - CrossReferenceType is unspecified.
        /// </summary>
        None,

        /// <summary>
        /// Project.
        /// </summary>
        Project,

        /// <summary>
        /// Trace Assembly Archive.  The Description specifies the actual string 
        /// with spaces that appears in the genbank file.
        /// </summary>
        [Description("Trace Assembly Archive")]
        TraceAssemblyArchive,


        /// <summary>
        /// BioProject Type.
        /// </summary>
        BioProject
    }
}
