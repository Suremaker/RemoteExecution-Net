using RemoteExecution.Channels;
using RemoteExecution.Config;
using RemoteExecution.Dispatchers;

namespace RemoteExecution.Connections
{
	public class ClientConnection : RemoteConnection, IClientConnection
	{
		public ClientConnection(string connectionUri)
			: base(connectionUri, new OperationDispatcher(), new ClientConfig())
		{
		}

		public ClientConnection(string connectionUri, IOperationDispatcher dispatcher)
			: base(connectionUri, dispatcher, new ClientConfig())
		{
		}

		public ClientConnection(string connectionUri, IClientConfig config)
			: base(connectionUri, new OperationDispatcher(), config)
		{
		}

		public ClientConnection(string connectionUri, IOperationDispatcher dispatcher, IClientConfig config)
			: base(connectionUri, dispatcher, config)
		{
		}

		public ClientConnection(IClientChannel channel, IOperationDispatcher dispatcher, IClientConfig config)
			: base(channel, dispatcher, config)
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