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
	public class RemoteConnection : IRemoteConnection
	{
		protected readonly IDuplexChannel Channel;
		private readonly ITaskScheduler _scheduler;
		public event Action Closed;

		public RemoteConnection(IDuplexChannel channel, IOperationDispatcher dispatcher, IClientConfig config)
		{
			OperationDispatcher = dispatcher;
			Channel = channel;
			_scheduler = config.TaskScheduler;
			RemoteExecutor = config.RemoteExecutorFactory.CreateRemoteExecutor(channel, dispatcher.MessageDispatcher);

			Channel.Received += OnMessageReceived;
			Channel.ChannelClosed += OnChannelClose;
		}

		public RemoteConnection(string connectionUri, IOperationDispatcher dispatcher, IClientConfig config)
			: this(TransportLayerResolver.CreateClientChannelFor(new Uri(connectionUri)), dispatcher, config)
		{
		}

		#region IRemoteConnection Members

		public void Dispose()
		{
			Channel.Dispose();
		}

		public IRemoteExecutor RemoteExecutor { get; private set; }
		public IOperationDispatcher OperationDispatcher { get; private set; }
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