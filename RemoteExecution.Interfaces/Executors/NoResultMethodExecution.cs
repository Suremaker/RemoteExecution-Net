namespace RemoteExecution.Executors
{
	/// <summary>
	/// Enumeration determining if no result methods (returning void) should be treated as one way or two way.
	/// </summary>
	public enum NoResultMethodExecution
	{
		/// <summary>
		/// Two way method - remote executor always waits for response, even if it is void.
		/// </summary>
		TwoWay,
		/// <summary>
		/// One way method - remote executor does not wait for response (if exception is thrown on remote end, it will not be passed back to method caller)
		/// </summary>
		OneWay
	}
}