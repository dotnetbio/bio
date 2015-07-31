using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Bio;
using Bio.IO.BAM;
using Bio.IO.SAM;

namespace SamUtil
{
    /// <summary>
    /// Sequence coverage analysis class
    /// </summary>
    public class SequenceCoverage
    {
        #region Fields
        private static SimpleConsensusResolver resolver = new SimpleConsensusResolver(Alphabets.AmbiguousDNA);
        #endregion

        #region Public Fields

        /// <summary>
        /// Usage.
        /// </summary>
        public bool Help;

        /// <summary>
        /// Input file is in SAM format.
        /// </summary>
        public bool SAMInput;

        #endregion Public Fields

        /// <summary>
        /// Display Sequence Item occurences percentage
        /// </summary>
        /// <param name="inputFile">Path of the input file</param>
        /// <param name="possibleOccurence">True to display Nculeaotide distribution</param>
        public void DisplaySequenceItemOccurences(string inputFile,
            bool possibleOccurence)
        {

            if (string.IsNullOrEmpty(inputFile))
            {
                throw new InvalidOperationException("Input File Not specified");
            }

            SequenceAlignmentMap alignmentMapobj = null;

            if (!SAMInput)
            {
                BAMParser bamParser = new BAMParser();
                alignmentMapobj = bamParser.ParseOne<SequenceAlignmentMap>(inputFile);
            }
            else
            {
                SAMParser samParser = new SAMParser();
                alignmentMapobj = samParser.ParseOne<SequenceAlignmentMap>(inputFile);
            }

            IList<string> chromosomes = alignmentMapobj.GetRefSequences();

            if (possibleOccurence)
            {
                Console.Write("Nucleotide Distribution:");
                Console.Write("\r\nPosition\tA\tT\tG\tC\tPossibility Of Occurences");
                foreach (string str in chromosomes)
                {
                    GetCoverage(str, alignmentMapobj, "true");
                }
            }
            else
            {
                Console.Write("Coverage Profile:");
                Console.Write("\r\nPosition\tA\tT\tG\tC");
                foreach (string str in chromosomes)
                {
                    GetCoverage(str, alignmentMapobj, "false");
                }
            }


        }

        /// <summary>
        /// Display Coverage plot
        /// </summary>
        /// <param name="readname">Chromosoem name</param>
        /// <param name="alignmentMapobj">Alignment object</param>
        /// <param name="possibility">True for Nucleaotide distribution</param>
        private void GetCoverage(string readname, SequenceAlignmentMap alignmentMapobj,
            string possibility)
        {
            List<byte> distinctChars =
                new List<byte> { Alphabets.DNA.A, Alphabets.DNA.T, Alphabets.DNA.G, Alphabets.DNA.C };

            // Dictionary to hold coverage profile.
            ConcurrentDictionary<long, double[]> coverageProfile =
                                        new ConcurrentDictionary<long, double[]>();

            // Get the position specific alphabet count.
            foreach (SAMAlignedSequence read in alignmentMapobj.QuerySequences)
            {
                for (int i = 0; i < read.QuerySequence.Count; i++)
                {
                    double[] values;
                    coverageProfile.TryGetValue(read.Pos + i, out values);
                    if (values == null)
                    {
                        coverageProfile[read.Pos + i] = new double[distinctChars.Count];
                    }

                    byte item = read.QuerySequence[i];

                    if (Alphabets.DNA.ValidateSequence(new byte[] { item }, 0, 1))
                    {
                        for (int k = 0; k < coverageProfile[read.Pos + i].Length; k++)
                        {
                            coverageProfile[read.Pos + i][k]++;
                        }
                    }
                    else
                    {
                        coverageProfile[read.Pos + i][distinctChars.IndexOf(item)]++;
                    }
                }
            }


            // Get the position specific alphabet coverage.
            foreach (long i in coverageProfile.Keys)
            {
                double count = coverageProfile[i].Sum();
                for (int j = 0; j < distinctChars.Count; j++)
                {
                    coverageProfile[i][j] = coverageProfile[i][j] / count;
                }
            }

            // Display 
            foreach (long pos in coverageProfile.Keys)
            {
                double[] values = coverageProfile[pos];
                if (possibility == "true")
                {
                    string possibleOccurence = GetMoreOccurences(values[0], values[1], values[2], values[3]);
                    Console.Write("\r\n{0}\t\t{1:0.00}%\t{2:0.00}%\t{3:0.00}%\t{4:0.00}%\t\t{5}", pos.ToString(),
                        values[0] * 100, values[1] * 100, values[2] * 100, values[3] * 100, possibleOccurence);
                }
                else
                {
                    Console.Write("\r\n{0}\t\t{1:0.00}\t{2:0.00}\t{3:0.00}\t{4:0.00}", pos.ToString(),
                        values[0], values[1], values[2], values[3]);
                }


            }

        }

        /// <summary>
        /// Get the sequence item percentage with possibility of occurences
        /// </summary>
        /// <param name="aper">Percentage of A occurences</param>
        /// <param name="tper">Percentage of C occurences</param>
        /// <param name="gper">Percentage of G occurences</param>
        /// <param name="cper">Percentage of T occurences</param>
        /// <returns></returns>
        private static string GetMoreOccurences(
            double aper, double tper, double gper, double cper)
        {
            HashSet<byte> symbols = new HashSet<byte>();

            if (aper > 0.45)
            {
                symbols.Add(Alphabets.DNA.A);
            }

            if (tper > 0.45)
            {
                symbols.Add(Alphabets.DNA.T);
            }
            if (gper > 0.45)
            {
                symbols.Add(Alphabets.DNA.G);
            }
            if (cper > 0.45)
            {
                symbols.Add(Alphabets.DNA.C);
            }

            byte item = resolver.GetConsensus(symbols.ToArray());

            return new string(new[] { (char)item });
        }
    }
}
