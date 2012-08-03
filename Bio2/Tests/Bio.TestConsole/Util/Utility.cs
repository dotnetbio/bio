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
using Bio;
using SD = System.Diagnostics;
using Bio.Util.Logging;

namespace Bio.TestConsole.Util
{
    /// <summary>
    /// This class contains the all the common functions/variables used by all the automation test cases.
    /// </summary>
    internal class Utility
    {
        internal XmlUtility xmlUtil;
        internal static string standardOut = string.Empty;
        internal static string standardErr = string.Empty;

        /// <summary>
        /// Constructor which sets the file path
        /// </summary>
        /// <param name="filePath"></param>
        internal Utility(string filePath)
        {
            xmlUtil = new XmlUtility(filePath);
        }

        /// <summary>
        /// Gets the IAlphabet for the alphabet string passed.
        /// </summary>
        /// <param name="alphabet">Protein/Dna/Rna</param>
        /// <returns>IAphabet equivalent.</returns>
        internal static IAlphabet GetAlphabet(string alphabet)
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
        internal static int[] RandomNumberGenerator(int maxRange, int count)
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
        /// If the file doesn't exist throw the exception.
        /// </summary>
        /// <param name="filePath">File path, the content of which to be read.</param>
        /// <returns>Content of the Text file.</returns>
        internal static string GetFileContent(string filePath)
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
        internal static FastQFormatType GetFastQFormatType(string formatType)
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
        /// Launches the specified process with a redirected output stream.
        /// </summary>
        /// <param name="processName">The process to be launched.</param>
        /// <param name="cmdArguments">The command line arguments to the process.</param>
        /// Suppressed this message to execute the below code which requires exposure of methods
        internal static void RunProcess(string processName, string cmdArguments)
        {
            using (StreamWriter writer = new StreamWriter(processName))
            {
                writer.WriteLine(cmdArguments);
            }

            using (SD.Process p = new SD.Process())
            {
                SD.ProcessStartInfo processStartInfo = new SD.ProcessStartInfo(processName);
                processStartInfo.UseShellExecute = false;
                processStartInfo.RedirectStandardError = true;
                processStartInfo.RedirectStandardOutput = true;

                p.OutputDataReceived += new SD.DataReceivedEventHandler(OnOutputDataReceived);
                p.ErrorDataReceived += new SD.DataReceivedEventHandler(OnOutputDataReceived);
                p.StartInfo = processStartInfo;
                p.Start();

                ApplicationLog.WriteLine("");

                standardOut = p.StandardOutput.ReadToEnd();
                standardErr = p.StandardError.ReadToEnd();

                ApplicationLog.WriteLine(standardOut);
                ApplicationLog.WriteLine(standardErr);

                p.WaitForExit();

                processStartInfo = null;
            }

        }

        /// <summary>
        /// Logs the output data received.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event data.</param>
        internal static void OnOutputDataReceived(object sender, SD.DataReceivedEventArgs e)
        {
            ApplicationLog.WriteLine(e.Data);
            standardOut = standardOut + e.Data;
        }

        /// <summary>
        /// Compare the results/output file
        /// </summary>
        /// <param name="file1">File 1 to compare</param>
        /// <param name="file2">File 2 to compare with</param>
        /// <returns>True, if both files are the same.</returns>
        internal static bool CompareFiles(string file1, string file2)
        {
            FileInfo fileInfoObj1 = new FileInfo(file1);
            FileInfo fileInfoObj2 = new FileInfo(file2);

            if (fileInfoObj1.Length != fileInfoObj2.Length)
                return false;

            byte[] bytesFile1 = File.ReadAllBytes(file1);
            byte[] bytesFile2 = File.ReadAllBytes(file2);

            if (bytesFile1.Length != bytesFile2.Length)
                return false;

            for (int i = 0; i <= bytesFile2.Length - 1; i++)
            {
                if (bytesFile1[i] != bytesFile2[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Validates the Scaffolds of one file in another file.
        /// </summary>
        /// <param name="filePath1">File1 containing Expected Scaffolds</param>
        /// <param name="filePath2">File1 containing Actual Scaffolds</param>
        /// <returns>True if Actual file contents all scaffolds of expected file.</returns>
        internal static bool ValidateScaffoldsInAFile(string filePath1, string filePath2)
        {
            string file1Content = GetFileContent(filePath1);
            string[] split = file1Content.Split('>');

            string fileContent2 = GetFileContent(filePath2);

            for (int i = 0; i < split.Length; i++)
            {
                if (!fileContent2.Contains(split[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
