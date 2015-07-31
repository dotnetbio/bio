using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.Util.ArgumentParser;
using System.Collections.ObjectModel;
using Bio.IO.FastA;

namespace Bio.TestAutomation.BioUtil.ArgumentParser
{
    /// <summary>
    /// Bvt Test Cases for TypeFactory Class
    /// </summary>
    [TestClass]
    public class TypeFactoryBvtTestCases
    {

        /// <summary>
        /// Validates TryGetType
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateTryGetType()
        {
            Type returnType;
            int num = 0;
            Assert.IsTrue(TypeFactory.TryGetType("int", num.GetType(), out returnType));
            Assert.IsTrue(TypeFactory.TryGetType("int32", num.GetType(), out returnType));

            long longVar = 0;
            Assert.IsTrue(TypeFactory.TryGetType("long", longVar.GetType(), out returnType));
            Assert.IsTrue(TypeFactory.TryGetType("int64", longVar.GetType(), out returnType));
            
            string strVar = string.Empty;
            Assert.IsTrue(TypeFactory.TryGetType("string", strVar.GetType(), out returnType));

            double doubleVar = 1.234;
            Assert.IsTrue(TypeFactory.TryGetType("double", doubleVar.GetType(), out returnType));

            Collection<int> intColl = new Collection<int>();
            intColl.Add(1);
            Assert.IsTrue(TypeFactory.TryGetType("System.Collections.ObjectModel.Collection`1[System.Int32]", intColl.GetType(), out returnType));
        }
    }
}
