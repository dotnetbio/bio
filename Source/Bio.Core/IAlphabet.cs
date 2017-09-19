using System.Collections.Generic;

namespace Bio
{
    /// <summary>
    /// An alphabet defines a set of symbols common to a particular representation
    /// of a biological sequence. The symbols in these alphabets are those you would find
    /// as the individual sequence items in an ISequence variable.
    /// <para>
    /// The symbols in an alphabet may represent a particular biological structure
    /// or they may represent information helpful in understanding a sequence. For instance
    /// gap symbol, termination symbol, and symbols representing items whose
    /// definition remains ambiguous are all allowed.
    /// </para>
    /// </summary>
    public interface IAlphabet : IEnumerable<byte>
    {
        /// <summary>
        /// Gets a human readable name for the alphabet. 
        /// For example "DNA", "RNA", or "Amino Acid".
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a value indicating whether the alphabet has one or more symbols 
        /// that represent a gap.
        /// </summary>
        bool HasGaps { get; }

        /// <summary>
        /// Gets a value indicating whether the alphabet has one or more symbols 
        /// that represent an ambiguous item (i.e. and item for which it is not 
        /// precisely known what it represents).
        /// </summary>
        bool HasAmbiguity { get; }

        /// <summary>
        /// Gets a value indicating whether the alphabet has one or more symbols 
        /// that represent terminal items.
        /// </summary>
        bool HasTerminations { get; }

        /// <summary>
        /// Gets a value indicating whether this alphabet supports complement or not.
        /// </summary>
        bool IsComplementSupported { get; }

        /// <summary>
        /// Gets the count of symbols present in this alphabet.
        /// This includes basic symbols, gaps, terminations and ambiguous symbols present in this alphabet.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Provides array like access to the symbols in this alphabet.
        /// </summary>
        /// <param name="index">Index of symbol present in this alphabet.</param>
        /// <returns>A byte which represents the symbol.</returns>
        byte this[int index] { get; }

        /// <summary>
        /// Gets the complement for the specified symbol.
        /// This is to address the scenarios like in case of DNA complement of A is T etc.
        /// Note: If the complement is not supported then this method returns false.
        ///       To know whether this alphabet supports complement or not, Use IsComplementSupported property.
        /// </summary>
        /// <param name="symbol">Symbol for which the complement symbol is needed.</param>
        /// <param name="complementSymbol">Complement symbol.</param>
        /// <returns>Returns true if complement symbol is found and returned, else returns false.</returns>
        bool TryGetComplementSymbol(byte symbol, out byte complementSymbol);

        /// <summary>
        /// Gets the complements for the specified symbols.
        /// This is to address the scenarios like in case of DNA complement of A is T etc.
        /// Note: If the complement is not supported then this method returns false.
        ///       To know whether this alphabet supports complement or not, Use IsComplementSupported property.
        /// </summary>
        /// <param name="symbols">Symbol for which the complement symbol is needed.</param>
        /// <param name="complementSymbols">Complement symbol.</param>
        /// <returns>Returns true if complement symbol is found and returned, else returns false.</returns>
        bool TryGetComplementSymbol(byte[] symbols, out byte[] complementSymbols);

        /// <summary>
        /// Gets the default gap symbol if present in the alphabet.
        /// </summary>
        /// <param name="defaultGapSymbol">Default gap symbol if the alphabet has one.</param>
        /// <returns>Returns true if the default gap symbol is returned in defaultGapSymbol parameter,
        /// else returns false.</returns>
        bool TryGetDefaultGapSymbol(out byte defaultGapSymbol);

        /// <summary>
        /// Gets the default termination symbol if present in the alphabet.
        /// </summary>
        /// <param name="defaultTerminationSymbol">Default termination symbol if the alphabet has one.</param>
        /// <returns>Returns true if the default termination symbol is returned in defaultTerminationSymbol 
        /// parameter, else returns false.</returns>
        bool TryGetDefaultTerminationSymbol(out byte defaultTerminationSymbol);

        /// <summary>
        /// Gets the gap symbols if present in the alphabet.
        /// </summary>
        /// <param name="gapSymbols">Gap symbols as hashset if the alphabet has one or more gap symbols.</param>
        /// <returns>Returns true if the gap symbols are returned in gapSymbols parameter, else returns false.</returns>
        bool TryGetGapSymbols(out HashSet<byte> gapSymbols);

