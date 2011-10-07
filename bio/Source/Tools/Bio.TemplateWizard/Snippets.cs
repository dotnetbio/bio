using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Bio.TemplateWizard
{
    /// <summary>
    /// Static class which will serve as a factory of code snippets
    /// </summary>
    public static class Snippets
    {
        /// <summary>
        /// Embedded resource name which contain the snippets.
        /// </summary>
        private const string SnippetsResourceName = "Bio.TemplateWizard.CodeSnippets.txt";

        /// <summary>
        /// Dictionary of all available snippets
        /// </summary>
        private static Dictionary<string, Snippet> loadedSnippets;

        /// <summary>
        /// Gets a snippet object from available snippets
        /// </summary>
        /// <param name="tag">Tag of the snippet to retrieve</param>
        /// <returns>Snippet object if found, else null</returns>
        public static Snippet GetSnippet(string tag)
        {
            // Load snippets if not loaded
            if (loadedSnippets == null)
            {
                LoadSnippets();
            }

            Snippet reqSnippet;
            if (loadedSnippets.TryGetValue(tag, out reqSnippet))
            {
                return reqSnippet;
            }

            return null;
        }

        /// <summary>
        /// Load all available snippets from disk
        /// </summary>
        private static void LoadSnippets()
        {
            // Get the embedded resource stream
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream snipsStream = assembly.GetManifestResourceStream(SnippetsResourceName))
            {
                if (snipsStream != null)
                {
                    using (StreamReader reader = new StreamReader(snipsStream))
                    {
                        loadedSnippets = new Dictionary<string, Snippet>();
                        string currentLine = string.Empty;
                        Snippet currentSnip = null;
                        StringBuilder snippetBuilder = new StringBuilder();

                        // Read all snippets
                        while (!reader.EndOfStream)
                        {
                            currentLine = reader.ReadLine();

                            // Check for start of a new snippet
                            if (currentLine.StartsWith("#snippet", System.StringComparison.OrdinalIgnoreCase))
                            {
                                // Save loaded snip
                                if (currentSnip != null)
                                {
                                    currentSnip.Code = snippetBuilder.ToString();
                                    loadedSnippets.Add(currentSnip.Tag, currentSnip);
                                }

                                //Start new snip
                                currentSnip = new Snippet();
                                currentSnip.Tag = currentLine.Split('-')[1].Trim();

                                snippetBuilder.Clear();
                            }
                            else if (currentLine.StartsWith("#namespace", System.StringComparison.OrdinalIgnoreCase) && currentSnip != null)
                            {
                                currentSnip.Namespaces.Add(currentLine.Split('-')[1].Trim());
                            }
                            else if (currentLine.StartsWith("#assembly", System.StringComparison.OrdinalIgnoreCase) && currentSnip != null)
                            {
                                currentSnip.Assembly = currentLine.Split('-')[1].Trim();
                            }
                            else
                            {
                                // Add this line to the snip
                                snippetBuilder.AppendLine(currentLine);
                            }
                        }

                        // Save last loaded snip
                        if (currentSnip != null)
                        {
                            currentSnip.Code = snippetBuilder.ToString();
                            loadedSnippets.Add(currentSnip.Tag, currentSnip);
                        }
                    }
                }
            }
        }
    }
}

