namespace Bio.TestUtils.SimulatorUtility
{
    /// <summary>
    /// This class contains the output of test case executed by TestCaseSimulator.
    /// </summary>
    public class TestCaseOutput
    {
        private object _result = null;

        private bool _isMockOutput = false;

        /// <summary>
        /// Public constructor.
        /// </summary>
        /// <param name="result">Test case output.</param>
        /// <param name="isMockOutput">Is outupt mock.</param>
        public TestCaseOutput(object result, bool isMockOutput)
        {
            _result = result;
            _isMockOutput = isMockOutput;
        }

        /// <summary>
        /// Gets output of the test case.
        /// </summary>
        public object Result
        {
            get
            {
                return _result;
            }
        }

        /// <summary>
        /// Gets a values indicating whether the output is mock.
        /// </summary>
        public bool IsMockOutput
        {
            get
            {
                return _isMockOutput;
            }
        }
    }
}
