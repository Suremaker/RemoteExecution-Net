namespace RemoteExecution.Dispatchers.Handlers
{
	public interface IResponseHandler : IMessageHandler
	{
		object GetValue();
		void WaitForResponse();
	}
}