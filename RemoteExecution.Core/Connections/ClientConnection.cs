using RemoteExecution.Channels;
using RemoteExecution.Config;
using RemoteExecution.Dispatchers;

namespace RemoteExecution.Connections
{
	/// <summary>
	/// Client connection class allowing to execute operations remotely 
	/// or configure handlers for operations incoming from remote end.
	/// Client connection has to be opened with Open() method before use 
	/// and can be reopened after it has been closed.
	/// </summary>
	public class ClientConnection : RemoteConnection, IClientConnection
	{
		/// <summary>
		/// Creates client connection instance with channel constructed from connectionUri, default operation dispatcher and connection configuration (<see cref="DefaultConfig"/>).
		/// </summary>
		/// <param name="connectionUri">Connection uri used to create channel.</param>
		public ClientConnection(string connectionUri)
			: base(connectionUri, new OperationDispatcher(), new ConnectionConfig())
		{
		}

		/// <summary>
		/// Creates client connection instance with channel constructed from connectionUri, given operation dispatcher and default connection configuration (<see cref="DefaultConfig"/>).
		/// </summary>
		/// <param name="connectionUri">Connection uri used to create channel.</param>
		/// <param name="dispatcher">Operation dispatcher used to handle incoming operation requests from remote end.</param>
		public ClientConnection(string connectionUri, IOperationDispatcher dispatcher)
			: base(connectionUri, dispatcher, new ConnectionConfig())
		{
		}
		/// <summary>
		/// Creates client connection instance with channel constructed from connectionUri, given connection configuration and default operation dispatcher.
		/// </summary>
		/// <param name="connectionUri">Connection uri used to create channel.</param>
		/// <param name="config">Connection configuration.</param>
		public ClientConnection(string connectionUri, IConnectionConfig config)
			: base(connectionUri, new OperationDispatcher(), config)
		{
		}
		/// <summary>
		/// Creates client connection instance with channel constructed from connectionUri, given connection configuration and operation dispatcher.
		/// </summary>
		/// <param name="connectionUri">Connection uri used to create channel.</param>
		/// <param name="dispatcher">Operation dispatcher used to handle incoming operation requests from remote end.</param>
		/// <param name="config">Connection configuration.</param>
		public ClientConnection(string connectionUri, IOperationDispatcher dispatcher, IConnectionConfig config)
			: base(connectionUri, dispatcher, config)
		{
		}
		/// <summary>
		/// Creates client connection instance with given channel, connection configuration and operation dispatcher.
		/// </summary>
		/// <param name="channel">Communication channel used by connection.</param>
		/// <param name="dispatcher">Operation dispatcher used to handle incoming operation requests from remote end.</param>
		/// <param name="config">Connection configuration.</param>
		public ClientConnection(IClientChannel channel, IOperationDispatcher dispatcher, IConnectionConfig config)
			: base(channel, dispatcher, config)
		{
		}

		#region IClientConnection Members

		/// <summary>
		/// Opens connection and make it ready for sending and handling operation requests.
		/// If connection has been closed, this method reopens it.
		/// </summary>
		public void Open()
		{
			((IClientChannel)Channel).Open();
		}

		#endregion
	}
}