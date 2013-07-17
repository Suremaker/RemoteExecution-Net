using RemoteExecution.Channels;

namespace RemoteExecution.Handlers
{
	public interface IResponseHandler : IHandler
	{
		IMessageChannel TargetChannel { get; }
		object GetValue();
		void WaitForResponse();
	}
}