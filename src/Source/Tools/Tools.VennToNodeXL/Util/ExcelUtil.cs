using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;

namespace Microsoft.Research.CommunityTechnologies.AppLib
{
    //*****************************************************************************
    //  Class: ExcelUtil
    //
    /// <summary>
    /// Static utility methods for working with Excel from Visual Studio Tools for
    /// Office (VSTO) projects.
    /// </summary>
    ///
    /// <remarks>
    /// All methods are static.
    /// </remarks>
    //*****************************************************************************

    public static class ExcelUtil
    {
        //*************************************************************************
        //  Property: SpecialCellsBeingCalled
        //
        /// <summary>
        /// Gets a flag indicating whether a call to Range.SpecialCells() is in
        /// progress.
        /// </summary>
        ///
        /// <value>
        /// true if a call to Range.SpecialCells() is in progress.
        /// </value>
        ///
        /// <remarks>
        /// There is a bug in Excel's Range.SpecialCells() method that causes the
        /// Microsoft.Office.Tools.Excel.ListObject.SelectionChange event to fire
        /// if the range has hidden cells when the method is called.  ExcelUtil
        /// sets this property to true during all such calls.  An application can
        /// work around the Excel bug by checking this property during the
        /// SelectionChange event and ignoring the event if the property is true.
        ///
        /// <para>
        /// WARNING: This is not thread-safe and is not a good solution.  The
        /// correct way to do this would be to turn ExcelUtil into a non-static
        /// class and make m_bSpecialCellsBeingCalled an instance member instead of
        /// a static member.
        /// </para>
        ///
        /// <para>
        /// In the ExcelTemplate VSTO project for which this class was originally
        /// written, each workbook gets its own AppDomain, each of which gets its
        /// own copy of ExcelUtil.  Therefore this static implementation works in
        /// that project, but it won't work in the general case.
        /// </para>
        ///
        /// </remarks>
        //*************************************************************************

        public static Boolean
        SpecialCellsBeingCalled
        {
            get
            {
                return (m_bSpecialCellsBeingCalled);
            }
        }

        //*************************************************************************
        //  Method: ExcelApplicationIsReady()
        //
        /// <summary>
        /// Determines whether the Excel application is ready to accept method
        /// calls.
        /// </summary>
        ///
        /// <param name="application">
        /// Excel Application object.
        /// </param>
        ///
        /// <returns>
        /// true if the Excel application is ready to accept method calls.
        /// </returns>
        //*************************************************************************

        public static Boolean
        ExcelApplicationIsReady
        (
            Microsoft.Office.Interop.Excel.Application application
        )
        {
            Debug.Assert(application != null);

            // Calling into Excel while a cell is in edit mode or while Excel is
            // displaying a modal dialog (see notes below) can raise an exception.
            // There is an Application.Ready property that is supposed to determine
            // whether the application is ready to accept calls, but it always
            // returns true while a cell is in edit mode.  The following workaround
            // was suggested in several online postings.  It checks whether the
            // File,Open command is enabled.  Excel disables this command while a
            // cell is in edit mode.

            return (application.CommandBars.FindControl(
                Missing.Value, 23, Missing.Value, Missing.Value).Enabled);


            // Repro case for the problem with modal dialogs:
            //
            //   1. Open NodeXL workbook Test2.xlsx and read the workbook into
            //      the graph.
            //
            //   2. Open NodeXL workbook Test1.xlsx and read the workbook into
            //      the graph.
            //
            //   3. Close Excel.  Excel asks if you want to save the changes to
            //      Test1.xlsx, which is on top.  Say no.
            //
            //   4. Closing Test1.xlsx uncovers Test2.xlsx, which then redraws
            //      its graph.  This causes TaskPane_GraphDrawn() to run, which
            //      makes various calls into Excel.
            //
            //   5. Excel is busy with "do you want to save..." dialogs, and
            //      at least one Excel call ( Application.Intersection() )
            //      throws a COMException with error code 0x800AC472.
        }

        //*************************************************************************
        //  Method: TryGetWorksheet()
        //
        /// <summary>
        /// Attempts to get a worksheet by name.
        /// </summary>
        ///
        /// <param name="workbook">
        /// Workbook to get a worksheet from.
        /// </param>
        ///
        /// <param name="worksheetName">
        /// Name of the worksheet to get.
        /// </param>
        ///
        /// <param name="worksheet">
        /// Where the requested worksheet gets stored if true is returned.
        /// </param>
        ///
        /// <returns>
        /// true if successful.
        /// </returns>
        ///
        /// <remarks>
        /// If <paramref name="workbook" /> contains a worksheet named <paramref
        /// name="worksheetName" />, the worksheet is stored at <paramref
        /// name="worksheet" /> and true is returned.  false is returned otherwise.
        /// </remarks>
        //*************************************************************************

        public static Boolean
        TryGetWorksheet
        (
            Microsoft.Office.Interop.Excel.Workbook workbook,
            String worksheetName,
            out Worksheet worksheet
        )
        {
            Debug.Assert(workbook != null);
            Debug.Assert(!String.IsNullOrEmpty(worksheetName));

            worksheet = null;

            Object oSheet;

            try
            {
                oSheet = workbook.Sheets[worksheetName];
            }
            catch (COMException)
            {
                return (false);
            }

            if (!(oSheet is Worksheet))
            {
                return (false);
            }

            worksheet = (Worksheet)oSheet;

            return (true);
        }

        //*************************************************************************
        //  Method: ActivateWorkbook()
        //
        /// <overloads>
        /// Activates a workbook.
        /// </overloads>
        ///
        /// <summary>
        /// Activates a workbook.
        /// </summary>
        ///
        /// <param name="workbook">
        /// The workbook to activate.
        /// </param>
        //*************************************************************************

        public static void
        ActivateWorkbook
        (
            Microsoft.Office.Interop.Excel.Workbook workbook
        )
        {
            Debug.Assert(workbook != null);

            // See this posting for an explanation of the strange cast:
            //
            // http://blogs.officezealot.com/maarten/archive/2006/01/02/8918.aspx

            ((Microsoft.Office.Interop.Excel._Workbook)workbook).Activate();
        }

        //*************************************************************************
        //  Method: ActivateWorksheet()
        //
        /// <overloads>
        /// Activates a worksheet.
        /// </overloads>
        ///
        /// <summary>
        /// Activates a worksheet.
        /// </summary>
        ///
        /// <param name="worksheet">
        /// The worksheet to activate.
        /// </param>
        //*************************************************************************

        public static void
        ActivateWorksheet
        (
            Microsoft.Office.Interop.Excel.Worksheet worksheet
        )
        {
            Debug.Assert(worksheet != null);

            // See this posting for an explanation of the strange cast:
            //
            // http://blogs.officezealot.com/maarten/archive/2006/01/02/8918.aspx

            ((Microsoft.Office.Interop.Excel._Worksheet)worksheet).Activate();
        }

        //*************************************************************************
        //  Method: ActivateWorksheet()
        //
        /// <summary>
        /// Activates a worksheet that contains a table (ListObject).
        /// </summary>
        ///
        /// <param name="table">
        /// The table whose parent worksheet should be activated.
        /// </param>
        //*************************************************************************

        public static void
        ActivateWorksheet
        (
            ListObject table
        )
        {
            Debug.Assert(table != null);

            Debug.Assert(table.Parent is Worksheet);

            ActivateWorksheet((Worksheet)table.Parent);
        }

        //*************************************************************************
        //  Method: WorksheetIsActive()
        //
        /// <summary>
        /// Determines whether a worksheet is the active worksheet.
        /// </summary>
        ///
        /// <param name="worksheet">
        /// Worksheet to test.
        /// </param>
        ///
        /// <returns>
        /// true if <paramref name="worksheet" /> is the active worksheet in its
        /// workbook.
        /// </returns>
        //*************************************************************************

        public static Boolean
        WorksheetIsActive
        (
            Microsoft.Office.Interop.Excel.Worksheet worksheet
        )
        {
            Debug.Assert(worksheet != null);
            Debug.Assert(worksheet.Parent is Workbook);

            return (((Workbook)worksheet.Parent).ActiveSheet == worksheet);
        }

        //*************************************************************************
        //  Method: GetCellAddress()
        //
        /// <summary>
        /// Gets a cell's address in A1 style.
        /// </summary>
        ///
        /// <param name="worksheet">
        /// Worksheet containing the cell.
        /// </param>
        ///
        /// <param name="rowOneBased">
        /// One-based row number of the cell.  Sample: 3.
        /// </param>
        ///
        /// <param name="columnOneBased">
        /// One-based column number of the cell.  Sample: 2.
        /// </param>
        ///
        /// <returns>
        /// The A1-style address of the cell.  Sample: "B3".
        /// </returns>
        //*************************************************************************

        public static String
        GetCellAddress
        (
            Microsoft.Office.Interop.Excel.Worksheet worksheet,
            Int32 rowOneBased,
            Int32 columnOneBased
        )
        {
            Debug.Assert(worksheet != null);
            Debug.Assert(rowOneBased >= 1);
            Debug.Assert(columnOneBased >= 1);

            return (GetRangeAddress(
                (Range)worksheet.Cells[rowOneBased, columnOneBased]));
        }

        //*************************************************************************
        //  Method: GetRangeAddress()
        //
        /// <summary>
        /// Gets a Range's address in A1 style.
        /// </summary>
        ///
        /// <param name="range">
        /// Range to get the address of.
        /// </param>
        ///
        /// <returns>
        /// The A1-style address of <paramref name="range" />.  Sample: "A1:B2".
        /// </returns>
        //*************************************************************************

        public static String
        GetRangeAddress
        (
            Range range
        )
        {
            Debug.Assert(range != null);

            return (range.get_Address(
                false, false, XlReferenceStyle.xlA1, Missing.Value, Missing.Value
                ));
        }

        //*************************************************************************
        //  Method: GetRangeAddressAbsolute()
        //
        /// <summary>
        /// Gets a Range's address in absolute $A1 style.
        /// </summary>
        ///
        /// <param name="range">
        /// Range to get the absolute address of.
        /// </param>
        ///
        /// <returns>
        /// The absolute $A1-style address of <paramref name="range" />.  Sample:
        /// "$A1:$B2".
        /// </returns>
        //*************************************************************************

        public static String
        GetRangeAddressAbsolute
        (
            Range range
        )
        {
            Debug.Assert(range != null);

            return (range.get_Address(
                true, true, XlReferenceStyle.xlA1, Missing.Value, Missing.Value
                ));
        }

        //*************************************************************************
        //  Method: TryGetNamedRange()
        //
        /// <summary>
        /// Attempts to get a named range from a worksheet.
        /// </summary>
        ///
        /// <param name="worksheet">
        /// Worksheet to get the named range from.
        /// </param>
        ///
        /// <param name="rangeName">
        /// Name of the range to get.
        /// </param>
        ///
        /// <param name="namedRange">
        /// Where the named range gets stored if true is returned.
        /// </param>
        ///
        /// <returns>
        /// true if successful.
        /// </returns>
        ///
        /// <remarks>
        /// If a range named <paramref name="rangeName" /> exists in <paramref
        /// name="worksheet" />, the named range is stored at <paramref
        /// name="namedRange" /> and true is returned.  false is returned
        /// otherwise.
        /// </remarks>
        //*************************************************************************

        public static Boolean
        TryGetNamedRange
        (
            Microsoft.Office.Interop.Excel.Worksheet worksheet,
            String rangeName,
            out Range namedRange
        )
        {
            Debug.Assert(worksheet != null);
            Debug.Assert(!String.IsNullOrEmpty(rangeName));

            namedRange = null;

            try
            {
                namedRange = worksheet.get_Range(rangeName, Missing.Value);

                return (true);
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                return (false);
            }
        }

        //*************************************************************************
        //  Method: TryGetVisibleRange()
        //
        /// <summary>
        /// Attempts to reduce a range to visible cells only.
        /// </summary>
        ///
        /// <param name="range">
        /// The range to reduce.
        /// </param>
        ///
        /// <param name="visibleRange">
        /// Where the visible range gets stored if true is returned.  The range
        /// may consist of multiple areas.
        /// </param>
        ///
        /// <returns>
        /// true if the visible range was obtained.
        /// </returns>
        ///
        /// <remarks>
        /// If <paramref name="range" /> contains at least one cell that is
        /// visible, the range of visible cells is stored at <paramref
        /// name="visibleRange" /> and true is returned.  false is returned
        /// otherwise.
        ///
        /// <para>
        /// WARNING: Due to an apparent bug in Excel, this method can cause the
        /// Microsoft.Office.Tools.Excel.ListObject.SelectionChange event to fire.
        /// </para>
        ///
        /// </remarks>
        //*************************************************************************

        public static Boolean
        TryGetVisibleRange
        (
            Range range,
            out Range visibleRange
        )
        {
            Debug.Assert(range != null);

            visibleRange = null;

            // WARNING: If the range contains hidden cells, range.SpecialCells()
            // causes the Microsoft.Office.Tools.Excel.ListObject.SelectionChange
            // event to fire.  It shouldn't, but it does.  Allow the application to
            // work around this Excel bug by checking the SpeciealCellsBeingCalled
            // property from within the application's event handler.

            m_bSpecialCellsBeingCalled = true;

            try
            {
                if (range.Rows.Count == 1 && range.Columns.Count == 1)
                {
                    // The Range.SpecialCells() call below does not work for single
                    // cells.  For example, if range is "B3", Range.SpecialCells()
                    // returns "A:Y,AB:XFD".

                    if ((Boolean)range.EntireRow.Hidden ||
                        (Boolean)range.EntireColumn.Hidden)
                    {
                        return (false);
                    }

                    visibleRange = range;
                    return (true);
                }

                visibleRange = range.SpecialCells(
                    XlCellType.xlCellTypeVisible, Missing.Value);
            }
            catch (COMException)
            {
                // This can definitely occur.

                return (false);
            }
            finally
            {
                m_bSpecialCellsBeingCalled = false;
            }

            // Can a null visibleRange occur as well?  The documentation doesn't
            // say.

            return (visibleRange != null);
        }

