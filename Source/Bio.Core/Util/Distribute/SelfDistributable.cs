using Bio.Util.ArgumentParser;

namespace Bio.Util.Distribute
{
    /// <summary>
    ///     Convince base class for classes that wish to be executed from the command line with a -Distribute option for
    ///     submitting to cluster.
    /// </summary>
    public abstract class SelfDistributable : IDistributable, IRunnable
    {
        private IDistribute distribute = new Locally { ParallelOptions = ParallelOptionsScope.FullyParallelOptions };

        /// <summary>
        ///     How to distribute this object.
        /// </summary>
        public IDistribute Distribute
        {
            get
            {
                return this.distribute;
            }
            set
            {
                this.distribute = value;
            }
        }

        /// <summary>
        ///     The name
        /// </summary>
        public virtual string JobName
        {
            get
            {
                return this.GetType().ToTypeString();
            }
        }

        /// <summary>
        ///     Run tasks.
        /// </summary>
        /// <param name="tasksToRun">Tasks To Run.</param>
        /// <param name="taskCount">Task Count.</param>
        public abstract void RunTasks(RangeCollection tasksToRun, long taskCount);

        /// <summary>
        ///     Cleanup.
        /// </summary>
        /// <param name="taskCount">Task Count.</param>
        public abstract void Cleanup(long taskCount);

        /// <summary>
        ///     A do-nothing implementation.
        /// </summary>
        public virtual void Cancel()
        {
        }

        /// <summary>
        ///     Run.
        /// </summary>
        public void Run()
        {
            IDistribute distributeNow = this.Distribute;
            this.Distribute = null;
            distributeNow.Distribute(this);
                // clear distribute in case we're submitting to cluster. That's a lot of command line junk to send to the cluster for no reason!
            this.Distribute = distributeNow;
        }
    }
}