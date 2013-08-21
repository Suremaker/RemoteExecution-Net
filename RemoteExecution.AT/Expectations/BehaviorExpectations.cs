using System;
using System.Linq;
using NUnit.Framework;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Connections;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Endpoints;
using RemoteExecution.Core.Endpoints.Listeners;
using RemoteExecution.Core.Executors;
using RemoteExecution.Core.Schedulers;
using RemoteExecution.Core.Serializers;
using RemoteExecution.Lidgren.Channels;
using RemoteExecution.Lidgren.Endpoints.Listeners;

namespace RemoteExecution.AT.Expectations
{
	public abstract class TestContext
	{
		protected IServerEndpoint StartServer()
		{
			IOperationDispatcher dispatcher = new OperationDispatcher();

			var server = new GenericServerEndpoint(CreateServerListener(), new ServerEndpointConfig(), () => dispatcher);
			server.Start();
			return server;
		}

		protected IClientConnection CreateClientConnection()
		{
			return new ClientConnection(CreateClientChannel(), new RemoteExecutorFactory(), new OperationDispatcher(), new AsyncTaskScheduler());
		}

		protected abstract IServerListener CreateServerListener();
		protected abstract IClientChannel CreateClientChannel();
	}

	public abstract class BehaviorExpectations : TestContext
	{
		[Test, Ignore("WIP")]
		public void Should_successfuly_connect_and_disconnect_client()
		{
			using (var server = StartServer())
			{
				using (var client = CreateClientConnection())
				{
					client.Open();

					Assert.That(client.IsOpen);
					Assert.That(server.ActiveConnections.Count(), Is.EqualTo(1));
				}
				Assert.That(server.ActiveConnections.Count(), Is.EqualTo(0));
			}
		}
	}

	[TestFixture]
	public class LidgrenProviderTests : BehaviorExpectations
	{
		private readonly string _applicationId = Guid.NewGuid().ToString();
		private const string _host = "localhost";
		private const ushort _port = 3251;

		protected override IServerListener CreateServerListener()
		{
			return new LidgrenServerListener(_applicationId, _port, new BinaryMessageSerializer());
		}

		protected override IClientChannel CreateClientChannel()
		{
			return new LidgrenClientChannel(_applicationId, _host, _port, new BinaryMessageSerializer());
		}
	}
}