        //*************************************************************************
        //  Method: TryGetNonEmptyRange()
        //
        /// <overloads>
        /// Attempts to get the range that is actually used.
        /// </overloads>
        ///
        /// <summary>
        /// Attempts to get the range within a specified worksheet that is actually
        /// used.
        /// </summary>
        ///
        /// <param name="worksheet">
        /// Worksheet to use.
        /// </param>
        ///
        /// <param name="usedRange">
        /// Where the used range within <paramref name="worksheet" /> is stored if
        /// true is returned.
        /// </param>
        ///
        /// <returns>
        /// true if <paramref name="worksheet" /> contains at least one used cell.
        /// </returns>
        ///
        /// <remarks>
        /// This differs from Range.UsedRange, which includes cells that were once
        /// used but are now empty.
        /// </remarks>
        //*************************************************************************

        public static Boolean
        TryGetNonEmptyRange
        (
            Worksheet worksheet,
            out Range usedRange
        )
        {
            Debug.Assert(worksheet != null);

            usedRange = null;

            Range oEntireWorksheet = (Range)worksheet.get_Range(

                (Range)worksheet.Cells[1, 1],

                (Range)worksheet.Cells[
                    worksheet.Rows.Count, worksheet.Columns.Count]
                );

            return (TryGetNonEmptyRange(oEntireWorksheet, out usedRange));
        }

        //*************************************************************************
        //  Method: TryGetNonEmptyRange()
        //
        /// <summary>
        /// Attempts to get the range within a specified range that is actually
        /// used.
        /// </summary>
        ///
        /// <param name="range">
        /// Range to use.
        /// </param>
        ///
        /// <param name="usedRange">
        /// Where the used range within <paramref name="range" /> is stored if true
        /// is returned.
        /// </param>
        ///
        /// <returns>
        /// true if <paramref name="range" /> contains at least one used cell.
        /// </returns>
        ///
        /// <remarks>
        /// This differs from Range.UsedRange, which includes cells that were once
        /// used but are now empty.
        /// </remarks>
        //*************************************************************************

        public static Boolean
        TryGetNonEmptyRange
        (
            Range range,
            out Range usedRange
        )
        {
            Debug.Assert(range != null);

            usedRange = null;

            if (range.Rows.Count == 1 && range.Columns.Count == 1)
            {
                // The code below fails for the single-cell case -- iFirstColumn
                // ends up being greater than iLastColumn.  Check the cell
                // manually.

                if (range.get_Value(Missing.Value) == null)
                {
                    return (false);
                }

                usedRange = range;

                return (true);
            }

            const String WildCard = "*";

            Range oFirstRow = range.Find(WildCard,
                range.Cells[range.Rows.Count, 1], XlFindLookIn.xlValues,
                XlLookAt.xlPart, XlSearchOrder.xlByRows,
                XlSearchDirection.xlNext, false, false, Missing.Value);

            if (oFirstRow == null)
            {
                return (false);
            }

            Range oFirstColumn = range.Find(WildCard,
                range.Cells[1, range.Columns.Count], XlFindLookIn.xlValues,
                XlLookAt.xlPart, XlSearchOrder.xlByColumns,
                XlSearchDirection.xlNext, false, false, Missing.Value);

            Range oLastRow = range.Find(WildCard,
                range.Cells[1, 1], XlFindLookIn.xlValues,
                XlLookAt.xlPart, XlSearchOrder.xlByRows,
                XlSearchDirection.xlPrevious, false, false, Missing.Value);

            Range oLastColumn = range.Find(WildCard,
                range.Cells[1, 1], XlFindLookIn.xlValues,
                XlLookAt.xlPart, XlSearchOrder.xlByColumns,
                XlSearchDirection.xlPrevious, false, false, Missing.Value);

            Debug.Assert(oFirstColumn != null);
            Debug.Assert(oLastRow != null);
            Debug.Assert(oLastColumn != null);

            Int32 iFirstRow = oFirstRow.Row;
            Int32 iFirstColumn = oFirstColumn.Column;
            Int32 iLastRow = oLastRow.Row;
            Int32 iLastColumn = oLastColumn.Column;

            Worksheet oWorksheet = range.Worksheet;

            usedRange = (Range)oWorksheet.get_Range(
                (Range)oWorksheet.Cells[iFirstRow, iFirstColumn],
                (Range)oWorksheet.Cells[iLastRow, iLastColumn]
                );

            return (true);
        }

        //*************************************************************************
        //  Method: OffsetRange()
        //
        /// <summary>
        /// Offsets a Range by a specified number of rows and columns.
        /// </summary>
        ///
        /// <param name="range">
        /// The range to offset.
        /// </param>
        ///
        /// <param name="rowOffset">
        /// Number of rows to offset the range.  Can be negative, zero, or
        /// positive.
        /// </param>
        ///
        /// <param name="columnOffset">
        /// Number of columns to offset the range.  Can be negative, zero, or
        /// positive.
        /// </param>
        //*************************************************************************

        public static void
        OffsetRange
        (
            ref Range range,
            Int32 rowOffset,
            Int32 columnOffset
        )
        {
            Debug.Assert(range != null);

            range = range.get_Offset(rowOffset, columnOffset);
        }

        //*************************************************************************
        //  Method: ResizeRange()
        //
        /// <summary>
        /// Resizes a Range to a specified number of rows and columns.
        /// </summary>
        ///
        /// <param name="range">
        /// The range to resize.
        /// </param>
        ///
        /// <param name="rows">
        /// Number of rows to resize the range to.  Must be positive.
        /// </param>
        ///
        /// <param name="columns">
        /// Number of columns to resize the range to.  Must be positive.
        /// </param>
        //*************************************************************************

        public static void
        ResizeRange
        (
            ref Range range,
            Int32 rows,
            Int32 columns
        )
        {
            Debug.Assert(range != null);
            Debug.Assert(rows > 0);
            Debug.Assert(columns > 0);

            range = range.get_Resize(rows, columns);
        }

        //*************************************************************************
        //  Method: PasteValues()
        //
        /// <summary>
        /// Pastes the values and number formats stored in the clipboard into a
        /// range.
        /// </summary>
        ///
        /// <param name="range">
        /// Range to paste into.
        /// </param>
        //*************************************************************************

        public static void
        PasteValues
        (
            Range range
        )
        {
            Debug.Assert(range != null);

            range.PasteSpecial(XlPasteType.xlPasteValuesAndNumberFormats,
                XlPasteSpecialOperation.xlPasteSpecialOperationNone, false, false);
        }

        //*************************************************************************
        //  Method: GetValuesInRow()
        //
        /// <summary>
        /// Gets a specified number of cell values in a row starting at a specified
        /// cell.
        /// </summary>
        ///
        /// <param name="worksheet">
        /// Worksheet that contains the values to get.
        /// </param>
        ///
        /// <param name="rowOneBased">
        /// One-based row number.
        /// </param>
        ///
        /// <param name="firstColumnOneBased">
        /// One-based column number of the first cell to get.
        /// </param>
        ///
        /// <param name="columns">
        /// Number of cells to get.  Must be greater than zero.
        /// </param>
        ///
        /// <returns>
        /// The cell values.  A value can be null.
        /// </returns>
        //*************************************************************************

        public static Object[,]
        GetValuesInRow
        (
            Microsoft.Office.Interop.Excel.Worksheet worksheet,
            Int32 rowOneBased,
            Int32 firstColumnOneBased,
            Int32 columns
        )
        {
            Debug.Assert(worksheet != null);
            Debug.Assert(rowOneBased >= 1);
            Debug.Assert(firstColumnOneBased >= 1);
            Debug.Assert(columns >= 1);

            Range oRange = (Range)worksheet.get_Range(

                (Range)worksheet.Cells[rowOneBased, firstColumnOneBased],

                (Range)worksheet.Cells[rowOneBased,
                    firstColumnOneBased + columns - 1]
                );

            return (ExcelUtil.GetRangeValues(oRange));
        }

        //*************************************************************************
        //  Method: GetValuesInColumn()
        //
        /// <summary>
        /// Gets a specified number of cell values in a column starting at a
        /// specified cell.
        /// </summary>
        ///
        /// <param name="worksheet">
        /// Worksheet that contains the values to get.
        /// </param>
        ///
        /// <param name="firstRowOneBased">
        /// One-based row number of the first cell to get.
        /// </param>
        ///
        /// <param name="columnOneBased">
        /// One-based column number.
        /// </param>
        ///
        /// <param name="rows">
        /// Number of cells to get.  Must be greater than zero.
        /// </param>
        ///
        /// <returns>
        /// The cell values.  A value can be null.
        /// </returns>
        //*************************************************************************

        public static Object[,]
        GetValuesInColumn
        (
            Microsoft.Office.Interop.Excel.Worksheet worksheet,
            Int32 firstRowOneBased,
            Int32 columnOneBased,
            Int32 rows
        )
        {
            Debug.Assert(worksheet != null);
            Debug.Assert(firstRowOneBased >= 1);
            Debug.Assert(columnOneBased >= 1);
            Debug.Assert(rows >= 1);

            Range oRange = (Range)worksheet.get_Range(

                (Range)worksheet.Cells[firstRowOneBased, columnOneBased],

                (Range)worksheet.Cells[firstRowOneBased + rows - 1,
                    columnOneBased]
                );

            return (ExcelUtil.GetRangeValues(oRange));
        }

        //*************************************************************************
        //  Method: TryGetContiguousValuesInRow()
        //
        /// <summary>
        /// Attempts to get the contiguous values in a row starting at a specified
        /// cell.
        /// </summary>
        ///
        /// <param name="worksheet">
        /// Worksheet that contains the values to get.
        /// </param>
        ///
        /// <param name="rowOneBased">
        /// One-based row number.
        /// </param>
        ///
        /// <param name="firstColumnOneBased">
        /// One-based column number of the first cell to get.
        /// </param>
        ///
        /// <param name="values">
        /// Where the contiguous cell values get stored if true is returned.
        /// </param>
        ///
        /// <returns>
        /// true if the cell at [rowOneBased, firstColumnOneBased] is not empty.
        /// </returns>
        ///
        /// <remarks>
        /// If the [rowOneBased, firstColumnOneBased] cell is not empty, the values
        /// from that cell and all contiguous cells to the right get stored at
        /// <paramref name="values" /> and true is returned.  false is returned
        /// otherwise.
        ///
        /// <para>
        /// Note that any of the contiguous cells can contain strings consisting of
        /// nothing but spaces.
        /// </para>
        ///
        /// </remarks>
        //*************************************************************************

        public static Boolean
        TryGetContiguousValuesInRow
        (
            Microsoft.Office.Interop.Excel.Worksheet worksheet,
            Int32 rowOneBased,
            Int32 firstColumnOneBased,
            out Object[,] values
        )
        {
            Debug.Assert(worksheet != null);
            Debug.Assert(rowOneBased >= 1);
            Debug.Assert(firstColumnOneBased >= 1);

            return (TryGetContiguousValuesInRowOrColumn(worksheet, rowOneBased,
                firstColumnOneBased, true, out values));
        }

        //*************************************************************************
        //  Method: TryGetContiguousValuesInColumn()
        //
        /// <summary>
        /// Attempts to get the contiguous values in a column starting at a
        /// specified cell.
        /// </summary>
        ///
        /// <param name="worksheet">
        /// Worksheet that contains the values to get.
        /// </param>
        ///
        /// <param name="firstRowOneBased">
        /// One-based row number of the first cell to get.
        /// </param>
        ///
        /// <param name="columnOneBased">
        /// One-based column number.
        /// </param>
        ///
        /// <param name="values">
        /// Where the contiguous cell values get stored if true is returned.
        /// </param>
        ///
        /// <returns>
        /// true if the cell at [firstRowOneBased, columnOneBased] is not empty.
        /// </returns>
        ///
        /// <remarks>
        /// If the [firstRowOneBased, columnOneBased] cell is not empty, the values
        /// from that cell and all contiguous cells below get stored at <paramref
        /// name="values" /> and true is returned.  false is returned otherwise.
        ///
        /// <para>
        /// Note that any of the contiguous cells can contain strings consisting of
        /// nothing but spaces.
        /// </para>
        ///
        /// </remarks>
        //*************************************************************************

        public static Boolean
        TryGetContiguousValuesInColumn
        (
            Microsoft.Office.Interop.Excel.Worksheet worksheet,
            Int32 firstRowOneBased,
            Int32 columnOneBased,
            out Object[,] values
        )
        {
            Debug.Assert(worksheet != null);
            Debug.Assert(firstRowOneBased >= 1);
            Debug.Assert(columnOneBased >= 1);

            return (TryGetContiguousValuesInRowOrColumn(worksheet,
                firstRowOneBased, columnOneBased, false, out values));
        }

        //*************************************************************************
        //  Method: TryGetLastNonEmptyRow()
        //
        /// <summary>
        /// Attempts to get the one-based row number of the last row in a range
        /// that has a non-empty cell.
        /// </summary>
        ///
        /// <param name="range">
        /// The range to use.
        /// </param>
        ///
        /// <param name="rowOneBased">
        /// Where the one-based row number gets stored if true is returned.
        /// </param>
        ///
        /// <returns>
        /// true if the row number was obtained.
        /// </returns>
        ///
        /// <remarks>
        /// This method searches <paramref name="range" /> from the bottom up for
        /// the first non-empty cell.  If it finds one, it stores the cell's
        /// one-based row number at <paramref name="rowOneBased" /> and returns
        /// true.  false is returned otherwise.
        /// </remarks>
        //*************************************************************************

