using Lidgren.Network;
using RemoteExecution.Channels;
using RemoteExecution.Dispatchers;
using RemoteExecution.Messages;

namespace RemoteExecution.Connections
{
	internal class LidgrenNetworkConnection : INetworkConnection
	{
		public LidgrenNetworkConnection(NetConnection connection, IOperationDispatcher operationDispatcher)
		{
			Channel = new LidgrenMessageChannel(connection);
			Channel.Received += DispatchMessage;

			OperationDispatcher = operationDispatcher;
			RemoteExecutor = new RemoteExecutor(operationDispatcher, Channel);
		}

		public void Dispose()
		{
			OperationDispatcher.DispatchAbortResponsesFor(Channel, "Network connection has been closed.");
			Channel.Dispose();
			Channel.Received -= DispatchMessage;
		}

		public bool IsOpen { get { return Channel.IsOpen; } }
		public IOperationDispatcher OperationDispatcher { get; set; }
		public IRemoteExecutor RemoteExecutor { get; private set; }
		public LidgrenMessageChannel Channel { get; private set; }

		public void DispatchMessage(IMessage message)
		{
			OperationDispatcher.Dispatch(message, Channel);
		}
	}
}