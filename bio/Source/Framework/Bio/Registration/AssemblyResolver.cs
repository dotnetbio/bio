using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Win32;

namespace Bio.Registration
{
    /// <summary>
    /// Internal class gets the instance of defined (RegistrableAttribute) 
    /// attribute in Bio namespace.
    /// </summary>
    public static class AssemblyResolver
    {
        #region -- Public Methods --
        /// <summary>
        /// Gets the Bio installed path from current assembly location.
        /// </summary>
        /// <returns></returns>
        public static string BioInstallationPath
        {
            get
            {
                // NOTE: Previous implementation abused the try / catch handling instead of just
                // checking for null.

                // typical path is
                // \Program Files\.NET Bio\
                // it needs to get it from installer.
                // for any exe under Bio.
                var assembly = Assembly.GetEntryAssembly();
                if (assembly != null)
                    return Path.GetDirectoryName(assembly.Location);

                string codeBase = Assembly.GetCallingAssembly().CodeBase;
                Uri uri = new Uri(codeBase);

                // just for excel specific
                if (codeBase.Contains("exce..vsto"))
                {
                    // look into [HKEY_CURRENT_USER\Software\Microsoft\Office\Excel\Addins\ExcelWorkbench]
                    RegistryKey regKeyAppRoot = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Office\Excel\Addins\ExcelWorkbench");
                    if (regKeyAppRoot != null)
                        uri = new Uri(regKeyAppRoot.GetValue("Manifest").ToString());
                }

                return Uri.UnescapeDataString(Path.GetDirectoryName(uri.AbsolutePath));
            }
        }
                
        /// <summary>
        /// Resolves the local/loaded assembly with the registered attribute.
        /// </summary>
        /// <returns>List of objects.</returns>
        public static IList<object> Resolve()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return Resolve(assembly);
        }

        /// <summary>
        /// Resolves the specified assembly with the registered attribute.
        /// </summary>
        /// <param name="assemblyName">Full Assembly name.</param>
        /// <returns>List of objects.</returns>
        public static IList<object> Resolve(string assemblyName)
        {
            Assembly assembly;
            if (assemblyName.Contains(Path.DirectorySeparatorChar.ToString())
                || assemblyName.Contains(Path.AltDirectorySeparatorChar.ToString())
                || assemblyName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                assembly = Assembly.LoadFrom(assemblyName);
            else
                assembly = Assembly.Load(assemblyName);

            return Resolve(assembly);
        }

        #endregion -- Public Methods --

        #region -- Private Methods --
        /// <summary>
        /// Creates the instance of specified type.
        /// </summary>
        /// <param name="assembly">Assembly reference.</param>
        /// <returns>List of objects.</returns>
        private static IList<object> Resolve(Assembly assembly)         
        {
            List<object> resolvedTypes = new List<object>();
            Type[] availableTypes = assembly.GetExportedTypes();

            foreach (Type availableType in availableTypes)
            {
                RegistrableAttribute registrableAttribute = (RegistrableAttribute)Attribute.GetCustomAttribute(
                    availableType, typeof(RegistrableAttribute));
                if (registrableAttribute != null && registrableAttribute.IsRegistrable)
                {
                    try
                    {
                        resolvedTypes.Add(Activator.CreateInstance(availableType));
                    }
                    catch
                    {
                    }
                }
            }

            return resolvedTypes;
        }
        #endregion -- Private Methods --
    }
}