        public static Boolean
        TryGetLastNonEmptyRow
        (
            Range range,
            out Int32 rowOneBased
        )
        {
            Debug.Assert(range != null);

            rowOneBased = Int32.MinValue;

#if false

        This should really be done with the following code:

        {
        Range oCell = range.Find("*", MissingValue, XlFindLookIn.xlFormulas,
            XlLookAt.xlWhole, XlSearchOrder.xlByRows,
            XlSearchDirection.xlPrevious, true, true, Missing.Value
            );

        if (oCell == null)
        {
            return (false);
        }

        rowOneBased = oCell.Row;
        return (true);
        }

        However, Range.Find() has problems finding things when cells are
        hidden.  If the third parameter is XlFindLookIn.xlValues, it doesn't
        look in hidden cells at all.  If the third parameter is xlFormulas,
        it sometimes, but not always, looks in hidden cells.

        To work around these oddities, use a brute-force technique of reading
        all the cells and searching manually.  This is wasteful and slow, but
        it works.

#endif

            Object[,] aoValues = GetRangeValues(range);

            Int32 iRows = aoValues.GetUpperBound(0);
            Int32 iColumns = aoValues.GetUpperBound(1);

            for (Int32 iRowOneBased = iRows; iRowOneBased >= 1; iRowOneBased--)
            {
                for (Int32 iColumnOneBased = 1; iColumnOneBased <= iColumns;
                    iColumnOneBased++)
                {
                    if (aoValues[iRowOneBased, iColumnOneBased] != null)
                    {
                        rowOneBased = range.Row + iRowOneBased - 1;

                        return (true);
                    }
                }
            }

            return (false);
        }

        //*************************************************************************
        //  Method: GetRangeValues()
        //
        /// <summary>
        /// Gets a range's values as a two-dimensional, one-based array.
        /// </summary>
        ///
        /// <param name="range">
        /// Range to get the values for.
        /// </param>
        ///
        /// <returns>
        /// A two-dimensional array of Objects.  Each dimension is one-based.
        /// </returns>
        ///
        /// <remarks>
        /// This is an alternative to Range.get_value(), which doesn't return a
        /// consistent type.  When a range contains two or more cells,
        /// Range.get_value() returns a two-dimensional array of Objects.  When a
        /// range contains just one cell, however, Range.get_value() returns an
        /// Object.
        ///
        /// <para>
        /// In contrast, this method always returns a two-dimensional array of
        /// Objects.  In the one-cell case, the array contains a single Object.
        /// </para>
        ///
        /// <para>
        /// The returned array has one-based dimensions, so the Object
        /// corresponding to the first cell in the range is at [1,1].
        /// </para>
        /// 
        /// </remarks>
        //*************************************************************************

        public static Object[,]
        GetRangeValues
        (
            Range range
        )
        {
            Debug.Assert(range != null);

            if (range.Rows.Count > 1 || range.Columns.Count > 1)
            {
                return ((Object[,])range.get_Value(Missing.Value));
            }

            Object[,] aoCellValues = GetSingleElement2DArray();

            aoCellValues[1, 1] = range.get_Value(Missing.Value);

            return (aoCellValues);
        }

        //*************************************************************************
        //  Method: SetRangeValues()
        //
        /// <summary>
        /// Sets the values on a range.
        /// </summary>
        ///
        /// <param name="upperLeftCornerMarker">
        /// The values are set on the parent worksheet starting at this range's
        /// upper-left corner.  Only those cells that correspond to <paramref
        /// name="values" /> are set, so the only requirement for this range is
        /// that it's upper-left corner be in the desired location.  Its size is
        /// unimportant.
        /// </param>
        ///
        /// <param name="values">
        /// The values to set.  The array indexes can be either zero-based or
        /// one-based.
        /// </param>
        ///
        /// <remarks>
        /// This method copies <paramref name="values" /> to the parent worksheet
        /// of the <paramref name="upperLeftCornerMarker" /> range, starting at the 
        /// range's upper-left corner.  If the upper-left corner is B2 and
        /// <paramref name="values" /> is 3 rows by 2 columns, for example, then
        /// the values are copied to B2:C4.
        /// </remarks>
        //*************************************************************************

        public static void
        SetRangeValues
        (
            Range upperLeftCornerMarker,
            Object[,] values
        )
        {
            Debug.Assert(upperLeftCornerMarker != null);
            Debug.Assert(values != null);

            Debug.Assert(upperLeftCornerMarker.Parent is Worksheet);

            Worksheet oWorksheet = (Worksheet)upperLeftCornerMarker.Parent;

            Int32 iRow = upperLeftCornerMarker.Row;
            Int32 iColumn = upperLeftCornerMarker.Column;

            Range oRangeToSet = oWorksheet.get_Range(

                oWorksheet.Cells[iRow, iColumn],

                oWorksheet.Cells[
                    iRow + values.GetUpperBound(0) - values.GetLowerBound(0),
                    iColumn + values.GetUpperBound(1) - values.GetLowerBound(1)
                    ]
                );

            oRangeToSet.set_Value(Missing.Value, values);
        }

        //*************************************************************************
        //  Method: SetVisibleRangeValue()
        //
        /// <summary>
        /// Sets the values in the visible cells of a range to a specified value.
        /// </summary>
        ///
        /// <param name="range">
        /// Range whose visible cells should be set to a value.
        /// </param>
        ///
        /// <param name="value">
        /// The value to set.
        /// </param>
        ///
        /// <remarks>
        /// This method sets the value of every cell in <paramref name="range" />
        /// to <paramref name="value" />.
        /// </remarks>
        //*************************************************************************

        public static void
        SetVisibleRangeValue
        (
            Range range,
            Object value
        )
        {
            Debug.Assert(range != null);

            Range oVisibleRange;

            if (!ExcelUtil.TryGetVisibleRange(range, out oVisibleRange))
            {
                return;
            }

            foreach (Range oVisibleArea in oVisibleRange.Areas)
            {
                oVisibleArea.set_Value(Missing.Value, value);
            }
        }

        //*************************************************************************
        //  Method: SetVisibleTableColumnData()
        //
        /// <overloads>
        /// Sets the values in the visible data range of a table column to a
        /// specified value.
        /// </overloads>
        ///
        /// <summary>
        /// Sets the values in the visible data range of a table column to a
        /// specified value given a workbook.
        /// </summary>
        ///
        /// <param name="workbook">
        /// Workbook containing the column.
        /// </param>
        ///
        /// <param name="worksheetName">
        /// Name of the worksheet containing the column.
        /// </param>
        ///
        /// <param name="tableName">
        /// Name of the table containing the column.
        /// </param>
        ///
        /// <param name="columnName">
        /// Name of the column to set.
        /// </param>
        ///
        /// <param name="value">
        /// The value to set.
        /// </param>
        ///
        /// <remarks>
        /// This method sets the value of every visible data cell in <paramref
        /// name="columnName" /> to <paramref name="value" />.
        /// </remarks>
        //*************************************************************************

        public static void
        SetVisibleTableColumnData
        (
            Microsoft.Office.Interop.Excel.Workbook workbook,
            String worksheetName,
            String tableName,
            String columnName,
            Object value
        )
        {
            Debug.Assert(workbook != null);
            Debug.Assert(!String.IsNullOrEmpty(worksheetName));
            Debug.Assert(!String.IsNullOrEmpty(tableName));
            Debug.Assert(!String.IsNullOrEmpty(columnName));

            ListObject oTable;

            if (TryGetTable(workbook, worksheetName, tableName, out oTable))
            {
                SetVisibleTableColumnData(oTable, columnName, value);
            }
        }

        //*************************************************************************
        //  Method: SetVisibleTableColumnData()
        //
        /// <summary>
        /// Sets the values in the visible data range of a table column to a
        /// specified value given a table.
        /// </summary>
        ///
        /// <param name="table">
        /// Table containing the column.
        /// </param>
        ///
        /// <param name="columnName">
        /// Name of the column to set.
        /// </param>
        ///
        /// <param name="value">
        /// The value to set.
        /// </param>
        ///
        /// <remarks>
        /// This method sets the value of every visible data cell in <paramref
        /// name="columnName" /> to <paramref name="value" />.
        /// </remarks>
        //*************************************************************************

        public static void
        SetVisibleTableColumnData
        (
            ListObject table,
            String columnName,
            Object value
        )
        {
            Debug.Assert(table != null);
            Debug.Assert(!String.IsNullOrEmpty(columnName));

            Range oColumnData;

            if (TryGetTableColumnData(table, columnName, out oColumnData))
            {
                SetVisibleRangeValue(oColumnData, value);
            }
        }

        //*************************************************************************
        //  Method: SetVisibleSelectedTableColumnData()
        //
        /// <summary>
        /// Sets the selected, visible data cells of a table column to a specified
        /// value.
        /// </summary>
        ///
        /// <param name="table">
        /// Table containing the column.  The table must be within the active
        /// worksheet.
        /// </param>
        ///
        /// <param name="selectedRange">
        /// The application's current selected range.  The range may contain
        /// multiple areas.
        /// </param>
        ///
        /// <param name="columnName">
        /// Name of the column to set.
        /// </param>
        ///
        /// <param name="value">
        /// The value to set.
        /// </param>
        ///
        /// <remarks>
        /// This method sets the value of every selected, visible data cell in
        /// <paramref name="columnName" /> to <paramref name="value" />.
        /// </remarks>
        //*************************************************************************

        public static void
        SetVisibleSelectedTableColumnData
        (
            ListObject table,
            Range selectedRange,
            String columnName,
            Object value
        )
        {
            Debug.Assert(table != null);
            Debug.Assert(table.Parent is Worksheet);
            Debug.Assert(WorksheetIsActive((Worksheet)table.Parent));
            Debug.Assert(selectedRange != null);
            Debug.Assert(!String.IsNullOrEmpty(columnName));

            Range oSelectedTableRange, oVisibleSelectedTableRange;
            ListColumn oColumn;

            if (
                !TryGetSelectedTableRange(table, selectedRange,
                    out oSelectedTableRange)
                ||
                !TryGetVisibleRange(oSelectedTableRange,
                    out oVisibleSelectedTableRange)
                ||
                !TryGetTableColumn(table, columnName, out oColumn)
                )
            {
                return;
            }

            Worksheet oWorksheet = (Worksheet)table.Parent;
            Int32 iColumnOneBased = oColumn.Range.Columns.Column;

            foreach (Range oVisibleSelectedArea in
                oVisibleSelectedTableRange.Areas)
            {
                Range oRows = oVisibleSelectedArea.Rows;
                Int32 iFirstRowOneBased = oRows.Row;
                Int32 iRows = oRows.Count;

                Range oRangeToSet = (Range)oWorksheet.get_Range(
                    oWorksheet.Cells[iFirstRowOneBased, iColumnOneBased],
                    oWorksheet.Cells[iFirstRowOneBased + iRows - 1, iColumnOneBased]
                    );

                oRangeToSet.set_Value(Missing.Value, value);
            }
        }

        //*************************************************************************
        //  Method: SetRangeStyle()
        //
        /// <summary>
        /// Sets the style on a range.
        /// </summary>
        ///
        /// <param name="range">
        /// Range to set the style on.
        /// </param>
        ///
        /// <param name="style">
        /// Style to set, or null to apply Excel's normal style.  Sample: "Bad".
        /// </param>
        ///
        /// <remarks>
        /// This method should be called instead of Range.Style.  If the Style
        /// property is set on a range in an inactive worksheet, Excel messes up
        /// the selection in the range's worksheet, and sometimes other worksheets
        /// as well.  This method works around that bug so that the selection isn't
        /// modified at all.
        /// </remarks>
        //*************************************************************************

        public static void
        SetRangeStyle
        (
            Range range,
            String style
        )
        {
            Debug.Assert(range != null);

            Application oApplication = range.Application;
            Boolean bOldScreenUpdating = false;
            Boolean bSwitchWorksheets = false;

            // Check whether the specified range is in the active worksheet.

            Object oOldActiveSheet = oApplication.ActiveSheet;
            Worksheet oOldActiveWorksheet = null;

            if (oOldActiveSheet != null && oOldActiveSheet is Worksheet)
            {
                oOldActiveWorksheet = (Worksheet)oOldActiveSheet;
                Worksheet oRangeWorksheet = range.Worksheet;

                if (oOldActiveWorksheet != oRangeWorksheet)
                {
                    bSwitchWorksheets = true;
                    bOldScreenUpdating = oApplication.ScreenUpdating;

                    oApplication.ScreenUpdating = false;
                    ActivateWorksheet(oRangeWorksheet);
                }
            }

            if (style == null)
            {
                style = "Normal";
            }

            try
            {
                range.Style = style;
            }
            catch (COMException)
            {
                // This can occur if the specified style is missing.
            }

            if (bSwitchWorksheets)
            {
                // Restore the original active worksheet.

                ActivateWorksheet(oOldActiveWorksheet);

                oApplication.ScreenUpdating = bOldScreenUpdating;
            }
        }

        //*************************************************************************
        //  Method: ForceCellText()
        //
        /// <summary>
        /// Forces a string to always appear as text when inserted into a cell.
        /// </summary>
        ///
        /// <param name="cellText">
        /// String that should always appear as text.
        /// </param>
        ///
        /// <returns>
        /// <paramref name="cellText" /> with a prepended apostrophe.
        /// </returns>
        ///
        /// <remarks>
        /// When the string "5/1", for example, is programmatically inserted into
        /// an Excel cell, Excel will convert it to a date even if the cell is
        /// formatted as Text.  If the string is prepended with an apostrophe,
        /// Excel will always treat it as text, regardless of the cell format.
        /// </remarks>
        //*************************************************************************

