using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using RemoteExecution.Connections;
using RemoteExecution.Dispatchers;
using RemoteExecution.Endpoints;
using RemoteExecution.Executors;
using RemoteExecution.IT.Services;

namespace RemoteExecution.IT
{
	[TestFixture]
	public class IntegrationTests
	{
		private TestableServerEndpoint _serverEndpoint;

		private const int _maxConnections = 10;
		private const ushort _port = 3232;
		private const string _appId = "testAppId";
		private const string _localhost = "localhost";

		[TestFixtureSetUp]
		public void SetUp()
		{
			_serverEndpoint = new TestableServerEndpoint(_appId, _maxConnections, _port);
			_serverEndpoint.StartListening();
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			_serverEndpoint.Dispose();
		}

		[Test]
		public void ShouldProperlyConnectAndDisconnectClient()
		{
			using (var client = new ClientEndpoint(_appId, new OperationDispatcher()))
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
			using (var clientConnection = client.ConnectTo(_localhost, _port))
			{
				Assert.That(clientConnection.RemoteExecutor.Create<IRemoteService>().Hello(), Is.EqualTo("world"));
			}
		}

		[Test]
		public void ShouldRetrieveProperConnectionId()
		{
			using (var client1 = new ClientEndpoint(_appId, new OperationDispatcher()))
			using (var client2 = new ClientEndpoint(_appId, new OperationDispatcher()))
			using (var clientConnection1 = client1.ConnectTo(_localhost, _port))
			using (var clientConnection2 = client2.ConnectTo(_localhost, _port))
			{
				Assert.That(clientConnection1.RemoteExecutor.Create<IRemoteService>().GetConnectionId(), Is.EqualTo(1));
				Assert.That(clientConnection2.RemoteExecutor.Create<IRemoteService>().GetConnectionId(), Is.EqualTo(2));
			}
		}

		[Test]
		public void ShouldServerReturnValueRetrievedFromClient()
		{
			const int baseValue = 32;
			var operationDispatcher = new OperationDispatcher();
			operationDispatcher.RegisterRequestHandler(LoggingProxy.For<IClientService>(new ClientService(baseValue), "CLIENT SERVICE"));

			using (var client = (IClientEndpoint)new ClientEndpoint(_appId, operationDispatcher))
			using (var clientConnection = client.ConnectTo(_localhost, _port))
			{
				Assert.That(clientConnection.RemoteExecutor.Create<IRemoteService>().ExecuteChainedMethod(), Is.EqualTo(baseValue * 2));
			}
		}

		[Test]
		public void ShouldCallOneWayMethodSynchronouslyByDefault()
		{
			var operationDispatcher = new OperationDispatcher();
			var clientService = new ClientService(33);
			operationDispatcher.RegisterRequestHandler(LoggingProxy.For<IClientService>(clientService, "CLIENT SERVICE"));

			using (var client = (IClientEndpoint)new ClientEndpoint(_appId, operationDispatcher))
			using (var clientConnection = client.ConnectTo(_localhost, _port))
			{
				var remoteService = LoggingProxy.For(clientConnection.RemoteExecutor.Create<IRemoteService>(), "CLIENT");
				var sleepTime = TimeSpan.FromSeconds(1);

				var watch = new Stopwatch();
				watch.Start();
				remoteService.Sleep(sleepTime);
				watch.Stop();
				Assert.That(watch.Elapsed, Is.GreaterThanOrEqualTo(sleepTime));
				Assert.That(clientService.TimeSpan, Is.EqualTo(sleepTime));
			}
		}

		[Test]
		public void ShouldCallOneWayMethodAsynchronously()
		{
			var operationDispatcher = new OperationDispatcher();
			var clientService = new ClientService(33);
			operationDispatcher.RegisterRequestHandler(LoggingProxy.For<IClientService>(clientService, "CLIENT SERVICE"));

			using (var client = (IClientEndpoint)new ClientEndpoint(_appId, operationDispatcher))
			using (var clientConnection = client.ConnectTo(_localhost, _port))
			{
				var remoteService = LoggingProxy.For(clientConnection.RemoteExecutor.Create<IRemoteService>(NoResultMethodExecution.OneWay), "CLIENT");
				var sleepTime = TimeSpan.FromSeconds(1);

				var watch = new Stopwatch();
				watch.Start();
				remoteService.Sleep(sleepTime);
				watch.Stop();
				Assert.That(watch.Elapsed, Is.LessThan(sleepTime));
				Assert.That(clientService.TimeSpan, Is.Not.EqualTo(sleepTime));

				Thread.Sleep(sleepTime.Add(TimeSpan.FromMilliseconds(100)));
				Assert.That(clientService.TimeSpan, Is.EqualTo(sleepTime));
			}
		}

		[Test]
		public void ShouldThrowExceptionFromRemoteOperation()
		{
			using (var client = new ClientEndpoint(_appId, new OperationDispatcher()))
			using (var clientConnection = client.ConnectTo(_localhost, _port))
			{
				var remoteService = clientConnection.RemoteExecutor.Create<IRemoteService>();

				var ex = Assert.Throws<MyException>(remoteService.ThrowException);
				Assert.That(ex.Message, Is.EqualTo("test"));
			}
		}

		[Test]
		public void ShouldThrowExceptionIfRemoteOperationIsCalledOnClosedConnection()
		{
			using (var client = new ClientEndpoint(_appId, new OperationDispatcher()))
			using (var clientConnection = client.ConnectTo(_localhost, _port))
			{
				var remoteService = clientConnection.RemoteExecutor.Create<IRemoteService>();
				_serverEndpoint.Dispose();
				Thread.Sleep(100);
				var ex = Assert.Throws<NotConnectedException>(() => remoteService.Hello());
				Assert.That(ex.Message, Is.EqualTo("Network connection is not opened."));
			}
		}

		[Test]
		public void ShouldThrowExceptionIfConnectionIsClosedDuringRemoteOperationCall()
		{
			using (var client = new ClientEndpoint(_appId, new OperationDispatcher()))
			using (var clientConnection = client.ConnectTo(_localhost, _port))
			{
				var remoteService = clientConnection.RemoteExecutor.Create<IRemoteService>();

				var ex = Assert.Throws<OperationAbortedException>(remoteService.CloseConnectionOnServerSide);
				Assert.That(ex.Message, Is.EqualTo("Network connection has been closed."));
			}
		}

		[Test]
		public void ShouldServerBroadcastMessage()
		{
			var clients = CreateClients(_maxConnections);
			try
			{
				const int expectedNumber = 55;

				clients[0].Item1.Connection.RemoteExecutor.Create<IRemoteService>().Broadcast(expectedNumber);
				Thread.Sleep(1000);
				Assert.That(clients.Count(c => c.Item2.ReceivedNumber == expectedNumber), Is.EqualTo(clients.Count));

			}
			finally
			{
				foreach (var client in clients)
					client.Item1.Dispose();
			}
		}

		private List<Tuple<ClientEndpoint, BroadcastService>> CreateClients(int count)
		{
			var clients = new List<Tuple<ClientEndpoint, BroadcastService>>();
			for (int i = 0; i < count; ++i)
			{
				var operationDispatcher = new OperationDispatcher();
				var broadcastService = new BroadcastService();
				operationDispatcher.RegisterRequestHandler(LoggingProxy.For<IBroadcastService>(broadcastService, "CLIENT SERVICE"));
				var client = new ClientEndpoint(_appId, operationDispatcher);

				clients.Add(new Tuple<ClientEndpoint, BroadcastService>(client, broadcastService));

				client.ConnectTo(_localhost, _port);
			}
			return clients;
		}
	}
}
