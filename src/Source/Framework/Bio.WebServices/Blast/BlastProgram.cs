namespace Bio.Web.Blast
{
    /// <summary>
    /// Constants for the known BLAST program styles
    /// </summary>
    public sealed class BlastProgram
    {
        /// <summary>
        /// BLASTN program
        /// </summary>
        public const string Blastn = "blastn" ;
        /// <summary>
        /// BLASTP program
        /// </summary>
        public const string Blastp = "blastp";
        /// <summary>
        /// BLASTX program
        /// </summary>
        public const string Blastx = "blastx";
        /// <summary>
        /// TBLASTN program
        /// </summary>
        public const string Tblastn = "tblastn";
        /// <summary>
        /// TBLASTX program
        /// </summary>
        public const string Tblastx = "tblastx";
        /// <summary>
        /// NCBI MegaBlast (BLASTN+)
        /// </summary>
        public const string Megablast = "megablast";
    }
}