        public static String
        ForceCellText
        (
            String cellText
        )
        {
            Debug.Assert(cellText != null);

            return ("'" + cellText);
        }

        //*************************************************************************
        //  Method: GetRangeFormulas()
        //
        /// <summary>
        /// Gets a range's formulas as a two-dimensional, one-based array.
        /// </summary>
        ///
        /// <param name="range">
        /// Range to get the formulas for.
        /// </param>
        ///
        /// <returns>
        /// A two-dimensional array of Objects.  Each dimension is one-based.
        /// </returns>
        ///
        /// <remarks>
        /// This is an alternative to Range.Formula, which doesn't return a
        /// consistent type.  When a range contains two or more cells,
        /// Range.Formula returns a two-dimensional array of Objects.  When a
        /// range contains just one cell, however, Range.Formula() returns an
        /// Object.
        ///
        /// <para>
        /// In contrast, this method always returns a two-dimensional array of
        /// Objects.  In the one-cell case, the array contains a single Object.
        /// </para>
        ///
        /// <para>
        /// The returned array has one-based dimensions, so the Object
        /// corresponding to the first cell in the range is at [1,1].
        /// </para>
        /// 
        /// </remarks>
        //*************************************************************************

        public static Object[,]
        GetRangeFormulas
        (
            Range range
        )
        {
            Debug.Assert(range != null);

            if (range.Rows.Count > 1 || range.Columns.Count > 1)
            {
                return ((Object[,])range.Formula);
            }

            Object[,] aoCellFormulas = GetSingleElement2DArray();

            aoCellFormulas[1, 1] = range.Formula;

            return (aoCellFormulas);
        }

        //*************************************************************************
        //  Method: SelectRange()
        //
        /// <summary>
        /// Selects a range.
        /// </summary>
        ///
        /// <param name="range">
        /// The range to select.
        /// </param>
        ///
        /// <remarks>
        /// This method activates the range's worksheet before selecting the
        /// range.
        /// </remarks>
        //*************************************************************************

        public static void
        SelectRange
        (
            Range range
        )
        {
            Debug.Assert(range != null);

            range.Application.Goto(range, false);
        }

        //*************************************************************************
        //  Method: TryGetNonEmptyStringFromCell()
        //
        /// <overloads>
        /// Attempts to get a non-empty string from a worksheet cell.
        /// </overloads>
        ///
        /// <summary>
        /// Attempts to get a non-empty string from a worksheet cell given the
        /// worksheet.
        /// </summary>
        ///
        /// <param name="worksheet">
        /// The worksheet to get the non-empty string from.
        /// </param>
        ///
        /// <param name="rowOneBased">
        /// One-based row number to read.
        /// </param>
        ///
        /// <param name="columnOneBased">
        /// One-based column number to read.
        /// </param>
        ///
        /// <param name="nonEmptyString">
        /// Where a string gets stored if true is returned.
        /// </param>
        ///
        /// <returns>
        /// true if successful.
        /// </returns>
        ///
        /// <remarks>
        /// If the specified cell value contains anything besides spaces, the cell
        /// value is trimmed and stored at <paramref name="nonEmptyString" />, and
        /// true is returned.  false is returned otherwise.
        /// </remarks>
        //*************************************************************************

        public static Boolean
        TryGetNonEmptyStringFromCell
        (
            Worksheet worksheet,
            Int32 rowOneBased,
            Int32 columnOneBased,
            out String nonEmptyString
        )
        {
            Debug.Assert(worksheet != null);
            Debug.Assert(rowOneBased >= 1);
            Debug.Assert(columnOneBased >= 1);

            return (TryGetNonEmptyStringFromCell(
                (Range)worksheet.Cells[rowOneBased, columnOneBased],
                out nonEmptyString)
                );
        }

        //*************************************************************************
        //  Method: TryGetNonEmptyStringFromCell()
        //
        /// <summary>
        /// Attempts to get a non-empty string from a worksheet cell given a
        /// one-cell range from the worksheet.
        /// </summary>
        ///
        /// <param name="oneCellRange">
        /// A range containing one cell.
        /// </param>
        ///
        /// <param name="nonEmptyString">
        /// Where a string gets stored if true is returned.
        /// </param>
        ///
        /// <returns>
        /// true if successful.
        /// </returns>
        ///
        /// <remarks>
        /// If the specified cell value contains anything besides spaces, the cell
        /// value is trimmed and stored at <paramref name="nonEmptyString" />, and
        /// true is returned.  false is returned otherwise.
        /// </remarks>
        //*************************************************************************

        public static Boolean
        TryGetNonEmptyStringFromCell
        (
            Range oneCellRange,
            out String nonEmptyString
        )
        {
            Debug.Assert(oneCellRange != null);
            Debug.Assert(oneCellRange.Rows.Count == 1);
            Debug.Assert(oneCellRange.Columns.Count == 1);

            nonEmptyString = null;

            return (TryGetNonEmptyStringFromCell(GetRangeValues(oneCellRange),
                1, 1, out nonEmptyString));
        }

        //*************************************************************************
        //  Method: TryGetNonEmptyStringFromCell()
        //
        /// <summary>
        /// Attempts to get a non-empty string from a worksheet cell given an array
        /// of cell values read from the worksheet.
        /// </summary>
        ///
        /// <param name="cellValues">
        /// Two-dimensional array of values read from the worksheet.
        /// </param>
        ///
        /// <param name="rowOneBased">
        /// One-based row number to read.
        /// </param>
        ///
        /// <param name="columnOneBased">
        /// One-based column number to read.
        /// </param>
        ///
        /// <param name="nonEmptyString">
        /// Where a string gets stored if true is returned.
        /// </param>
        ///
        /// <returns>
        /// true if successful.
        /// </returns>
        ///
        /// <remarks>
        /// If the specified cell value contains anything besides spaces, the cell
        /// value is trimmed and stored at <paramref name="nonEmptyString" />, and
        /// true is returned.  false is returned otherwise.
        /// </remarks>
        //*************************************************************************

        public static Boolean
        TryGetNonEmptyStringFromCell
        (
            Object[,] cellValues,
            Int32 rowOneBased,
            Int32 columnOneBased,
            out String nonEmptyString
        )
        {
            Debug.Assert(cellValues != null);
            Debug.Assert(rowOneBased >= 1);
            Debug.Assert(columnOneBased >= 1);

            nonEmptyString = null;

            Object oObject = cellValues[rowOneBased, columnOneBased];

            if (oObject == null)
            {
                return (false);
            }

            String sString = oObject.ToString().Trim();

            if (sString.Length == 0)
            {
                return (false);
            }

            nonEmptyString = sString;

            return (true);
        }

        //*************************************************************************
        //  Method: TryGetDoubleFromCell()
        //
        /// <summary>
        /// Attempts to get a Double from a worksheet cell.
        /// </summary>
        ///
        /// <param name="cellValues">
        /// Two-dimensional array of values read from the worksheet.
        /// </param>
        ///
        /// <param name="rowOneBased">
        /// One-based row number to read.
        /// </param>
        ///
        /// <param name="columnOneBased">
        /// One-based column number to read.
        /// </param>
        ///
        /// <param name="theDouble">
        /// Where a Double gets stored if true is returned.  ("double" is a
        /// keyword.)
        /// </param>
        ///
        /// <returns>
        /// true if successful.
        /// </returns>
        ///
        /// <remarks>
        /// If the specified cell value contains a valid Double, the Double is
        /// stored at <paramref name="theDouble" /> and true is returned.  false is
        /// returned otherwise.
        /// </remarks>
        //*************************************************************************

        public static Boolean
        TryGetDoubleFromCell
        (
            Object[,] cellValues,
            Int32 rowOneBased,
            Int32 columnOneBased,
            out Double theDouble
        )
        {
            Debug.Assert(cellValues != null);
            Debug.Assert(rowOneBased >= 1);
            Debug.Assert(columnOneBased >= 1);

            theDouble = Double.MinValue;

            Object oObject = cellValues[rowOneBased, columnOneBased];

            if (oObject is Double)
            {
                theDouble = (Double)oObject;
                return (true);
            }

            return (false);
        }

        //*************************************************************************
        //  Method: CellContainsFormula()
        //
        /// <summary>
        /// Determines whether a cell contains a formula.
        /// </summary>
        ///
        /// <param name="cellValues">
        /// A two-dimensional array of Objects.  Each dimension is one-based.
        /// </param>
        ///
        /// <param name="rowOneBased">
        /// One-based row number to check.
        /// </param>
        ///
        /// <param name="columnOneBased">
        /// One-based column number to check.
        /// </param>
        ///
        /// <returns>
        /// true if the cell at the specified location in <paramref
        /// name="cellValues" /> contains a formula.
        /// </returns>
        //*************************************************************************

        public static Boolean
        CellContainsFormula
        (
            Object[,] cellValues,
            Int32 rowOneBased,
            Int32 columnOneBased
        )
        {
            Debug.Assert(cellValues != null);
            Debug.Assert(rowOneBased >= 1);
            Debug.Assert(columnOneBased >= 1);

            String sNonEmptyString;

            return (
                TryGetNonEmptyStringFromCell(cellValues, rowOneBased,
                    columnOneBased, out sNonEmptyString)
                &&
                sNonEmptyString.IndexOf('=') >= 0
                );

        }

        //*************************************************************************
        //  Method: TryGetTable()
        //
        /// <overloads>
        /// Attempts to get a table (ListObject) by name.
        /// </overloads>
        ///
        /// <summary>
        /// Attempts to get a table (ListObject) from a workbook by worksheet name
        /// and table name.
        /// </summary>
        ///
        /// <param name="workbook">
        /// Workbook to get the table from.
        /// </param>
        ///
        /// <param name="worksheetName">
        /// Name of the worksheet containing the table.
        /// </param>
        ///
        /// <param name="tableName">
        /// Name of the table to get.
        /// </param>
        ///
        /// <param name="table">
        /// Where the requested table gets stored if true is returned.
        /// </param>
        ///
        /// <returns>
        /// true if successful.
        /// </returns>
        ///
        /// <remarks>
        /// If <paramref name="workbook" /> contains a worksheet named <paramref
        /// name="worksheetName" /> that has a table (ListObject) named
        /// <paramref name="tableName" />, the ListObject is stored at <paramref
        /// name="table" /> and true is returned.  false is returned otherwise.
        /// </remarks>
        //*************************************************************************

        public static Boolean
        TryGetTable
        (
            Microsoft.Office.Interop.Excel.Workbook workbook,
            String worksheetName,
            String tableName,
            out ListObject table
        )
        {
            Debug.Assert(workbook != null);
            Debug.Assert(!String.IsNullOrEmpty(worksheetName));
            Debug.Assert(!String.IsNullOrEmpty(tableName));

            table = null;

            Microsoft.Office.Interop.Excel.Worksheet oWorksheet;

            return (TryGetWorksheet(workbook, worksheetName, out oWorksheet) &&
                TryGetTable(oWorksheet, tableName, out table));
        }

        //*************************************************************************
        //  Method: TryGetTable()
        //
        /// <summary>
        /// Attempts to get a table (ListObject) from a worksheet by table name.
        /// </summary>
        ///
        /// <param name="worksheet">
        /// Worksheet to get the table from.
        /// </param>
        ///
        /// <param name="tableName">
        /// Name of the table to get.
        /// </param>
        ///
        /// <param name="table">
        /// Where the requested table gets stored if true is returned.
        /// </param>
        ///
        /// <returns>
        /// true if successful.
        /// </returns>
        ///
        /// <remarks>
        /// If <paramref name="worksheet" /> contains a table (ListObject) named
        /// <paramref name="tableName" />, the ListObject is stored at <paramref
        /// name="table" /> and true is returned.  false is returned otherwise.
        /// </remarks>
        //*************************************************************************

        public static Boolean
        TryGetTable
        (
            Microsoft.Office.Interop.Excel.Worksheet worksheet,
            String tableName,
            out ListObject table
        )
        {
            Debug.Assert(worksheet != null);
            Debug.Assert(!String.IsNullOrEmpty(tableName));

            table = null;

            try
            {
                table = worksheet.ListObjects[tableName];

                return (true);
            }
            catch (COMException)
            {
                return (false);
            }
        }

        //*************************************************************************
        //  Method: TableIsEmpty()
        //
        /// <summary>
        /// Determines whether a table is empty.
        /// </summary>
        ///
        /// <param name="table">
        /// The table to test.
        /// </param>
        ///
        /// <returns>
        /// true if the data body range of the table is empty.
        /// </returns>
        //*************************************************************************

        public static Boolean
        TableIsEmpty
        (
            ListObject table
        )
        {
            Debug.Assert(table != null);

            // Note that the default state of a table in a new workbook is one
            // empty row.

            Range oDataBodyRange = table.DataBodyRange;

            Range oUsedRange;

            return (
                oDataBodyRange == null
                ||
                !TryGetNonEmptyRange(oDataBodyRange, out oUsedRange)
                );
        }

        //*************************************************************************
        //  Method: GetOffsetOfFirstEmptyTableRow()
        //
        /// <summary>
        /// Gets the offset of the first empty row in a table.
        /// </summary>
        ///
        /// <param name="table">
        /// The table to get the offset for.
        /// </param>
        ///
        /// <returns>
        /// The offset of the first empty row in <paramref name="table" />,
        /// measured from the first row in the table's data body range.
        /// </returns>
        ///
        /// <remarks>
        /// If the table is empty, 0 is returned.  If the table has data in its
        /// first N data rows, for example, N is returned.
        /// </remarks>
        //*************************************************************************

