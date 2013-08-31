using System;
using BroadcastServices.Contracts;
using Examples.Utils;
using RemoteExecution.Config;
using RemoteExecution.Connections;
using RemoteExecution.Endpoints;

namespace BroadcastServices.Server
{
	class Host : StatefulServerEndpoint
	{
		private readonly IBroadcastService _broadcastService;
		readonly SharedContext _sharedContext = new SharedContext();

		public Host(string uri)
			: base(uri, new ServerConfig())
		{
			_broadcastService = Aspects.WithTimeMeasure(BroadcastRemoteExecutor.Create<IBroadcastService>(), ConsoleColor.DarkCyan);
			ConnectionClosed += OnConnectionClose;
		}

		protected override void InitializeConnection(IRemoteConnection connection)
		{
			var userContext = new ClientContext();
			_sharedContext.AddClient(connection, userContext);

			var registrationService = Aspects.WithTimeMeasure<IRegistrationService>(new RegistrationService(userContext, _broadcastService));
			var userInfoService = Aspects.WithTimeMeasure<IUserInfoService>(new UserInfoService(_sharedContext, userContext));

			connection.OperationDispatcher
				.RegisterHandler(registrationService)
				.RegisterHandler(userInfoService);
		}

		protected void OnConnectionClose(IRemoteConnection connection)
		{
			var user = _sharedContext.GetUser(connection);
			_sharedContext.RemoveClient(connection);
			if (user.IsRegistered)
				_broadcastService.UserLeft(user.Name);
		}
	}
}