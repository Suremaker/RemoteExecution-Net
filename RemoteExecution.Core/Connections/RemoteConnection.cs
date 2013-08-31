using System;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Config;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Dispatchers.Messages;
using RemoteExecution.Core.Executors;
using RemoteExecution.Core.Schedulers;
using RemoteExecution.Core.TransportLayer;

namespace RemoteExecution.Core.Connections
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