using System;
using System.IO;

namespace Bio.IO
{
    /// <summary>
    /// A class that has a name and that can create streams. It can be used as a generalization of file names, FileInfo, reading data from a string,
    /// reading data from a resource, etc.
    /// </summary>
    public class NamedStreamCreator : INamedStreamCreator
    {
        /// <summary>
        /// Constructor for NamedStreamCreator
        /// </summary>
        /// <param name="name">The name of the data source. This might be a file name, a resource name, the name of a text box on a form, etc. This name is mostly used
        /// in error messages when there is an error in the data to help the user understand which data source has the problem.</param>
        /// <param name="creator">A factor that creates a stream.</param>
        public NamedStreamCreator(string name, Func<Stream> creator)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (creator == null)
            {
                throw new ArgumentNullException("creator");
            }

            this.Name = name;
            this.Creator = creator;
        }
        /// <summary>
        /// The name of the data source. This might be a file name, a resource name, the name of a text box on a form, etc. This name is mostly used
        /// in error messages when there is an error in the data to help the user understand which data source has the problem.
        /// </summary>
        public string Name { get; private set; }
        
        /// <summary>
        /// A factory that creates a stream.
        /// </summary>
        public Func<Stream> Creator { get; private set; }
    }
}
