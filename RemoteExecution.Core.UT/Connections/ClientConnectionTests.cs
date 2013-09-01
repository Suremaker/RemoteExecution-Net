using NUnit.Framework;
using RemoteExecution.Channels;
using RemoteExecution.Config;
using RemoteExecution.Connections;
using Rhino.Mocks;

namespace RemoteExecution.Core.UT.Connections
{
	[TestFixture]
	public class ClientConnectionTests : ConnectionTestBase<IClientConnection, IClientChannel>
	{
		protected override IClientConnection CreateSubject()
		{
			return new ClientConnection(Channel, OperationDispatcher, new ConnectionConfig { RemoteExecutorFactory = RemoteExecutorFactory, TaskScheduler = Scheduler });
		}

		[Test]
		public void Should_use_channel_to_open_connection()
		{
			Subject.Open();
			Channel.AssertWasCalled(c => c.Open());
		}
	}
}