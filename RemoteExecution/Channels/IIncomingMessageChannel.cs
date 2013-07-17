using System;
using RemoteExecution.Messages;

namespace RemoteExecution.Channels
{
	public interface IIncomingMessageChannel
	{
		event Action<IMessage> Received;
		bool IsOpen { get; }
	}
}