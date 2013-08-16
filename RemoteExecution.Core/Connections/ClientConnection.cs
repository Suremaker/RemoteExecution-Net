using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Executors;

namespace RemoteExecution.Core.Connections
{
	public class ClientConnection : RemoteConnection, IClientConnection
	{
		public ClientConnection(IClientChannel channel, IRemoteExecutorFactory remoteExecutorFactory, IOperationDispatcher dispatcher)
			: base(channel, remoteExecutorFactory, dispatcher)
		{
		}

		public void Open()
		{
			((IClientChannel)Channel).Open();
		}
	}
}