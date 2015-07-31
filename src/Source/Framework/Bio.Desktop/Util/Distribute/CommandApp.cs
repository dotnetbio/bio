using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bio.Util;
using System.IO;
using Bio.Util.ArgumentParser;
using System.Diagnostics;

namespace Bio.Util.Distribute
{
    /// <summary>
    /// Utility for distributing an arbitrary command.
    /// </summary>
    public class CommandApp : SelfDistributable
    {
        /// <summary>
        /// Executable Name.
        /// </summary>
        [Parse(ParseAction.Required)]
        public string ExeName { get; set; }

        /// <summary>
        /// Command Args.
        /// </summary>
        [Parse(ParseAction.Required)]
        public string CommandArgs { get; set; }

        /// <summary>
        /// Run Tasks.
        /// </summary>
        /// <param name="tasksToRun">Tasks to Run.</param>
        /// <param name="taskCount">Task Count.</param>
        public override void RunTasks(RangeCollection tasksToRun, long taskCount)
        {
#if !SILVERLIGHT
            using (Process proc = new Process())
            {
                proc.StartInfo.FileName = ExeName;
                proc.StartInfo.Arguments = CommandArgs;
                proc.StartInfo.UseShellExecute = false;
                proc.Start();
                proc.WaitForExit();
                if (proc.ExitCode > 0)
                    throw new Exception("Unable to execute " + ExeName + " " + CommandArgs);
            }
#endif
        }

        /// <summary>
        /// Cleanup.
        /// </summary>
        /// <param name="taskCount">Task Count.</param>
        public override void Cleanup(long taskCount)
        {
            return;
        }
    }
}
