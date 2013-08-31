using NUnit.Framework;
using RemoteExecution.Channels;
using RemoteExecution.Config;
using RemoteExecution.Connections;

namespace RemoteExecution.Core.UT.Connections
{
	[TestFixture]
	public class RemoteConnectionTests : ConnectionTestBase<IRemoteConnection, IDuplexChannel>
	{
		protected override IRemoteConnection CreateSubject()
		{
			return new RemoteConnection(Channel, OperationDispatcher, new ClientConfig { RemoteExecutorFactory = RemoteExecutorFactory, TaskScheduler = Scheduler });
		}
	}
}
