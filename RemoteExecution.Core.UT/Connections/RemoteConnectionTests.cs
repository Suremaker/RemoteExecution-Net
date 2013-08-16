using NUnit.Framework;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Connections;

namespace RemoteExecution.Core.UT.Connections
{
	[TestFixture]
	public class RemoteConnectionTests : ConnectionTestBase<IRemoteConnection, IDuplexChannel>
	{
		protected override IRemoteConnection CreateSubject()
		{
			return new RemoteConnection(Channel, RemoteExecutorFactory, OperationDispatcher);
		}
	}
}
