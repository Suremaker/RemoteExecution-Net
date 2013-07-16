using System;
using BroadcastServices.Contracts;
using Examples.Utils;
using RemoteExecution.Connections;
using RemoteExecution.Dispatchers;
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
			_broadcastService = Aspects.WithTimeMeasure(BroadcastRemoteExecutor.Create<IBroadcastService>(), ConsoleColor.DarkCyan);
		}

		protected override IOperationDispatcher GetDispatcherForNewConnection()
		{
			return new OperationDispatcher();
		}

		protected override void OnNewConnection(INetworkConnection connection)
		{
			var userContext = new ClientContext();
			_sharedContext.AddClient(connection, userContext);

			IRegistrationService registrationService = Aspects.WithTimeMeasure<IRegistrationService>(new RegistrationService(userContext, _broadcastService));
			IUserInfoService userInfoService = Aspects.WithTimeMeasure<IUserInfoService>(new UserInfoService(_sharedContext, userContext));

			connection.OperationDispatcher.RegisterRequestHandler(registrationService);
			connection.OperationDispatcher.RegisterRequestHandler(userInfoService);
		}

		protected override void OnConnectionClose(INetworkConnection connection)
		{
			var user = _sharedContext.GetUser(connection);
			_sharedContext.RemoveClient(connection);
			if (user.IsRegistered)
				_broadcastService.UserLeft(user.Name);
		}
	}
}