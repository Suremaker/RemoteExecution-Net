using System;
using System.Collections.Concurrent;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Endpoints.Listeners;

namespace RemoteExecution.Core.TransportLayer
{
	public static class TransportLayerResolver
	{
		private static readonly ConcurrentDictionary<string, ITransportLayerProvider> _providers = new ConcurrentDictionary<string, ITransportLayerProvider>();

		public static void Register(ITransportLayerProvider provider)
		{
			if (!_providers.TryAdd(provider.Scheme, provider))
				throw new ArgumentException(string.Format("There is already registered provider for '{0}' scheme.", provider.Scheme));
		}

		public static IClientChannel CreateClientChannelFor(Uri uri)
		{
			return Resolve(uri.Scheme).CreateClientChannelFor(uri);
		}

		public static IServerConnectionListener CreateConnectionListenerFor(Uri uri)
		{
			return Resolve(uri.Scheme).CreateConnectionListenerFor(uri);
		}

		private static ITransportLayerProvider Resolve(string scheme)
		{
			ITransportLayerProvider provider;
			if (!_providers.TryGetValue(scheme, out provider))
				throw new UnknownTransportLayerException(string.Format("Unable to resolve transport layer for '{0}' scheme.", scheme));
			return provider;
		}
	}
}
