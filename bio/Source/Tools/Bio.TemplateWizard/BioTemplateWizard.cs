using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;
using Microsoft.Win32;

namespace Bio.TemplateWizard
{
    /// <summary>
    /// Entry point of wizard which will be invoked by VS
    /// </summary>
    public class VSTemplateWizard : IWizard
    {
        /// <summary>
        /// Key containing installation path of Bio
        /// </summary>
        private const string BioRegistryInstallationPathKeyName = "InstallationPath";

        /// <summary>
        /// Called by VS before opening a project item
        /// </summary>
        /// <param name="projectItem">Item being opened</param>
        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        /// <summary>
        /// Called by VS after generating the project
        /// </summary>
        /// <param name="project">Project which got generated</param>
        public void ProjectFinishedGenerating(Project project)
        {   
        }

        /// <summary>
        /// Called by VS after loading a project item
        /// </summary>
        /// <param name="projectItem">Item which is loaded.</param>
        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
        }

        /// <summary>
        /// Called by VS once any custom code is done executing
        /// </summary>
        public void RunFinished()
        {
        }

        /// <summary>
        /// Called by VS before initiating any other project creation activity.
        /// Any customizations / wizards goes in here.
        /// </summary>
        /// <param name="automationObject">VS application object. (DTE object)</param>
        /// <param name="replacementsDictionary">Dictionary which holds name-value pairs to make replacements of placeholders in any project item</param>
        /// <param name="runKind">Context of item creation. (ex: project / project item)</param>
        /// <param name="customParams">Any other environment variables set by VS</param>
        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            if (replacementsDictionary == null)
            {
                replacementsDictionary = new Dictionary<string, string>();
            }

            // Show the wizard
            using (WizardForm wizard = new WizardForm())
            {
                Application.EnableVisualStyles();
                wizard.ShowDialog();

                // Replace placeholder for Bio reference.
                replacementsDictionary["$BioReferencePath$"] = wizard.BioAssemblyPath + @"Bio.dll";

                // Load snippets and push it to code file
                StringBuilder finalSnips = new StringBuilder();
                List<string> namespaces = new List<string>();
                List<string> assemblies = new List<string>();

                // Get what snippets are selected by user
                foreach (string tag in wizard.SnippetTags)
                {
                    Snippet currentSnip = Snippets.GetSnippet(tag);
                    if (currentSnip != null)
                    {
                        // Get the code snippet
                        finalSnips.Append(currentSnip.Code);

                        // Add any namespaces required
                        foreach (string namespaceRef in currentSnip.Namespaces)
                        {
                            if (!namespaces.Contains(namespaceRef))
                            {
                                namespaces.Add(namespaceRef);
                            }
                        }

                        // Check for any assembly references
                        if (!string.IsNullOrEmpty(currentSnip.Assembly))
                        {
                            assemblies.Add(currentSnip.Assembly);
                        }
                    }
                }

                // Add code snippets to file
                replacementsDictionary["$CodeSnippets$"] = finalSnips.ToString().Trim();

                // If Parser option is selected call the Parser method
                if (wizard.SnippetTags.Contains("Parsing"))
                {
                    Snippet callParser = Snippets.GetSnippet("CallParsing");
                    replacementsDictionary["$CallParser$"] = callParser.Code;
                }
                else
                {
                    replacementsDictionary["$CallParser$"] = string.Empty;
                }

                // If Formatter option is selected call the Export method
                if (wizard.SnippetTags.Contains("Formatting"))
                {
                    string formattingCodeWithoutParser = string.Empty;

                    // If Parser option is not selected
                    if (!wizard.SnippetTags.Contains("Parsing"))
                    {
                        Snippet withoutParser = Snippets.GetSnippet("CallFormattingWithoutParser");
                        formattingCodeWithoutParser = withoutParser.Code;
                    }

                    Snippet callFormatter = Snippets.GetSnippet("CallFormatting");
                    replacementsDictionary["$CallFormatter$"] = formattingCodeWithoutParser + callFormatter.Code;
                }
                else
                {
                    replacementsDictionary["$CallFormatter$"] = string.Empty;
                }

                // if the parser option or formatter option is selected
                if ((wizard.SnippetTags.Contains("Parsing")) || (wizard.SnippetTags.Contains("Formatting")))
                {
                    Snippet argParserClass = Snippets.GetSnippet("ArgumentParserClass");
                    Snippet argParserInstance = Snippets.GetSnippet("ArgumentParserInstance");
                    
                    replacementsDictionary["$ArgumentParserInstance$"] = argParserInstance.Code;
                    replacementsDictionary["$ArgumentParserClass$"] = argParserClass.Code;

                    if (argParserInstance.Namespaces.Count > 0)
                    {
                        foreach (string nameSpace in argParserInstance.Namespaces)
                        {
                            if (!namespaces.Contains(nameSpace))
                            {
                                namespaces.Add(nameSpace);
                            }
                        }
                    }
                }
                else
                {
                    replacementsDictionary["$ArgumentParserInstance$"] = string.Empty;
                    replacementsDictionary["$ArgumentParserClass$"] = string.Empty;
                }
                // Add necessary namespaces to file
                StringBuilder finalNamespaces = new StringBuilder();
                foreach (string namespaceRef in namespaces)
                {
                    finalNamespaces.AppendLine("using " + namespaceRef);
                }
                replacementsDictionary["$BioNamespaces$"] = finalNamespaces.ToString().Trim();

                // TODO: find this path from registry
                // Add additional assemblies if required
                StringBuilder finalAssemblies = new StringBuilder();
                if (assemblies.Contains("WebServiceHandlers"))
                {
                    string assemblyReference = "<Reference Include=\"Bio.WebServiceHandlers\">" + Environment.NewLine +
                                                    @"<HintPath>" + wizard.BioAssemblyPath + @"Bio.WebServiceHandlers.dll</HintPath>" + Environment.NewLine +
                                                "</Reference>";
                    finalAssemblies.AppendLine(assemblyReference);
                }
                if (assemblies.Contains("PAMSAM"))
                {
                    string assemblyReference = "<Reference Include=\"Bio.PAMSAM\">" + Environment.NewLine +
                                                    @"<HintPath>" + wizard.BioAssemblyPath + @"Bio.PAMSAM.dll</HintPath>" + Environment.NewLine +
                                                "</Reference>";
                    finalAssemblies.AppendLine(assemblyReference);
                }
                if (assemblies.Contains("Padena"))
                {
                    string assemblyReference = "<Reference Include=\"Bio.Padena\">" + Environment.NewLine +
                                                    @"<HintPath>" + wizard.BioAssemblyPath + @"Bio.Padena.dll</HintPath>" + Environment.NewLine +
                                                "</Reference>";
                    finalAssemblies.AppendLine(assemblyReference);
                }
                replacementsDictionary["$OtherAssemblies$"] = finalAssemblies.ToString().Trim();

            }
        }

        /// <summary>
        /// Called by VS to check if a particular item should be added to the project
        /// </summary>
        /// <param name="filePath">Path to the item being added</param>
        /// <returns>True to add, false not to add.</returns>
        public bool ShouldAddProjectItem(string filePath)
        {
            // Always return true as we dont want to skip any files in the template.
            return true;
        }
    }
}
