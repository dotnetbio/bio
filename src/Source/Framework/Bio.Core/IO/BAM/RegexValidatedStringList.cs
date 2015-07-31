using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bio.Util;
using System.Text.RegularExpressions;

namespace Bio.IO.BAM
{
    /// <summary>
    /// A list of strings where each item in the list has been validated to meet the conditions of a particular 
    /// regular expression, used to verify that any item retrieved from this list follows the condition given.
    /// </summary>
    public class RegexValidatedStringList : IList<string>
    {
        /// <summary>
        /// Private list of validated items
        /// </summary>
        private List<string> items;
        /// <summary>
        /// Regular expression used to validate items
        /// </summary>
        private Regex regex;
        /// <summary>
        /// Validate that a string meets the criteria required to be added to the list
        /// </summary>
        /// <param name="toAdd"></param>
        /// <returns></returns>
        public bool ValidateItem(string toAdd)
        {
            return Helper.IsValidRegexValue(regex, toAdd);
            
        }
        /// <summary>
        /// Internal method that throws an error if the item is unacceptable, this error should never be thrown and users of the
        /// class can avoid this by calling ValidateItem before attempting to add the string to the list.
        /// </summary>
        /// <param name="toAdd"></param>
        private void throwErrorIfUnacceptableItem(string toAdd)
        {
            bool ok = ValidateItem(toAdd);
            if (!ok)
            {
                throw new ArgumentException("Tried to add: " + toAdd + " to a list of items where this was an acceptable value");
            }
            
        }

        /// <summary>
        /// Initialize a list with a regular expression that all items must conform to.
        /// </summary>
        /// <param name="validationRegEx"></param>
        public RegexValidatedStringList(string validationRegEx)
        {
            regex = new Regex(validationRegEx);
            items = new List<string>();
        }
        /// <summary>
        /// Initialize a list with a regular expression that all items must conform to.
        /// </summary>
        /// <param name="validationRegEx"></param>
        public RegexValidatedStringList(Regex validationRegEx)
        {
            regex = validationRegEx;
            items = new List<string>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(string item)
        {
            return items.IndexOf(item);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, string item)
        {
            throwErrorIfUnacceptableItem(item);
            items.Insert(index, item);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            items.RemoveAt(index);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string this[int index]
        {
            get
            {
                return items[index];
            }
            set
            {
                throwErrorIfUnacceptableItem(value);
                items[index] = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(string item)
        {
            throwErrorIfUnacceptableItem(item);
            items.Add(item);
        }
        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            items.Clear();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(string item)
        {
            return items.Contains(item);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(string[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }
        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return items.Count; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(string item)
        {
            return items.Remove(item);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<string> GetEnumerator()
        {
            return items.GetEnumerator();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }
    }
}
