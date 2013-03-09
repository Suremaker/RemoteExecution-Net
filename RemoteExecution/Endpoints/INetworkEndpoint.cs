using System;

namespace RemoteExecution.Endpoints
{
	public interface INetworkEndpoint : IDisposable
	{
		bool ProcessMessage();
	}
}