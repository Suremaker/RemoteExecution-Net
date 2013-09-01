namespace RemoteExecution.Channels
{
	/// <summary>
	/// Broadcast channel interface allowing to send broadcast messages to all opened channels.
	/// It can be used only on server side.
	/// </summary>
	public interface IBroadcastChannel : IOutputChannel
	{
		/// <summary>
		/// Returns count of receivers who will get sent message.
		/// </summary>
		int ReceiverCount { get; }
	}
}