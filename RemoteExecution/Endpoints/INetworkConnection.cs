using System;
using RemoteExecution.Channels;
using RemoteExecution.Dispatchers;

namespace RemoteExecution.Endpoints
{
	public interface INetworkConnection : IMessageChannel, IDisposable
	{
		bool IsOpen { get; }
		IOperationDispatcher OperationDispatcher { get; }
	}
}