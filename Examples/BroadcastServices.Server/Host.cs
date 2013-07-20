using System;
using BroadcastServices.Contracts;
using Examples.Utils;
using RemoteExecution.Connections;
using RemoteExecution.Endpoints;

namespace BroadcastServices.Server
{
	class Host : StatefulServerEndpoint
	{
		readonly SharedContext _sharedContext = new SharedContext();
		private readonly IBroadcastService _broadcastService;

		public Host(ServerEndpointConfig config)
			: base(config)
		{
			_broadcastService = Aspects.WithTimeMeasure(BroadcastRemoteExecutor.Create<IBroadcastService>(), ConsoleColor.DarkCyan);
		}

		protected override void OnConnectionClose(INetworkConnection connection)
		{
			var user = _sharedContext.GetUser(connection);
			_sharedContext.RemoveClient(connection);
			if (user.IsRegistered)
				_broadcastService.UserLeft(user.Name);
		}

		protected override void RegisterServicesFor(INetworkConnection connection)
		{
			var userContext = new ClientContext();
			_sharedContext.AddClient(connection, userContext);

			var registrationService = Aspects.WithTimeMeasure<IRegistrationService>(new RegistrationService(userContext, _broadcastService));
			var userInfoService = Aspects.WithTimeMeasure<IUserInfoService>(new UserInfoService(_sharedContext, userContext));

			connection.OperationDispatcher
				.RegisterRequestHandler(registrationService)
				.RegisterRequestHandler(userInfoService);
		}
	}
}