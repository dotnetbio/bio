using System.Collections.Generic;
using System;

namespace DeveloperPreRequisiteCheck
{
    /// <summary>
    /// This class implements IComponentValidator. 
    /// This class validates if the specified version of office runtime is installed on the m/c.
    /// </summary>
    public class VSTOValidator : IComponentValidator
    {
        /// <summary>
        /// Path of registry where the version of installed office runtime can be found.
        /// </summary>
        private readonly string RegistryPath;

        /// <summary>
        /// Registry key where the version of installed office runtime can be found.
        /// </summary>
        private const string RegistryKey = "Version";

        /// <summary>
        /// Minimum required version of Visual Studio Runtime.
        /// </summary>
        private const string MinimumVersion = "3.0.0.0";

        /// <summary>
        /// Parameter required by Validator component.
        /// </summary>
        private Dictionary<string, string> _parameters = null;

        /// <summary>
        /// Default Constructor: Creates an instance of VSTOValidator class.
        /// </summary>
        public VSTOValidator()
        {
            _parameters = new Dictionary<string, string>();

            // Check for 32 bit / 64 bit OS
            if (IntPtr.Size == 4)
                RegistryPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\vsto runtime Setup\v4R";
            else
                RegistryPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\vsto runtime Setup\v4R";
        }

        /// <summary>
        /// Gets the name of component.
        /// </summary>
        public string Name
        {
            get { return Properties.Resources.VSTO_NAME; }
        }

        /// <summary>
        /// Gets the short description of component.
        /// </summary>
        public string Description
        {
            get { return Properties.Resources.VSTO_DESCRIPTION; }
        }

        /// <summary>
        /// Gets the minimum supported version of component.
        /// </summary>
        public string Version
        {
            get { return MinimumVersion; }
        }

        /// <summary>
        /// Gets the parameter required by Validator component.
        /// </summary>
        public Dictionary<string, string> Parameters { get { return _parameters; } }
        
        /// <summary>
        /// Validate if the component is installed.
        ///  1. If not, provide a message to install the component.
        ///  2.	If yes, provide a message directing user to copy the folders/assemblies to required target folder.
        /// </summary>
        /// <returns>Validation result.</returns>
        public ValidationResult Validate()
        {
            string version = string.Empty;
            ValidationResult result = null;

            if (Utility.ReadRegistry(RegistryPath, RegistryKey, out version))
            {
                if (Utility.CompareVersion(MinimumVersion, version))
                {
                    result = new ValidationResult(true,
                        string.Format(Properties.Resources.VSTO_FOUND, version));
                }
                else
                {
                    result = new ValidationResult(false, Properties.Resources.VSTO_NOTFOUND);
                }
            }
            else
            {
                result = new ValidationResult(false, Properties.Resources.VSTO_NOTFOUND);
            }

            return result;
        }
    }
}
