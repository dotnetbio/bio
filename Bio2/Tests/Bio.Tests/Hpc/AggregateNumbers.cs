using System.IO;
using System.Linq;
using System.Threading;
using Bio.Util;
using Bio.Util.ArgumentParser;
using Bio.Util.Distribute;

namespace Bio.Tests
{
    /// <summary>
    /// Example distributable application that take a list of numbers from an input file and performs some aggregate operation. The result is stored in a Result file.
    /// </summary>
    public class AggregateNumbers : SelfDistributable
    {
        /// <summary>
        /// The input file must contain a list of doubles, one per line.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        [Parse(ParseAction.Required, typeof(InputFile))]
        public FileInfo InputFile;

        /// <summary>
        /// The operation you want done.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        [Parse(ParseAction.Required)]
        public IBinaryOperator Operator;

        /// <summary>
        /// Where the results will live.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        [Parse(ParseAction.Optional, typeof(OutputFile))]
        public FileInfo ResultFile = new FileInfo(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6) + @"\TestUtils\results.txt");

        /// <summary>
        /// How long to sleep between each step.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        public int SleepMilliseconds = 1000;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tasksToRun"></param>
        /// <param name="taskCount"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Double.Parse(System.String)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public override void RunTasks(RangeCollection tasksToRun, long taskCount)
        {
            var allTheWorkQuery = InputFile.ReadEachLine();
            var myWorkAndAnIndex = SpecialFunctions.DivideWork(allTheWorkQuery, tasksToRun, taskCount, 1, new RangeCollection());

            double myTotal = double.NaN;
            bool isFirst = true;
            foreach (var numberAndIndex in myWorkAndAnIndex)
            {
                double number = double.Parse(numberAndIndex.Key);
                Thread.Sleep(SleepMilliseconds); //make the work take longer
                if (isFirst)
                {
                    myTotal = number;
                    isFirst = false;
                }
                else
                {
                    myTotal = Operator.Aggregate(myTotal, number);
                }
            }

            if (!isFirst)   // make sure we actually did something.
            {
                //var myUniqueResultFileName = GetFileTaskFileName(tasksToRun.ToString());

                //ResultFile = new FileInfo(myUniqueResultFileName);
                using (TextWriter writer = File.CreateText(ResultFile.FullName))
                {
                    writer.WriteLine(myTotal);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskCount"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Double.Parse(System.String)")]
        public override void Cleanup(long taskCount)
        {
        }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object[])")]
        //private string GetFileTaskFileName(string taskLabel)
        //{
        //    var relativeDirectory = Path.GetDirectoryName(ResultFile.ToString());

        //    var myUniqueResultFileName = string.Format("{0}\\{1}.{2}{3}",
        //        string.IsNullOrEmpty(relativeDirectory) ? "." : relativeDirectory,
        //        Path.GetFileNameWithoutExtension(ResultFile.Name),
        //        taskLabel,
        //        Path.GetExtension(ResultFile.Name));
        //    return myUniqueResultFileName;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        static void Main(string[] args)
        {
            CommandArguments.ConstructAndRun<AggregateNumbers>(args);
        }
    }
}
