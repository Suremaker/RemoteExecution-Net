using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Dispatchers.Messages;
using RemoteExecution.Core.Executors;

namespace RemoteExecution.Core.Connections
{
	public class RemoteConnection : IRemoteConnection
	{
		private readonly IDuplexChannel _channel;

		public RemoteConnection(IDuplexChannel channel, IRemoteExecutorFactory remoteExecutorFactory, IOperationDispatcher dispatcher)
		{
			Dispatcher = dispatcher;
			_channel = channel;
			Executor = remoteExecutorFactory.CreateRemoteExecutor(channel, dispatcher.MessageDispatcher);

			_channel.Received += OnMessageReceived;
			_channel.ChannelClosed += OnChannelClose;
		}

		private void OnChannelClose()
		{
			Dispatcher.MessageDispatcher.GroupDispatch(_channel.Id, new ExceptionResponseMessage(string.Empty, typeof(OperationAbortedException), "Connection has been closed."));
		}

		private void OnMessageReceived(IMessage msg)
		{
			Dispatcher.MessageDispatcher.Dispatch(msg);
		}

		#region IRemoteConnection Members

		public void Dispose()
		{
			_channel.Dispose();
		}

		public IRemoteExecutor Executor { get; private set; }
		public IOperationDispatcher Dispatcher { get; private set; }
		public bool IsOpen { get { return _channel.IsOpen; } }

		#endregion
	}
}