using System;
using System.Linq;
using System.Threading.Tasks;
using Bio.Util.ArgumentParser;

namespace Bio.Util.Distribute
{
    /// <summary>
    /// Runs tasks locally.
    /// </summary>
    public class Locally : IDistribute, IParsable
    {
        /// <summary>
        /// Task count.
        /// </summary>
        private int taskCount = 1;

        /// <summary>
        /// Range Collection tasks.
        /// </summary>
        private RangeCollection tasks = new RangeCollection(0);

        /// <summary>
        /// Parallel options.
        /// </summary>
        private ParallelOptions parallelOptions = ParallelOptionsScope.FullyParallelOptions;

        /// <summary>
        /// cleanUp.
        /// </summary>
        private bool cleanup = true;

        /// <summary>
        /// How many pieces should the work be divided into?
        /// </summary>
        public int TaskCount
        {
            get { return taskCount; }
            set { taskCount = value; }
        }

        /// <summary>
        /// The set of Tasks that should be run by this instance
        /// </summary>
        public RangeCollection Tasks
        {
            get { return tasks; }
            set { tasks = value; }
        }

        /// <summary>
        /// Specifies whether cleanup should be run when this task is complete.
        /// </summary>
        public bool Cleanup
        {
            get { return cleanup; }
            set { cleanup = value; }
        }

        /// <summary>
        /// Specifies the local parallel options
        /// </summary>
        [Parse(ParseAction.Optional, typeof(ParallelOptionsParser))]
        public ParallelOptions ParallelOptions
        {
            get { return parallelOptions; }
            set { parallelOptions = value; }
        }

        /// <summary>
        /// Runs Tasks locally on distributableObject.
        /// </summary>
        /// <param name="distributableObject">The object that will run the tasks.</param>
        public void Distribute(IDistributable distributableObject)
        {
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                distributableObject.Cancel();
                Environment.ExitCode = -1073741510; // exit by control break
            };

            using (ParallelOptionsScope.Create(ParallelOptions))
            {
                distributableObject.RunTasks(Tasks, TaskCount);

                if (Cleanup)
                    distributableObject.Cleanup(TaskCount);
            }
        }

        /// <summary>
        /// Finalize Parse.
        /// </summary>
        public void FinalizeParse()
        {
            Helper.CheckCondition<ParseException>(TaskCount > 0, "TaskCount must be at least 1.");
            Helper.CheckCondition<ParseException>(!Tasks.Any() || Tasks.FirstElement >= 0 && Tasks.LastElement < TaskCount, "The tasks range {0} is not between 0 and TaskCount {1}", Tasks, TaskCount);
        }
    }
}
