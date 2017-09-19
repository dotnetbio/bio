using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
#if !SILVERLIGHT
using System.Runtime.Serialization.Formatters.Binary;
#endif
using System.Collections;

namespace Bio.Util.ArgumentParser
{
    /// <summary>
    /// IRunnable interface.
    /// </summary>
    public interface IRunnable
    {
        /// <summary>
        /// Run
        /// </summary>
        void Run();
    }
}
