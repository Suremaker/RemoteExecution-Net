using System;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Dispatchers;

namespace RemoteExecution.Core.Connections
{
	public interface IRemoteConnection : IChannelProvider
	{
		IOperationDispatcher Dispatcher { get; }
		Guid Id { get; }
	}
}
