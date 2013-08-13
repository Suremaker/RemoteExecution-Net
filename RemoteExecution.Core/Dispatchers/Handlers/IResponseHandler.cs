namespace RemoteExecution.Core.Dispatchers.Handlers
{
	public interface IResponseHandler : IMessageHandler
	{
		object GetValue();
		void WaitForResponse();
	}
}