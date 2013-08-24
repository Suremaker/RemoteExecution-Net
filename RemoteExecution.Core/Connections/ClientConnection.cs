using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Executors;
using RemoteExecution.Core.Schedulers;

namespace RemoteExecution.Core.Connections
{
	public class ClientConnection : RemoteConnection, IClientConnection
	{
		public ClientConnection(string connectionUri, IRemoteExecutorFactory remoteExecutorFactory, IOperationDispatcher dispatcher, ITaskScheduler scheduler)
			: base(connectionUri, remoteExecutorFactory, dispatcher, scheduler)
		{
		}

		public ClientConnection(IClientChannel channel, IRemoteExecutorFactory remoteExecutorFactory, IOperationDispatcher dispatcher, ITaskScheduler scheduler)
			: base(channel, remoteExecutorFactory, dispatcher, scheduler)
		{
		}

		#region IClientConnection Members

		public void Open()
		{
			((IClientChannel)Channel).Open();
		}

		#endregion
	}
}