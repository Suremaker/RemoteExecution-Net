using System;
using RemoteExecution.Connections;
using RemoteExecution.Executors;

namespace RemoteExecution.Endpoints
{
	public interface IClientEndpoint : IDisposable
	{
		INetworkConnection ConnectTo(string host, ushort port);
		IRemoteExecutor RemoteExecutor { get; }
	}
}