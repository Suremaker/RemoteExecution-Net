using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using RemoteExecution.Dispatching;
using RemoteExecution.Endpoints;
using RemoteExecution.IT.Services;

namespace RemoteExecution.IT
{
	[TestFixture]
	public class IntegrationTests
	{
		private TestableServerEndpoint _serverEndpoint;

		private const int _maxConnections = 2;
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
				var remoteExecutor = new RemoteExecutor(clientConnection);
				Assert.That(remoteExecutor.Create<IRemoteService>().Hello(), Is.EqualTo("world"));
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
				Assert.That(new RemoteExecutor(clientConnection1).Create<IRemoteService>().GetConnectionId(), Is.EqualTo(1));
				Assert.That(new RemoteExecutor(clientConnection2).Create<IRemoteService>().GetConnectionId(), Is.EqualTo(2));
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
				var remoteExecutor = new RemoteExecutor(clientConnection);
				Assert.That(remoteExecutor.Create<IRemoteService>().ExecuteChainedMethod(), Is.EqualTo(baseValue * 2));
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
				var remoteService = LoggingProxy.For(new RemoteExecutor(clientConnection).Create<IRemoteService>(), "CLIENT");
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
				var remoteService = LoggingProxy.For(new RemoteExecutor(clientConnection).Create<IRemoteService>(OneWayMethodExcecution.Asynchronized), "CLIENT");
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
				var remoteService = new RemoteExecutor(clientConnection).Create<IRemoteService>();

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
				var remoteService = new RemoteExecutor(clientConnection).Create<IRemoteService>();
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
				var remoteService = new RemoteExecutor(clientConnection).Create<IRemoteService>();

				var ex = Assert.Throws<OperationAbortedException>(remoteService.CloseConnectionOnServerSide);
				Assert.That(ex.Message, Is.EqualTo("Network connection has been closed."));
			}
		}
	}
}
