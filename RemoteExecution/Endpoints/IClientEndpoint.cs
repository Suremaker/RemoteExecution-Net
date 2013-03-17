using System;

namespace RemoteExecution.Endpoints
{
	public interface IClientEndpoint : IDisposable
	{
		INetworkConnection ConnectTo(string host, ushort port);		
	}
}