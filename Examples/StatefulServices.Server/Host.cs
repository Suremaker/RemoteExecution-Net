using RemoteExecution.Core.Config;
using RemoteExecution.Core.Connections;
using RemoteExecution.Core.Endpoints;
using StatefulServices.Contracts;

namespace StatefulServices.Server
{
	class Host : StatefulServerEndpoint
	{
		readonly SharedContext _sharedContext = new SharedContext();

		public Host(string uri)
			: base(uri, new ServerConfig())
		{
			ConnectionClosed += OnConnectionClose;
		}

		protected override void InitializeConnection(IRemoteConnection connection)
		{
			var clientContext = new ClientContext();
			_sharedContext.AddClient(connection, clientContext);

			connection.OperationDispatcher
					  .RegisterHandler<IRegistrationService>(new RegistrationService(clientContext))
					  .RegisterHandler<IUserInfoService>(new UserInfoService(_sharedContext, clientContext));
		}

		private void OnConnectionClose(IRemoteConnection connection)
		{
			_sharedContext.RemoveClient(connection);
		}
	}
}