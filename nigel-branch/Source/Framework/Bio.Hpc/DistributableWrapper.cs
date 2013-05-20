using System;
using Bio.Util;
using Bio.Util.ArgumentParser;
using Bio.Util.Distribute;

namespace Bio.Hpc
{
    /// <summary>
    /// Implementation of IDistributable
    /// </summary>
    class DistributableWrapper : IDistributable
    {
        #region private members
        private CommandArguments _argCollection;
        #endregion

        #region public methods
        /// <summary>
        /// Gets the job name
        /// </summary>
        public string JobName { get { return SpecialFunctions.GetEntryOrCallingAssembly().GetName().Name; } }

        /// <summary>
        /// Constructor with cmd arg
        /// </summary>
        /// <param name="argCollection">Command Arguments</param>
        public DistributableWrapper(CommandArguments argCollection)
        {
            _argCollection = argCollection;
            _argCollection.ExtractOptional<int>("TaskCount", -1);   // get rid of this, because it will be added again later.
            _argCollection.ExtractOptional<RangeCollection>("Tasks", null);   // get rid of this, because it will be added again later.
            _argCollection.ExtractOptional<bool>("Cleanup", true);   // get rid of this, because it will be added again later.
        }

        /// <summary>
        /// Run the tasks
        /// </summary>
        /// <param name="tasksToRun">Tasks to be run</param>
        /// <param name="taskCount">number of tasks</param>
        public void RunTasks(RangeCollection tasksToRun, long taskCount)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Clean up after run
        /// </summary>
        /// <param name="taskCount">number of tasks</param>
        public void Cleanup(long taskCount)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Cancel the tasks
        /// </summary>
        public void Cancel()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a string representation in which the Distributor is turned into a CommandArguments string and appended to the end of the argument collection.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = _argCollection.ToString();
            return result;
        }
        #endregion
    }
}
