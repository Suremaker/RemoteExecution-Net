using System;
using System.Collections.Concurrent;
using RemoteExecution.Channels;
using RemoteExecution.Endpoints.Listeners;

namespace RemoteExecution.TransportLayer
{
	/// <summary>
	/// Transport layer resolver class allowing to resolve client channel or server connection listener basing on given uri.
	/// </summary>
	public static class TransportLayerResolver
	{
		private static readonly ConcurrentDictionary<string, ITransportLayerProvider> _providers = new ConcurrentDictionary<string, ITransportLayerProvider>();

		/// <summary>
		/// Creates client channel basing on given uri.
		/// </summary>
		/// <param name="uri">Connection uri used to create channel.</param>
		/// <returns>Client channel.</returns>
		/// <exception cref="UnknownTransportLayerException">Thrown if there is no registered providers for given uri scheme.</exception>
		public static IClientChannel CreateClientChannelFor(Uri uri)
		{
			return Resolve(uri.Scheme).CreateClientChannelFor(uri);
		}

		/// <summary>
		/// Creates server connection listener basing on given uri.
		/// </summary>
		/// <param name="uri">Listener uri used to create server connection listener.</param>
		/// <returns>Server connection listener.</returns>
		/// <exception cref="UnknownTransportLayerException">Thrown if there is no registered providers for given uri scheme.</exception>
		public static IServerConnectionListener CreateConnectionListenerFor(Uri uri)
		{
			return Resolve(uri.Scheme).CreateConnectionListenerFor(uri);
		}

		/// <summary>
		/// Registers given transport layer provider for uri scheme that is supported by it.
		/// </summary>
		/// <param name="provider">Provider to register.</param>
		/// <exception cref="ArgumentException">Thrown if given scheme has already associated provider.</exception>
		public static void Register(ITransportLayerProvider provider)
		{
			if (!_providers.TryAdd(provider.Scheme, provider))
				throw new ArgumentException(string.Format("There is already registered provider for '{0}' scheme.", provider.Scheme));
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