        public static Int32
        GetOffsetOfFirstEmptyTableRow
        (
            ListObject table
        )
        {
            Debug.Assert(table != null);

            Int32 iLastNonEmptyRowOneBased;
            Range oDataBodyRange = table.DataBodyRange;

            if (
                oDataBodyRange != null
                &&
                ExcelUtil.TryGetLastNonEmptyRow(oDataBodyRange,
                    out iLastNonEmptyRowOneBased)
                )
            {
                return (iLastNonEmptyRowOneBased - oDataBodyRange.Row + 1);
            }

            return (0);
        }

        //*************************************************************************
        //  Method: TryGetVisibleTableRange()
        //
        /// <summary>
        /// Attempts to get a range containing visible data rows from a table.
        /// </summary>
        ///
        /// <param name="table">
        /// Table to get a range from.
        /// </param>
        ///
        /// <param name="visibleTableRange">
        /// Where the range gets stored if true is returned.  The range contains at
        /// least one visible row and may consist of multiple areas.
        /// </param>
        ///
        /// <returns>
        /// true if the range was obtained.
        /// </returns>
        ///
        /// <remarks>
        /// If <paramref name="table" /> contains at least one data row that is
        /// visible, the range of visible rows is stored at <paramref
        /// name="visibleTableRange" /> and true is returned.  false is returned
        /// otherwise.
        /// </remarks>
        //*************************************************************************

        public static Boolean
        TryGetVisibleTableRange
        (
            ListObject table,
            out Range visibleTableRange
        )
        {
            Debug.Assert(table != null);

            visibleTableRange = null;

            // Read the range that contains the table data.  If the table is
            // filtered, the range may contain multiple areas.

            Range oDataBodyRange = table.DataBodyRange;

            // Reduce the range to visible rows.

            return (oDataBodyRange != null &&
                TryGetVisibleRange(oDataBodyRange, out visibleTableRange));
        }

        //*************************************************************************
        //  Method: TryGetVisibleSelectedTableRange()
        //
        /// <summary>
        /// Attempts to get the visible, selected range within a table after
        /// activating the table's parent worksheet.
        /// </summary>
        ///
        /// <param name="workbook">
        /// Workbook to get the table from.
        /// </param>
        ///
        /// <param name="worksheetName">
        /// Name of the worksheet containing the table.
        /// </param>
        ///
        /// <param name="tableName">
        /// Name of the table.
        /// </param>
        ///
        /// <param name="table">
        /// Where the table gets stored if true is returned.
        /// </param>
        ///
        /// <param name="visibleSelectedTableRange">
        /// Where the visible, selected range within the table gets stored if true
        /// is returned.  The range may contain multiple areas.
        /// </param>
        ///
        /// <returns>
        /// true if the current selected range intersects the table and at least
        /// part of the intersection is visible.
        /// </returns>
        //*************************************************************************

        public static Boolean
        TryGetVisibleSelectedTableRange
        (
            Microsoft.Office.Interop.Excel.Workbook workbook,
            String worksheetName,
            String tableName,
            out ListObject table,
            out Range visibleSelectedTableRange
        )
        {
            Debug.Assert(workbook != null);
            Debug.Assert(!String.IsNullOrEmpty(worksheetName));
            Debug.Assert(!String.IsNullOrEmpty(tableName));

            visibleSelectedTableRange = null;

            Range oSelectedTableRange;

            return (
                TryGetSelectedTableRange(workbook, worksheetName, tableName,
                    out table, out oSelectedTableRange)
                &&
                TryGetVisibleRange(oSelectedTableRange,
                    out visibleSelectedTableRange)
                );
        }

        //*************************************************************************
        //  Method: TryGetSelectedTableRange()
        //
        /// <overloads>
        /// Attempts to get the selected range within a table.
        /// </overloads>
        ///
        /// <summary>
        /// Attempts to get the selected range within a table after activating the
        /// table's parent worksheet.
        /// </summary>
        ///
        /// <param name="workbook">
        /// Workbook to get the table from.
        /// </param>
        ///
        /// <param name="worksheetName">
        /// Name of the worksheet containing the table.
        /// </param>
        ///
        /// <param name="tableName">
        /// Name of the table.
        /// </param>
        ///
        /// <param name="table">
        /// Where the table gets stored if true is returned.
        /// </param>
        ///
        /// <param name="selectedTableRange">
        /// Where the selected range within the table gets stored if true is
        /// returned.  The range may contain multiple areas.
        /// </param>
        ///
        /// <returns>
        /// true if the current selected range intersects the table.
        /// </returns>
        //*************************************************************************

        public static Boolean
        TryGetSelectedTableRange
        (
            Microsoft.Office.Interop.Excel.Workbook workbook,
            String worksheetName,
            String tableName,
            out ListObject table,
            out Range selectedTableRange
        )
        {
            Debug.Assert(workbook != null);
            Debug.Assert(!String.IsNullOrEmpty(worksheetName));
            Debug.Assert(!String.IsNullOrEmpty(tableName));

            selectedTableRange = null;
            table = null;

            Worksheet oWorksheet;

            if (
                TryGetWorksheet(workbook, worksheetName, out oWorksheet)
                &&
                TryGetTable(oWorksheet, tableName, out table)
                )
            {
                ActivateWorksheet(oWorksheet);

                Object oSelectedRange = workbook.Application.Selection;

                if (
                    oSelectedRange is Range
                    &&
                    TryGetSelectedTableRange(table, (Range)oSelectedRange,
                        out selectedTableRange)
                    )
                {
                    return (true);
                }
            }

            return (false);
        }

        //*************************************************************************
        //  Method: TryGetSelectedTableRange()
        //
        /// <summary>
        /// Attempts to get the selected range within a table.
        /// </summary>
        ///
        /// <param name="table">
        /// Table to get the selected range within.  The table must be within the
        /// active worksheet.
        /// </param>
        ///
        /// <param name="selectedRange">
        /// The application's current selected range.  The range may contain
        /// multiple areas.
        /// </param>
        ///
        /// <param name="selectedTableRange">
        /// Where the selected range within the table gets stored if true is
        /// returned.  The range may contain multiple areas.
        /// </param>
        ///
        /// <returns>
        /// true if the current selected range intersects the table.
        /// </returns>
        //*************************************************************************

        public static Boolean
        TryGetSelectedTableRange
        (
            ListObject table,
            Range selectedRange,
            out Range selectedTableRange
        )
        {
            Debug.Assert(table != null);
            Debug.Assert(table.Parent is Worksheet);
            Debug.Assert(WorksheetIsActive((Worksheet)table.Parent));
            Debug.Assert(selectedRange != null);

            selectedTableRange = null;

            Range oDataBodyRange = table.DataBodyRange;

            if (oDataBodyRange == null)
            {
                return (false);
            }

            // The selected range can extend outside the table.  Get the
            // intersection of the table with the selection.

            return (ExcelUtil.TryIntersectRanges(selectedRange,
                oDataBodyRange, out selectedTableRange));
        }

        //*************************************************************************
        //  Method: TryGetTableColumn()
        //
        /// <summary>
        /// Attempts to get a table column given the column name.
        /// </summary>
        ///
        /// <param name="table">
        /// Table to get the column from.
        /// </param>
        ///
        /// <param name="columnName">
        /// Name of the column to get.
        /// </param>
        ///
        /// <param name="column">
        /// Where the column named <paramref name="columnName" /> gets stored if
        /// true is returned.
        /// </param>
        ///
        /// <returns>
        /// true if successful.
        /// </returns>
        ///
        /// <remarks>
        /// If <paramref name="table" /> contains a column named <paramref
        /// name="columnName" />, the column is stored at <paramref
        /// name="column" /> and true is returned.  false is returned otherwise.
        /// </remarks>
        //*************************************************************************

        public static Boolean
        TryGetTableColumn
        (
            ListObject table,
            String columnName,
            out ListColumn column
        )
        {
            Debug.Assert(table != null);
            Debug.Assert(!String.IsNullOrEmpty(columnName));

            column = null;

            try
            {
                column = table.ListColumns[columnName];

                return (true);
            }
            catch (COMException)
            {
                return (false);
            }
        }

        //*************************************************************************
        //  Method: TryGetOrAddTableColumn()
        //
        /// <summary>
        /// Attempts to get a table column or add the column if it doesn't exist.
        /// </summary>
        ///
        /// <param name="table">
        /// Table to get the column from.
        /// </param>
        ///
        /// <param name="columnName">
        /// Name of the column to get.
        /// </param>
        ///
        /// <param name="columnWidthChars">
        /// Width of the added column, in characters, or <see
        /// cref="AutoColumnWidth" /> to set the width automatically.
        /// </param>
        ///
        /// <param name="columnStyle">
        /// Style of the added column, or null to apply Excel's normal style.
        /// Sample: "Bad".
        /// </param>
        ///
        /// <param name="listColumn">
        /// Where the column gets stored if true is returned.
        /// </param>
        ///
        /// <returns>
        /// true if the column was retrieved or added.
        /// </returns>
        //*************************************************************************

        public static Boolean
        TryGetOrAddTableColumn
        (
            ListObject table,
            String columnName,
            Double columnWidthChars,
            String columnStyle,
            out ListColumn listColumn
        )
        {
            Debug.Assert(table != null);
            Debug.Assert(!String.IsNullOrEmpty(columnName));

            Debug.Assert(columnWidthChars == AutoColumnWidth ||
                columnWidthChars >= 0);

            return (
                TryGetTableColumn(table, columnName, out listColumn)
                ||
                TryAddTableColumn(table, columnName, columnWidthChars, columnStyle,
                    out listColumn)
                );
        }

        //*************************************************************************
        //  Method: TryGetTableColumnDataAndValues()
        //
        /// <summary>
        /// Attempts to get the data range and values of one column of a table.
        /// </summary>
        ///
        /// <param name="table">
        /// Table to get the column data range and values from.
        /// </param>
        ///
        /// <param name="columnName">
        /// Name of the column to get data range and values for.
        /// </param>
        ///
        /// <param name="tableColumnData">
        /// Where the column data range gets stored if true is returned.  The data
        /// range includes only that part of the column within the table's data
        /// body range.  This excludes any header or totals row.
        /// </param>
        ///
        /// <param name="tableColumnDataValues">
        /// Where the values for <paramref name="tableColumnData" /> get stored if
        /// true is returned.
        /// </param>
        ///
        /// <returns>
        /// true if successful.
        /// </returns>
        ///
        /// <remarks>
        /// If <paramref name="table" /> contains a column named <paramref
        /// name="columnName" />, the column's data range is stored at <paramref
        /// name="tableColumnData" />, the values for the data range are stored at
        /// <paramref name="tableColumnDataValues" />, and true is returned.  false
        /// is returned otherwise.
        /// </remarks>
        //
        //  TODO: This hasn't been tested with a totals row.
        //*************************************************************************

        public static Boolean
        TryGetTableColumnDataAndValues
        (
            ListObject table,
            String columnName,
            out Range tableColumnData,
            out Object[,] tableColumnDataValues
        )
        {
            Debug.Assert(table != null);
            Debug.Assert(!String.IsNullOrEmpty(columnName));

            tableColumnData = null;
            tableColumnDataValues = null;

            if (!ExcelUtil.TryGetTableColumnData(table, columnName,
                out tableColumnData))
            {
                return (false);
            }

            tableColumnDataValues = ExcelUtil.GetRangeValues(tableColumnData);

            return (true);
        }

        //*************************************************************************
        //  Method: TryGetTableColumnData()
        //
        /// <overloads>
        /// Attempts to get the data range of one column of a table.
        /// </overloads>
        ///
        /// <summary>
        /// Attempts to get the data range of one column of a table given a table
        /// and column name.
        /// </summary>
        ///
        /// <param name="table">
        /// Table to get the column data range from.
        /// </param>
        ///
        /// <param name="columnName">
        /// Name of the column to get data range for.
        /// </param>
        ///
        /// <param name="tableColumnData">
        /// Where the column data range gets stored if true is returned.  The data
        /// range includes only that part of the column within the table's data
        /// body range.  This excludes any header or totals row.
        /// </param>
        ///
        /// <returns>
        /// true if successful.
        /// </returns>
        ///
        /// <remarks>
        /// If <paramref name="table" /> contains a column named <paramref
        /// name="columnName" />, the column's data range is stored at <paramref
        /// name="tableColumnData" /> and true is returned.  false is returned
        /// otherwise.
        /// </remarks>
        //
        //  TODO: This hasn't been tested with a totals row.
        //*************************************************************************

        public static Boolean
        TryGetTableColumnData
        (
            ListObject table,
            String columnName,
            out Range tableColumnData
        )
        {
            Debug.Assert(table != null);
            Debug.Assert(!String.IsNullOrEmpty(columnName));

            tableColumnData = null;

            ListColumn oColumn;

            return (
                TryGetTableColumn(table, columnName, out oColumn)
                &&
                TryGetTableColumnData(oColumn, out tableColumnData)
                );
        }

        //*************************************************************************
        //  Method: TryGetTableColumnData()
        //
        /// <summary>
        /// Attempts to get the data range of one column of a table given the
        /// column.
        /// </summary>
        ///
        /// <param name="column">
        /// Column to get the data range from.
        /// </param>
        ///
        /// <param name="tableColumnData">
        /// Where the column data range gets stored if true is returned.  The data
        /// range includes only that part of the column within the table's data
        /// body range.  This excludes any header or totals row.
        /// </param>
        ///
        /// <returns>
        /// true if successful.
        /// </returns>
        //
        //  TODO: This hasn't been tested with a totals row.
        //*************************************************************************

