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
			var userContext = new UserContext();
			_sharedContext.AddClient(connection, userContext);
			connection.OperationDispatcher.RegisterRequestHandler<IRegistrationService>(new RegistrationService(userContext));
			connection.OperationDispatcher.RegisterRequestHandler<IUserInfoService>(new UserInfoService(_sharedContext,userContext));
			return true;
		}

		protected override void OnConnectionClose(INetworkConnection connection)
		{
			_sharedContext.RemoveClient(connection);
		}
	}
}