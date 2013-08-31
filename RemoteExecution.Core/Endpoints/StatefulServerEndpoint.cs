using RemoteExecution.Config;
using RemoteExecution.Connections;
using RemoteExecution.Dispatchers;
using RemoteExecution.Endpoints.Listeners;

namespace RemoteExecution.Endpoints
{
	public abstract class StatefulServerEndpoint : ServerEndpoint
	{
		protected StatefulServerEndpoint(IServerConnectionListener listener, IServerConfig config)
			: base(listener, config)
		{
			OnConnectionInitialize += InitializeConnection;
		}

		protected StatefulServerEndpoint(string listenerUri, IServerConfig config)
			: base(listenerUri, config)
		{
			OnConnectionInitialize += InitializeConnection;
		}

		protected override IOperationDispatcher GetOperationDispatcher()
		{
			return new OperationDispatcher();
		}

		protected abstract void InitializeConnection(IRemoteConnection connection);
	}
}