        public static Boolean
        TryGetTableColumnData
        (
            ListColumn column,
            out Range tableColumnData
        )
        {
            Debug.Assert(column != null);

            tableColumnData = null;

            Debug.Assert(column.Parent is ListObject);

            ListObject oTable = (ListObject)column.Parent;

            Range oDataBodyRange = oTable.DataBodyRange;

            if (oDataBodyRange == null)
            {
                // This happens when the user deletes the first data row of a one-
                // row table.  It looks like an empty row is there, but the
                // DataBodyRange is actually null.

                Int32 iRow;

                // Is there a header row?

                Range oRangeToUse = oTable.HeaderRowRange;

                if (oRangeToUse != null)
                {
                    // Yes.  Use the row after the header row.

                    iRow = oRangeToUse.Row + 1;
                }
                else
                {
                    // No.  Use the first row of the table.

                    oRangeToUse = oTable.Range;

                    iRow = oRangeToUse.Row;
                }

                Debug.Assert(oTable.Parent is Worksheet);

                Worksheet oWorksheet = (Worksheet)oTable.Parent;

                oDataBodyRange = oWorksheet.get_Range(

                    oWorksheet.Cells[iRow, oRangeToUse.Column],

                    oWorksheet.Cells[iRow,
                        oRangeToUse.Column + oRangeToUse.Columns.Count - 1]
                    );
            }

            return (ExcelUtil.TryIntersectRanges(oDataBodyRange, column.Range,
                out tableColumnData));
        }

        //*************************************************************************
        //  Method: TryGetVisibleTableColumnData()
        //
        /// <summary>
        /// Attempts to get the visible data range of one column of a table.
        /// </summary>
        ///
        /// <param name="table">
        /// Table to get the visible column data range from.
        /// </param>
        ///
        /// <param name="columnName">
        /// Name of the column to get visible data range for.
        /// </param>
        ///
        /// <param name="visibleTableColumnData">
        /// Where the visible column data range gets stored if true is returned.
        /// The data range includes only that part of the column within the table's
        /// data body range.  This excludes any header or totals row.  The range
        /// may consist of multiple areas.
        /// </param>
        ///
        /// <returns>
        /// true if successful.
        /// </returns>
        ///
        /// <remarks>
        /// If <paramref name="table" /> contains a column named <paramref
        /// name="columnName" />, the column's visible data range is stored at
        /// <paramref name="visibleTableColumnData" /> and true is returned.  false
        /// is returned otherwise.
        /// </remarks>
        //
        //  TODO: This hasn't been tested with a totals row.
        //*************************************************************************

        public static Boolean
        TryGetVisibleTableColumnData
        (
            ListObject table,
            String columnName,
            out Range visibleTableColumnData
        )
        {
            Debug.Assert(table != null);
            Debug.Assert(!String.IsNullOrEmpty(columnName));

            visibleTableColumnData = null;

            Range oTempRange;

            return (
                TryGetTableColumnData(table, columnName, out oTempRange)
                &&
                TryGetVisibleRange(oTempRange, out visibleTableColumnData)
                );
        }

        //*************************************************************************
        //  Method: TryClearTableColumnDataContents()
        //
        /// <summary>
        /// Attempts to clear the contents of the data range of one column of a
        /// table.
        /// </summary>
        ///
        /// <param name="table">
        /// Table containing the column.
        /// </param>
        ///
        /// <param name="columnName">
        /// Name of the column to clear.
        /// </param>
        ///
        /// <returns>
        /// true if successful.
        /// </returns>
        //
        //  TODO: This hasn't been tested with a totals row.
        //*************************************************************************

        public static Boolean
        TryClearTableColumnDataContents
        (
            ListObject table,
            String columnName
        )
        {
            Debug.Assert(table != null);
            Debug.Assert(!String.IsNullOrEmpty(columnName));

            Range oTableColumnData;

            if (!TryGetTableColumnData(table, columnName, out oTableColumnData))
            {
                return (false);
            }

            oTableColumnData.ClearContents();
            return (true);
        }

        //*************************************************************************
        //  Method: TryAddTableColumnWithRowNumbers()
        //
        /// <summary>
        /// Attempts to add a row-number column to the end of a table.
        /// </summary>
        ///
        /// <param name="table">
        /// Table to add a column to.
        /// </param>
        ///
        /// <param name="columnName">
        /// Name of the column to add.
        /// </param>
        ///
        /// <param name="columnWidthChars">
        /// Width of the new column, in characters, or <see
        /// cref="AutoColumnWidth" /> to set the width automatically.
        /// </param>
        ///
        /// <param name="columnStyle">
        /// Style of the new column, or null to apply Excel's normal style.
        /// Sample: "Bad".
        /// </param>
        ///
        /// <param name="listColumn">
        /// Where the added column gets stored if true is returned.
        /// </param>
        ///
        /// <returns>
        /// true if the column was added.
        /// </returns>
        ///
        /// <remarks>
        /// This method adds the specified column, then fills it with the worksheet
        /// row number of each table row.
        /// </remarks>
        //*************************************************************************

        public static Boolean
        TryAddTableColumnWithRowNumbers
        (
            ListObject table,
            String columnName,
            Double columnWidthChars,
            String columnStyle,
            out ListColumn listColumn
        )
        {
            Debug.Assert(table != null);
            Debug.Assert(!String.IsNullOrEmpty(columnName));

            Debug.Assert(columnWidthChars == AutoColumnWidth ||
                columnWidthChars >= 0);

            listColumn = null;

            if (!TryAddTableColumn(table, columnName, columnWidthChars,
                columnStyle, out listColumn))
            {
                return (false);
            }

            Range oDataBodyRange = listColumn.DataBodyRange;

            if (oDataBodyRange != null)
            {
                // Fill the column with a ROW() formulas.

                oDataBodyRange.set_Value(Missing.Value, "=ROW()");

                // Convert the formulas to static values.

                oDataBodyRange.Copy(Missing.Value);

                PasteValues(oDataBodyRange);
            }

            return (true);
        }

        //*************************************************************************
        //  Method: TryAddTableColumn()
        //
        /// <summary>
        /// Attempts to add a column to the end of a table.
        /// </summary>
        ///
        /// <param name="table">
        /// Table to add a column to.
        /// </param>
        ///
        /// <param name="columnName">
        /// Name of the column to add.
        /// </param>
        ///
        /// <param name="columnWidthChars">
        /// Width of the new column, in characters, or <see
        /// cref="AutoColumnWidth" /> to set the width automatically.
        /// </param>
        ///
        /// <param name="columnStyle">
        /// Style of the new column, or null to apply Excel's normal style.
        /// Sample: "Bad".
        /// </param>
        ///
        /// <param name="listColumn">
        /// Where the added column gets stored if true is returned.
        /// </param>
        ///
        /// <returns>
        /// true if the column was added.
        /// </returns>
        //*************************************************************************

        public static Boolean
        TryAddTableColumn
        (
            ListObject table,
            String columnName,
            Double columnWidthChars,
            String columnStyle,
            out ListColumn listColumn
        )
        {
            Debug.Assert(table != null);
            Debug.Assert(!String.IsNullOrEmpty(columnName));

            Debug.Assert(columnWidthChars == AutoColumnWidth ||
                columnWidthChars >= 0);

            return (TryAddOrInsertTableColumn(table, columnName, -1,
                columnWidthChars, columnStyle, out listColumn));
        }

        //*************************************************************************
        //  Method: TryInsertTableColumn()
        //
        /// <summary>
        /// Attempts to insert a column into a table.
        /// </summary>
        ///
        /// <param name="table">
        /// Table to insert a column into.
        /// </param>
        ///
        /// <param name="columnName">
        /// Name of the column to insert.
        /// </param>
        ///
        /// <param name="oneBasedColumnIndex">
        /// One-based index of the column after it is inserted.
        /// </param>
        ///
        /// <param name="columnWidthChars">
        /// Width of the new column, in characters, or <see
        /// cref="AutoColumnWidth" /> to set the width automatically.
        /// </param>
        ///
        /// <param name="columnStyle">
        /// Style of the new column, or null to apply Excel's normal style.
        /// Sample: "Bad".
        /// </param>
        ///
        /// <param name="listColumn">
        /// Where the inserted column gets stored if true is returned.
        /// </param>
        ///
        /// <returns>
        /// true if the column was inserted.
        /// </returns>
        //*************************************************************************

        public static Boolean
        TryInsertTableColumn
        (
            ListObject table,
            String columnName,
            Int32 oneBasedColumnIndex,
            Double columnWidthChars,
            String columnStyle,
            out ListColumn listColumn
        )
        {
            Debug.Assert(table != null);
            Debug.Assert(!String.IsNullOrEmpty(columnName));
            Debug.Assert(oneBasedColumnIndex >= 1);

            Debug.Assert(columnWidthChars == AutoColumnWidth ||
                columnWidthChars >= 0);

            return (TryAddOrInsertTableColumn(table, columnName,
                oneBasedColumnIndex, columnWidthChars, columnStyle,
                out listColumn));
        }

        //*************************************************************************
        //  Method: GetTableColumnNames()
        //
        /// <summary>
        /// Gets the names of the columns in a table.
        /// </summary>
        ///
        /// <param name="table">
        /// Table to get the column names from.
        /// </param>
        ///
        /// <param name="columnNamesToExclude">
        /// An array of zero or more column names to exclude from the returned
        /// array.
        /// </param>
        ///
        /// <param name="columnNameBasesToExclude">
        /// An array of zero or more column name bases to exclude from the returned
        /// array.  If the array includes "Custom", for example, then a column
        /// named "Custom 1" will be excluded.
        /// </param>
        ///
        /// <returns>
        /// An array of zero or more column names.
        /// </returns>
        ///
        /// <remarks>
        /// This method returns the names of all the columns in <paramref
        /// name="table" />, except for those names contained in <paramref
        /// name="columnNamesToExclude" /> and those names that start with the
        /// bases contained in <paramref name="columnNameBasesToExclude" />.
        /// </remarks>
        //*************************************************************************

        public static String[]
        GetTableColumnNames
        (
            ListObject table,
            String[] columnNamesToExclude,
            String[] columnNameBasesToExclude
        )
        {
            Debug.Assert(table != null);
            Debug.Assert(columnNamesToExclude != null);
            Debug.Assert(columnNameBasesToExclude != null);

            List<String> oTableColumnNames = new List<String>();

            // Loop through the table's columns.

            foreach (ListColumn oColumn in table.ListColumns)
            {
                String sColumnName = oColumn.Name;

                if (String.IsNullOrEmpty(sColumnName))
                {
                    goto Skip;
                }

                if (Array.IndexOf(columnNamesToExclude, sColumnName) >= 0)
                {
                    goto Skip;
                }

                foreach (String sColumnNameBaseToExclude in
                    columnNameBasesToExclude)
                {
                    if (sColumnName.StartsWith(sColumnNameBaseToExclude))
                    {
                        goto Skip;
                    }
                }

                oTableColumnNames.Add(sColumnName);

            Skip:
                ;
            }

            return (oTableColumnNames.ToArray());
        }

        //*************************************************************************
        //  Method: ClearTable()
        //
        /// <overloads>
        /// Clears a table of all contents.
        /// </overloads>
        ///
        /// <summary>
        /// Clears a table specified by ListObject of all contents.
        /// </summary>
        ///
        /// <param name="table">
        /// Table to clear.
        /// </param>
        ///
        /// <remarks>
        /// This method reduces a table to its header row and totals row, if
        /// present, and one data body row.  The data body row contains the
        /// original formatting and data validation, but its contents are cleared.
        /// </remarks>
        //*************************************************************************

        public static void
        ClearTable
        (
            ListObject table
        )
        {
            Debug.Assert(table != null);

            // First, clear the data body rows.  This clears the contents but
            // retains formatting and data validation.

            Range oDataBodyRange = table.DataBodyRange;

            if (oDataBodyRange != null)
            {
                oDataBodyRange.ClearContents();
            }

            // Reduce the size of the table to get rid of the formatting.

            Int32 iRows = 1;

            if (table.HeaderRowRange != null)
            {
                iRows++;
            }

            if (table.TotalsRowRange != null)
            {
                iRows++;
            }

            Range oTableRange = table.Range;

            oTableRange = oTableRange.get_Resize(iRows, oTableRange.Columns.Count);

            table.Resize(oTableRange);
        }

        //*************************************************************************
        //  Method: ClearTable()
        //
        /// <summary>
        /// Clears a table specified by worksheet and table names of all contents.
        /// </summary>
        ///
        /// <param name="workbook">
        /// Workbook to get the table from.
        /// </param>
        ///
        /// <param name="worksheetName">
        /// Name of the worksheet containing the table.
        /// </param>
        ///
        /// <param name="tableName">
        /// Name of the table to clear.
        /// </param>
        ///
        /// <remarks>
        /// This method reduces a table to its header row and totals row, if
        /// present, and one data body row.  The data body row contains the
        /// original formatting and data validation, but its contents are cleared.
        ///
        /// <para>
        /// If the specified table doesn't exist, this method does nothing.
        /// </para>
        ///
        /// </remarks>
        //*************************************************************************

        public static void
        ClearTable
        (
            Microsoft.Office.Interop.Excel.Workbook workbook,
            String worksheetName,
            String tableName
        )
        {
            Debug.Assert(workbook != null);
            Debug.Assert(!String.IsNullOrEmpty(worksheetName));
            Debug.Assert(!String.IsNullOrEmpty(tableName));

            ListObject oTable;

            if (ExcelUtil.TryGetTable(workbook, worksheetName, tableName,
                out oTable))
            {
                ExcelUtil.ClearTable(oTable);
            }
        }

