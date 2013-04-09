/****************************************************************************
 * IndexedItemBvtTestCases.cs
 * 
 * This file contains the IndexedItem BVT test cases.
 * 
******************************************************************************/

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Bio;
using Bio.IO;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.IO.FastA;

namespace Bio.TestAutomation
{
    /// <summary>
    /// Test Automation code for Bio IndexedItem BVT level validations.
    /// </summary>
    [TestClass]
    public class IndexedItemBvtTestCases
    {

        #region Global Variables

        ASCIIEncoding encodingObj = new ASCIIEncoding();

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static IndexedItemBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region IndexedItem Bvt TestCases

        /// <summary>
        /// Validate a GetHashCode() method in IndexedItem.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        [Ignore]
        public void ValidateIndexedItemGetHashCode()
        {
            // Another test which fails when we version .NET; removing.
            IndexedItem<byte> indexedObj = new IndexedItem<byte>(0, encodingObj.GetBytes("A")[0]);
            Assert.AreEqual(372029391, indexedObj.GetHashCode());
            ApplicationLog.WriteLine("IndexedItem BVT: Successfully validated the Hashcode.");
        }

        /// <summary>
        /// Validate a CompareTo() method in IndexedItem.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateIndexedItemCompareTo()
        {
            IndexedItem<byte> indexedObj = new IndexedItem<byte>(0, encodingObj.GetBytes("A")[0]);
            IndexedItem<byte> indexedObj2 = new IndexedItem<byte>(0, encodingObj.GetBytes("A")[0]);
            IndexedItem<byte> indexedObj3 = new IndexedItem<byte>(0, encodingObj.GetBytes("A")[0]);
            Assert.AreEqual(0, indexedObj.CompareTo(indexedObj2));
            Assert.AreEqual(0, indexedObj.CompareTo((object)indexedObj3));
            Assert.AreEqual(1, indexedObj.CompareTo(null));

            ApplicationLog.WriteLine(
                "IndexedItem BVT: Successfully validated the CompareTo() method.");
            Console.WriteLine(
                "IndexedItem BVT: Successfully validated the CompareTo() method.");
        }

        /// <summary>
        /// Validate a Equals() method in IndexedItem.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateIndexedItemEquals()
        {
            IndexedItem<byte> indexedObj = new IndexedItem<byte>(0, encodingObj.GetBytes("A")[0]);
            IndexedItem<byte> indexedObj2 = new IndexedItem<byte>(0, encodingObj.GetBytes("A")[0]);
            IndexedItem<byte> indexedObj3 = new IndexedItem<byte>(0, encodingObj.GetBytes("A")[0]);
            Assert.IsTrue(indexedObj.Equals(indexedObj2));
            Assert.IsTrue(indexedObj.Equals((object)indexedObj3));

            ApplicationLog.WriteLine(
                "IndexedItem BVT: Successfully validated the Equals() method.");
            Console.WriteLine(
                "IndexedItem BVT: Successfully validated the Equals() method.");
        }

        /// <summary>
        /// Validate the operators in IndexedItem.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateIndexedItemOperators()
        {
            IndexedItem<byte> indexedObj = new IndexedItem<byte>(0, encodingObj.GetBytes("A")[0]);
            IndexedItem<byte> indexedObj1 = new IndexedItem<byte>(0, encodingObj.GetBytes("A")[0]);
            IndexedItem<byte> indexedObj2 = new IndexedItem<byte>(0, encodingObj.GetBytes("G")[0]);

            Assert.IsTrue(indexedObj == indexedObj1);

            Assert.IsFalse(indexedObj == indexedObj2);

            Assert.IsFalse(indexedObj < indexedObj2);
            Assert.IsTrue(indexedObj <= indexedObj2);

            Assert.IsFalse(indexedObj2 > indexedObj);

            Assert.IsTrue(indexedObj2 >= indexedObj);

            Assert.IsTrue(indexedObj1 != indexedObj2);

            ApplicationLog.WriteLine(
                "IndexedItem BVT: Successfully validated all the properties.");
            Console.WriteLine(
                "IndexedItem BVT: Successfully validated all the properties.");
        }

        #endregion IndexedItem Bvt TestCases
    }
}

