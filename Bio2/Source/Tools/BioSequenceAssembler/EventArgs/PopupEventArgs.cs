namespace SequenceAssembler
{
    #region -- Using Directives --
    using System;
    using System.Collections.ObjectModel;
    using Bio;
    #endregion
    /// <summary>
    /// This defines the custom Event Arguments for describing closure of 
    /// pop up occurred with success or failure status.
    /// </summary>
    public class PopupEventArgs : EventArgs
    {
        #region -- Private Members --

        /// <summary>
        /// Describes the the status of the Popup
        /// </summary>
        private bool status;

        #endregion

        #region -- Constructor --

        /// <summary>
        /// Initiliazes the PopupEventArgs with the 
        /// State of the Pop up.        
        /// </summary>
        /// <param name="state">Popup State</param>      
        public PopupEventArgs(bool state)
        {
            this.status = state;
        }
        #endregion

        #region -- Public Properties --

        /// <summary>
        /// Gets a value indicating whether the status of the Popup is true or false.
        /// </summary>
        public bool Status
        {
            get
            {
                return this.status;
            }
        }
        #endregion
    }
}
