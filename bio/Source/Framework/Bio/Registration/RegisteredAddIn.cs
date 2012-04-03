using System.ComponentModel.Composition.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;

namespace Bio.Registration
{
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

        /// <summary>
        /// Container we used to locate types
        /// </summary>
        private static CompositionContainer container = null;

        #region -- Public Properties--
        /// <summary>
        /// Gets the AddIns folder from Bio installation. 
        /// </summary>
        public static string AddinFolderPath
        {
            get
            {
                string path = Path.Combine(AssemblyResolver.BioInstallationPath, AddinFolder);
                return Directory.Exists(path) ? path : null;
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
                return Directory.Exists(path) ? path : null;
            }
        }

        #endregion -- Public Properties--

        #region -- Public Methods --
        /// <summary>
        /// Gets the instances for both primary folder as well as any add-in folder.
        /// </summary>
        /// <typeparam name="T">Type to look for</typeparam>
        /// <param name="contractName">MEF contract name</param>
        /// <param name="assemblyPath">Assembly path</param>
        /// <param name="filter">Filter</param>
        /// <returns>Located elements (T)</returns>
        internal static IList<T> GetComposedInstancesFromAssemblyPath<T>(string contractName, string assemblyPath, string filter)
            where T : class
        {
            List<T> instances = Compose<T>(contractName, assemblyPath, filter);
            instances.AddRange(GetInstancesFromAssemblyPath<T>(assemblyPath, filter));
            return instances;
        }

        /// <summary>
        /// Gets the instances from any given assembly path with specific filter.
        /// </summary>
        /// <typeparam name="T">Generic - any interface.</typeparam>
        /// <param name="assemblyPath">Assemblies folder location.</param>
        /// <param name="filter">File filter.</param>
        /// <returns>List of Ts.</returns>
        public static IList<T> GetInstancesFromAssemblyPath<T>(string assemblyPath, string filter)
            where T : class
        {
            List<T> instances = new List<T>();

            if (!string.IsNullOrEmpty(assemblyPath))
            {
                // Next, search the add-ins directly for the proper attributes
                foreach (string filename in Directory.GetFiles(assemblyPath, filter))
                {
                    IList<object> registeredInstances = AssemblyResolver.Resolve(filename);
                    IList<T> instancesT = Register<T>(registeredInstances);
                    foreach (T obj in instancesT.Where(
                        obj => obj != null
                            && !instances.Any(at =>
                                string.Compare(at.GetType().FullName, obj.GetType().FullName, StringComparison.OrdinalIgnoreCase) == 0)))
                    {
                        instances.Add(obj);
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
        /// This uses MEF to perform a compose operation.
        /// </summary>
        /// <typeparam name="T">Type to look for</typeparam>
        /// <param name="contractName">MEF contract name</param>
        /// <param name="assemblyPath">Assembly path</param>
        /// <param name="filter">Filter for files</param>
        /// <returns></returns>
        private static List<T> Compose<T>(string contractName, string assemblyPath, string filter) 
            where T : class
        {
            if (container == null)
            {
                ComposablePartCatalog partCatalog;

                var assembly = Assembly.GetEntryAssembly();
                if (assembly != null)
                {
                    partCatalog = new AggregateCatalog(
                        new AssemblyCatalog(assembly),
                        GetCatalogForDirectoryFileSpec(AssemblyResolver.BioInstallationPath, DLLFilter));
                }
                else
                    partCatalog = GetCatalogForDirectoryFileSpec(AssemblyResolver.BioInstallationPath, DLLFilter);

                // Add the add-in folder if we have one
                if (!string.IsNullOrEmpty(assemblyPath) && Directory.Exists(assemblyPath))
                {
                    var addinCatalog = GetCatalogForDirectoryFileSpec(assemblyPath, filter);
                    var defaultCatalogEp = new CatalogExportProvider(partCatalog);
                    container = new CompositionContainer(addinCatalog, defaultCatalogEp);
                    defaultCatalogEp.SourceProvider = container;
                }
                else
                    container = new CompositionContainer(partCatalog);
            }

            List<T> availableTypes = new List<T>();
            var exportList = container.GetExports<T>(contractName);
            foreach (Lazy<T> foundType in exportList)
            {
                try
                {
                    T value = foundType.Value;
                    if (value != null)
                        availableTypes.Add(value);
                }
                catch (Exception ex)
                {
                    // Ignore if can't create.
                    Debug.WriteLine(ex.ToString());
                }
            }

            return availableTypes;
        }

        /// <summary>
        /// This method creates an aggregate catalog for each located assembly
        /// in the specified directory.  We internally handle exceptions here so
        /// unloadable assemblies are ignored.
        /// </summary>
        /// <param name="directoryName">Directory to probe</param>
        /// <param name="fileSpec">File specification</param>
        /// <returns>Catalog</returns>
        private static AggregateCatalog GetCatalogForDirectoryFileSpec(string directoryName, string fileSpec)
        {
            string[] files = Directory.GetFiles(directoryName, fileSpec, SearchOption.TopDirectoryOnly);

            AggregateCatalog aggCat = new AggregateCatalog();
            foreach (string file in files)
            {
                try
                {
                    var name = AssemblyName.GetAssemblyName(file);
                    if (name != null)
                    {
                        var asm = Assembly.Load(name);
                        if (asm != null)
                        {
                            // Create an assembly catalog -- only keep it around if we have
                            // parts to resolve or needed parts inside it.
                            var catalog = new AssemblyCatalog(asm);
                            if (catalog.Parts.Any())
                                aggCat.Catalogs.Add(catalog);
                            else
                                catalog.Dispose();
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Ignore if we cannot load the assembly.
                    Debug.WriteLine(ex.ToString());
                }
            }
            return aggCat;
        }

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
    