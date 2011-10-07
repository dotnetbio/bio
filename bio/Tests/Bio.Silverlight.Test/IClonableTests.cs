using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.Silverlight.Test
{
    [TestClass]
    public class IClonableTests
    {
        /// <summary>
        /// Method to test Intersect operation.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void IClonableTest()
        {
            MyCloneable master = new MyCloneable() { value = 10 };
            MyCloneable clone = master.Clone() as MyCloneable;
            Assert.IsTrue(clone.value == 10);
        }

        /// <summary>
        /// Test class for testing IClonable
        /// </summary>
        internal class MyCloneable : ICloneable
        {
            /// <summary>
            /// Test value
            /// </summary>
            public int value;

            /// <summary>
            /// Creates a deep copy of the given object.
            /// </summary>
            /// <returns></returns>
            public object Clone()
            {
                return new MyCloneable() { value = this.value };
            }
        }
    }
}
