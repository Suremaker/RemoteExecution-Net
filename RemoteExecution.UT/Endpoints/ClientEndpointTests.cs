using System;
using NUnit.Framework;
using RemoteExecution.Connections;
using RemoteExecution.Dispatchers;
using RemoteExecution.Endpoints;
using RemoteExecution.Endpoints.Adapters;
using RemoteExecution.Executors;
using Rhino.Mocks;

namespace RemoteExecution.UT.Endpoints
{
	[TestFixture]
	public class ClientEndpointTests
	{
		private ClientEndpoint _subject;
		private IClientEndpointAdapter _endpointAdapter;
		private IOperationDispatcher _operationDispatcher;
		private Action<INetworkConnection> _newConnectionHandler;

		[SetUp]
		public void SetUp()
		{
			_endpointAdapter = MockRepository.GenerateMock<IClientEndpointAdapter>();
			_operationDispatcher = MockRepository.GenerateMock<IOperationDispatcher>();

			_newConnectionHandler = null;
			_endpointAdapter.Stub(a => a.NewConnectionHandler = null).IgnoreArguments().WhenCalled(SetConnectionHandler);

			_subject = new ClientEndpoint(_endpointAdapter, _operationDispatcher);
		}

		private void SetConnectionHandler(MethodInvocation mi)
		{
			_newConnectionHandler = (Action<INetworkConnection>)mi.Arguments[0];
		}

		[Test]
		public void Should_connect_to_using_adapter()
		{
			const string host = "localhost";
			const ushort port = 5555;
			StubConnectToReturnActiveConnection();

			_subject.ConnectTo(host, port);
			_endpointAdapter.AssertWasCalled(a => a.ConnectTo(host, port));
		}

		[Test]
		public void Should_return_active_connection()
		{
			var connection = StubConnectToReturnActiveConnection();

			Assert.That(_subject.ConnectTo("localhost", 5555), Is.SameAs(connection));
			Assert.That(_subject.Connection, Is.SameAs(connection));
		}

		[Test]
		public void Should_return_remote_executor_of_opened_connection()
		{
			var remoteExecutor = MockRepository.GenerateMock<IRemoteExecutor>();
			var connection = StubConnectToReturnActiveConnection();
			connection.Stub(c => c.RemoteExecutor).Return(remoteExecutor);

			_subject.ConnectTo("localhost", 5555);
			Assert.That(_subject.RemoteExecutor, Is.SameAs(remoteExecutor));
		}

		private INetworkConnection StubConnectToReturnActiveConnection()
		{
			var connection = MockRepository.GenerateMock<INetworkConnection>();
			_endpointAdapter.Stub(a => a.ConnectTo(Arg<string>.Is.Anything, Arg<ushort>.Is.Anything)).WhenCalled(inv => _newConnectionHandler(connection));
			return connection;
		}
	}
}