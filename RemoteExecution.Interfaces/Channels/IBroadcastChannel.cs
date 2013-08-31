namespace RemoteExecution.Channels
{
	public interface IBroadcastChannel : IOutputChannel
	{
		int ReceiverCount { get; }
	}
}