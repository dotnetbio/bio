namespace Bio.IO.Wiggle
{
    /// <summary>
    /// Constants to use with Wiggle object model.
    /// </summary>
    public static class WiggleSchema
    {
        /// <summary>
        /// Track line identifier.
        /// </summary>
        public const string Track = "track";

        /// <summary>
        /// Wiggle data format identifier.
        /// </summary>
        public const string FixedStep = "fixedStep";

        /// <summary>
        /// Wiggle data format identifier.
        /// </summary>
        public const string VariableStep = "variableStep";
        
        /// <summary>
        /// Chromosome name key.
        /// </summary>
        public const string Chrom = "chrom";

        /// <summary>
        /// Span value key.
        /// </summary>
        public const string Span = "span";

        /// <summary>
        /// Step value key.
        /// </summary>
        public const string Step = "step";

        /// <summary>
        /// Start value key.
        /// </summary>
        public const string Start = "start";

        /// <summary>
        /// Type value key.
        /// </summary>
        public const string Type = "type";

        /// <summary>
        /// Wiggle format specification '0' identifier.
        /// </summary>
        public const string Wiggle0 = "wiggle_0";

        /// <summary>
        /// Denotes starting of a comment line.
        /// </summary>
        public const string CommentLineStart = "#";
    }
}
