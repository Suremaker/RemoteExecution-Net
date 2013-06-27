using RemoteExecution.Messaging;

namespace RemoteExecution.Handling
{
	public interface IResponseHandler : IHandler
	{
		IMessageChannel TargetChannel { get; }
		void WaitForResponse();
		object GetValue();
	}
}