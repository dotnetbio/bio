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
        /// Trace Assembly Archive. 
        /// </summary>
        TraceAssemblyArchive,


        /// <summary>
        /// BioProject Type.
        /// </summary>
        BioProject
    }
}
