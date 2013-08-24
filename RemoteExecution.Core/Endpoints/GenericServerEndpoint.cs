using System;
using RemoteExecution.Core.Connections;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Endpoints.Listeners;

namespace RemoteExecution.Core.Endpoints
{
	public class GenericServerEndpoint : ServerEndpoint
	{
		private readonly Func<IOperationDispatcher> _operationDispatcherCreator;

		public GenericServerEndpoint(string listenerUri, IServerEndpointConfig config, Func<IOperationDispatcher> operationDispatcherCreator, Action<IServerEndpoint, IRemoteConnection> connectionInitializer = null)
			: base(listenerUri, config)
		{
			_operationDispatcherCreator = operationDispatcherCreator;
			if (connectionInitializer != null)
				OnConnectionInitialize += connection => connectionInitializer(this, connection);
		}

		public GenericServerEndpoint(IServerConnectionListener listener, IServerEndpointConfig config, Func<IOperationDispatcher> operationDispatcherCreator, Action<IServerEndpoint, IRemoteConnection> connectionInitializer = null)
			: base(listener, config)
		{
			_operationDispatcherCreator = operationDispatcherCreator;
			if (connectionInitializer != null)
				OnConnectionInitialize += connection => connectionInitializer(this, connection);
		}

		protected override IOperationDispatcher GetOperationDispatcher()
		{
			return _operationDispatcherCreator.Invoke();
		}
	}
}