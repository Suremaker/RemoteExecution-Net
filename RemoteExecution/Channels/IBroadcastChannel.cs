namespace RemoteExecution.Channels
{
	public interface IBroadcastChannel : IOutgoingMessageChannel
	{
		int ConnectionCount { get; }
	}
}