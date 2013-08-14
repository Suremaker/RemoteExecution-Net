using System;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Executors;

namespace RemoteExecution.Core.Connections
{
	public interface IRemoteConnection : IDisposable
	{
		IRemoteExecutor Executor { get; }
		IOperationDispatcher Dispatcher { get; }
		Guid Id { get; }
		bool IsOpen { get; }
	}

	public interface IClientConnection : IRemoteConnection
	{
		void Open();
	}
}
