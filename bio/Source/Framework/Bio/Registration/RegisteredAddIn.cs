namespace Bio.Registration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Self registration is used to get the collection of object which uses the 
    /// specific custom attribute as part of registration process with Bio.
    /// </summary>
    public static class RegisteredAddIn
    {
        /// <summary>
        /// Dll filter.
        /// </summary>
        public const string DLLFilter = "*.dll";

        /// <summary>
        /// Folder for Add-ins.
        /// </summary>
        private const string AddinFolder = @"Add-ins";

        /// <summary>
        /// The core folder.
        /// </summary>
        private const string CoreFolder = @"..\..\Microsoft Biology Framework";
        #region -- Public Properties--
        /// <summary>
        /// Gets the AddIns folder from Bio installation. 
        /// </summary>
        public static string AddinFolderPath
        {
            get
            {    
                string path = Path.Combine(AssemblyResolver.BioInstallationPath, AddinFolder);
                if (Directory.Exists(path))
                {
                    return path;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the Core folder from Bio installation. 
        /// </summary>
        public static string CoreFolderPath
        {
            get
            {
                string path = Path.Combine(AssemblyResolver.BioInstallationPath, CoreFolder);
                if (Directory.Exists(path))
                {
                    return path;
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion -- Public Properties--

        #region -- Public Methods --
        /// <summary>
        /// Gets all registered alphabets in core folder and addins (optional) folders.
        /// </summary>
        /// <param name="includeAddinFolder">Include add-ins folder or not.</param>
        /// <returns>List of registered alphabets.</returns>
        public static IList<IAlphabet> GetAlphabets(bool includeAddinFolder)
        {
            IList<IAlphabet> registeredAlphabets = new List<IAlphabet>();

            if (includeAddinFolder)
            {
                IList<IAlphabet> addInAlphabets;
                if (null != AddinFolderPath)
                {
                    addInAlphabets = GetInstancesFromAssemblyPath<IAlphabet>(AddinFolderPath, DLLFilter);
                    if (null != addInAlphabets && addInAlphabets.Count > 0)
                    {
                        foreach (IAlphabet alphabet in addInAlphabets)
                        {
                            if (alphabet != null && registeredAlphabets.FirstOrDefault(IA => string.Compare(
                                IA.Name, alphabet.Name, StringComparison.OrdinalIgnoreCase) == 0) == null)
                            {
                                registeredAlphabets.Add(alphabet);
                            }
                        }
                    }
                }
            }

            return registeredAlphabets;
        }

        /// <summary>
        /// Gets the instances from any given assembly path with specific filter.
        /// </summary>
        /// <typeparam name="T">Generic - any interface.</typeparam>
        /// <param name="assemblyPath">Assemblies folder location.</param>
        /// <param name="filter">File filter.</param>
        /// <returns>List of Ts.</returns>
        public static IList<T> GetInstancesFromAssemblyPath<T>(string assemblyPath, string filter)
        {
            IList<T> instances = new List<T>();
            foreach (string filename in Directory.GetFiles(assemblyPath, filter))
            {
                IList<object> registeredInstances = AssemblyResolver.Resolve(filename);
                IList<T> instancesT = Register<T>(registeredInstances);
                foreach (T obj in instancesT)
                {
                    if (obj != null && instances.FirstOrDefault(IA => string.Compare(
                        IA.GetType().FullName, 
                        obj.GetType().FullName, 
                        StringComparison.OrdinalIgnoreCase) == 0) == null)
                    {
                        instances.Add((T)obj);
                    }
                }
            }

            return instances;
        }

        /// <summary>
        /// Gets the instances from any given assembly file. 
        /// </summary>
        /// <typeparam name="T">Generic - any interface.</typeparam>
        /// <param name="assemblyName">Assembly file.</param>
        /// <returns>List of Ts.</returns>
        public static IList<T> GetInstancesFromAssembly<T>(string assemblyName)
        {
            IList<object> registeredInstances = AssemblyResolver.Resolve(assemblyName);
            return Register<T>(registeredInstances);
        }

        /// <summary>
        /// Gets the instances from the executing assembly.
        /// </summary>
        /// <typeparam name="T">Generic - any interface.</typeparam>
        /// <returns>List of Ts.</returns>
        public static IList<T> GetInstancesFromExecutingAssembly<T>()
        {
            IList<object> registeredInstances = AssemblyResolver.Resolve();
            return Register<T>(registeredInstances);
        }
        #endregion -- Public Methods --

        #region -- Private Methods --

        /// <summary>
        /// Registers the set of registerable objects into collection.
        /// </summary>
        /// <typeparam name="T">Type of param.</typeparam>
        /// <param name="objects">List of objects.</param>
        /// <returns>Returns an list of instances.</returns>
        private static IList<T> Register<T>(IList<object> objects)
        {
            IList<T> instances = new List<T>();
            if (objects.Count > 0)
            {
                foreach (object obj in objects)
                {
                    if (null != obj.GetType().GetInterface(typeof(T).FullName))
                    {
                        instances.Add((T)obj);
                    }
                }
            }

            return instances;
        }
        
        #endregion -- Private Methods --
    }
}
