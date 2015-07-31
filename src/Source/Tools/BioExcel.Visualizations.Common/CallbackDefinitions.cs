namespace BiodexExcel.Visualizations.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using Bio;

    /// <summary>
    /// Delegate used to point to a methods which will invode the selection helper window and return the selected address as string.
    /// </summary>
    /// <param name="selectionDialog">Selection Dialog</param>
    public delegate void SelectionHelperCallback(ISelectionDialog selectionDialog);

    /// <summary>
    /// Signature of the method to be called when this form is submitted.
    /// </summary>
    /// <param name="selectionDialog">Selection dialog</param>
    public delegate void SequenceSelectionDialogSubmit(ISelectionDialog selectionDialog);

    /// <summary>
    /// delegate for event raised when a user selection is complete - Sequence
    /// </summary>
    /// <param name="selectedSequences">List of ISequence, selected by user</param>
    /// <param name="args">Any custom argument to be passed</param>
    public delegate void SequenceSelectionComplete(List<ISequence> selectedSequences, params object[] args);

    /// <summary>
    /// delegate for event raised when a user selection is complete - SequenceRange
    /// </summary>
    /// <param name="e">Event Argument</param>
    public delegate void InputSequenceRangeSelectionComplete(InputSequenceRangeSelectionEventArg e); // for BED
}
