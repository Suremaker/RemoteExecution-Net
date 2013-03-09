using System.Threading;
using NUnit.Framework;
using RemoteExecution.Dispatching;
using RemoteExecution.Endpoints;
using RemoteExecution.Endpoints.Processing;
using RemoteExecution.IT.Services;

namespace RemoteExecution.IT
{
	[TestFixture]
	public class IntegrationTests
	{
		private TestableServerEndpoint _serverEndpoint;

		private IMessageLoop _serverLoop;
		private const int _maxConnections = 2;
		private const ushort _port = 3232;
		private const string _appId = "testAppId";
		private const string _localhost = "localhost";

		[TestFixtureSetUp]
		public void SetUp()
		{
			_serverEndpoint = new TestableServerEndpoint(_appId, _maxConnections, _port);
			_serverEndpoint.StartListening();
			_serverLoop = new MessageLoop(_serverEndpoint);
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			_serverLoop.Dispose();
			_serverEndpoint.Dispose();
		}

		[Test]
		public void ShouldProperlyConnectAndDisconnectClient()
		{
			using (var client = new ClientEndpoint(_appId, new OperationDispatcher()))
			using (new MessageLoop(client))
			using (var clientConnection = client.ConnectTo(_localhost, _port))
			{
				Assert.That(clientConnection.IsOpen, Is.True);
				Assert.That(_serverEndpoint.ActiveConnections.Count, Is.EqualTo(1));
			}
			Thread.Sleep(250);
			Assert.That(_serverEndpoint.ActiveConnections.Count, Is.EqualTo(0));
		}

		[Test]
		public void ShouldCallRemoteOperation()
		{
			using (var client = new ClientEndpoint(_appId, new OperationDispatcher()))
			using (new MessageLoop(client))
			using (var clientConnection = client.ConnectTo(_localhost, _port))
			{
				var remoteExecutor = new RemoteExecutor(clientConnection);
				Assert.That(remoteExecutor.Create<IRemoteService>().Hello(), Is.EqualTo("world"));
			}
		}

		[Test]
		public void ShouldRetrieveProperConnectionId()
		{
			using (var client1 = new ClientEndpoint(_appId, new OperationDispatcher()))
			using (var client2 = new ClientEndpoint(_appId, new OperationDispatcher()))
			using (new MessageLoop(client1))
			using (new MessageLoop(client2))
			using (var clientConnection1 = client1.ConnectTo(_localhost, _port))
			using (var clientConnection2 = client2.ConnectTo(_localhost, _port))
			{
				Assert.That(new RemoteExecutor(clientConnection1).Create<IRemoteService>().GetConnectionId(), Is.EqualTo(1));
				Assert.That(new RemoteExecutor(clientConnection2).Create<IRemoteService>().GetConnectionId(), Is.EqualTo(2));
			}
		}

		[Test]
		public void ShouldServerReturnValueRetrievedFromClient()
		{
			const int baseValue = 32;
			var operationDispatcher = new OperationDispatcher();
			operationDispatcher.RegisterFor(LoggingProxy.For<IClientService>(new ClientService(baseValue)));

			using (var client = (IClientEndpoint)new ClientEndpoint(_appId, operationDispatcher))
			using (new MessageLoop(client))
			using (var clientConnection = client.ConnectTo(_localhost, _port))
			{
				var remoteExecutor = new RemoteExecutor(clientConnection);
				Assert.That(remoteExecutor.Create<IRemoteService>().ExecuteChainedMethod(), Is.EqualTo(baseValue * 2));
			}
		}
	}
}
