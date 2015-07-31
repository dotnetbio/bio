using System.Collections.Generic;
using Bio.TestUtils.SimulatorUtility;

namespace Bio.TestUtils.SimulatorUtility
{
    /// <summary>
    /// This class contains the parameters required by the simulator to execute a test case.
    /// </summary>
    public class TestCaseParameters
    {
        private string _testCaseId = string.Empty;

        private CallbackRealTestCase _callbackMethod = null;

        private Dictionary<string, object> _callbackParameters = null;

        private Dictionary<string, object> _parameters = null;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="testcaseId">Test case identifier.</param>
        /// <param name="parameters">Test case parameters.</param>
        /// <param name="callbackMethod">Real world test case callback method (Invoker 
        /// passes a function pointer that has to be called to execute a real world scenario).</param>
        /// <param name="callbackParameter">Callback parameters.</param>
        public TestCaseParameters(
                string testcaseId, 
                Dictionary<string, object> parameters, 
                CallbackRealTestCase callbackMethod, 
                Dictionary<string, object> callbackParameters)
        {
            _testCaseId = testcaseId;
            _parameters = parameters;
            _callbackMethod = callbackMethod;
            _callbackParameters = callbackParameters;
        }

        /// <summary>
        /// Gets unique test case identifier.
        /// </summary>
        public string TestCaseId
        {
            get
            {
                return _testCaseId;
            }
        }

        /// <summary>
        /// Gets Real world test case callback method (Invoker passes a function pointer that has to be called to execute a real world scenario).
        /// </summary>
        public CallbackRealTestCase CallbackMethod
        {
            get
            {
                return _callbackMethod;
            }
        }

        /// <summary>
        /// Gets real test case parameters.
        /// </summary>
        public Dictionary<string, object> CallbackParameters
        {
            get
            {
                return _callbackParameters;
            }
        }

        /// <summary>
        /// Gets test case parameters.
        /// </summary>
        public Dictionary<string, object> Parameters
        {
            get
            {
                return _parameters;
            }
        }
    }
}
