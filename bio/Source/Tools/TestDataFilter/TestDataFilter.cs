using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bio;
using Bio.IO.FastA;

namespace TestDataFilter
{
    /// <summary>
    /// Test Data Filter which is used to filter the repeating characters from a given fasta file.
    /// </summary>
    class TestDataFilter
    {
        /// <summary>
        /// Main method
        /// </summary>
        /// <param name="args">Arguments</param>
        static void Main(string[] args)
        {
            // Check if only 2 or 3 arguments are passed
            if (2 > args.Length || 4 < args.Length)
            {
                Console.WriteLine("Update all the parameters >>'Input File Path' 'Output File Path' 'Repeat Length'");
            }

            // Consider the repeat length as 15 if no value is passed.
            switch (args.Length)
            {
                case 3:
                    FilterTestData(args[0], args[1],
                        int.Parse(args[2], (IFormatProvider)null));
                    break;
                case 2:
                    FilterTestData(args[0], args[1], 15);
                    break;
                default:
                    Console.WriteLine("Invalid entry.");
                    break;
            }
        }

        /// <summary>
        /// Filters the test data for the input file
        /// </summary>
        /// <param name="inputFile">Input File</param>
        /// <param name="outputFile">Output File</param>
        /// <param name="repeatLength">Repeat Length</param>
        static void FilterTestData(string inputFile, string outputFile,
            int repeatLength)
        {
            if (File.Exists(inputFile))
            {
                Console.WriteLine(string.Format("Processing the file '{0}'.", inputFile));

                // Read the inputfile with the help of FastA Parser           
                using (FastAParser parserObj = new FastAParser(inputFile))
                {
                    using (FastAFormatter outputWriter = new FastAFormatter(outputFile))
                    {
                        IEnumerable<ISequence> inputReads = parserObj.Parse();

                        // Going through read by read in a given file
                        foreach (ISequence seq in inputReads)
                        {
                            // Get the First read in the file
                            byte[] actualRead = seq.ToArray();

                            // Assign the temporary local variables required
                            byte previousChar = actualRead[0];
                            int repeatLenCount = 0;
                            bool ignoreRead = false;

                            // Go through each and every character/byte in the read
                            for (int j = 1; j < actualRead.Length; j++)
                            {
                                // Check if the previous character is same as current.
                                if (previousChar == actualRead[j])
                                {
                                    repeatLenCount++;

                                    // if repeat length exceeds, skip this read and continue with other read
                                    if (repeatLenCount == repeatLength)
                                    {
                                        Console.WriteLine(string.Format("Character '{0}' repeated more than '{1}' times and read '{2}' is skipped",
                                         (char)previousChar, repeatLength, seq.ID));
                                        ignoreRead = true;
                                        break;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    repeatLenCount = 0;
                                    previousChar = actualRead[j];
                                    continue;
                                }
                            }

                            Console.WriteLine(string.Format("Read '{0}' Processed.", seq.ID));

                            // Check if the length exceeds the max length and write it to the output file
                            if (!ignoreRead)
                            {
                                outputWriter.Write(seq);
                            }
                        }
                    }
                }
                Console.WriteLine();
                Console.WriteLine("Filtering Completed!!");
            }
            else
                Console.WriteLine("Enter Valid File Path.");
        }
    }
}
