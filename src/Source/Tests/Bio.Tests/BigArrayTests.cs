using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Bio.Tests.Framework
{
    /// <summary>
    /// Tests BigArray class.
    /// </summary>
    [TestFixture]
    public class BigArrayTests
    {
        /// <summary>
        /// Tests creating Zero length BigArray.
        /// </summary>
        [Test]
        public void TestCreatingBigArrayWithZeroLength()
        {
            BigArray<int> bigArray = null;
            try
            {
                bigArray = new BigArray<int>(0);
            }
            catch
            {
                Assert.Fail();
            }

            Assert.IsNotNull(bigArray);
        }

        /// <summary>
        /// Test creating BigArray.
        /// </summary>
        [Test]
        public void TestCreatingBigArrayWithNonZeroLength()
        {
            int size = 100;
            BigArray<int> bigArray = null;
            try
            {
                bigArray = new BigArray<int>(size);
            }
            catch
            {
                Assert.Fail();
            }

            Assert.IsNotNull(bigArray);
            Assert.IsTrue(bigArray.Length == size);
        }

        #region Commented Test cases - these test requires more memory.
        ///// <summary>
        ///// Test creating BigArray with length more than a blocksize.
        ///// </summary>
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes"), TestMethod]
        //public void TestCreatingBigArrayWithMoreThanOneBlockSize()
        //{
        //    int size = int.MaxValue;
        //    BigArray<byte> bigArray = null;
        //    try
        //    {
        //        bigArray = new BigArray<byte>(size);
        //    }
        //    catch
        //    {
        //        Assert.Fail();
        //    }

        //    Assert.IsNotNull(bigArray);
        //    Assert.IsTrue(bigArray.Length == size);
        //    Assert.IsTrue(bigArray.BlockSize < size);

        //    try
        //    {
        //        for (int i = 0; i < size; i++)
        //        {
        //            bigArray[i] = (byte)(i % 256);
        //        }

        //        for (int i = 0; i < size; i++)
        //        {
        //            if (bigArray[i] != (byte)(i % 256))
        //            {
        //                Assert.Fail();
        //            }
        //        }

        //    }
        //    catch
        //    {
        //        Assert.Fail();
        //    }
        //}

        ///// <summary>
        ///// Test creating BigArray with length more than a integer max value.
        ///// </summary>
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes"), TestMethod]
        //public void TestCreatingBigArrayWithMoreThanIntMaxValue()
        //{
        //    long size = int.MaxValue + 100L;
        //    BigArray<byte> bigArray = null;
        //    try
        //    {
        //        bigArray = new BigArray<byte>(size);
        //    }
        //    catch
        //    {
        //        Assert.Fail();
        //    }

        //    Assert.IsNotNull(bigArray);
        //    Assert.IsTrue(bigArray.Length == size);
        //    Assert.IsTrue(bigArray.BlockSize < size);

        //    try
        //    {
        //        for (long i = 0; i < size; i++)
        //        {
        //            bigArray[i] = (byte)(i % 256);
        //        }

        //        for (long i = 0; i < size; i++)
        //        {
        //            if (bigArray[i] != (byte)(i % 256))
        //            {
        //                Assert.Fail();
        //            }
        //        }

        //    }
        //    catch
        //    {
        //        Assert.Fail();
        //    }
        //}
        #endregion

        /// <summary>
        /// Test IndexOf method
        /// </summary>
        [Test]
        public void TestIndexOf()
        {
            BigArray<int> bigArray = new BigArray<int>(10);
            bigArray[0] = 1;
            bigArray[1] = 2;
            bigArray[2] = 1;
            bigArray[3] = 3;
            bigArray[4] = 4;
            bigArray[5] = 5;
            bigArray[6] = 1;
            bigArray[7] = 6;
            bigArray[8] = 7;
            bigArray[9] = 8;

            Assert.IsTrue(bigArray.IndexOf(0) == -1);
            Assert.IsTrue(bigArray.IndexOf(1) == 0);
            Assert.IsTrue(bigArray.IndexOf(1, 1) == 2);
            Assert.IsTrue(bigArray.IndexOf(1, 2, 6) == 2);
            Assert.IsTrue(bigArray.IndexOf(1, 3, 4) == 6);
        }

        /// <summary>
        /// Test Clear method
        /// </summary>
        [Test]
        public void TestClear()
        {
            BigArray<int> bigArray = new BigArray<int>(10);
            bigArray[0] = 1;
            bigArray[1] = 2;
            bigArray[2] = 1;
            bigArray[3] = 3;
            bigArray[4] = 4;
            bigArray[5] = 5;
            bigArray[6] = 1;
            bigArray[7] = 6;
            bigArray[8] = 7;
            bigArray[9] = 8;

            Assert.IsTrue(bigArray.IndexOf(0) == -1);
            bigArray.Clear(3, 1);
            Assert.IsTrue(bigArray.IndexOf(0) == 3);
            bigArray.Clear();
            for (int i = 0; i < 10; i++)
            {
                Assert.IsTrue(bigArray[i] == 0);
            }
        }

        /// <summary>
        /// Test Resize method
        /// </summary>
        [Test]
        public void TestResize()
        {
            try
            {
                BigArray<int> bigArray = new BigArray<int>(5);
                Assert.IsTrue(bigArray.Length == 5);
                bigArray[0] = 1;
                bigArray[1] = 2;
                bigArray[2] = 1;
                bigArray[3] = 3;
                bigArray[4] = 4;
                bigArray.Resize(10);
                bigArray[5] = 5;
                bigArray[6] = 1;
                bigArray[7] = 6;
                bigArray[8] = 7;
                bigArray[9] = 8;
                Assert.IsTrue(bigArray.Length == 10);
            }
            catch
            {
                Assert.Fail();
            }
        }

        /// <summary>
        /// Test CopyTo Method
        /// </summary>
        [Test]
        public void TestCopyTo()
        {
            BigArray<int> bigArray = new BigArray<int>(10);
            bigArray[0] = 1;
            bigArray[1] = 2;
            bigArray[2] = 1;
            bigArray[3] = 3;
            bigArray[4] = 4;
            bigArray[5] = 5;
            bigArray[6] = 1;
            bigArray[7] = 6;
            bigArray[8] = 7;
            bigArray[9] = 8;

            int[] array = new int[10];
            try
            {
                int sourceIndex = 0;
                int destIndex = 0;
                int count = 10;
                bigArray.CopyTo(sourceIndex, array, count);
                for (int i = 0; i < count; i++)
                {
                    if (bigArray[i + sourceIndex] != array[i + destIndex])
                    {
                        Assert.Fail();
                    }
                }

                array = new int[5];
                sourceIndex = 4;
                destIndex = 0;
                count = 5;
                bigArray.CopyTo(sourceIndex, array, count);
                for (int i = 0; i < count; i++)
                {
                    if (bigArray[i + sourceIndex] != array[i + destIndex])
                    {
                        Assert.Fail();
                    }
                }

                array = new int[10];
                sourceIndex = 4;
                destIndex = 3;
                count = 5;
                bigArray.CopyTo(sourceIndex, array, destIndex, count);
                for (int i = 0; i < count; i++)
                {
                    if (bigArray[i + sourceIndex] != array[i + destIndex])
                    {
                        Assert.Fail();
                    }
                }
            }
            catch
            {
                Assert.Fail();
            }
        }

        /// <summary>
        /// Test GetEnumerator method
        /// </summary>
        [Test]
        public void TestGetEnumerator()
        {
            BigArray<int> bigArray = new BigArray<int>(10);
            bigArray[0] = 1;
            bigArray[1] = 2;
            bigArray[2] = 1;
            bigArray[3] = 3;
            bigArray[4] = 4;
            bigArray[5] = 5;
            bigArray[6] = 1;
            bigArray[7] = 6;
            bigArray[8] = 7;
            bigArray[9] = 8;

            int index = 0;
            foreach (int val in bigArray)
            {
                if (bigArray[index++] != val)
                {
                    Assert.Fail();
                }
            }
        }
    }
}
