using System.Text;

using Bio.Util.Logging;

using NUnit.Framework;

namespace Bio.Tests
{
    /// <summary>
    /// Test Automation code for Bio IndexedItem BVT level validations.
    /// </summary>
    [TestFixture]
    public class IndexedItemBvtTestCases
    {
        #region IndexedItem Bvt TestCases

        /// <summary>
        /// Validate a CompareTo() method in IndexedItem.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateIndexedItemCompareTo()
        {
            IndexedItem<byte> indexedObj = new IndexedItem<byte>(0, Encoding.ASCII.GetBytes("A")[0]);
            IndexedItem<byte> indexedObj2 = new IndexedItem<byte>(0, Encoding.ASCII.GetBytes("A")[0]);
            IndexedItem<byte> indexedObj3 = new IndexedItem<byte>(0, Encoding.ASCII.GetBytes("A")[0]);
            Assert.AreEqual(0, indexedObj.CompareTo(indexedObj2));
            Assert.AreEqual(0, indexedObj.CompareTo((object)indexedObj3));
            Assert.AreEqual(1, indexedObj.CompareTo(null));

            ApplicationLog.WriteLine(
                "IndexedItem BVT: Successfully validated the CompareTo() method.");
        }

        /// <summary>
        /// Validate a Equals() method in IndexedItem.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateIndexedItemEquals()
        {
            IndexedItem<byte> indexedObj = new IndexedItem<byte>(0, Encoding.ASCII.GetBytes("A")[0]);
            IndexedItem<byte> indexedObj2 = new IndexedItem<byte>(0, Encoding.ASCII.GetBytes("A")[0]);
            IndexedItem<byte> indexedObj3 = new IndexedItem<byte>(0, Encoding.ASCII.GetBytes("A")[0]);
            Assert.IsTrue(indexedObj.Equals(indexedObj2));
            Assert.IsTrue(indexedObj.Equals((object)indexedObj3));

            ApplicationLog.WriteLine(
                "IndexedItem BVT: Successfully validated the Equals() method.");
        }

        /// <summary>
        /// Validate the operators in IndexedItem.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateIndexedItemOperators()
        {
            IndexedItem<byte> indexedObj = new IndexedItem<byte>(0, Encoding.ASCII.GetBytes("A")[0]);
            IndexedItem<byte> indexedObj1 = new IndexedItem<byte>(0, Encoding.ASCII.GetBytes("A")[0]);
            IndexedItem<byte> indexedObj2 = new IndexedItem<byte>(0, Encoding.ASCII.GetBytes("G")[0]);

            Assert.IsTrue(indexedObj == indexedObj1);
            Assert.IsFalse(indexedObj == indexedObj2);
            Assert.IsFalse(indexedObj < indexedObj2);
            Assert.IsTrue(indexedObj <= indexedObj2);
            Assert.IsFalse(indexedObj2 > indexedObj);
            Assert.IsTrue(indexedObj2 >= indexedObj);
            Assert.IsTrue(indexedObj1 != indexedObj2);

            ApplicationLog.WriteLine(
                "IndexedItem BVT: Successfully validated all the properties.");
        }

        #endregion IndexedItem Bvt TestCases
    }
}

