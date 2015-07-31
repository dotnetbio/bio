using System;
using System.IO;

namespace Bio.IO
{
    /// <summary>
    /// Interface that has a name and that can create streams. It can be used as a generalization of file names, FileInfo, reading data from a string,
    /// reading data from a resource, etc.
    /// </summary>
    public interface INamedStreamCreator
    {
        /// <summary>
        /// The name of the data source. This might be a file name, a resource name, the name of a text box on a form, etc. This name is mostly used
        /// in error messages when there is an error in the data to help the user understand which data source has the problem.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A factor that creates a stream.
        /// </summary>
        Func<Stream> Creator { get; }
    }
}