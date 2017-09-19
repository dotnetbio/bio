using System;

namespace Bio.Util
{
    /// <summary>
    /// Class to hold status changed message.
    /// </summary>
    public class StatusChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the StatusChangedEventArgs class.
        /// </summary>
        public StatusChangedEventArgs()
        {
            this.StatusMessage = ".";
        }

        /// <summary>
        /// Initializes a new instance of the StatusChangedEventArgs class with the specified message.
        /// </summary>
        /// <param name="statusMessage">Status message.</param>
        public StatusChangedEventArgs(string statusMessage)
        {
            this.StatusMessage = statusMessage;
        }

        /// <summary>
        /// Gets or sets the status message.
        /// </summary>
        public string StatusMessage { get; set; }
    }
}
