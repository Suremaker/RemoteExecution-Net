using System;
using RemoteExecution.Core.Dispatchers;

namespace RemoteExecution.Core.Channels
{
	public interface IInputChannel : IChannel
	{
		event Action<IMessage> Received;
	}
}