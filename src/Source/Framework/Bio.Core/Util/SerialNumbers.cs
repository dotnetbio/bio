using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Globalization;

namespace Bio.Util
{
    /// <summary>
    /// Assigns serial number of objects
    /// </summary>
    public class SerialNumbers<T>
    {
        /// <summary>
        /// Create a SerialNumbers object for assigning serial numbers to values.
        /// </summary>
        public SerialNumbers()
        {
            ItemToSerialNumber = new Dictionary<T, int>();
            ItemList = new List<T>();
        }

        /// <summary>
        /// Create a new SerialNumbers object for assign serial numbers to values and assign serial numbers to the values in the sequence.
        /// </summary>
        /// <param name="sequence"></param>
        public SerialNumbers(IEnumerable<T> sequence)
            : this()
        {
            foreach (T item in sequence)
            {
                GetNewOrOld(item);
            }
        }

        ///!!!should this be restricted so that it cannot be changed without using the methods of the class?
        /// <summary>
        /// A mapping from items to serial numbers
        /// </summary>
        public Dictionary<T, int> ItemToSerialNumber;

        ///!!!should this be restricted so that it cannot be changed without using the methods of the class?
        /// <summary>
        /// A list of the items in order.
        /// </summary>
        public List<T> ItemList;

        /// <summary>
        /// Returns the serial number of an item. If the item has already been assigned a serial number, returns that number; 
        /// otherwise, assigns a new number to the item and returns that new number.
        /// </summary>
        /// <param name="item">The item</param>
        /// <returns>The item's serial number</returns>
        public int GetNewOrOld(T item)
        {
            if (!ItemToSerialNumber.ContainsKey(item))
            {
                Debug.Assert(ItemToSerialNumber.Count == ItemList.Count); // real assert
                int serialNumber = ItemToSerialNumber.Count;
                Debug.Assert(serialNumber == ItemList.Count); // real assert
                ItemToSerialNumber.Add(item, serialNumber);
                ItemList.Add(item);
                return serialNumber;
            }
            else
            {
                int serialNumber = ItemToSerialNumber[item];
                return serialNumber;
            }
        }

        /// <summary>
        /// Assigns a serial number to a new item. Raises an exception of the item is not new.
        /// </summary>
        /// <param name="item">The item</param>
        /// <returns>The items serial number</returns>
        public int GetNew(T item)
        {
            Helper.CheckCondition(!ItemToSerialNumber.ContainsKey(item), () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.ExpectedItemToNotExist, item));
            Debug.Assert(ItemToSerialNumber.Count == ItemList.Count); // real assert
            int serialNumber = ItemToSerialNumber.Count;
            Debug.Assert(serialNumber == ItemList.Count); // real assert
            ItemToSerialNumber.Add(item, serialNumber);
            ItemList.Add(item);
            return serialNumber;
        }


        /// <summary>
        /// Finds the serial number of item to which a serial number has already been assigned. Raises an exception of the item is new.
        /// </summary>
        /// <param name="item">The item</param>
        /// <returns>The serial number of that item.</returns>
        public int GetOld(T item)
        {
            Helper.CheckCondition(ItemToSerialNumber.ContainsKey(item), () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.ExpectedItemToExist, item));
            return ItemToSerialNumber[item];
        }


        /// <summary>
        /// The last serial number assigned.
        /// </summary>
        public int Last
        {
            get
            {
                return ItemList.Count - 1;
            }
        }

        /// <summary>
        /// Finds the serial number of item to which a serial number has already been assigned.
        /// </summary>
        /// <param name="item">The item</param>
        /// <param name="serialNumber">The serial number that was assigned to that item.</param>
        /// <returns>true if the item has previously been assigned a serial number; otherwise, false.</returns>
        public bool TryGetOld(T item, out int serialNumber)
        {
            return ItemToSerialNumber.TryGetValue(item, out serialNumber);
        }


        /// <summary>
        /// Tells if an item already has a serial number
        /// </summary>
        /// <param name="item">the item</param>
        /// <returns>true if the item has previously been assigned a serial number; otherwise, false.</returns>
        public bool Contains(T item)
        {
            return ItemToSerialNumber.ContainsKey(item);
        }

        /// <summary>
        /// The number of items to which serial numbers have been assigned. This is always one more than the largest serial number.
        /// </summary>
        public int Count
        {
            get
            {
                return ItemToSerialNumber.Count;
            }
        }

        /// <summary>
        /// Given a serialNumber, return the item with that serial number
        /// </summary>
        /// <param name="serialNumber">The serial number of interest</param>
        /// <returns>The item with that serial number</returns>
        public T GetItem(int serialNumber)
        {
            return ItemList[serialNumber];
        }

        /// <summary>
        /// Write the items in order to a TextWriter
        /// </summary>
        public void Save(TextWriter writer)
        {
            WriteEachLine(ItemList, writer);
        }

        private static void WriteEachLine<T1>(IEnumerable<T1> list, TextWriter writer)
        {
            foreach (T1 t in list)
            {
                writer.WriteLine(t.ToString());
            }
        }
    }
}
