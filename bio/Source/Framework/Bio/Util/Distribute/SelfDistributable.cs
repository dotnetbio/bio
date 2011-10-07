using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bio.Util;
using Bio.Util.ArgumentParser;

namespace Bio.Util.Distribute
{
    /// <summary>
    /// Convince base class for classes that wish to be executed from the command line with a -Distribute option for submitting to cluster.
    /// </summary>
    public abstract class SelfDistributable : IDistributable, IRunnable
    {
        private IDistribute distribute = new Locally() { ParallelOptions = ParallelOptionsScope.FullyParallelOptions };

        /// <summary>
        /// The name
        /// </summary>
        public virtual string JobName { get { return this.GetType().ToTypeString(); } }

        /// <summary>
        /// How to distribute this object.
        /// </summary>
        public IDistribute Distribute
        {
            get { 
            return distribute;
            }
            set
            {
                distribute = value;
            }
        }

        /// <summary>
        /// Run tasks.
        /// </summary>
        /// <param name="tasksToRun">Tasks To Run.</param>
        /// <param name="taskCount">Task Count.</param>
        abstract public void RunTasks(RangeCollection tasksToRun, long taskCount);

        /// <summary>
        /// Cleanup.
        /// </summary>
        /// <param name="taskCount">Task Count.</param>
        abstract public void Cleanup(long taskCount);

        /// <summary>
        /// Run.
        /// </summary>
        public void Run()
        {
            var distributeNow = Distribute;
            this.Distribute = null;
            distributeNow.Distribute(this);    // clear distribute in case we're submitting to cluster. That's a lot of command line junk to send to the cluster for no reason!
            this.Distribute = distributeNow;
        }

        /// <summary>
        /// A do-nothing implementation.
        /// </summary>
        public virtual void Cancel() { }
    }
}
