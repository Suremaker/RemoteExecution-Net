namespace RemoteExecution.Messaging
{
	public interface IBroadcastChannel : IMessageChannel
	{
		int ConnectionCount { get; }
	}
}