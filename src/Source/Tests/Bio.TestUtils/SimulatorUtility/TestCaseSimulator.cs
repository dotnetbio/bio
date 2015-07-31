namespace Bio.TestUtils.SimulatorUtility
{
    /// <summary>
    /// Takes a test case object and executes it, this class implements failover 
    /// logic for web test cases. This class provides flexibility to execute 
    /// a set of test cases in mock/real case scenario. If real case fails, it 
    /// failovers and return mock output result. And if count of failed test case 
    /// is more than maximum limit, it permanently switches to mock case scenario.
    /// </summary>
    public class TestCaseSimulator
    {
        /// <summary>
        /// Tracks the count of real test case that has failed.
        /// </summary>
        private int _RealTestCaseFailureCount = 0;

        /// <summary>
        /// Maximum number of real test failures allowed before permanently switching to execution of mock test case executions.
        /// </summary>
        private const int _MaximumRealTestCaseFailure = 3;

        /// <summary>
        /// Use a mock test case.
        /// </summary>
        private bool _UseMockTestCase = false;

        /// <summary>
        /// Default Constructor. (UseMockTestCase is set to 'false'
        /// </summary>
        public TestCaseSimulator()
        {
            _UseMockTestCase = false;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="useMockTestCase">Use mock test cases.</param>
        public TestCaseSimulator(bool useMockTestCase)
        {
            _UseMockTestCase = useMockTestCase;
        }

        /// <summary>
        /// Gets a value indicating whether to use a mock test case.
        /// </summary>
        public bool UseMockTestCase
        {
            get
            {
                return _UseMockTestCase;
            }
        }

        /// <summary>
        /// Simulate the test case. If service is available, execute the real test and return the output, 
        /// otherwise return the mock output.
        /// If “UseMockTestCase” is ‘true’
        ///     Get the mock result (object) from “MockOutputRepository”.
        ///     Create an instance of “TestCaseOutput” with mock result (object).
        ///     Return instance of “TestCaseOutput”.
        /// Else
        ///     Execute the real world function pointer and get the result (object).
        ///         If execution fails (On exception. Only WebException & SoapException?)
        ///             Increment “_RealTestCaseFailureCount”
        ///             If “_RealTestCaseFailureCount” > “_MaximumRealTestCaseFailure”, set _UseMockTestCase to true.
        ///             Get the mock result (object) from “MockOutputRepository”.
        ///     Create an instance of “TestCaseOutput” with output result (object).
        ///     Return instance of “TestCaseOutput”.
        /// </summary>
        /// <param name="parameters">Test case parameters</param>
        public TestCaseOutput Simulate(TestCaseParameters parameters)
        {
            if (!UseMockTestCase)
            {
                try
                {
                    return parameters.CallbackMethod(parameters.CallbackParameters);
                }
                catch
                { 
                    // Ignore the exception

                    _RealTestCaseFailureCount++;

                    if (_RealTestCaseFailureCount == _MaximumRealTestCaseFailure)
                    {
                        _UseMockTestCase = true;
                    }
                }
            }

            return new TestCaseOutput(
                MockOutputRepository.Instance.GetOutput(parameters.TestCaseId),
                true);
        }
    }
}
