/****************************************************************************
 * CloneLibraryInformationBvtTestCases.cs
 * 
 * This file contains the CloneLibraryInformation BVT test cases.
 * 
******************************************************************************/

using Bio.Util.Logging;
using Bio.TestAutomation.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.TestAutomation
{
    /// <summary>
    /// Test Automation code for Clone Library Information Bvt level validations.
    /// </summary>
    [TestClass]
    public class CloneLibraryInformationBvtTestCases
    {
        #region Global Variables

        readonly Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");        

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static CloneLibraryInformationBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        # region Clone library information Bvt test cases.

        /// <summary>
        /// Validate public Property :Library Name for Clone library information.
        /// Input Data : Value for properties from XML.
        /// Output Data : Validate the values set for properties.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateLibraryName()
        {            
            CloneLibraryInformation cloneLibrary = new CloneLibraryInformation();
            
            string libraryName = utilityObj.xmlUtil.GetTextValue(
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
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateEqualityOfTwoClones()
        {
            CloneLibraryInformation cloneLibrary1 = new CloneLibraryInformation();
            CloneLibraryInformation cloneLibrary2 = new CloneLibraryInformation();

            string libraryName = utilityObj.xmlUtil.GetTextValue(
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
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateInEqualityOfTwoClones()
        {
            CloneLibraryInformation cloneLibrary1 = new CloneLibraryInformation();
            CloneLibraryInformation cloneLibrary2 = new CloneLibraryInformation();

            string libraryName = utilityObj.xmlUtil.GetTextValue(
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



