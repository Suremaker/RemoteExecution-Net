using System;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Executors;

namespace RemoteExecution.Core.Connections
{
	public interface IRemoteConnection : IDisposable
	{
		event Action Closed;
		bool IsOpen { get; }
		IOperationDispatcher OperationDispatcher { get; }
		IRemoteExecutor RemoteExecutor { get; }
	}
}
