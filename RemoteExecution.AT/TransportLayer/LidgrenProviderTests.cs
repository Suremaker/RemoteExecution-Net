using System;
using NUnit.Framework;
using RemoteExecution.AT.Expectations;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Endpoints.Listeners;
using RemoteExecution.Core.Serializers;
using RemoteExecution.TransportLayer.Lidgren.Channels;
using RemoteExecution.TransportLayer.Lidgren.Endpoints.Listeners;

namespace RemoteExecution.AT.TransportLayer
{
	[TestFixture]
	public class LidgrenProviderTests : BehaviorExpectations
	{
		private readonly string _applicationId = Guid.NewGuid().ToString();
		private const string _host = "localhost";
		private const ushort _port = 3251;

		protected override IServerConnectionListener CreateServerListener()
		{
			return new LidgrenServerConnectionListener(_applicationId, _port, new BinaryMessageSerializer());
		}

		protected override IClientChannel CreateClientChannel()
		{
			return new LidgrenClientChannel(_applicationId, _host, _port, new BinaryMessageSerializer());
		}
	}
}