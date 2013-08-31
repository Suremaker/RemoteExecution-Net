using NUnit.Framework;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Config;
using RemoteExecution.Core.Connections;
using Rhino.Mocks;

namespace RemoteExecution.Core.UT.Connections
{
	[TestFixture]
	public class ClientConnectionTests : ConnectionTestBase<IClientConnection, IClientChannel>
	{
		protected override IClientConnection CreateSubject()
		{
			return new ClientConnection(Channel, OperationDispatcher, new ClientConfig { RemoteExecutorFactory = RemoteExecutorFactory, TaskScheduler = Scheduler });
		}

		[Test]
		public void Should_use_channel_to_open_connection()
		{
			Subject.Open();
			Channel.AssertWasCalled(c => c.Open());
		}
	}
}