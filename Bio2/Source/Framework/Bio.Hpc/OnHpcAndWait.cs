using System;
using System.Collections.Generic;
using System.Linq;
using Bio.Util.ArgumentParser;
using Bio.Util.Distribute;

namespace Bio.Hpc
{
    /// <summary>
    /// Helps to run it on HPC using distribute
    /// </summary>
    [Serializable]
    public class OnHpcAndWait : OnHpc
    {
        /// <summary>
        /// A list of file patterns that will be copied from the cluster working directory to the local working directory.
        /// </summary>
        public List<string> CopyResults = new List<string>();

        /// <summary>
        /// Max submit after task failed
        /// </summary>
        public int MaxSubmitAfterTasksFail = 0;

        /// <summary>
        /// Fired after the job has been submitted (but may still be waiting).
        /// </summary>
        public event EventHandler Submitted;

        /// <summary>
        /// Distributes the task over HPC
        /// </summary>
        /// <param name="distributableObject">distributable task</param>
        public override void Distribute(IDistributable distributableObject)
        {
            this.Name = distributableObject.JobName;
            ClusterSubmitter.SubmitAndWait(this, distributableObject, MaxSubmitAfterTasksFail, OnSubmitted);
            CopyResults.AddRange(ArgumentCollection.EnumerateValuesOfTypeFromParsable<OutputFile>(distributableObject).Select(file => file.ToString()).Distinct().Where(s => s != "-"));
            if (CopyResults.Count > 0)
                HpcLib.CopyFiles(CopyResults, ExternalRemoteDirectoryName, Environment.CurrentDirectory);
        }
        
        /// <summary>
        /// Clone
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            var result = base.Clone() as OnHpcAndWait;
            result.CopyResults = new List<string>(this.CopyResults);
            return result;
        }

        private void OnSubmitted()
        {
            if (Submitted != null)
            {
                Submitted(this, new EventArgs());
            }
        }
    }
}
