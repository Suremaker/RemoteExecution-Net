using System;
using RemoteExecution.Dispatchers.Messages;

namespace RemoteExecution.Channels
{
	/// <summary>
	/// Input channel interface allowing to receive messages.
	/// </summary>
	public interface IInputChannel : IChannel
	{
		/// <summary>
		/// Fires when new message has been received through this channel.
		/// </summary>
		event Action<IMessage> Received;
	}
}