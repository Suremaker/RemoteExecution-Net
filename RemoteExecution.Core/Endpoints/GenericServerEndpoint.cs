using System;
using RemoteExecution.Core.Connections;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Endpoints.Listeners;

namespace RemoteExecution.Core.Endpoints
{
	public class GenericServerEndpoint : ServerEndpoint
	{
		private readonly Func<IOperationDispatcher> _operationDispatcherCreator;

		public GenericServerEndpoint(IServerListener listener, IServerEndpointConfig config, Func<IOperationDispatcher> operationDispatcherCreator, Action<IRemoteConnection> connectionInitializer = null)
			: base(listener, config)
		{
			_operationDispatcherCreator = operationDispatcherCreator;
			if (connectionInitializer != null)
				OnConnectionInitialize += connectionInitializer;
		}

		protected override IOperationDispatcher GetOperationDispatcher()
		{
			return _operationDispatcherCreator.Invoke();
		}
	}
}