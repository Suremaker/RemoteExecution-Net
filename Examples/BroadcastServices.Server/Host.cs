using BroadcastServices.Contracts;
using RemoteExecution;
using RemoteExecution.Endpoints;

namespace BroadcastServices.Server
{
	class Host : ServerEndpoint
	{
		readonly SharedContext _sharedContext = new SharedContext();
		private readonly IBroadcastService _broadcastService;

		public Host(int maxConnections, ushort port)
			: base(Protocol.Id, maxConnections, port)
		{
			_broadcastService = new BroadcastRemoteExecutor(BroadcastChannel).Create<IBroadcastService>();
		}

		protected override bool HandleNewConnection(IConfigurableNetworkConnection connection)
		{
			var userContext = new UserContext();
			_sharedContext.AddClient(connection, userContext);
			connection.OperationDispatcher.RegisterRequestHandler<IRegistrationService>(new RegistrationService(userContext,_broadcastService));
			connection.OperationDispatcher.RegisterRequestHandler<IUserInfoService>(new UserInfoService(_sharedContext,userContext));
			return true;
		}

		protected override void OnConnectionClose(INetworkConnection connection)
		{
			var user = _sharedContext.GetUser(connection);
			_sharedContext.RemoveClient(connection);
			if(user.IsRegistered)
				_broadcastService.UserLeft(user.Name);
		}
	}
}