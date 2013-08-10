using System;
using RemoteExecution.Core.Dispatchers;

namespace RemoteExecution.Core.Channels
{
	public interface IIncomingMessageChannel
	{
		event Action<IMessage> Received;
		bool IsOpen { get; }
	}
}