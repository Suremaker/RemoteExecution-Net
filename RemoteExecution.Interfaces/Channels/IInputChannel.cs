using System;
using RemoteExecution.Dispatchers.Messages;

namespace RemoteExecution.Channels
{
	public interface IInputChannel : IChannel
	{
		event Action<IMessage> Received;
	}
}