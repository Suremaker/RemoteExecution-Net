using NUnit.Framework;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Config;
using RemoteExecution.Core.Connections;

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
