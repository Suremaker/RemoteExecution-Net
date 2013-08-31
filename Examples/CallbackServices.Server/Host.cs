using CallbackServices.Contracts;
using RemoteExecution.Config;
using RemoteExecution.Connections;
using RemoteExecution.Endpoints;

namespace CallbackServices.Server
{
	class Host : StatefulServerEndpoint
	{
		public Host(string uri)
			: base(uri, new ServerConfig())
		{
		}

		protected override void InitializeConnection(IRemoteConnection connection)
		{
			var clientCallback = connection.RemoteExecutor.Create<IClientCallback>();
			connection.OperationDispatcher.RegisterHandler<ILongRunningOperation>(new LongRunningOperation(clientCallback));
		}
	}
}