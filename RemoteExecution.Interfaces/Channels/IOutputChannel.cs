using RemoteExecution.Dispatchers.Messages;

namespace RemoteExecution.Channels
{
	/// <summary>
	/// Output channel interface allowing to send messages.
	/// </summary>
	public interface IOutputChannel : IChannel
	{
		/// <summary>
		/// Sends given message through this channel.
		/// </summary>
		/// <param name="message">Message to send.</param>
		void Send(IMessage message);
	}
}