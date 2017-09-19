using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Bio.Tests.Framework
{
    /// <summary>
    ///  Tests AATree class.
    /// </summary>
    [TestFixture]
    public class AATreeTests
    {
        /// <summary>
        /// Test AATree by adding values order by ascending order.
        /// </summary>
        [Test]
        public void TestByAddingValuesSortedByAscOrder()
        {
            AATree<int> aatree = new AATree<int>();
            int size = 100;
            List<int> list = new List<int>(size);
            for (int i = 0; i < size; i++)
            {
                aatree.Add(i);
                list.Add(i);
            }

            Assert.AreEqual(size, aatree.Count);
            IComparer<int> comparer = Comparer<int>.Default;
            Assert.IsTrue(ValidateInOrder(aatree, list, comparer));

            Assert.IsTrue(aatree.Contains(50));
        }

        /// <summary>
        /// Test AATree by adding values sorder by descending order.
        /// </summary>
        [Test]
        public void TestByAddingValuesSortedByDescOrder()
        {
            AATree<int> aatree = new AATree<int>();
            int size = 100;
            List<int> list = new List<int>(size);
            for (int i = size - 1; i >= 0; i--)
            {
                aatree.Add(i);
                list.Add(i);
            }

            list.Sort();

            Assert.AreEqual(size, aatree.Count);
            IComparer<int> comparer = Comparer<int>.Default;
            Assert.IsTrue(ValidateInOrder(aatree, list, comparer));

            Assert.IsTrue(aatree.Contains(50));
        }

        /// <summary>
        /// Test AATree by adding random values.
        /// </summary>
        [Test]
        public void TestByAddingRandomValues()
        {
            Random rnd = new Random();

            AATree<int> aatree = new AATree<int>();
            int size = 100;
            List<int> list = new List<int>(size);
            while (list.Count < size)
            {
                int val = rnd.Next();
                if (aatree.Add(val))
                {
                    list.Add(val);
                }
            }

            list.Sort();

            Assert.AreEqual(size, aatree.Count);

            IComparer<int> comparer = Comparer<int>.Default;
            Assert.IsTrue(ValidateInOrder(aatree, list, comparer));

            Assert.IsTrue(aatree.Contains(list[50]));

        }

        /// <summary>
        /// Test Remove method.
        /// </summary>
        [Test]
        public void TestRemoveMethodWithValuesSortByAscOrder()
        {
            IComparer<int> comparer = Comparer<int>.Default;
            AATree<int> aatree = new AATree<int>();
            int size = 10000;
            List<int> list = new List<int>(size);
            for (int i = 0; i < size; i++)
            {
                int val = i;
                if (aatree.Add(val))
                {
                    list.Add(val);
                }
            }

            list.Sort();

            Assert.AreEqual(size, aatree.Count);
            Assert.IsTrue(ValidateInOrder(aatree, list, comparer));



            int index = 10;
            Assert.IsTrue(aatree.Remove(list[index]));
            list.RemoveAt(index);
            Assert.IsTrue(ValidateInOrder(aatree, list, comparer));

            index = 20;
            Assert.IsTrue(aatree.Remove(list[index]));
            list.RemoveAt(index);
            Assert.IsTrue(ValidateInOrder(aatree, list, comparer));

            index = 30;
            Assert.IsTrue(aatree.Remove(list[index]));
            list.RemoveAt(index);
            Assert.IsTrue(ValidateInOrder(aatree, list, comparer));

            index = 40;
            Assert.IsTrue(aatree.Remove(list[index]));
            list.RemoveAt(index);
            Assert.IsTrue(ValidateInOrder(aatree, list, comparer));

            index = 50;
            Assert.IsTrue(aatree.Remove(list[index]));
            list.RemoveAt(index);
            Assert.IsTrue(ValidateInOrder(aatree, list, comparer));

            index = list.Count - 1;
            Assert.IsTrue(aatree.Remove(list[index]));
            list.RemoveAt(index);
            Assert.IsTrue(ValidateInOrder(aatree, list, comparer));

            index = 0;
            Assert.IsTrue(aatree.Remove(list[index]));
            list.RemoveAt(index);
            Assert.IsTrue(ValidateInOrder(aatree, list, comparer));

            Assert.AreEqual(list.Count, aatree.Count);
        }
        /// <summary>
        /// Test Remove method.
        /// </summary>
        [Test]
        public void TestRemoveMethodWithValuesSortByDescOrder()
        {
            IComparer<int> comparer = Comparer<int>.Default;
            AATree<int> aatree = new AATree<int>();
            int size = 10000;
            List<int> list = new List<int>(size);
            for (int i = size - 1; i >= 0; i--)
            {
                int val = i;
                if (aatree.Add(val))
                {
                    list.Add(val);
                }
            }

            list.Sort();

            Assert.AreEqual(size, aatree.Count);
            Assert.IsTrue(ValidateInOrder(aatree, list, comparer));

            int index = 10;
            Assert.IsTrue(aatree.Remove(list[index]));
            list.RemoveAt(index);
            Assert.IsTrue(ValidateInOrder(aatree, list, comparer));

            index = 20;
            Assert.IsTrue(aatree.Remove(list[index]));
            list.RemoveAt(index);
            Assert.IsTrue(ValidateInOrder(aatree, list, comparer));

            index = 30;
            Assert.IsTrue(aatree.Remove(list[index]));
            list.RemoveAt(index);
            Assert.IsTrue(ValidateInOrder(aatree, list, comparer));

            index = 40;
            Assert.IsTrue(aatree.Remove(list[index]));
            list.RemoveAt(index);
            Assert.IsTrue(ValidateInOrder(aatree, list, comparer));

            index = 50;
            Assert.IsTrue(aatree.Remove(list[index]));
            list.RemoveAt(index);
            Assert.IsTrue(ValidateInOrder(aatree, list, comparer));

            index = list.Count - 1;
            Assert.IsTrue(aatree.Remove(list[index]));
            list.RemoveAt(index);
            Assert.IsTrue(ValidateInOrder(aatree, list, comparer));

            index = 0;
            Assert.IsTrue(aatree.Remove(list[index]));
            list.RemoveAt(index);
            Assert.IsTrue(ValidateInOrder(aatree, list, comparer));

            Assert.AreEqual(list.Count, aatree.Count);
        }
        /// <summary>
        /// Test Remove method.
        /// </summary>
        [Test]
        public void TestRemoveMethodWithRandomValues()
        {
            Random rnd = new Random();
            IComparer<int> comparer = Comparer<int>.Default;
            AATree<int> aatree = new AATree<int>();
            int size = 1000;
            List<int> list = new List<int>(size);
            while (list.Count < size)
            {
                int val = rnd.Next();
                if (aatree.Add(val))
                {
                    list.Add(val);
                }
            }

            list.Sort();

            Assert.AreEqual(size, aatree.Count);
            Assert.IsTrue(ValidateInOrder(aatree, list, comparer));

            int index = 10;
            Assert.IsTrue(aatree.Remove(list[index]));
            list.RemoveAt(index);
            Assert.IsTrue(ValidateInOrder(aatree, list, comparer));
            index = 20;
            Assert.IsTrue(aatree.Remove(list[index]));
            list.RemoveAt(index);
            Assert.IsTrue(ValidateInOrder(aatree, list, comparer));

            index = 30;
            Assert.IsTrue(aatree.Remove(list[index]));
            list.RemoveAt(index);
            Assert.IsTrue(ValidateInOrder(aatree, list, comparer));

            index = 40;
            Assert.IsTrue(aatree.Remove(list[index]));
            list.RemoveAt(index);
            Assert.IsTrue(ValidateInOrder(aatree, list, comparer));

            index = 50;
            Assert.IsTrue(aatree.Remove(list[index]));
            list.RemoveAt(index);
            Assert.IsTrue(ValidateInOrder(aatree, list, comparer));

            index = list.Count - 1;
            Assert.IsTrue(aatree.Remove(list[index]));
            list.RemoveAt(index);
            Assert.IsTrue(ValidateInOrder(aatree, list, comparer));

            index = 0;
            Assert.IsTrue(aatree.Remove(list[index]));
            list.RemoveAt(index);
            Assert.IsTrue(ValidateInOrder(aatree, list, comparer));

            Assert.AreEqual(list.Count, aatree.Count);
        }


        private static bool ValidateInOrder<T>(AATree<T> aaTree, List<T> sortedList, IComparer<T> comparer)
        {
            bool result = true;
            int index = 0;
            foreach (T val in aaTree.InOrderTraversal())
            {
                if (comparer.Compare(val, sortedList[index++]) != 0)
                {
                    result = false;
                    break;
                }
            }

            if (index != sortedList.Count)
            {
                result = false;
            }

            return result;
        }
    }
}
