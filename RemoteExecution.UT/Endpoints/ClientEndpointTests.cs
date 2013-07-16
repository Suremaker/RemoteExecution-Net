using NUnit.Framework;
using RemoteExecution.Connections;
using RemoteExecution.Endpoints;
using RemoteExecution.Endpoints.Adapters;
using Rhino.Mocks;

namespace RemoteExecution.UT.Endpoints
{
	[TestFixture]
	public class ClientEndpointTests
	{
		private ClientEndpoint _subject;
		private IClientEndpointAdapter _endpointAdapter;

		[SetUp]
		public void SetUp()
		{
			_endpointAdapter = MockRepository.GenerateMock<IClientEndpointAdapter>();
			_subject = new ClientEndpoint(_endpointAdapter);
		}

		[Test]
		public void Should_connect_to_using_adapter()
		{
			const string host = "localhost";
			const ushort port = 5555;
			StubActiveConnection();

			_subject.ConnectTo(host, port);
			_endpointAdapter.AssertWasCalled(a => a.ConnectTo(host, port));
		}

		[Test]
		public void Should_return_active_connection()
		{
			var connection = StubActiveConnection();

			Assert.That(_subject.Connection, Is.SameAs(connection));
		}

		private INetworkConnection StubActiveConnection()
		{
			var connection = MockRepository.GenerateMock<INetworkConnection>();
			_endpointAdapter.Stub(a => a.ActiveConnections).Return(new[] { connection });
			return connection;
		}
	}
}