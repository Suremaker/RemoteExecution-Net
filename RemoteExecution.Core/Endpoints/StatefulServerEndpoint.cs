using RemoteExecution.Core.Config;
using RemoteExecution.Core.Connections;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Endpoints.Listeners;

namespace RemoteExecution.Core.Endpoints
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