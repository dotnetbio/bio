using System;
using Bio.IO.AppliedBiosystems.DataParsers;

namespace Bio.IO.AppliedBiosystems.DataTypes
{
    /// <summary>
    /// String data item.
    /// </summary>
   public class PStringDataItem : DataItem
   {
       /// <summary>
       /// Item value.
       /// </summary>
      public string Value { get; set; }

      #region IAb1DataItem Members

        /// <summary>
        /// The name of the data item.
        /// </summary>
        public override string Name
      {
         get { return "pString"; }
      }

        /// <summary>
        /// Element type definition.
        /// </summary>
        public override int Type
      {
         get { return 18; }
      }

        /// <summary>
        /// Size in bytes of the element.
        /// </summary>
        public override int Size
      {
         get { return 1; }
      }

        /// <summary>
        /// Accept a visitor.
        /// </summary>
        /// <param name="visitor"></param>
        public override void Accept(IAb1DataVisitor visitor)
        {
            if (visitor == null) throw new ArgumentNullException("visitor");
            visitor.Visit(this);
        }

        /// <summary>
        /// Create an instance of the pstring type.
        /// </summary>
        /// <returns></returns>
        public override IAb1DataItem Create()
      {
         return new PStringDataItem();
      }

      #endregion
   }
}
