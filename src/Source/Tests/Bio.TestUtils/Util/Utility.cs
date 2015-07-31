/****************************************************************************
 * Utility.cs
 * 
 *   This file contains the all the common functions in the automation test cases.
 * 
***************************************************************************/

using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Bio.TestAutomation.Util
{
    /// <summary>
    /// This class contains the all the common functions/variables used by all the automation test cases.
    /// </summary>
    public class Utility
    {
        public XmlUtility xmlUtil;

        /// <summary>
        /// Constructor which sets the filepath
        /// </summary>
        /// <param name="filePath"></param>
        public Utility(string filePath)
        {
            xmlUtil = new XmlUtility(filePath);
        }

        /// <summary>
        /// Gets the IAlphabet for the alphabet string passed.
        /// </summary>
        /// <param name="alphabet">Protein/Dna/Rna</param>
        /// <returns>IAphabet equivalent.</returns>
        public static IAlphabet GetAlphabet(string alphabet)
        {
            IAlphabet alp = null;

            switch (alphabet.ToLower(CultureInfo.CurrentCulture))
            {
                case "protein":
                    alp = Alphabets.Protein;
                    break;
                case "rna":
                    alp = Alphabets.RNA;
                    break;
                case "ambiguousrna":
                    alp = AmbiguousRnaAlphabet.Instance;
                    break;
                case "ambiguousdna":
                    alp = AmbiguousDnaAlphabet.Instance;
                    break;
                case "ambiguousprotein":
                    alp = AmbiguousProteinAlphabet.Instance;
                    break;
                case "dna":
                    alp = Alphabets.DNA;
                    break;
                default:
                    break;
            }

            return alp;
        }

        /// <summary>
        /// Generate random number array inside supplied max range
        /// </summary>
        /// <param name="maxRange">Max value of random number</param>
        /// <param name="count">Return array size.</param>
        /// <returns>Array of Random numbers</returns>
        public static int[] RandomNumberGenerator(int maxRange, int count)
        {
            int[] randomNumbers = new int[count];

            int index = 0;
            while (index < randomNumbers.Length)
            {
                Random rndNumberGenerator = new Random();
                int rndNumber = rndNumberGenerator.Next(maxRange);

                // Add the unique number to the list
                if (!randomNumbers.Contains(rndNumber))
                {
                    randomNumbers[index] = rndNumber;
                    index++;
                }
            }
            return randomNumbers;
        }

        /// <summary>
        /// Gets the file content for the file path passed. 
        /// If the file doesnt exist throw the exception.
        /// </summary>
        /// <param name="filePath">File path, the content of which to be read.</param>
        /// <returns>Content of the Text file.</returns>
        public static string GetFileContent(string filePath)
        {
            string fileContent = string.Empty;

            // Check if the File path exists, if not throw exception.
            if (File.Exists(filePath))
            {
                using (StreamReader textFile = new StreamReader(filePath))
                {
                    fileContent = textFile.ReadToEnd();
                }
            }
            else
            {
                throw new FileNotFoundException(string.Format((IFormatProvider)null, "File '{0}' not found.", filePath));
            }

            return fileContent;
        }

        /// <summary>
        /// Gets the FastQFormatType for the format passed.
        /// </summary>
        /// <param name="formatType">Illumina/Sanger/Solexa</param>
        /// <returns>FastQFormat</returns>
        public static FastQFormatType GetFastQFormatType(string formatType)
        {
            FastQFormatType format = FastQFormatType.Illumina_v1_3;

            switch (formatType)
            {
                case "Illumina":
                    format = FastQFormatType.Illumina_v1_3;
                    break;
                case "Sanger":
                    format = FastQFormatType.Sanger;
                    break;
                case "Solexa":
                    format = FastQFormatType.Solexa_Illumina_v1_0;
                    break;
                default:
                    break;
            }

            return format;
        }

        /// <summary>
        /// Gets default encoded quality scores.
        /// </summary>
        /// <param name="encodedQualityScore">ecoded quality score.</param>
        /// <param name="length">No of quality scores required.</param>
        public static byte[] GetEncodedQualityScores(byte encodedQualityScore, int length)
        {
            byte[] encodedQualityScores = new byte[length];
            for (int i = 0; i < length; i++)
            {
                encodedQualityScores[i] = encodedQualityScore;
            }

            return encodedQualityScores;
        }

        /// <summary>
        /// Gets default encoded quality scores.
        /// </summary>
        /// <param name="formatType">Fastq format type.</param>
        /// <param name="length">No of quality scores required.</param>
        public static string GetDefaultEncodedQualityScores(FastQFormatType formatType, int length)
        {
            char[] encodedQualityScores = new char[length];
            for (int i = 0; i < length; i++)
            {
                encodedQualityScores[i] = (char)QualitativeSequence.GetDefaultQualScore(formatType);
            }

            return new string(encodedQualityScores);
        }

    }
}
