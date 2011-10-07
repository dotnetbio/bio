namespace Bio.Web
{
    /// <summary>
    /// Function pointer to a method that has to be invoked after the asynchronous web call completes.
    /// </summary>
    /// <param name="response">Response of asynchronous web method call.</param>
    public delegate void AsyncWebMethodCompleted(AsyncWebMethodResponse response);
}
