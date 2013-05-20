using System.Collections.Generic;
using System.Globalization;

namespace DeveloperPreRequisiteCheck
{
    /// <summary>
    /// This class implements IComponentValidator. 
    /// This class validates if the specified version of excel is installed on the m/c.
    /// </summary>
    public class ExcelValidator : IComponentValidator
    {
        /// <summary>
        /// Path of registry where the version of installed excel can be found.
        /// </summary>
        private const string RegistryPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\Excel.Application\CurVer";

        /// <summary>
        /// Minimum required version of MS Office (Excel).
        /// </summary>
        private const string MinimumVersion = "Excel.Application.12";

        /// <summary>
        /// Parameter required by Validator component.
        /// </summary>
        private Dictionary<string, string> _parameters = null;

        /// <summary>
        /// Default Constructor: Creates an instance of ExcelValidator class.
        /// </summary>
        public ExcelValidator()
        {
            _parameters = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets the name of component.
        /// </summary>
        public string Name
        {
            get { return Properties.Resources.EXCEL_NAME; }
        }

        /// <summary>
        /// Gets the short description of component.
        /// </summary>
        public string Description
        {
            get { return Properties.Resources.EXCEL_DESCRIPTION; }
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

            if (Utility.ReadRegistry(RegistryPath, string.Empty, out version))
            {
                if (IsVersionValid(version))
                {
                    result = new ValidationResult(true,
                        string.Format(Properties.Resources.EXCEL_FOUND, version));
                }
                else
                {
                    result = new ValidationResult(false, Properties.Resources.EXCEL_NOTFOUND);
                }
            }
            else
            {
                result = new ValidationResult(false, Properties.Resources.EXCEL_NOTFOUND);
            }

            return result;
        }

        /// <summary>
        /// Check if the given version is greater than or equal to minimum version.
        /// </summary>
        /// <param name="version">Current version</param>
        /// <returns>Is version valid</returns>
        private static bool IsVersionValid(string version)
        {
            bool isValid = false;

            string[] minimumVersion = MinimumVersion.Split('.');
            string[] currentVersion = version.Split('.');

            if (currentVersion.Length == 3
                && 0 == string.Compare(minimumVersion[0], currentVersion[0], true)
                && 0 == string.Compare(minimumVersion[1], currentVersion[1], true))
            {
                if (int.Parse(minimumVersion[2], CultureInfo.CurrentCulture)
                    <= int.Parse(currentVersion[2], CultureInfo.CurrentCulture))
                {
                    isValid = true;
                }
            }

            return isValid;
        }
    }
}
