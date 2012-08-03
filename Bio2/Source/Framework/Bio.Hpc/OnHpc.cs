using System;
using Bio.Util.Distribute;

namespace Bio.Hpc
{
    /// <summary>
    /// Helps to run it on HPC using distribute
    /// </summary>
    [Serializable]
    public class OnHpc : ClusterSubmitterArgs, IDistribute
    {
        /// <summary>
        /// Distributes the task over HPC
        /// </summary>
        /// <param name="distributableObject">distributable tasks</param>
        public virtual void Distribute(IDistributable distributableObject)
        {
            this.Name = distributableObject.JobName;
            ClusterSubmitter.Submit(this, distributableObject);
        }
    }
}
