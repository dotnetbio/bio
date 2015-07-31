using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Bio.Util.Logging;

namespace Bio.IO.GenBank
{
   /// <summary>
   /// Not all 3rd party programs respect the GenBank locus format.  Due to this we cannot expect each item to lie in exact
   /// indices with respect to the locus.  In order to parse this information based off of tokens we do have to make certain
   /// assumptions about the locus data, however this is well documented and for all but the ID field we know what the data type
   /// will be and what values it may contain.
   /// </summary>
   public sealed class GenBankLocusTokenParser
   {
      private string line;

      /// <summary>
      /// Parses a locus string into a <see cref="GenBankLocusInfo"/>.
      /// </summary>
      /// <param name="locusText">Locus text.</param>
      /// <returns>
      /// Locus containing the info in the passed in string.
      /// </returns>
      public GenBankLocusInfo Parse(string locusText)
      {
         line = locusText;

         var locus = new GenBankLocusInfo();
         var tokenParsers = GetLocusTokenParsers(locus);

         foreach (string token in line.Split(' '))
         {
            if (string.IsNullOrEmpty(token)) continue;

            string token1 = token;
            Func<string, bool> usedParser = tokenParsers.FirstOrDefault(parser => parser(token1));

            if (usedParser == null)
            {
               Trace.Report(string.Format(CultureInfo.InvariantCulture,
                                          Properties.Resource.GenBankFailedToParseLocusTokenFormat, token, line));
            }

            tokenParsers.Remove(usedParser);
         }

         if (String.IsNullOrEmpty(locus.SequenceType))
         {
            string message = String.Format(Properties.Resource.GenBankUnknownLocusFormat, line);
            Trace.Report(message);
            throw new Exception(message);
         }

         if (locus.SequenceType.ToLowerInvariant() == "aa")
         {
            locus.MoleculeType = MoleculeType.Protein;
         }

         return locus;
      }

      /// <summary>
      /// The LOCUS format has defined positions for each individual value in the LOCUS but through experimentation
      /// and some reading this format is not followed.  Instead we have to parse each token and interpret which value
      /// each token belongs too.  Luckily there is a standard set of values for all but the DATE and LOCUS ID, which we can 
      /// infer based on the string.
      /// </summary>
      /// <param name="locus"></param>
      /// <returns></returns>
      private static List<Func<string, bool>> GetLocusTokenParsers(GenBankLocusInfo locus)
      {
         // 
         // The order of token parsers matter, the items which we know definitions for must be parsed
         // before those which are inferred. 
         //
         return
            new List<Func<string, bool>>
               {
                  //
                  // 1. LOCUS Token
                  //

                  token => token == "LOCUS",
                  
                  //
                  // 2. Strand Topology
                  //

                  token =>
                  {
                     locus.StrandTopology = FirstOrDefault(
                        LocusConstants.MoleculeTopologies,
                        topology => topology.Key.ToLowerInvariant() == token.ToLowerInvariant(),
                        new KeyValuePair<string, SequenceStrandTopology>("", SequenceStrandTopology.None)).Value;

                     return locus.StrandTopology != SequenceStrandTopology.None;
                  },
                  
                  //
                  // 3. Strand & Molecule Definition Token
                  //

                  token =>
                  {
                     //
                     // Strand and molecule definition are one token defining two separate attributes, such 
                     // as ds-DNA so the parsing occurs on one token.
                     // 

                     string s = token.ToLowerInvariant();

                     locus.Strand = FirstOrDefault(
                        LocusConstants.SequenceStrandTypes,
                        strand => s.StartsWith(strand.Key.ToLowerInvariant()),
                        new KeyValuePair<string, SequenceStrandType>("", SequenceStrandType.None)).Value;

                     if (locus.Strand != SequenceStrandType.None)
                     {
                        token = token.Remove(0,
                                             LocusConstants.SequenceStrandTypes.First(
                                                strand => strand.Value == locus.Strand).Key.Length);
                     }

                     locus.MoleculeType = FirstOrDefault(
                        LocusConstants.AlphabetTypes,
                        moleculeType => token.ToLowerInvariant() == moleculeType.Key.ToLowerInvariant(),
                        new KeyValuePair<string, MoleculeType>("", MoleculeType.Invalid)).Value;

                     return locus.MoleculeType != MoleculeType.Invalid
                           || locus.Strand != SequenceStrandType.None;
                  },

                  //
                  // 4. Division Code
                  //

                  token =>
                  {
                     locus.DivisionCode = FirstOrDefault(
                        LocusConstants.SequenceDivisionCodes,
                        divisionCode => divisionCode.Key.ToLowerInvariant() == token.ToLowerInvariant(),
                        new KeyValuePair<string, SequenceDivisionCode>("", SequenceDivisionCode.None)).Value;

                     return locus.DivisionCode != SequenceDivisionCode.None;
                  },

                  //
                  // 4. Sequence Length
                  //

                  token =>
                  {
                     int length;
                     bool result = int.TryParse(token, out length);
                     if (result)
                        locus.SequenceLength = length;
                     return result;
                  },

                  // 
                  // 5. Sequence Type
                  //

                  token =>
                  {
                     locus.SequenceType = LocusConstants.SequenceTypes.FirstOrDefault(
                        sequenceType => sequenceType.ToLowerInvariant() == token.ToLowerInvariant());

                     return !string.IsNullOrEmpty(locus.SequenceType);
                  },

                  // 
                  // 6. Date
                  //

                  token =>
                  {
                     DateTime dateTime;
                     bool result = DateTime.TryParse(token, out dateTime);
                     if (result)
                        locus.Date = dateTime;
                     return result;
                  },

                  // 
                  // 7. Sequence Name / ID
                  //

                  token =>
                  {
                     locus.Name = token;
                     return true;
                  }
               };
      }

