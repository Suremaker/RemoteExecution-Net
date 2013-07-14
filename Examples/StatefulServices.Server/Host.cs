using RemoteExecution.Endpoints;
using StatefulServices.Contracts;

namespace StatefulServices.Server
{
	class Host : ServerEndpoint
	{
		readonly SharedContext _sharedContext = new SharedContext();

		public Host(int maxConnections, ushort port)
			: base(Protocol.Id, maxConnections, port)
		{
		}

		protected override bool HandleNewConnection(IConfigurableNetworkConnection connection)
		{
			var clientContext = new ClientContext();
			_sharedContext.AddClient(connection, clientContext);
			connection.OperationDispatcher.RegisterRequestHandler<IRegistrationService>(new RegistrationService(clientContext));
			connection.OperationDispatcher.RegisterRequestHandler<IUserInfoService>(new UserInfoService(_sharedContext, clientContext));
			return true;
		}

		protected override void OnConnectionClose(INetworkConnection connection)
		{
			_sharedContext.RemoveClient(connection);
		}
	}
}