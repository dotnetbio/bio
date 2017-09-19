using System;
using Bio.IO.AppliedBiosystems.DataParsers;

namespace Bio.IO.AppliedBiosystems.DataTypes
{
    /// <summary>
    /// This item contains custom users specific information, at the moment the parser ignores this.  Once a use cases arises that
    /// requires this functionality it may be implemented.  This is unlikely as this is a legacy data type.
    /// </summary>
    public class UserDataItem : DataItem
    {
        /// <summary>
        /// Item value.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// The name of the data item.
        /// </summary>
        public override string Name
        {
            get { return "user"; }
        }

        /// <summary>
        /// Element type definition.
        /// </summary>
        public override int Type
        {
            get { return 1024; }
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
        /// Create an instance of the user type.
        /// </summary>
        /// <returns></returns>
        public override IAb1DataItem Create()
        {
            return new UserDataItem();
        }
    }
}
