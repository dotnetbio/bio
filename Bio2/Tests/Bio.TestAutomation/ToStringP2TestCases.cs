/****************************************************************************
 * ToStringP2TestCases.cs
 * 
 * This file contains the ToString P2 test cases for all classes.
 * 
******************************************************************************/
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.Util;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;

namespace Bio.TestAutomation
{
    /// <summary>
    /// P2 Test cases for ToString and ConvertToString of all classes
    /// </summary>
    [TestClass]
    public class ToStringP2TestCases
    {
        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
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
        /// Invalidates ConvertToString for class Sequence
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InvalidateSequenceConvertToString()
        {
            try
            {
                //check with blank sequene
                Sequence seq = new Sequence(Alphabets.DNA, "");
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
                Sequence seq = new Sequence(Alphabets.DNA, "ATGCCCC");
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
                Sequence seq = new Sequence(Alphabets.DNA, "ATGCCCC");
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
                Sequence seq = new Sequence(Alphabets.DNA, "ATGCCCC");
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
                Sequence seq = new Sequence(Alphabets.DNA, "ATGCCCC");
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
        /// Invalidates ConvertToString for class DerivedSequence
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InvalidateDerivedSequenceConvertToString()
        {
            try
            {
                //check with blank sequene
                Sequence baseSeq = new Sequence(Alphabets.DNA, "");
                DerivedSequence seq = new DerivedSequence(baseSeq, false, false);
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
                Sequence baseSeq = new Sequence(Alphabets.DNA, "ATGCCCC");
                DerivedSequence seq = new DerivedSequence(baseSeq, false, false);
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
                Sequence baseSeq = new Sequence(Alphabets.DNA, "ATGCCCC");
                DerivedSequence seq = new DerivedSequence(baseSeq, false, false);
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
                Sequence baseSeq = new Sequence(Alphabets.DNA, "ATGCCCC");
                DerivedSequence seq = new DerivedSequence(baseSeq, false, false);
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
                Sequence baseSeq = new Sequence(Alphabets.DNA, "ATGCCCC");
                DerivedSequence seq = new DerivedSequence(baseSeq, false, false);
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
                DerivedSequence seq = new DerivedSequence(baseSeq, false, false);
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
