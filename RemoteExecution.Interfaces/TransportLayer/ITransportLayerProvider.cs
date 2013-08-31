using System;
using RemoteExecution.Channels;
using RemoteExecution.Endpoints.Listeners;

namespace RemoteExecution.TransportLayer
{
	public interface ITransportLayerProvider
	{
		string Scheme { get; }
		IClientChannel CreateClientChannelFor(Uri uri);
		IServerConnectionListener CreateConnectionListenerFor(Uri uri);
	}
}