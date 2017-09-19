using System.Collections.Generic;

namespace Bio.TestUtils.SimulatorUtility
{
    /// <summary>
    /// This delegate represents a function pointer to be called to execute a real 
    /// test case scenario. Take an object as parameter and returns an object as output.
    /// </summary>
    /// <param name="parameters">Real test case parameters</param>
    /// <returns>Return test case output</returns>
    public delegate TestCaseOutput CallbackRealTestCase(Dictionary<string, object> parameters);
}