        //*************************************************************************
        //  Method: ClearTables()
        //
        /// <summary>
        /// Clears multiple tables specified by worksheet and table names of all
        /// contents.
        /// </summary>
        ///
        /// <param name="workbook">
        /// Workbook to get the tables from.
        /// </param>
        ///
        /// <param name="worksheetNameTableNamePairs">
        /// One or more pairs of names.  The first name in each pair is the
        /// worksheet name and the second name in each pair is the table name.
        /// </param>
        ///
        /// <remarks>
        /// This method reduces one or more tables to their header row and totals
        /// row, if present, and one data body row.  The data body row contains the
        /// original formatting and data validation, but its contents are cleared.
        ///
        /// <para>
        /// If a specified table doesn't exist, this method skips it.
        /// </para>
        ///
        /// </remarks>
        //*************************************************************************

        public static void
        ClearTables
        (
            Microsoft.Office.Interop.Excel.Workbook workbook,
            params String[] worksheetNameTableNamePairs
        )
        {
            Debug.Assert(workbook != null);
            Debug.Assert(worksheetNameTableNamePairs != null);
            Debug.Assert(worksheetNameTableNamePairs.Length % 2 == 0);

            Int32 iNames = worksheetNameTableNamePairs.Length;

            for (Int32 i = 0; i < iNames; i += 2)
            {
                ClearTable(workbook,
                    worksheetNameTableNamePairs[i + 0],
                    worksheetNameTableNamePairs[i + 1]
                    );
            }
        }

        //*************************************************************************
        //  Method: ClearTableAutoFilters()
        //
        /// <summary>
        /// Clears any AutoFiltering from a table.
        /// </summary>
        ///
        /// <param name="table">
        /// The table to clear AutoFiltering on.
        /// </param>
        //*************************************************************************

        public static void
        ClearTableAutoFilters
        (
            ListObject table
        )
        {
            Debug.Assert(table != null);

            AutoFilter oAutoFilter = table.AutoFilter;

            if (oAutoFilter == null)
            {
                return;
            }

            Filters oFilters = oAutoFilter.Filters;
            Int32 iFilters = oFilters.Count;
            Range oTableRange = table.Range;

            for (Int32 i = 1; i <= iFilters; i++)
            {
                if (oFilters[i].On)
                {
                    oTableRange.AutoFilter(i, Missing.Value,
                        XlAutoFilterOperator.xlAnd, Missing.Value, Missing.Value);
                }
            }
        }

        //*************************************************************************
        //  Method: TryIntersectRanges()
        //
        /// <summary>
        /// Attempts to get the intersection of two ranges.
        /// </summary>
        ///
        /// <param name="range1">
        /// First range.  Can be null.
        /// </param>
        ///
        /// <param name="range2">
        /// Range to intersect with <paramref name="range1" />.  Can be null.
        /// </param>
        ///
        /// <param name="intersection">
        /// Where the intersection of <paramref name="range1" /> and <paramref
        /// name="range2" /> gets stored if true is returned.
        /// </param>
        ///
        /// <returns>
        /// true if the intersection is not null.
        /// </returns>
        //*************************************************************************

        public static Boolean
        TryIntersectRanges
        (
            Range range1,
            Range range2,
            out Range intersection
        )
        {
            intersection = null;

            if (range1 != null && range2 != null)
            {
                intersection = range1.Application.Intersect(range1, range2,

                    Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                    Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                    Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                    Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                    Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                    Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                    Missing.Value, Missing.Value, Missing.Value, Missing.Value
                    );
            }

            return (intersection != null);
        }

        //*************************************************************************
        //  Method: TryUnionRanges()
        //
        /// <summary>
        /// Attempts to get the union of two ranges.
        /// </summary>
        ///
        /// <param name="range1">
        /// First range.  Can be null.
        /// </param>
        ///
        /// <param name="range2">
        /// Range to union with <paramref name="range1" />.  Can be null.
        /// </param>
        ///
        /// <param name="union">
        /// Where the union of <paramref name="range1" /> and <paramref
        /// name="range2" /> gets stored if true is returned.
        /// </param>
        ///
        /// <returns>
        /// true if the union is not null.
        /// </returns>
        //*************************************************************************

        public static Boolean
        TryUnionRanges
        (
            Range range1,
            Range range2,
            out Range union
        )
        {
            union = null;

            if (range1 != null && range2 != null)
            {
                union = range1.Application.Union(range1, range2,

                    Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                    Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                    Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                    Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                    Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                    Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                    Missing.Value, Missing.Value, Missing.Value, Missing.Value
                    );
            }
            else if (range1 != null)
            {
                union = range1;
            }
            else if (range2 != null)
            {
                union = range2;
            }

            return (union != null);
        }

        //*************************************************************************
        //  Method: GetOneBasedRowNumbers()
        //
        /// <summary>
        /// Gets the one-based row number of every row in a range.
        /// </summary>
        ///
        /// <param name="range">
        /// Range from which to get the one-based row numbers.
        /// </param>
        ///
        /// <returns>
        /// An array of one-based row numbers, one for each row in <paramref
        /// name="range" />.  The order of row numbers in the array is undefined.
        /// The returned array is never null.
        /// </returns>
        //*************************************************************************

        public static Int32[]
        GetOneBasedRowNumbers
        (
            Range range
        )
        {
            Debug.Assert(range != null);

            // One way to do this is to loop through the areas in range, then loop
            // through the rows in each area.  This wouldn't scale well, however,
            // because all the calls to get areas and rows would have to be
            // marshalled, and marshalling is slow.
            //
            // Instead, let Excel do all the work in a single marshalled call that
            // converts the range to an address, then parse the address.

            String sAddress = GetRangeAddress(range);

            return (GetOneBasedRowNumbers(sAddress));
        }

        //*************************************************************************
        //  Method: GetOneBasedRowNumbers()
        //
        /// <summary>
        /// Gets the one-based row number of every row in a range.
        /// </summary>
        ///
        /// <param name="rangeAddress">
        /// Range address from which to get the one-based row numbers.  Can be
        /// empty, but can't be null.  Sample range address:
        /// "A3,B16,A30,A24:F24,C2:C244,A5:F5".
        /// </param>
        ///
        /// <returns>
        /// An array of one-based row numbers, one for each row in <paramref
        /// name="range" />.  The order of row numbers in the array is undefined.
        /// The returned array is never null.
        /// </returns>
        //*************************************************************************

