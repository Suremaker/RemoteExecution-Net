using System;
using RemoteExecution.Messages;

namespace RemoteExecution.Channels
{
	public interface IIncomingMessageChannel
	{
		bool IsOpen { get; }
		event Action<IMessage> Received;
	}
}