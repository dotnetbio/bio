namespace SequenceAssembler
{
    #region -- Using Directives --

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;

    #endregion -- Using Directives --

    /// <summary>
    /// MessageDialogBox is a wrapper around MessageDialog.
    /// This class is responsible for displaying MessageDialog instance
    /// and also setting a flag to indicate which button was pressed.
    /// </summary>
    public static class MessageDialogBox
    {
        #region -- Private Methods --

        /// <summary>
        /// Specifies which MessageDialog button that the user has clicked. 
        /// </summary>
        private static MessageDialogResult result;

        /// <summary>
        /// Holds a reference to the dialog currently being shown.
        /// </summary>
        private static MessageDialog dialog;

        #endregion -- Private Methods --

        #region -- Public Methods --

        /// <summary>
        /// Show displays our custom MessageDialogBox. The user is allowed
        /// to pass the message, caption and the buttons that has to be displayed
        /// in MessageDialogBox.
        /// </summary>
        /// <param name="message">Message to be displayed in the dialog box.</param>
        /// <param name="caption">Caption of the dialog box</param>
        /// <param name="buttons">Buttons which will be displayed to</param>
        /// <returns>Specifies which MessageDialog button that the user has clicked. </returns>
        public static MessageDialogResult Show(string message, string caption, MessageDialogButton buttons)
        {
            dialog = new MessageDialog();
            dialog.Initialize(message, caption, buttons);

            // initialize the default result
            if (dialog.btnOk.Visibility == Visibility.Visible)
            {
                result = MessageDialogResult.OK;
            }
            else if (dialog.btnNo.Visibility == Visibility.Visible)
            {
                result = MessageDialogResult.No;
            }

            dialog.btnYes.Click += new RoutedEventHandler(OnYesClicked);
            dialog.btnNo.Click += new RoutedEventHandler(OnNoClicked);
            dialog.btnOk.Click += new RoutedEventHandler(OnOkClicked);
            dialog.KeyUp += new System.Windows.Input.KeyEventHandler(OnDialogKeyUp);
            dialog.Closed += new EventHandler(OnDialogClosed);
            dialog.ShowDialog();

            return result;
        }

        /// <summary>
        /// This event is fired when the dialog is closed 
        /// from the Close button
        /// </summary>
        /// <param name="sender">message dialog</param>
        /// <param name="e">Event Data</param>
        private static void OnDialogClosed(object sender, EventArgs e)
        {
            if (result != MessageDialogResult.Yes)
            {
                if (dialog.btnOk.Visibility == Visibility.Visible)
                {
                    result = MessageDialogResult.OK;
                }
                else if (dialog.btnNo.Visibility == Visibility.Visible)
                {
                    result = MessageDialogResult.No;
                }
            }            
        }
        
        #endregion -- Public Methods --

        #region -- Private Methods --

        /// <summary>
        /// This event will be fired when OK button is clicked on MessageDialogBox
        /// by the user.
        /// </summary>
        /// <param name="sender">MessageDialogBox instance.</param>
        /// <param name="e">Event data.</param>
        private static void OnOkClicked(object sender, RoutedEventArgs e)
        {
            result = MessageDialogResult.OK;
            dialog.Close();
        }

        /// <summary>
        /// This event will be fired when Escape button is pressed, when MessageDialogBox
        /// is visible to the user. If dialog contains OK button then escape would set 
        /// the Ok as result and close the dialog else if the dialog contains yes or no, 
        /// the result would be set to no and dialog will be closed.
        /// </summary>
        /// <param name="sender">MessageDialogBox instance.</param>
        /// <param name="e">Event data.</param>
        private static void OnDialogKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                if (dialog.btnOk.Visibility == Visibility.Visible)
                {
                    result = MessageDialogResult.OK;
                }
                else if (dialog.btnNo.Visibility == Visibility.Visible)
                {
                    result = MessageDialogResult.No;
                }

                dialog.Close();
            }
        }

        /// <summary>
        /// This event will be fired when No button is clicked on MessageDialogBox
        /// by the user.
        /// </summary>
        /// <param name="sender">MessageDialogBox instance.</param>
        /// <param name="e">Event data.</param>
        private static void OnNoClicked(object sender, RoutedEventArgs e)
        {
            result = MessageDialogResult.No;            
            dialog.Close();
        }

        /// <summary>
        /// This event will be fired when Yes button is clicked on MessageDialogBox
        /// by the user.
        /// </summary>
        /// <param name="sender">MessageDialogBox instance.</param>
        /// <param name="e">Event data.</param>
        private static void OnYesClicked(object sender, RoutedEventArgs e)
        {
            result = MessageDialogResult.Yes;            
            dialog.Close();
        }

        #endregion -- Private Methods --
    }
}