      private static T FirstOrDefault<T>(IEnumerable<T> list, Func<T, bool> predicate, T defaultValue)
      {
         foreach (var item in list.Where(predicate))
         {
            return item;
         }

         return defaultValue;
      }

      #region Constants

      /// <summary>
      /// List of text to enumeration mappings to better organize and contain variable information with respect to parsing
      /// the locus.
      /// </summary>
      public static class LocusConstants
      {
         /// <summary>
         /// Maps all sequences division code strings to their respective enumeration.
         /// </summary>
         public static readonly List<KeyValuePair<string, SequenceDivisionCode>> SequenceDivisionCodes =
            new List<KeyValuePair<string, SequenceDivisionCode>>
            {
               new KeyValuePair<string, SequenceDivisionCode>("PRI", SequenceDivisionCode.PRI),
               new KeyValuePair<string, SequenceDivisionCode>("ROD", SequenceDivisionCode.ROD),
               new KeyValuePair<string, SequenceDivisionCode>("MAM", SequenceDivisionCode.MAM),
               new KeyValuePair<string, SequenceDivisionCode>("VRT", SequenceDivisionCode.VRT),
               new KeyValuePair<string, SequenceDivisionCode>("INV", SequenceDivisionCode.INV),
               new KeyValuePair<string, SequenceDivisionCode>("PLN", SequenceDivisionCode.PLN),
               new KeyValuePair<string, SequenceDivisionCode>("BCT", SequenceDivisionCode.BCT),
               new KeyValuePair<string, SequenceDivisionCode>("VRL", SequenceDivisionCode.VRL),
               new KeyValuePair<string, SequenceDivisionCode>("PHG", SequenceDivisionCode.PHG),
               new KeyValuePair<string, SequenceDivisionCode>("SYN", SequenceDivisionCode.SYN),
               new KeyValuePair<string, SequenceDivisionCode>("UNA", SequenceDivisionCode.UNA),
               new KeyValuePair<string, SequenceDivisionCode>("EST", SequenceDivisionCode.EST),
               new KeyValuePair<string, SequenceDivisionCode>("PAT", SequenceDivisionCode.PAT),
               new KeyValuePair<string, SequenceDivisionCode>("STS", SequenceDivisionCode.STS),
               new KeyValuePair<string, SequenceDivisionCode>("GSS", SequenceDivisionCode.GSS),
               new KeyValuePair<string, SequenceDivisionCode>("HTG", SequenceDivisionCode.HTG),
               new KeyValuePair<string, SequenceDivisionCode>("HTC", SequenceDivisionCode.HTC),
               new KeyValuePair<string, SequenceDivisionCode>("ENV", SequenceDivisionCode.ENV),
               new KeyValuePair<string, SequenceDivisionCode>("CON", SequenceDivisionCode.CON)
            };

         /// <summary>
         /// Maps each known molecule type string to its enumeration definition.
         /// </summary>
         public static readonly List<KeyValuePair<string, MoleculeType>> AlphabetTypes =
            new List<KeyValuePair<string, MoleculeType>>
            {
               new KeyValuePair<string, MoleculeType>("NA", MoleculeType.NA),
               new KeyValuePair<string, MoleculeType>("DNA", MoleculeType.DNA),
               new KeyValuePair<string, MoleculeType>("RNA", MoleculeType.RNA),
               new KeyValuePair<string, MoleculeType>("tRNA", MoleculeType.tRNA),
               new KeyValuePair<string, MoleculeType>("rRNA", MoleculeType.rRNA),
               new KeyValuePair<string, MoleculeType>("mRNA", MoleculeType.mRNA),
               new KeyValuePair<string, MoleculeType>("uRNA", MoleculeType.uRNA),
               new KeyValuePair<string, MoleculeType>("snRNA", MoleculeType.snRNA),
               new KeyValuePair<string, MoleculeType>("snoRNA", MoleculeType.snoRNA)
            };

         /// <summary>
         /// Maps each known topology string to its enumeration definition.
         /// </summary>
         public static readonly List<KeyValuePair<string, SequenceStrandTopology>> MoleculeTopologies =
            new List<KeyValuePair<string, SequenceStrandTopology>>
            {
               new KeyValuePair<string, SequenceStrandTopology>("linear", SequenceStrandTopology.Linear),
               new KeyValuePair<string, SequenceStrandTopology>("circular", SequenceStrandTopology.Circular)
            };

         /// <summary>
         /// List of sequence types expected.
         /// </summary>
         public static readonly List<string> SequenceTypes = new List<string> { "bp", "aa" };

         /// <summary>
         /// Maps each strand string to its enumeration definition.
         /// </summary>
         public static readonly List<KeyValuePair<string, SequenceStrandType>> SequenceStrandTypes =
            new List<KeyValuePair<string, SequenceStrandType>>
            {
               new KeyValuePair<string, SequenceStrandType>("ss-", SequenceStrandType.Single),
               new KeyValuePair<string, SequenceStrandType>("ds-", SequenceStrandType.Double),
               new KeyValuePair<string, SequenceStrandType>("ms-", SequenceStrandType.Mixed)
            };
      }

      #endregion
   }
}
