using System;
using NUnit.Framework;
using RemoteExecution.Core.Connections;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Endpoints;
using RemoteExecution.Core.Endpoints.Listeners;
using RemoteExecution.Core.Executors;
using RemoteExecution.Core.Schedulers;
using RemoteExecution.Core.Serializers;
using RemoteExecution.TransportLayer.Lidgren.Channels;
using RemoteExecution.TransportLayer.Lidgren.Endpoints.Listeners;

namespace RemoteExecution.TransportLayer.Lidgren.IT
{
	[TestFixture]
	public class IntegrationTests
	{
		private IServerConnectionListener _connectionListener;
		private readonly string _applicationId = Guid.NewGuid().ToString();
		private IServerEndpoint _server;
		private IOperationDispatcher _dispatcher;
		private const string _host = "localhost";
		private const ushort _port = 3231;

		[TestFixtureSetUp]
		public void SetUp()
		{
			_connectionListener = new LidgrenServerConnectionListener(_applicationId, "*", _port, new BinaryMessageSerializer());

			_dispatcher = new OperationDispatcher();
			_dispatcher.RegisterHandler<ICalculator>(new Calculator());
			_dispatcher.RegisterHandler<IGreeter>(new Greeter());

			_server = new GenericServerEndpoint(_connectionListener, new ServerEndpointConfig(), () => _dispatcher);
			_server.Start();
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			_server.Dispose();
		}

		private ClientConnection CreateClientConnection()
		{
			return new ClientConnection(new LidgrenClientChannel(_applicationId, _host, _port, new BinaryMessageSerializer()), new RemoteExecutorFactory(), new OperationDispatcher(), new AsyncTaskScheduler());
		}

		[Test]
		public void Should_execute_remote_operations()
		{
			using (var client = CreateClientConnection())
			{
				client.Open();
				Assert.That(client.Executor.Create<ICalculator>().Add(3, 2), Is.EqualTo(5));
				Assert.That(client.Executor.Create<IGreeter>().Hello("Josh"), Is.EqualTo("Hello Josh!"));
			}
		}
	}
}
