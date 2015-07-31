using System.IO;
using System.Linq;

using Bio;
using Bio.IO;
using Bio.Util;
using Bio.Util.ArgumentParser;
using Bio.Util.Distribute;
using System.Globalization;
using System;

namespace SampleClusterApp
{
    /// <summary>
    /// Example distributable application that take a list of sequences from a FastA file 
    /// and performs GC Content operation. The result will be stored in a Result file.
    /// </summary>
    public class SequenceContent : SelfDistributable
    {
        /// <summary>
        /// The input file must contain a list of Sequences in FastA format.
        /// </summary>
        [Parse(ParseAction.Required, typeof(InputFile))]
        public FileInfo InputFile;

        /// <summary>
        /// Where the results will live.
        /// </summary>
        [Parse(ParseAction.Optional, typeof(OutputFile))]
        public FileInfo ResultFile = new FileInfo("results.txt");

        /// <summary>
        /// Runs the tasks
        /// </summary>
        /// <param name="tasksToRun">number of tasks to be ran in a range collection</param>
        /// <param name="taskCount">task count</param>
        public override void RunTasks(RangeCollection tasksToRun, long taskCount)
        {
            ISequenceParser parser = SequenceParsers.FindParserByFileName(InputFile.FullName);
            var allTheWorkQuery = parser.Parse(InputFile.FullName);
            var myWorkAndAnIndex = SpecialFunctions.DivideWork(allTheWorkQuery, tasksToRun, taskCount, 1, new RangeCollection());

            var myUniqueResultFileName = GetFileTaskFileName(tasksToRun.ToString());

            float gcCount = 0;
            long seqLength = 0;

            using (TextWriter writer = File.CreateText(myUniqueResultFileName))
            {
                // loop all sequences in current task
                foreach (var numberAndIndex in myWorkAndAnIndex)
                {
                    writer.WriteLine(">" + numberAndIndex.Key.ID);
                    foreach (byte val in numberAndIndex.Key)
                    {
                        seqLength++;
                        switch (val)
                        {
                            case (byte)'G':
                            case (byte)'g':
                            case (byte)'C':
                            case (byte)'c':
                                gcCount++;
                                break;
                        }
                    }

                    if (gcCount > 0)
                        writer.Write(((gcCount / (float)seqLength) * 100) + "%");
                    else
                        writer.Write(gcCount + "%");

                    seqLength = 0;
                    gcCount = 0;
                    writer.WriteLine();
                }
            }
        }

        /// <summary>
        /// Cleanup task
        /// </summary>
        public override void Cleanup(long taskCount)
        {
            var taskFilePattern = GetFileTaskFileName("*");
            var taskFiles = new DirectoryInfo(".").GetFiles(taskFilePattern).ToList();

            using (var writer = ResultFile.CreateText())
            {
                foreach (var file in taskFiles)
                {
                    writer.Write(File.ReadAllText(file.FullName));
                }
            }

            foreach (var file in taskFiles)
            {
                file.Delete();
            }
        }

        /// <summary>
        /// Main method
        /// Sample args :-InputFile InputFile("(C:\Temp\hpc.fasta)")
        /// Sample args with distribute: -Distribute OnHpcAndWait(MSR-L25-DEV29,26,Priority:AboveNormal,IsExclusive:True) -InputFile InputFile("(\\MSR-L25-DEV29\scratch\Bio.HPC\hpc.fasta)")
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.Error.WriteLine(Properties.Resources.SampleClusterApp_Splash);
            try
            {
                CommandArguments.ConstructAndRun<SequenceContent>(args, false);
            }
            catch (HelpException)
            {
                Console.Error.WriteLine(Properties.Resources.SampleClusterApp_Help);
                Environment.ExitCode = 10022;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                Environment.ExitCode = -532462766; // general failure.
            }
        }

        /// <summary>
        /// Get result file path
        /// </summary>
        /// <param name="taskLabel">task name</param>
        /// <returns></returns>
        private string GetFileTaskFileName(string taskLabel)
        {
            var relativeDirectory = Path.GetDirectoryName(ResultFile.ToString());

            var myUniqueResultFileName = string.Format(CultureInfo.CurrentCulture, "{0}\\{1}.{2}{3}",
                string.IsNullOrEmpty(relativeDirectory) ? "." : relativeDirectory,
                Path.GetFileNameWithoutExtension(ResultFile.Name),
                taskLabel,
                Path.GetExtension(ResultFile.Name));

            return myUniqueResultFileName;
        }

    }
}
