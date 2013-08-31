using System;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Endpoints.Listeners;

namespace RemoteExecution.Core.TransportLayer
{
	public interface ITransportLayerProvider
	{
		string Scheme { get; }
		IClientChannel CreateClientChannelFor(Uri uri);
		IServerConnectionListener CreateConnectionListenerFor(Uri uri);
	}
}