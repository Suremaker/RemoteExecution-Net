using System;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Endpoints.Listeners;
using RemoteExecution.Core.Serializers;
using RemoteExecution.Core.TransportLayer;
using RemoteExecution.TransportLayer.Lidgren.Channels;
using RemoteExecution.TransportLayer.Lidgren.Endpoints.Listeners;

namespace RemoteExecution.TransportLayer.Lidgren
{
	public class LidgrenProvider : ITransportLayerProvider
	{
		private static readonly IMessageSerializer _serializer = new BinaryMessageSerializer();

		public IClientChannel CreateClientChannelFor(Uri uri)
		{
			VerifyScheme(uri);
			return new LidgrenClientChannel(GetApplicationId(uri), uri.Host, GetPort(uri), _serializer);
		}

		private ushort GetPort(Uri uri)
		{
			if (uri.Port <= 0)
				throw new ArgumentException("No port provided.");
			return (ushort)uri.Port;
		}

		private string GetApplicationId(Uri uri)
		{
			var applicationId = uri.LocalPath.Trim('/');

			if (string.IsNullOrWhiteSpace(applicationId))
				throw new ArgumentException("No application id provided.");

			if(applicationId.Contains("/"))
				throw new ArgumentException("Application id cannot contain '/' character.");
			return applicationId;
		}

		public IServerConnectionListener CreateConnectionListenerFor(Uri uri)
		{
			VerifyScheme(uri);
			return new LidgrenServerConnectionListener(GetApplicationId(uri), uri.Host, GetPort(uri), _serializer);
		}

		private void VerifyScheme(Uri uri)
		{
			if (uri.Scheme != Scheme)
				throw new ArgumentException("Invalid scheme.");
		}

		public string Scheme { get { return "net"; } }
	}
}