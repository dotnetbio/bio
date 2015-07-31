using System;

namespace Microsoft.Research.CommunityTechnologies.AppLib
{
    //*****************************************************************************
    //  Enum: ExcelColumnFormat
    //
    /// <summary>
    /// Specifies the format of an Excel table column.
    /// </summary>
    //*****************************************************************************

    public enum ExcelColumnFormat
    {
        /// <summary>
        /// The column contains numbers.
        /// </summary>

        Number,

        /// <summary>
        /// The column contains dates.  Sample: 1/1/2008.
        /// </summary>

        Date,

        /// <summary>
        /// The column contains times.  Sample: 3:40 PM.
        /// </summary>

        Time,

        /// <summary>
        /// The column contains date/times.  Sample: 1/1/2008 3:40 pm.
        /// </summary>

        DateAndTime,

        /// <summary>
        /// The column contains something else.
        /// </summary>

        Other,
    }
}
