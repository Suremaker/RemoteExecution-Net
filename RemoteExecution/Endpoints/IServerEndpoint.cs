using System;
using RemoteExecution.Executors;

namespace RemoteExecution.Endpoints
{
	public interface IServerEndpoint : IDisposable
	{
		void StartListening();
		IBroadcastRemoteExecutor BroadcastRemoteExecutor { get; }
	}
}