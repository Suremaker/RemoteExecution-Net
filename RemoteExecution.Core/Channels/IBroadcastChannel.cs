namespace RemoteExecution.Core.Channels
{
	public interface IBroadcastChannel : IOutputChannel
	{
		int ReceiverCount { get; }
	}
}