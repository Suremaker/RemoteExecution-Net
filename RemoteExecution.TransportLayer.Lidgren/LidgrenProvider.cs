using System;
using RemoteExecution.Channels;
using RemoteExecution.Endpoints.Listeners;
using RemoteExecution.Serializers;
using RemoteExecution.TransportLayer;

namespace RemoteExecution
{
	public class LidgrenProvider : ITransportLayerProvider
	{
		private static readonly IMessageSerializer _serializer = new BinaryMessageSerializer();

		#region ITransportLayerProvider Members

		public IClientChannel CreateClientChannelFor(Uri uri)
		{
			VerifyScheme(uri);
			return new LidgrenClientChannel(GetApplicationId(uri), uri.Host, GetPort(uri), _serializer);
		}

		public IServerConnectionListener CreateConnectionListenerFor(Uri uri)
		{
			VerifyScheme(uri);
			return new LidgrenServerConnectionListener(GetApplicationId(uri), uri.Host, GetPort(uri), _serializer);
		}

		public string Scheme { get { return "net"; } }

		#endregion

		private string GetApplicationId(Uri uri)
		{
			var applicationId = uri.LocalPath.Trim('/');

			if (string.IsNullOrWhiteSpace(applicationId))
				throw new ArgumentException("No application id provided.");

			if(applicationId.Contains("/"))
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