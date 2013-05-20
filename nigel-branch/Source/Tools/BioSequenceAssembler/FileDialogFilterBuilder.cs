namespace SequenceAssembler
{
    #region -- Using Directives--
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    #endregion

    /// <summary>
    /// The FileDialogFilterBuilder would take the supported file Types' 
    /// extensions as String array as Input and would create the 
    /// filter expression for the OpenFile Dialog.
    /// </summary>
    public class FileDialogFilterBuilder
    {
        #region -- Private members --
        /// <summary>
        /// List of information about the formats
        /// </summary>
        private Collection<FilterInfo> infos = new Collection<FilterInfo>();
        #endregion

        #region -- public Properties --
        /// <summary>
        /// Gets the list of informations for file filtering.
        /// </summary>
        public Collection<FilterInfo> Info
        {
            get
            {
                return this.infos;
            }
        }
        #endregion

        #region -- Public Methods --

        /// <summary>
        /// Add filter info for "all file types".
        /// </summary>
        /// <param name="title">Title of filter.</param>
        public void AddAllFileTypes(string title)
        {
            FilterInfo info = new FilterInfo(title);
            info.SetExtensions("*");
            this.infos.Add(info);
        }

        /// <summary>
        /// Builds filter string for the OpenFile dialog.
        /// The filter expression looks like |Description|*.ext1;*.ext2;*.ext3|
        /// </summary>
        /// <returns>The filter expression string</returns>
        public string ToFilterString()
        {
            StringBuilder filterString = new StringBuilder();
            for (int i = 0; i < this.infos.Count; i++)
            {
                FilterInfo info = this.infos[i];
                if (i > 0)
                {
                    filterString.Append('|');
                }

                filterString.Append(info.Title).Append(' ');

                //// create the visible Filter string for the user.
                filterString = CreateVisibleFilterString(filterString, info);

                //// create the actual fileter string for the user.
                filterString = CreateActualFilterString(filterString, info);
            }

            return filterString.ToString();
        }
        #endregion

        #region -- Private Static method --
        /// <summary>
        /// It would create the string expression shown to the user, from the 
        /// given filter info.
        /// </summary>
        /// <param name="filterString">Filter String to be updated</param>
        /// <param name="info">File format type information</param>
        /// <returns>Updated Filter String</returns>
        private static StringBuilder CreateVisibleFilterString(StringBuilder filterString, FilterInfo info)
        {
            //// get the extensions to be shown from the info.
            string[] visibleExtensions = info.VisibleExtensions.ToArray();
            int len = visibleExtensions != null ? visibleExtensions.Length : -1;
            if (len > 0)
            {
                filterString.Append('(');
                for (int index = 0; index < len; index++)
                {
                    if (index > 0)
                    {
                        //// append comma after every extensions
                        filterString.Append(", ");
                    }

                    //// Get the extension "ext" from *.ext format 
                    string extension = visibleExtensions[index];

                    //// append the *.ext
                    filterString.Append("*.").Append(extension);
                }

                filterString.Append(")|");
            }
            else
            {
                //// Start the new File type filter type.
                filterString.Append('|');
            }

            return filterString;
        }

        /// <summary>
        /// This would create the functional Filter string which 
        /// would be used by the FileDialog to filter files.
        /// </summary>
        /// <param name="filterString">filterstring to be updated</param>
        /// <param name="info">Filter information from the Framewrok</param>
        /// <returns>Updated Filter string</returns>
        private static StringBuilder CreateActualFilterString(StringBuilder filterString, FilterInfo info)
        {
            //// get the extensions to be Filtered from the info.
            string[] extensions = info.Extensions.ToArray();
            int len = extensions != null ? extensions.Length : -1;
            if (len > 0)
            {
                for (int index = 0; index < len; index++)
                {
                    if (index > 0)
                    {
                        //// separate the extension by ';' character.
                        filterString.Append(';');
                    }
                    //// Get the extension "ext" from *.ext format 
                    string extension = extensions[index];
                    filterString.Append("*.").Append(extension);
                }
            }

            return filterString;
        }
        #endregion
    }
}
