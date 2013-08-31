using System;
using RemoteExecution.Dispatchers;
using RemoteExecution.Executors;

namespace RemoteExecution.Connections
{
	public interface IRemoteConnection : IDisposable
	{
		event Action Closed;
		bool IsOpen { get; }
		IOperationDispatcher OperationDispatcher { get; }
		IRemoteExecutor RemoteExecutor { get; }
	}
}
