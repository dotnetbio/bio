using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.Util;

namespace Bio.TestAutomation.Util
{
    /// <summary>
    /// FileAccessComparer Bvt Test cases
    /// </summary>
    [TestClass]
    public class FileAccessComparerBvtTestCases
    {
        #region Bvt Test cases

        /// <summary>
        /// Validates Equals method of the FileAccessComparer class
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateEquals()
        {
            FileAccessComparer faComparer = new FileAccessComparer();
            string filename1 = Path.GetTempFileName();
            string filename2 = Path.GetTempFileName();

            using (File.Create(filename1)) { }
            using (File.Create(filename2)) { }
            
            FileInfo fileinfo1 = new FileInfo(filename1);
            FileInfo fileinfo2 = new FileInfo(filename2);

            Assert.IsTrue(faComparer.Equals(fileinfo1,fileinfo1));
            Assert.IsFalse(faComparer.Equals(fileinfo1, fileinfo2));
            Assert.IsFalse(faComparer.Equals(fileinfo1, null));
            Assert.IsFalse(faComparer.Equals(null, fileinfo2));
            Assert.IsTrue(faComparer.Equals(null, null));

            File.Delete(filename1);
            File.Delete(filename2);

        }

        /// <summary>
        /// Validates GetHashCode method of the FileAccessComparer class
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGetHashCode()
        {
            FileAccessComparer faComparer = new FileAccessComparer();
            string filename1 = Path.GetTempFileName();
            using (File.Create(filename1)) { }
            FileInfo fileinfo1 = new FileInfo(filename1);
            int hash = faComparer.GetHashCode(fileinfo1);
            Assert.AreEqual(fileinfo1.Name.GetHashCode(), hash);
            File.Delete(filename1);

            //check with null file info
            // this throws ArgumentNullException .. not valid.
            //Assert.AreEqual(0, faComparer.GetHashCode(null));
        }

        #endregion Bvt Test cases
    }
}
