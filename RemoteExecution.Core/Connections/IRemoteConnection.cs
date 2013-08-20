using System;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Executors;

namespace RemoteExecution.Core.Connections
{
	public interface IRemoteConnection : IDisposable
	{
		event Action Closed;
		IOperationDispatcher Dispatcher { get; }
		IRemoteExecutor Executor { get; }
		bool IsOpen { get; }
	}
}
