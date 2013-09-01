using System;
using RemoteExecution.Channels;
using RemoteExecution.Endpoints.Listeners;
using RemoteExecution.Serializers;
using RemoteExecution.TransportLayer;

namespace RemoteExecution
{
	/// <summary>
	/// Lidgren transport layer provider allowing to create client channel or server connection listener objects for given uri using Lidgren framework.
	/// </summary>
	public class LidgrenProvider : ITransportLayerProvider
	{
		private static readonly IMessageSerializer _serializer = new BinaryMessageSerializer();

		#region ITransportLayerProvider Members

		/// <summary>
		/// Creates client channel for given uri.
		/// This implementation supports scheme in following format: net://[host]:[port]/[applicationId]
		/// </summary>
		/// <param name="uri">Uri used to configure client channel.</param>
		/// <returns>Client channel.</returns>
		/// <exception cref="ArgumentException">Thrown when uri has wrong scheme or contains wrong content.</exception>
		public IClientChannel CreateClientChannelFor(Uri uri)
		{
			VerifyScheme(uri);
			return new LidgrenClientChannel(GetApplicationId(uri), uri.Host, GetPort(uri), _serializer);
		}

		/// <summary>
		/// Creates server connection listener for given uri.
		/// This implementation supports scheme in following format: net://[ip_to_listen_on]:[port]/[applicationId]
		/// [ip_to_listen_on] could be 0.0.0.0, which means that connection listener would be listening on all network interfaces.
		/// </summary>
		/// <param name="uri">Uri used to configure server connection listener.</param>
		/// <returns>Server connection listener</returns>
		/// <exception cref="ArgumentException">Thrown when uri has wrong scheme or contains wrong content.</exception>
		public IServerConnectionListener CreateConnectionListenerFor(Uri uri)
		{
			VerifyScheme(uri);
			return new LidgrenServerConnectionListener(GetApplicationId(uri), uri.Host, GetPort(uri), _serializer);
		}

		/// <summary>
		/// Returns supported scheme.
		/// This implementation supports net:// scheme.
		/// </summary>
		public string Scheme { get { return "net"; } }

		#endregion

		private string GetApplicationId(Uri uri)
		{
			var applicationId = uri.LocalPath.Trim('/');

			if (string.IsNullOrWhiteSpace(applicationId))
				throw new ArgumentException("No application id provided.");

			if (applicationId.Contains("/"))
				throw new ArgumentException("Application id cannot contain '/' character.");
			return applicationId;
		}

		private ushort GetPort(Uri uri)
		{
			if (uri.Port <= 0)
				throw new ArgumentException("No port provided.");
			return (ushort)uri.Port;
		}

		private void VerifyScheme(Uri uri)
		{
			if (uri.Scheme != Scheme)
				throw new ArgumentException("Invalid scheme.");
		}
	}
}