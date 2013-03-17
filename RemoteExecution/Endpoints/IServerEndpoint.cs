using System;

namespace RemoteExecution.Endpoints
{
	public interface IServerEndpoint : IDisposable
	{
		void StartListening();
	}
}