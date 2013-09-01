using System;
using RemoteExecution.Channels;
using RemoteExecution.Config;
using RemoteExecution.Dispatchers;
using RemoteExecution.Dispatchers.Messages;
using RemoteExecution.Executors;
using RemoteExecution.Schedulers;
using RemoteExecution.TransportLayer;

namespace RemoteExecution.Connections
{
	/// <summary>
	/// Remote connection class allowing to execute operations remotely or configure handlers for operations incoming from remote end.
	/// </summary>
	public class RemoteConnection : IRemoteConnection
	{
		/// <summary>
		/// Channel used by remote connection.
		/// </summary>
		protected readonly IDuplexChannel Channel;
		private readonly ITaskScheduler _scheduler;

		/// <summary>
		/// Fires when connection is closed on this or remote end.
		/// </summary>
		public event Action Closed;

		/// <summary>
		/// Creates instance of remote connection with given channel.
		/// </summary>
		/// <param name="channel">Communication channel used by connection.</param>
		/// <param name="dispatcher">Operation dispatcher used to handle incoming operation requests from remote end.</param>
		/// <param name="config">Connection configuration.</param>
		public RemoteConnection(IDuplexChannel channel, IOperationDispatcher dispatcher, IConnectionConfig config)
		{
			OperationDispatcher = dispatcher;
			Channel = channel;
			_scheduler = config.TaskScheduler;
			RemoteExecutor = config.RemoteExecutorFactory.CreateRemoteExecutor(channel, dispatcher.MessageDispatcher);

			Channel.Received += OnMessageReceived;
			Channel.Closed += OnChannelClose;
		}

		/// <summary>
		/// Creates instance of remote connection with channel constructed from connectionUri.
		/// </summary>
		/// <param name="connectionUri">Connection uri used to create channel.</param>
		/// <param name="dispatcher">Operation dispatcher used to handle incoming operation requests from remote end.</param>
		/// <param name="config">Connection configuration.</param>
		public RemoteConnection(string connectionUri, IOperationDispatcher dispatcher, IConnectionConfig config)
			: this(TransportLayerResolver.CreateClientChannelFor(new Uri(connectionUri)), dispatcher, config)
		{
		}

		#region IRemoteConnection Members

		/// <summary>
		/// Closes connection.
		/// </summary>
		public void Dispose()
		{
			Channel.Dispose();
		}

		/// <summary>
		/// Returns remote executor used for executing operations on remote end.
		/// </summary>
		public IRemoteExecutor RemoteExecutor { get; private set; }

		/// <summary>
		/// Returns operation dispatcher used for handling incoming operations.
		/// </summary>
		public IOperationDispatcher OperationDispatcher { get; private set; }

		/// <summary>
		/// Returns true if connection is opened, otherwise false.
		/// </summary>
		public bool IsOpen { get { return Channel.IsOpen; } }

		#endregion

		private void OnChannelClose()
		{
			OperationDispatcher.MessageDispatcher.GroupDispatch(Channel.Id, new ExceptionResponseMessage(string.Empty, typeof(OperationAbortedException), "Connection has been closed."));
			if (Closed != null)
				Closed();
		}

		private void OnMessageReceived(IMessage msg)
		{
			_scheduler.Execute(() => OperationDispatcher.MessageDispatcher.Dispatch(msg));
		}
	}
}