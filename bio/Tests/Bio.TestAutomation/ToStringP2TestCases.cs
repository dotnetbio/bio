/****************************************************************************
 * ToStringP2TestCases.cs
 * 
 * This file contains the ToString P2 test cases for all classes.
 * 
******************************************************************************/

using System;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.TestAutomation
{
    /// <summary>
    ///     P2 Test cases for ToString and ConvertToString of all classes
    /// </summary>
    [TestClass]
    public class ToStringP2TestCases
    {
        #region Constructor

        /// <summary>
        ///     Static constructor to open log and make other settings needed for test
        /// </summary>
        static ToStringP2TestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        /// <summary>
        ///     Invalidates ConvertToString for class Sequence
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InvalidateSequenceConvertToString()
        {
            try
            {
                //check with blank sequence
                var seq = new Sequence(Alphabets.DNA, "");
                string seqStr = seq.ConvertToString(0, 3);
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException aorex)
            {
                ApplicationLog.WriteLine("Successfully caught ArgumentOutOfRangeException : " + aorex.Message);
            }

            try
            {
                //check with length greater than actual length
                var seq = new Sequence(Alphabets.DNA, "ATGCCCC");
                string seqStr = seq.ConvertToString(0, 30);
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException aorex)
            {
                ApplicationLog.WriteLine("Successfully caught ArgumentOutOfRangeException : " + aorex.Message);
            }

            try
            {
                //check with length < 0
                var seq = new Sequence(Alphabets.DNA, "ATGCCCC");
                string seqStr = seq.ConvertToString(0, -30);
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException aorex)
            {
                ApplicationLog.WriteLine("Successfully caught ArgumentOutOfRangeException : " + aorex.Message);
            }

            try
            {
                //check with start index < 0
                var seq = new Sequence(Alphabets.DNA, "ATGCCCC");
                string seqStr = seq.ConvertToString(-10, 3);
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException aorex)
            {
                ApplicationLog.WriteLine("Successfully caught ArgumentOutOfRangeException : " + aorex.Message);
            }

            try
            {
                //check with start index greater than actual length
                var seq = new Sequence(Alphabets.DNA, "ATGCCCC");
                string seqStr = seq.ConvertToString(30, 3);
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException aorex)
            {
                ApplicationLog.WriteLine("Successfully caught ArgumentOutOfRangeException : " + aorex.Message);
            }

            try
            {
                //check with null sequence
                Sequence seq = null;
                string seqStr = seq.ConvertToString(0, 3);
                Assert.Fail();
            }
            catch (NullReferenceException nre)
            {
                ApplicationLog.WriteLine("Successfully caught ArgumentOutOfRangeException : " + nre.Message);
            }
        }

        /// <summary>
        ///     Invalidates ConvertToString for class DerivedSequence
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InvalidateDerivedSequenceConvertToString()
        {
            try
            {
                //check with blank sequene
                var baseSeq = new Sequence(Alphabets.DNA, "");
                var seq = new DerivedSequence(baseSeq, false, false);
                string seqStr = seq.ConvertToString(0, 3);
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException aorex)
            {
                ApplicationLog.WriteLine("Successfully caught ArgumentOutOfRangeException : " + aorex.Message);
            }

            try
            {
                //check with length greater than actual length
                var baseSeq = new Sequence(Alphabets.DNA, "ATGCCCC");
                var seq = new DerivedSequence(baseSeq, false, false);
                string seqStr = seq.ConvertToString(0, 30);
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException aorex)
            {
                ApplicationLog.WriteLine("Successfully caught ArgumentOutOfRangeException : " + aorex.Message);
            }

            try
            {
                //check with length < 0
                var baseSeq = new Sequence(Alphabets.DNA, "ATGCCCC");
                var seq = new DerivedSequence(baseSeq, false, false);
                string seqStr = seq.ConvertToString(0, -30);
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException aorex)
            {
                ApplicationLog.WriteLine("Successfully caught ArgumentOutOfRangeException : " + aorex.Message);
            }

            try
            {
                //check with start index < 0
                var baseSeq = new Sequence(Alphabets.DNA, "ATGCCCC");
                var seq = new DerivedSequence(baseSeq, false, false);
                string seqStr = seq.ConvertToString(-10, 3);
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException aorex)
            {
                ApplicationLog.WriteLine("Successfully caught ArgumentOutOfRangeException : " + aorex.Message);
            }

            try
            {
                //check with start index greater than actual length
                var baseSeq = new Sequence(Alphabets.DNA, "ATGCCCC");
                var seq = new DerivedSequence(baseSeq, false, false);
                string seqStr = seq.ConvertToString(30, 3);
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException aorex)
            {
                ApplicationLog.WriteLine("Successfully caught ArgumentOutOfRangeException : " + aorex.Message);
            }

            try
            {
                //check with null sequence
                Sequence baseSeq = null;
                var seq = new DerivedSequence(baseSeq, false, false);
                string seqStr = seq.ConvertToString(0, 3);
                Assert.Fail();
            }
            catch (NullReferenceException nre)
            {
                ApplicationLog.WriteLine("Successfully caught ArgumentOutOfRangeException : " + nre.Message);
            }
        }
    }
}