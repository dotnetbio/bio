namespace SequenceAssembler
{
    #region -- Using Directives--
    using System.Collections.ObjectModel;

    #endregion
    /// <summary>
    /// Filter Info would store the various details about the 
    /// format for which the filter expression has to be created.
    /// It would provide all the necessary details to the FileDialogFilterBuilder.
    /// </summary>
    public class FilterInfo
    {
        #region -- Private members --
        /// <summary>
        /// Describes the Visible Extensions ( Extensions to be visible by the user)
        /// on the dialog.
        /// </summary>
        private string[] visibleExtensions;

        /// <summary>
        /// Array of extension used for file filtering.
        /// </summary>
        private string[] extensions;
        #endregion

        #region -- Constructor --
        /// <summary>
        /// Initializes the FileInfo with the given title 
        /// </summary>
        /// <param name="title">Title of filter item.</param>
        public FilterInfo(string title)
        {
            this.Title = title;
        }

        /// <summary>
        /// Initializes the FileInfo with the given title and Extensions
        /// </summary>
        /// <param name="title">Title of filter item.</param>
        /// <param name="extensions">Array of file extensions.</param>
        public FilterInfo(string title, params string[] extensions)
            : this(title)
        {
            this.SetExtensions(extensions);
        }
        #endregion

        #region -- Public properties --
        /// <summary>
        /// Gets or sets the Title of filter item.
        /// </summary>
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the Visible Extensions ( Extensions to be visible by the user)
        /// on the dialog.
        /// </summary>
        public Collection<string> VisibleExtensions
        {
            get
            {
                Collection<string> exts = new Collection<string>();
                foreach (string ext in this.visibleExtensions)
                {
                    exts.Add(ext);
                }

                return exts;
            }
        }

        /// <summary>
        /// Gets Array of extension used for file filtering.
        /// </summary>
        public Collection<string> Extensions
        {
            get
            {
                Collection<string> exts = new Collection<string>();
                foreach (string ext in this.visibleExtensions)
                {
                    exts.Add(ext);
                }

                return exts;
            }
        }

        #endregion

        #region -- Public methods --
        /// <summary>
        /// Copy extensions to Extension and VisibleExtensions property.
        /// </summary>
        /// <param name="ext">Array of file extensions.</param>
        public void SetExtensions(params string[] ext)
        {
            if (ext != null)
            {
                int len = ext.Length;
                this.visibleExtensions = new string[len];
                this.extensions = new string[len];
                ext.CopyTo(this.visibleExtensions, 0);
                ext.CopyTo(this.extensions, 0);
            }
            else
            {
                this.visibleExtensions = this.extensions = null;
            }
        }
        #endregion
    }
}