        /// <summary>
        /// Gets the termination symbols if present in the alphabet.
        /// </summary>
        /// <param name="terminationSymbols">Termination symbols as hashset if the alphabet has one or more termination symbols.</param>
        /// <returns>Returns true if the termination symbols are returned in trminationSymbols parameter, else returns false.</returns>
        bool TryGetTerminationSymbols(out HashSet<byte> terminationSymbols);

        /// <summary>
        /// Gets the symbols that are valid for this alphabet.
        /// This Method can be used for better performance where lot of 
        /// validation happens like in case of Parsers.
        /// </summary>
        HashSet<byte> GetValidSymbols();

        /// <summary>
        /// Gets the ambiguous symbols present in alphabet.
        /// </summary>
        HashSet<byte> GetAmbiguousSymbols();

        /// <summary>
        /// Maps A to A  and a to A
        /// that is key will contain unique values.
        /// This will be used in the IsValidSymbol method to address Scenarios like a == A, G == g etc.
        /// </summary>
        byte[] GetSymbolValueMap();

        /// <summary>
        /// Find the consensus symbol for a set of symbols.
        /// </summary>
        /// <param name="symbols">Set of sequence items.</param>
        /// <returns>Consensus symbol.</returns>
        byte GetConsensusSymbol(HashSet<byte> symbols);

        /// <summary>
        /// Gets the ambiguous symbol for the specified set of symbols.
        /// </summary>
        /// <param name="symbols">Set of symbols for which the ambiguous symbol is required.</param>
        /// <param name="ambiguousSymbol">Ambiguous symbol.</param>
        /// <returns>Returns true if the ambiguous symbol found and returned in ambiguousSymbol parameter, else returns false.</returns>
        bool TryGetAmbiguousSymbol(HashSet<byte> symbols, out byte ambiguousSymbol);

        /// <summary>
        /// Gets the set of basic symbols for the specified ambiguous symbol.
        /// </summary>
        /// <param name="ambiguousSymbol">Ambiguous symbol for which the basic symbols is required.</param>
        /// <param name="basicSymbols">Set of basic symbols belongs to the specified ambiguous symbol.</param>
        /// <returns>Returns true if the basic symbols are found and returned in basicSymbols parameter, else returns false.</returns>
        bool TryGetBasicSymbols(byte ambiguousSymbol, out HashSet<byte> basicSymbols);

        /// <summary>
        /// Compares two items and specifies whether they are same or not.
        /// If the any of the bytes (Nucleotides) passed not belongs to
        /// this alphabet then this method throws an exception.
        /// TO Address scenarios like, N!=N, M != A etc.
        /// For the Scenarios like A == a, g == G use IsValidSymbol method.
        /// </summary>
        /// <param name="x">First symbol to compare.</param>
        /// <param name="y">Second symbol to compare.</param>
        /// <returns>Returns true if x equals y.</returns>
        bool CompareSymbols(byte x, byte y);

        /// <summary>
        /// Validates if all symbols match with the specified alphabet type.
        /// </summary>
        /// <param name="symbols">Symbols to be validated.</param>
        /// <param name="offset">Offset from where validation should start.</param>
        /// <param name="length">Number of symbols to validate from the specified offset.</param>
        /// <returns>True if the validation succeeds, else false.</returns>
        bool ValidateSequence(byte[] symbols, long offset, long length);

        /// <summary>
        /// Checks if the provided item is a gap character or not
        /// </summary>
        /// <param name="item">Item to be checked</param>
        /// <returns>True if the specified item is a gap</returns>
        bool CheckIsGap(byte item);

        /// <summary>
        /// Checks if the provided item is an ambiguous character or not
        /// </summary>
        /// <param name="item">Item to be checked</param>
        /// <returns>True if the specified item is a ambiguous</returns>
        bool CheckIsAmbiguous(byte item);

        /// <summary>
        /// Gets the friendly name of a given symbol.
        /// </summary>
        /// <param name="item">Symbol to find friendly name.</param>
        /// <returns>Friendly name of the given symbol.</returns>
        string GetFriendlyName(byte item);
    }
}
