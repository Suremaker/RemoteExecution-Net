using System;
using RemoteExecution.Core.Config;
using RemoteExecution.Core.Connections;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Endpoints.Listeners;

namespace RemoteExecution.Core.Endpoints
{
	public class GenericServerEndpoint : ServerEndpoint
	{
		private Func<IOperationDispatcher> _operationDispatcherCreator;

		public GenericServerEndpoint(string listenerUri, Func<IOperationDispatcher> operationDispatcherCreator, Action<IServerEndpoint, IRemoteConnection> connectionInitializer = null)
			: this(listenerUri, new ServerConfig(), operationDispatcherCreator, connectionInitializer) { }

		public GenericServerEndpoint(string listenerUri, IServerConfig config, Func<IOperationDispatcher> operationDispatcherCreator, Action<IServerEndpoint, IRemoteConnection> connectionInitializer = null)
			: base(listenerUri, config)
		{
			Initialize(operationDispatcherCreator, connectionInitializer);
		}

		public GenericServerEndpoint(IServerConnectionListener listener, Func<IOperationDispatcher> operationDispatcherCreator, Action<IServerEndpoint, IRemoteConnection> connectionInitializer = null)
			: this(listener, new ServerConfig(), operationDispatcherCreator, connectionInitializer) { }

		public GenericServerEndpoint(IServerConnectionListener listener, IServerConfig config, Func<IOperationDispatcher> operationDispatcherCreator, Action<IServerEndpoint, IRemoteConnection> connectionInitializer = null)
			: base(listener, config)
		{
			Initialize(operationDispatcherCreator, connectionInitializer);
		}

		protected override IOperationDispatcher GetOperationDispatcher()
		{
			return _operationDispatcherCreator.Invoke();
		}

		private void Initialize(Func<IOperationDispatcher> operationDispatcherCreator, Action<IServerEndpoint, IRemoteConnection> connectionInitializer)
		{
			_operationDispatcherCreator = operationDispatcherCreator;
			if (connectionInitializer != null)
				OnConnectionInitialize += connection => connectionInitializer(this, connection);
		}
	}
}