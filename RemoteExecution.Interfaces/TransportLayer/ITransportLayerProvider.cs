using System;
using RemoteExecution.Channels;
using RemoteExecution.Endpoints.Listeners;

namespace RemoteExecution.TransportLayer
{
	/// <summary>
	/// Transport layer provider allowing to create client channel or server connection listener objects for given uri.
	/// </summary>
	public interface ITransportLayerProvider
	{
		/// <summary>
		/// Returns supported scheme.
		/// </summary>
		string Scheme { get; }
		/// <summary>
		/// Creates client channel for given uri.
		/// </summary>
		/// <param name="uri">Uri used to configure client channel.</param>
		/// <returns>Client channel.</returns>
		/// <exception cref="ArgumentException">Thrown when uri has wrong scheme or contains wrong content.</exception>
		IClientChannel CreateClientChannelFor(Uri uri);

		/// <summary>
		/// Creates server connection listener for given uri.
		/// </summary>
		/// <param name="uri">Uri used to configure server connection listener.</param>
		/// <returns>Server connection listener</returns>
		/// <exception cref="ArgumentException">Thrown when uri has wrong scheme or contains wrong content.</exception>
		IServerConnectionListener CreateConnectionListenerFor(Uri uri);
	}
}