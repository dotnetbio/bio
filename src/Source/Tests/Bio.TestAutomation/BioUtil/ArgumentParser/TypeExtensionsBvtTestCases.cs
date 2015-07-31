using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.Util.ArgumentParser;
using System.Collections.ObjectModel;
using System.Reflection;
using Bio.IO.FastA;

namespace Bio.TestAutomation.BioUtil.ArgumentParser
{
    enum enumForTypeExtensions
    {
        one,
        two,
        three
    }

    /// <summary>
    /// BVT Test Cases for TypeExtensions class
    /// </summary>
    [TestClass]
    public class TypeExtensionsBvtTestCases
    {
        public int publicField1;
        public int publicField2;
        /// <summary>
        /// Validates ToTypeString
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateToTypeString()
        {
            Assert.AreEqual("int", TypeExtensions.ToTypeString(typeof(int)));
            Assert.AreEqual("long", TypeExtensions.ToTypeString(typeof(long)));
            Assert.AreEqual("bool", TypeExtensions.ToTypeString(typeof(bool)));
            Assert.AreEqual("Collection<int>", TypeExtensions.ToTypeString(typeof(Collection<int>)));
        }

        /// <summary>
        /// Validates GetEnumNames
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGetEnumNames()
        {
            string[] enums = TypeExtensions.GetEnumNames(typeof(enumForTypeExtensions));
            Assert.AreEqual("one", enums[0]);
            Assert.AreEqual("two", enums[1]);
            Assert.AreEqual("three", enums[2]);

            string[] enumsNull = TypeExtensions.GetEnumNames(null);
            Assert.AreEqual(null, enumsNull);
        }

        /// <summary>
        /// Validates GetImplementingTypes
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGetImplementingTypes()
        {
            IEnumerable<Type> types = TypeExtensions.GetImplementingTypes(typeof(IParsable));
            Assert.IsTrue(types.Contains(typeof(InputFile)));
            Assert.IsTrue(types.Contains(typeof(OutputFile)));
        }


        /// <summary>
        /// Validates GetDerivedTypes
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGetDerivedTypes()
        {
            IEnumerable<Type> types = TypeExtensions.GetDerivedTypes(typeof(ParsableFile));
            Assert.IsTrue(types.Contains(typeof(InputFile)));
            Assert.IsTrue(types.Contains(typeof(OutputFile)));
        }


        /// <summary>
        /// Validates Implements
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateImplements()
        {
            Assert.IsFalse(TypeExtensions.Implements(null, typeof(IParsable)));
            Assert.IsFalse(TypeExtensions.Implements(typeof(OutputFile), null));
            Assert.IsTrue(TypeExtensions.Implements(typeof(OutputFile), typeof(IParsable)));
        }


        /// <summary>
        /// Validates IsSubclassOfOrImplements
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateIsSubclassOfOrImplements()
        {
            Assert.IsFalse(TypeExtensions.IsSubclassOfOrImplements(null, typeof(IParsable)));
            Assert.IsFalse(TypeExtensions.IsSubclassOfOrImplements(typeof(OutputFile), null));
            Assert.IsTrue(TypeExtensions.IsSubclassOfOrImplements(typeof(OutputFile), typeof(IParsable)));
            Assert.IsTrue(TypeExtensions.IsSubclassOfOrImplements(typeof(OutputFile), typeof(ParsableFile)));
        }


        /// <summary>
        /// Validates IsInstanceOf
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateIsInstanceOf()
        {
            Assert.IsFalse(TypeExtensions.IsInstanceOf(null, typeof(IParsable)));
            Assert.IsFalse(TypeExtensions.IsInstanceOf(typeof(OutputFile), null));
            Assert.IsTrue(TypeExtensions.IsInstanceOf(typeof(OutputFile), typeof(IParsable)));
            Assert.IsTrue(TypeExtensions.IsInstanceOf(typeof(OutputFile), typeof(ParsableFile)));
            Assert.IsTrue(TypeExtensions.IsInstanceOf(typeof(OutputFile), typeof(OutputFile)));
        }


        /// <summary>
        /// Validates GetPropertiesOfType
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGetPropertiesOfType()
        {
            IEnumerable<PropertyInfo> properties = TypeExtensions.GetPropertiesOfType(typeof(FastAParser), typeof(string));
            Assert.AreEqual("Filename", properties.ElementAt(0).Name);
            Assert.AreEqual("Name", properties.ElementAt(1).Name);
            Assert.AreEqual("Description", properties.ElementAt(2).Name);
            Assert.AreEqual("SupportedFileTypes", properties.ElementAt(3).Name);
            Assert.AreEqual(null, TypeExtensions.GetPropertiesOfType(null, typeof(string)));
            Assert.AreEqual(null, TypeExtensions.GetPropertiesOfType(typeof(FastAParser), null));
        }


        /// <summary>
        /// Validates GetFieldsOfType
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGetFieldsOfType()
        {
            IEnumerable<FieldInfo> fields = TypeExtensions.GetFieldsOfType(typeof(TypeExtensionsBvtTestCases), typeof(int));
            Assert.AreEqual("publicField1", fields.ElementAt(0).Name);
            Assert.AreEqual("publicField2", fields.ElementAt(1).Name);
            Assert.AreEqual(null, TypeExtensions.GetFieldsOfType(null, typeof(string)));
            Assert.AreEqual(null, TypeExtensions.GetFieldsOfType(typeof(FastAParser), null));

        }

        /// <summary>
        /// Validates GetFieldsAndPropertiesOfType
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGetFieldsAndPropertiesOfType()
        {
            IEnumerable<MemberInfo> fields = TypeExtensions.GetFieldsAndPropertiesOfType(typeof(TypeExtensionsBvtTestCases), typeof(int));
            Assert.AreEqual("publicField1", fields.ElementAt(0).Name);
            Assert.AreEqual("publicField2", fields.ElementAt(1).Name);
            Assert.AreEqual(null, TypeExtensions.GetFieldsAndPropertiesOfType(null, typeof(string)));
            Assert.AreEqual(null, TypeExtensions.GetFieldsAndPropertiesOfType(typeof(FastAParser), null));

            IEnumerable<MemberInfo> properties = TypeExtensions.GetFieldsAndPropertiesOfType(typeof(FastAParser), typeof(string));
            Assert.AreEqual("Filename", properties.ElementAt(0).Name);
            Assert.AreEqual("Name", properties.ElementAt(1).Name);
            Assert.AreEqual("Description", properties.ElementAt(2).Name);
            Assert.AreEqual("SupportedFileTypes", properties.ElementAt(3).Name);
            Assert.AreEqual(null, TypeExtensions.GetFieldsAndPropertiesOfType(null, typeof(string)));
            Assert.AreEqual(null, TypeExtensions.GetFieldsAndPropertiesOfType(typeof(FastAParser), null));
        }

    }
}
