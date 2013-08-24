using System;
using RemoteExecution.Core.Channels;
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

		public RemoteConnection(IDuplexChannel channel, IRemoteExecutorFactory remoteExecutorFactory, IOperationDispatcher dispatcher, ITaskScheduler scheduler)
		{
			Dispatcher = dispatcher;
			Channel = channel;
			_scheduler = scheduler;
			Executor = remoteExecutorFactory.CreateRemoteExecutor(channel, dispatcher.MessageDispatcher);

			Channel.Received += OnMessageReceived;
			Channel.ChannelClosed += OnChannelClose;
		}

		public RemoteConnection(string connectionUri, IRemoteExecutorFactory remoteExecutorFactory, IOperationDispatcher dispatcher, ITaskScheduler scheduler)
			: this(TransportLayerResolver.CreateClientChannelFor(new Uri(connectionUri)), remoteExecutorFactory, dispatcher, scheduler)
		{
		}

		#region IRemoteConnection Members

		public void Dispose()
		{
			Channel.Dispose();
		}

		public IRemoteExecutor Executor { get; private set; }
		public IOperationDispatcher Dispatcher { get; private set; }
		public bool IsOpen { get { return Channel.IsOpen; } }

		#endregion

		private void OnChannelClose()
		{
			Dispatcher.MessageDispatcher.GroupDispatch(Channel.Id, new ExceptionResponseMessage(string.Empty, typeof(OperationAbortedException), "Connection has been closed."));
			if (Closed != null)
				Closed();
		}

		private void OnMessageReceived(IMessage msg)
		{
			_scheduler.Execute(() => Dispatcher.MessageDispatcher.Dispatch(msg));
		}
	}
}