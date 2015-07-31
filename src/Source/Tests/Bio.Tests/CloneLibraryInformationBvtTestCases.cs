using Bio.TestAutomation.Util;
using Bio.Util.Logging;

using NUnit.Framework;

namespace Bio.Tests
{
    /// <summary>
    /// Test Automation code for Clone Library Information Bvt level validations.
    /// </summary>
    [TestFixture]
    public class CloneLibraryInformationBvtTestCases
    {
        readonly Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");        

        # region Clone library information Bvt test cases.

        /// <summary>
        /// Validate public Property :Library Name for Clone library information.
        /// Input Data : Value for properties from XML.
        /// Output Data : Validate the values set for properties.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateLibraryName()
        {            
            CloneLibraryInformation cloneLibrary = new CloneLibraryInformation();
            
            string libraryName = this.utilityObj.xmlUtil.GetTextValue(
                                 Constants.CloneLibraryInformationNode, Constants.LibraryNameNode);

            cloneLibrary.LibraryName = libraryName;
            Assert.AreEqual(libraryName, cloneLibrary.LibraryName);

            ApplicationLog.WriteLine(string.Concat("CloneLibraryInformation BVT: Validation of Public property: Library Name",
                                    cloneLibrary.LibraryName, " completed successfully."));
        }

        /// <summary>
        /// Validate equality for two clones.
        /// Input Data : Value for Library Name from XML.
        /// Output Data : Both clones should be equal.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateEqualityOfTwoClones()
        {
            CloneLibraryInformation cloneLibrary1 = new CloneLibraryInformation();
            CloneLibraryInformation cloneLibrary2 = new CloneLibraryInformation();

            string libraryName = this.utilityObj.xmlUtil.GetTextValue(
                                 Constants.CloneLibraryInformationNode, Constants.LibraryNameNode);
            cloneLibrary1.LibraryName = libraryName;
            cloneLibrary2 = cloneLibrary1;
            Assert.IsTrue(cloneLibrary1.Equals(cloneLibrary2));

            ApplicationLog.WriteLine(string.Concat("CloneLibraryInformation BVT: Validation of Overridden method: Equals",
                                    cloneLibrary1.LibraryName, " completed successfully."));
            
        }

        /// <summary>
        /// Validate In-Equality for two clones.
        /// Input Data : Value for Library Name from XML.
        /// Output Data : Both clones shouldn't be equal.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateInEqualityOfTwoClones()
        {
            CloneLibraryInformation cloneLibrary1 = new CloneLibraryInformation();
            CloneLibraryInformation cloneLibrary2 = new CloneLibraryInformation();

            string libraryName = this.utilityObj.xmlUtil.GetTextValue(
                                 Constants.CloneLibraryInformationNode, Constants.LibraryNameNode);
            cloneLibrary1.LibraryName = libraryName;
            cloneLibrary2.LibraryName = libraryName + "newValue";
            Assert.IsFalse(cloneLibrary1.Equals(cloneLibrary2));

            ApplicationLog.WriteLine(string.Concat("CloneLibraryInformation BVT: Validation of Overridden method: Not Equals",
                                    cloneLibrary2.LibraryName, " completed successfully."));

        }

        #endregion Clone library information Bvt test cases.
    }
}



