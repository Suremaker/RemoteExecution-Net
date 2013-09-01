namespace RemoteExecution.Dispatchers.Handlers
{
	/// <summary>
	/// Interface for response handler.
	/// </summary>
	public interface IResponseHandler : IMessageHandler
	{
		/// <summary>
		/// Returns response value.
		/// It should be called after WaitForResponse() returns.
		/// </summary>
		/// <returns></returns>
		object GetValue();
		/// <summary>
		/// Blocks until response is handled.
		/// </summary>
		void WaitForResponse();
	}
}