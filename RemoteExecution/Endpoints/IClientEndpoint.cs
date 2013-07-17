using System;
using RemoteExecution.Connections;
using RemoteExecution.Executors;

namespace RemoteExecution.Endpoints
{
	public interface IClientEndpoint : IDisposable
	{
		IRemoteExecutor RemoteExecutor { get; }
		INetworkConnection ConnectTo(string host, ushort port);
	}
}