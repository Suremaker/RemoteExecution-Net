using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Dispatchers.Messages;
using RemoteExecution.Core.Executors;

namespace RemoteExecution.Core.Connections
{
	public class RemoteConnection : IRemoteConnection
	{
		protected readonly IDuplexChannel Channel;

		public RemoteConnection(IDuplexChannel channel, IRemoteExecutorFactory remoteExecutorFactory, IOperationDispatcher dispatcher)
		{
			Dispatcher = dispatcher;
			Channel = channel;
			Executor = remoteExecutorFactory.CreateRemoteExecutor(channel, dispatcher.MessageDispatcher);

			Channel.Received += OnMessageReceived;
			Channel.ChannelClosed += OnChannelClose;
		}

		private void OnChannelClose()
		{
			Dispatcher.MessageDispatcher.GroupDispatch(Channel.Id, new ExceptionResponseMessage(string.Empty, typeof(OperationAbortedException), "Connection has been closed."));
		}

		private void OnMessageReceived(IMessage msg)
		{
			Dispatcher.MessageDispatcher.Dispatch(msg);
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
	}
}