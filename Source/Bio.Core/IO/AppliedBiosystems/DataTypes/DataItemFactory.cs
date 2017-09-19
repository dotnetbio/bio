using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Bio.IO.AppliedBiosystems.DataTypes
{
    /// <summary>
    /// Create data items basd on abi directory entry element type.
    /// </summary>
    public static class DataItemFactory
    {
        /// <summary>
        /// Supported data items.
        /// </summary>
        private static ReadOnlyCollection<IAb1DataItem> SupportedItems = new ReadOnlyCollection<IAb1DataItem>
                                                              (new List<IAb1DataItem>{
                                                                  new BoolDataItem(),
                                                                  new ByteDataItem(),
                                                                  new CharDataItem(),
                                                                  new DateDataItem(),
                                                                  new DoubleDataItem(),
                                                                  new FloatDataItem(),
                                                                  new LongDataItem(),
                                                                  new PStringDataItem(),
                                                                  new CStringDataItem(),
                                                                  new ShortDataItem(),
                                                                  new ThumbDataItem(),
                                                                  new TimeDataItem(),
                                                                  new UserDataItem(),
                                                                  new WordDataItem()
                                                              });

        /// <summary>
        /// Attempts to create a data item.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static IAb1DataItem TryCreateDataItem(Ab1DirectoryEntry entry)
        {
            IAb1DataItem item = SupportedItems.FirstOrDefault(dataItem => dataItem.Type == entry.ElementTypeCode);
            if (item == null)
                return null;

            item = item.Create();
            item.Entry = entry;
            return item;
        }
    }
}