        public static Int32[]
        GetOneBasedRowNumbers
        (
            String rangeAddress
        )
        {
            Debug.Assert(rangeAddress != null);

            // Excel delimites areas with commas.  Split the address into area
            // addresses.

            String[] asAreaAddresses = rangeAddress.Split(
                new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            // Create a dictionary to keep track of unique row numbers.  The key
            // is the row number and the value is unused.

            Dictionary<Int32, Char> oUniqueRowNumbers =
                new Dictionary<Int32, Char>();

            foreach (String sAreaAddress in asAreaAddresses)
            {
                // If the area address includes a colon, split it further.

                String[] asAreaSubaddresses = sAreaAddress.Split(
                    new Char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                Debug.Assert(asAreaSubaddresses.Length >= 1);

                Int32 iFirstRowNumber =
                    GetOneBasedRowNumber(asAreaSubaddresses[0]);

                if (asAreaSubaddresses.Length == 1)
                {
                    oUniqueRowNumbers[iFirstRowNumber] = ' ';
                }
                else
                {
                    Int32 iSecondRowNumber =
                        GetOneBasedRowNumber(asAreaSubaddresses[1]);

                    for (Int32 iRowNumber = iFirstRowNumber;
                        iRowNumber <= iSecondRowNumber; iRowNumber++)
                    {
                        oUniqueRowNumbers[iRowNumber] = ' ';
                    }
                }
            }

            // Copy the unique row numbers to an array.

            return (CollectionUtil.DictionaryKeysToArray<Int32, Char>(
                oUniqueRowNumbers));
        }

        //*************************************************************************
        //  Method: GetOneBasedRowNumber()
        //
        /// <summary>
        /// Gets the one-based row number from an A1-style cell address.
        /// </summary>
        ///
        /// <param name="cellAddressA1Style">
        /// Cell address from which to get the one-based row number.  Sample cell
        /// address: "B16".
        /// </param>
        ///
        /// <returns>
        /// The one-based row number for <paramref name="cellAddressA1Style" />.
        /// Sample return value: 16.
        /// </returns>
        ///
        /// <seealso cref="GetColumnLetter" />
        /// <seealso cref="ParseCellAddress" />
        //*************************************************************************

        public static Int32
        GetOneBasedRowNumber
        (
            String cellAddressA1Style
        )
        {
            Int32 iOneBasedRowNumber;
            String sColumnLetter;

            ParseCellAddress(cellAddressA1Style, out iOneBasedRowNumber,
                out sColumnLetter);

            return (iOneBasedRowNumber);
        }

        //*************************************************************************
        //  Method: GetColumnLetter()
        //
        /// <summary>
        /// Gets the column letter (or letters) from an A1-style cell address.
        /// </summary>
        ///
        /// <param name="cellAddressA1Style">
        /// Cell address from which to get the column letter.  Sample cell
        /// address: "B16".
        /// </param>
        ///
        /// <returns>
        /// The column letter (or letters) for <paramref
        /// name="cellAddressA1Style" />.  Sample return value: "B".
        /// </returns>
        ///
        /// <seealso cref="GetOneBasedRowNumber" />
        /// <seealso cref="ParseCellAddress" />
        //*************************************************************************

        public static String
        GetColumnLetter
        (
            String cellAddressA1Style
        )
        {
            Int32 iOneBasedRowNumber;
            String sColumnLetter;

            ParseCellAddress(cellAddressA1Style, out iOneBasedRowNumber,
                out sColumnLetter);

            return (sColumnLetter);
        }

        //*************************************************************************
        //  Method: GetColumnFormat()
        //
        /// <summary>
        /// Determines the format of a table column.
        /// </summary>
        ///
        /// <param name="column">
        /// The table column to check.
        /// </param>
        ///
        /// <returns>
        /// The column format.
        /// </returns>
        //*************************************************************************

        public static ExcelColumnFormat
        GetColumnFormat
        (
            ListColumn column
        )
        {
            Debug.Assert(column != null);

            Range oColumnData = column.DataBodyRange;

            Debug.Assert(oColumnData != null);
            Debug.Assert(oColumnData.Rows.Count > 0);

            // Look at the type of the value in the first cell.

            Debug.Assert(oColumnData.Cells[1, 1] is Range);

            Range oFirstDataCell = (Range)oColumnData.Cells[1, 1];
            Object oFirstDataCellValue = oFirstDataCell.get_Value(Missing.Value);

            Debug.Assert(oFirstDataCellValue != null);

            if (oFirstDataCellValue is DateTime)
            {
                if (CellContainsTime(oFirstDataCell))
                {
                    // Sample: 1/1/2008 3:40 pm.

                    return (ExcelColumnFormat.DateAndTime);
                }
                else
                {
                    // Sample: 1/1/2008.

                    return (ExcelColumnFormat.Date);
                }
            }
            else if (oFirstDataCellValue is Double)
            {
                // Cells formatted as a time are returned as Double.  Another test
                // is required to distinguish times from real Doubles.

                if (CellContainsTime(oFirstDataCell))
                {
                    // Sample: 3:40 pm.

                    return (ExcelColumnFormat.Time);
                }
                else
                {
                    // Sample: 123.

                    return (ExcelColumnFormat.Number);
                }
            }
            else
            {
                return (ExcelColumnFormat.Other);
            }
        }

        //*************************************************************************
        //  Method: CellContainsTime()
        //
        /// <summary>
        /// Determines whether a cell contains a time.
        /// </summary>
        ///
        /// <param name="cell">
        /// The cell to check.
        /// </param>
        ///
        /// <returns>
        /// true if the cell contains a time.
        /// </returns>
        //*************************************************************************

        public static Boolean
        CellContainsTime
        (
            Range cell
        )
        {
            Debug.Assert(cell != null);

            // The easiest (but not perfect) test is to check the number format for
            // hours.

            Object oNumberFormat = cell.NumberFormat;

            Debug.Assert(oNumberFormat is String);

            return (((String)cell.NumberFormat).Contains("h"));
        }

        //*************************************************************************
        //  Method: ParseCellAddress()
        //
        /// <summary>
        /// Gets the one-based row number and column letter from an A1-style cell
        /// address.
        /// </summary>
        ///
        /// <param name="cellAddressA1Style">
        /// Cell address to parse.  Sample cell address: "B16".
        /// </param>
        ///
        /// <param name="oneBasedRowNumber">
        /// Where the one-based row number for <paramref
        /// name="cellAddressA1Style" /> gets stored.  Sample value: 16.
        /// </param>
        ///
        /// <param name="columnLetter">
        /// Where the column letter (or letters) for <paramref
        /// name="cellAddressA1Style" /> gets stored.  Sample value: "B".
        /// </param>
        ///
        /// <seealso cref="GetOneBasedRowNumber" />
        /// <seealso cref="GetColumnLetter" />
        //*************************************************************************

        public static void
        ParseCellAddress
        (
            String cellAddressA1Style,
            out Int32 oneBasedRowNumber,
            out String columnLetter
        )
        {
            Debug.Assert(!String.IsNullOrEmpty(cellAddressA1Style));
            Debug.Assert(cellAddressA1Style.IndexOf(':') == -1);

            oneBasedRowNumber = Int32.MinValue;
            columnLetter = null;

            // Don't use regular expressions here.  They are too slow.

            Int32 iLength = cellAddressA1Style.Length;

            for (Int32 i = 0; i < iLength; i++)
            {
                if (Char.IsDigit(cellAddressA1Style[i]))
                {
                    oneBasedRowNumber =
                        Int32.Parse(cellAddressA1Style.Substring(i));

                    columnLetter = cellAddressA1Style.Substring(0, i);

                    return;
                }
            }
        }

        //*************************************************************************
        //  Method: GetTableContextCommandBar()
        //
        /// <summary>
        /// Gets the table context CommandBar.
        /// </summary>
        ///
        /// <param name="application">
        /// Excel Application object.
        /// </param>
        ///
        /// <remarks>
        /// The context menu that appears when you right-click a table (ListObject)
        /// cell.
        /// </remarks>
        //*************************************************************************

        public static Microsoft.Office.Core.CommandBar
        GetTableContextCommandBar
        (
            Microsoft.Office.Interop.Excel.Application application
        )
        {
            Debug.Assert(application != null);

            return (application.CommandBars[TableContextCommandBarID]);
        }

        //*************************************************************************
        //  Method: TryAddOrInsertTableColumn()
        //
        /// <summary>
        /// Attempts to add or insert a column into a table.
        /// </summary>
        ///
        /// <param name="table">
        /// Table to add or insert a column into.
        /// </param>
        ///
        /// <param name="columnName">
        /// Name of the column to add or insert.
        /// </param>
        ///
        /// <param name="oneBasedColumnIndex">
        /// One-based index of the column after it is inserted, or -1 to add the
        /// column to the end of the table.
        /// </param>
        ///
        /// <param name="columnWidthChars">
        /// Width of the new column, in characters, or <see
        /// cref="AutoColumnWidth" /> to set the width automatically.
        /// </param>
        ///
        /// <param name="columnStyle">
        /// Style of the new column, or null to apply Excel's normal style.
        /// Sample: "Bad".
        /// </param>
        ///
        /// <param name="listColumn">
        /// Where the new column gets stored if true is returned.
        /// </param>
        ///
        /// <returns>
        /// true if the column was added or inserted.
        /// </returns>
        //*************************************************************************

        public static Boolean
        TryAddOrInsertTableColumn
        (
            ListObject table,
            String columnName,
            Int32 oneBasedColumnIndex,
            Double columnWidthChars,
            String columnStyle,
            out ListColumn listColumn
        )
        {
            Debug.Assert(table != null);
            Debug.Assert(!String.IsNullOrEmpty(columnName));
            Debug.Assert(oneBasedColumnIndex == -1 || oneBasedColumnIndex >= 1);

            Debug.Assert(columnWidthChars == AutoColumnWidth ||
                columnWidthChars >= 0);

            listColumn = null;

            Object oPosition;
            ListColumns oColumns = table.ListColumns;
            Int32 iColumns = oColumns.Count;
            Double[] adColumnWidthChars = null;

            if (oneBasedColumnIndex == -1)
            {
                oPosition = Missing.Value;
            }
            else
            {
                oPosition = oneBasedColumnIndex;

                // When inserting a column, Excel messes up the widths of the
                // columns after the insertion point.  Save the widths of those
                // columns.

                if (oneBasedColumnIndex <= iColumns)
                {
                    adColumnWidthChars =
                        new Double[iColumns - oneBasedColumnIndex + 1];

                    for (Int32 iOneBasedIndex = oneBasedColumnIndex;
                        iOneBasedIndex <= iColumns; iOneBasedIndex++)
                    {
                        adColumnWidthChars[iOneBasedIndex - oneBasedColumnIndex] =
                            (Double)oColumns[iOneBasedIndex].Range.ColumnWidth;
                    }
                }
            }

            try
            {
                listColumn = oColumns.Add(oPosition);
            }
            catch (COMException oCOMException)
            {
                if (oCOMException.ErrorCode == -2146827284)
                {
                    // This can happen, for example, if adding a table column
                    // would cause a merged cells to unmerge, the user is asked
                    // if he wants to allow the unmerge, and he says no.

                    return (false);
                }

                throw;
            }

            // Set various properties on the new column.

            listColumn.Name = columnName;

            Range oColumnRange = listColumn.Range;

            if (columnWidthChars == AutoColumnWidth)
            {
                oColumnRange.EntireColumn.AutoFit();
            }
            else
            {
                oColumnRange.ColumnWidth = columnWidthChars;
            }

            oColumnRange.Validation.Delete();

            SetRangeStyle(oColumnRange, columnStyle);

            if (adColumnWidthChars != null)
            {
                // Restore the widths of the columns after the insertion point.

                for (Int32 iOneBasedIndex = oneBasedColumnIndex;
                    iOneBasedIndex <= iColumns; iOneBasedIndex++)
                {
                    oColumns[iOneBasedIndex + 1].Range.ColumnWidth =
                        adColumnWidthChars[iOneBasedIndex - oneBasedColumnIndex];
                }
            }

            return (true);
        }

        //*************************************************************************
        //  Method: TryEvaluateDoubleFunction()
        //
        /// <summary>
        /// Attempts to evaluate an Excel function that should return a Double.
        /// </summary>
        ///
        /// <param name="application">
        /// Excel Application object.
        /// </param>
        ///
        /// <param name="functionCall">
        /// Function call text.  Sample: "=SUM(A:A)".
        /// </param>
        ///
        /// <param name="functionReturn">
        /// Where the Double returned by the function gets stored if true is
        /// returned.
        /// </param>
        ///
        /// <returns>
        /// true if the returned a Double, false if it failed and returned an
        /// Int32 error code.
        /// </returns>
        //*************************************************************************

        public static Boolean
        TryEvaluateDoubleFunction
        (
            Application application,
            String functionCall,
            out Double functionReturn
        )
        {
            Debug.Assert(application != null);
            Debug.Assert(!String.IsNullOrEmpty(functionCall));

            functionReturn = Double.MinValue;

            Object oFunctionReturn = application.Evaluate(functionCall);

            // If the function failed, it returned an Int32 error code.  See this
            // post for details on how .NET deals with CVErr values:
            //
            // Dealing with CVErr Values in .NET - Part I: The Problem
            //
            // http://xldennis.wordpress.com/2006/11/22/dealing-with-cverr-values-
            // in-net-%E2%80%93-part-i-the-problem/

            if (oFunctionReturn is Double)
            {
                functionReturn = (Double)oFunctionReturn;
                return (true);
            }

            return (false);
        }

        //*************************************************************************
        //  Method: GetSingleElement2DArray()
        //
        /// <summary>
        /// Gets a two-dimensional Object array with one row and one column.
        /// </summary>
        ///
        /// <returns>
        /// A two-dimensional Object array with one row and one column.  Each
        /// dimension is one-based.  The single element in the array is initialized
        /// to null.
        /// </returns>
        //*************************************************************************

        public static Object[,]
        GetSingleElement2DArray()
        {
            return (GetSingleColumn2DArray(1));
        }

        //*************************************************************************
        //  Method: GetSingleColumn2DArray()
        //
        /// <summary>
        /// Gets a two-dimensional Object array with N rows and one column.
        /// </summary>
        ///
        /// <param name="rows">
        /// Number of rows to include in the array.  Must be greater than or equal
        /// to zero.
        /// </param>
        ///
        /// <returns>
        /// A two-dimensional Object array with N rows and one column.  Each
        /// dimension is one-based.  The elements in the array are initialized to
        /// null.
        /// </returns>
        //*************************************************************************

        public static Object[,]
        GetSingleColumn2DArray
        (
            Int32 rows
        )
        {
            Debug.Assert(rows >= 0);

            Object[,] aoSingleColumn2DArray = (Object[,])Array.CreateInstance(
                typeof(Object), new Int32[] { rows, 1 }, new Int32[] { 1, 1 });

            return (aoSingleColumn2DArray);
        }

        //*************************************************************************
        //  Method: GetSingleRow2DArray()
        //
        /// <summary>
        /// Gets a two-dimensional Object array with 1 row and N columns.
        /// </summary>
        ///
        /// <param name="columns">
        /// Number of columns to include in the array.  Must be greater than or
        /// equal to zero.
        /// </param>
        ///
        /// <returns>
        /// A two-dimensional Object array with one row and N columns.  Each
        /// dimension is one-based.  The elements in the array are initialized to
        /// null.
        /// </returns>
        //*************************************************************************

        public static Object[,]
        GetSingleRow2DArray
        (
            Int32 columns
        )
        {
            Debug.Assert(columns >= 0);

            Object[,] aoSingleColumn2DArray = (Object[,])Array.CreateInstance(
                typeof(Object), new Int32[] { 1, columns }, new Int32[] { 1, 1 });

            return (aoSingleColumn2DArray);
        }

        //*************************************************************************
        //  Method: GetSingleColumn2DStringArray()
        //
        /// <summary>
        /// Gets a two-dimensional String array with N rows and one column.
        /// </summary>
        ///
        /// <param name="rows">
        /// Number of rows to include in the array.  Must be greater than or equal
        /// to zero.
        /// </param>
        ///
        /// <returns>
        /// A two-dimensional String array with N rows and one column.  Each
        /// dimension is one-based.  The elements in the array are initialized to
        /// null.
        /// </returns>
        //*************************************************************************

        public static String[,]
        GetSingleColumn2DStringArray
        (
            Int32 rows
        )
        {
            Debug.Assert(rows >= 0);

            String[,] asSingleColumn2DArray = (String[,])Array.CreateInstance(
                typeof(String), new Int32[] { rows, 1 }, new Int32[] { 1, 1 });

            return (asSingleColumn2DArray);
        }

        //*************************************************************************
        //  Method: GetSingleRow2DStringArray()
        //
        /// <summary>
        /// Gets a two-dimensional String array with 1 row and N columns.
        /// </summary>
        ///
        /// <param name="columns">
        /// Number of columns to include in the array.  Must be greater than or
        /// equal to zero.
        /// </param>
        ///
        /// <returns>
        /// A two-dimensional String array with one row and N columns.  Each
        /// dimension is one-based.  The elements in the array are initialized to
        /// null.
        /// </returns>
        //*************************************************************************

        public static String[,]
        GetSingleRow2DStringArray
        (
            Int32 columns
        )
        {
            Debug.Assert(columns >= 0);

            String[,] asSingleRow2DArray = (String[,])Array.CreateInstance(
                typeof(String), new Int32[] { 1, columns }, new Int32[] { 1, 1 });

            return (asSingleRow2DArray);
        }

        //*************************************************************************
        //  Method: TryGetContiguousValuesInRowOrColumn()
        //
        /// <summary>
        /// Attempts to get the contiguous values in a row or column starting at a
        /// specified cell.
        /// </summary>
        ///
        /// <param name="worksheet">
        /// Worksheet that contains the values to get.
        /// </param>
        ///
        /// <param name="rowOneBased">
        /// One-based row number of the first cell to get.
        /// </param>
        ///
        /// <param name="columnOneBased">
        /// One-based column number of the first cell to get.
        /// </param>
        ///
        /// <param name="inRow">
        /// true to get the contiguous values in the row, false to get the
        /// contiguous values in the column.
        /// </param>
        ///
        /// <param name="values">
        /// Where the contiguous cell values get stored if true is returned.
        /// </param>
        ///
        /// <returns>
        /// true if the cell at [rowOneBased, columnOneBased] is not empty.
        /// </returns>
        ///
        /// <remarks>
        /// If the [rowOneBased, columnOneBased] cell is not empty, the values
        /// from that cell and all contiguous cells to the right or below get
        /// stored at <paramref name="values" /> and true is returned.  false is
        /// returned otherwise.
        ///
        /// <para>
        /// Note that any of the contiguous cells can contain strings consisting of
        /// nothing but spaces.
        /// </para>
        ///
        /// </remarks>
        //*************************************************************************

        private static Boolean
        TryGetContiguousValuesInRowOrColumn
        (
            Microsoft.Office.Interop.Excel.Worksheet worksheet,
            Int32 rowOneBased,
            Int32 columnOneBased,
            Boolean inRow,
            out Object[,] values
        )
        {
            Debug.Assert(worksheet != null);
            Debug.Assert(rowOneBased >= 1);
            Debug.Assert(columnOneBased >= 1);

            values = null;

            Range oFirstCell = (Range)worksheet.Cells[rowOneBased, columnOneBased];

            String sTemp;

            if (!ExcelUtil.TryGetNonEmptyStringFromCell(oFirstCell, out sTemp))
            {
                return (false);
            }

            Range oNextCell = (Range)worksheet.Cells[
                rowOneBased + (inRow ? 0 : 1),
                columnOneBased + (inRow ? 1 : 0)
                ];

            Range oLastCell;

            // If the next cell is empty, Range.get_End() can't be used because it
            // jumps beyond the empty cell, possibly to the end of the worksheet.

            if (!ExcelUtil.TryGetNonEmptyStringFromCell(oNextCell, out sTemp))
            {
                oLastCell = oFirstCell;
            }
            else
            {
                oLastCell = oFirstCell.get_End(
                    inRow ? XlDirection.xlToRight : XlDirection.xlDown);
            }

            values = ExcelUtil.GetRangeValues(
                worksheet.get_Range(oFirstCell, oLastCell));

            return (true);
        }


        //*************************************************************************
        //  Private members
        //*************************************************************************

        /// true if a call to Range.SpecialCells() is in progress.

        private static Boolean m_bSpecialCellsBeingCalled = false;


        //*************************************************************************
        //  Public constants
        //*************************************************************************

        /// <summary>
        /// ID of the command bar that appears when you right-click a table
        /// (ListObject) cell.  The English name of this command bar is
        /// "List Range Popup".
        /// </summary>

        public static readonly Int32 TableContextCommandBarID = 72;

        /// <summary>
        /// For methods that take a column width parameter, specify this constant
        /// to automatically set the column width.
        /// </summary>

        public static readonly Double AutoColumnWidth = -1;
    }

}
