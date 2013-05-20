using System;
using System.Linq;

namespace DeveloperPreRequisiteCheck
{
    /// <summary>
    /// Entry point to Developer Pre-requisite check exe.
    /// Runs through a check for installation / existence of various components required 
    /// and reports appropriate actions to developer should perform to successfully 
    /// modify / compile .NET Bio solution.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// User option for enabling silent validation.
        /// </summary>
        private static readonly string[] SilentSwitch = new string[] { "/s", "/S", "-s", "-S" };
        private static readonly string[] ToolsSwitch = new string[] { "/t", "/T", "-t", "-T" }; // provide one of these switches to switch extra tools validation

        /// <summary>
        /// Entry method to Developer Pre-requisite check exe.
        /// </summary>
        /// <param name="args">command line arguments</param>
        public static int Main(string[] args)
        {
            bool isValidationSilent = false;
            bool isToolValidation = true; // default behaivor, is to check for tools dependencies 
            try
            {
                if (args.Length > 0)
                {
                    for (int counter = 0; counter < args.Length; counter++)
                    {
                        if (SilentSwitch.Contains(args[counter]))
                        {
                            isValidationSilent = true;
                        }
                        if (ToolsSwitch.Contains(args[counter]))
                        {
                            isToolValidation = false;
                        }
                    }
                    if(!isValidationSilent && isToolValidation)
                    {
                         throw new Exception(Properties.Resources.UNKNOWN_SWITCHES);
                    }
                 }

                Validator validator = new Validator(isValidationSilent, isToolValidation);
                return validator.Validate();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (!isValidationSilent)
                {
                    Console.WriteLine(Properties.Resources.PRESS_KEY);
                    Console.ReadKey();
                }
            }
        
            return 0;
        }
    }
}
