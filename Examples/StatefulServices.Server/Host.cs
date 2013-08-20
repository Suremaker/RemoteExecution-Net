using RemoteExecution.Connections;
using RemoteExecution.Dispatchers;
using RemoteExecution.Endpoints;
using StatefulServices.Contracts;

namespace StatefulServices.Server
{
	class Host : ServerEndpoint
	{
		readonly SharedContext _sharedContext = new SharedContext();

		public Host(ServerEndpointConfig config)
			: base(config)
		{
		}

		protected override IOperationDispatcher GetDispatcherForNewConnection()
		{
			return new OperationDispatcher();
		}

		protected override void OnConnectionClose(INetworkConnection connection)
		{
			_sharedContext.RemoveClient(connection);
		}

		protected override void OnNewConnection(INetworkConnection connection)
		{
			var clientContext = new ClientContext();
			_sharedContext.AddClient(connection, clientContext);

			connection.OperationDispatcher
			          .RegisterRequestHandler<IRegistrationService>(new RegistrationService(clientContext))
			          .RegisterRequestHandler<IUserInfoService>(new UserInfoService(_sharedContext, clientContext));
		}
	}
}