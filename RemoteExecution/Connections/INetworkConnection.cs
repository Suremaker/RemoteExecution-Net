using System;
using RemoteExecution.Dispatchers;
using RemoteExecution.Executors;

namespace RemoteExecution.Connections
{
	public interface INetworkConnection : IDisposable
	{
		bool IsOpen { get; }
		IOperationDispatcher OperationDispatcher { get; }
		IRemoteExecutor RemoteExecutor { get; }
	}
}