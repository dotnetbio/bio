using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Bio
{
    /// <summary>
    /// Class created for reading data from resource file having library information.
    /// Singleton design pattern is used to create only one instance of class. 
    /// </summary>
    public class CloneLibrary
    {
        #region Fields

        /// <summary>
        /// Private Instance.
        /// </summary>
        private static CloneLibrary instance;

        /// <summary>
        /// Object to use for lock.
        /// </summary>
        private static readonly object Guard = new object();

        /// <summary>
        /// List of Information about Clone libraries
        /// Duplicate libraries not allowed.
        /// </summary>
        private readonly Dictionary<string, CloneLibraryInformation> libraries = new Dictionary<string, CloneLibraryInformation>();

        #endregion

        #region Constructor

        /// <summary>
        /// Prevents a default instance of the CloneLibrary class from being created.
        /// Initializes a instance of the CloneLibrary class.
        /// </summary>
        private CloneLibrary()
        {
            this.ReadLibrary();
        }

        #endregion

        #region Singleton

        /// <summary>
        /// Gets an instance of this class.
        /// Property to make sure only one Instance of this class is created.
        /// </summary>
        public static CloneLibrary Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (Guard)
                    {
                        if (instance == null)
                        {
                            instance = new CloneLibrary();
                        }
                    }
                }

                return instance;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the information about libraries.
        /// </summary>
        public IList<CloneLibraryInformation> GetLibraries
        {
            get
            {
                return this.libraries.Values.ToList();
            }
        }

        #endregion

        #region Public Method

        /// <summary>
        /// Returns information about Library.
        /// </summary>
        /// <param name="libraryName"> Name of Library.</param>
        /// <returns>Struct containing Information about Library.</returns>
        public CloneLibraryInformation GetLibraryInformation(string libraryName)
        {
            CloneLibraryInformation cloneLibrary;

            if (!this.libraries.TryGetValue(libraryName, out cloneLibrary))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Properties.Resource.LibraryExist, libraryName));
            }

            return cloneLibrary;
        }

        /// <summary>
        /// Add Library to existing list of libraries.
        /// </summary>
        /// <param name="library">Library information.</param>
        public void AddLibrary(CloneLibraryInformation library)
        {
            if (null != library && !String.IsNullOrEmpty(library.LibraryName) &&
               library.MeanLengthOfInsert >= 0 && library.StandardDeviationOfInsert >= 0)
            {
                this.libraries[library.LibraryName] = library;
            }
            else
            {
                throw new ArgumentException(Properties.Resource.LibraryInvalidParameters);
            }           
        }

        /// <summary>
        /// Add Library to existing list of libraries.
        /// </summary>
        /// <param name="libraryName">Name of Library.</param>
        /// <param name="mean">Mean Length Of Insert.</param>
        /// <param name="standardDeviation">Standard Deviation Of Insert.</param>
        public void AddLibrary(string libraryName, float mean, float standardDeviation)
        {
            if (String.IsNullOrEmpty(libraryName) || mean < 0 || standardDeviation < 0)
            {
                throw new ArgumentException(Properties.Resource.LibraryInvalidParameters);
            }

            CloneLibraryInformation library = new CloneLibraryInformation()
            {
                LibraryName = libraryName,
                MeanLengthOfInsert = mean,
                StandardDeviationOfInsert = standardDeviation
            };
            
            this.libraries[libraryName] = library;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Read Libraries from file.
        /// </summary>
        private void ReadLibrary()
        {
            Stream libData = typeof(CloneLibrary).GetTypeInfo().Assembly.GetManifestResourceStream("Bio.CloneLibrary.Resources.Library.txt");
            if (libData == null)
                throw new Exception("Failed to load Clone Library data from resources.");

            using (TextReader reader = new StreamReader(libData))
            {
                var library = reader.ReadLine();
                while (!string.IsNullOrEmpty(library))
                {
                    this.Parse(library);
                    library = reader.ReadLine();
                }
            }
        }

        /// <summary>
        /// Parse Library and convert parsed data into structure.
        /// </summary>
        /// <param name="library">Name of Library.</param>
        private void Parse(string library)
        {
            string[] libraryInformation = library.Split(new char[] { ' ' }, 3);
            CloneLibraryInformation information = new CloneLibraryInformation()
            {
                LibraryName = libraryInformation[0],
                MeanLengthOfInsert = float.Parse(libraryInformation[1], CultureInfo.InvariantCulture),
                StandardDeviationOfInsert = float.Parse(libraryInformation[2], CultureInfo.InvariantCulture)
            };

            this.libraries.Add(information.LibraryName, information);
        }

        #endregion
    }
}
