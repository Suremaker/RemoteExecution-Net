using RemoteExecution.AT.Helpers.Contracts;
using RemoteExecution.AT.Helpers.Services;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Connections;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Endpoints;
using RemoteExecution.Core.Endpoints.Listeners;
using RemoteExecution.Core.Executors;
using RemoteExecution.Core.Schedulers;

namespace RemoteExecution.AT
{
	public abstract class TestContext
	{
		protected IServerEndpoint StartServer(int maxConnections = 128)
		{
			var server = CreateServer(maxConnections);
			server.Start();
			return server;
		}

		protected GenericServerEndpoint CreateServer(int maxConnections = 128)
		{
			return new GenericServerEndpoint(CreateServerListener(), new ServerEndpointConfig { MaxConnections = maxConnections }, () => new OperationDispatcher(), ConfigureConnection);
		}

		private void ConfigureConnection(IServerEndpoint endpoint, IRemoteConnection clientConnection)
		{
			clientConnection.Dispatcher
				.RegisterHandler<ICalculator>(new Calculator())
				.RegisterHandler<IGreeter>(new Greeter())
				.RegisterHandler<IRemoteService>(new RemoteService(clientConnection, endpoint.BroadcastExecutor));
		}

		protected IClientConnection CreateClientConnection()
		{
			return new ClientConnection(CreateClientChannel(), new RemoteExecutorFactory(), new OperationDispatcher(), new AsyncTaskScheduler());
		}

		protected IClientConnection OpenClientConnection()
		{
			var client = CreateClientConnection();
			client.Open();
			return client;
		}

		protected abstract IServerConnectionListener CreateServerListener();
		protected IClientConnection OpenClientConnectionWithCallback<TInterface>(TInterface clientSerivce)
		{
			var client = CreateClientConnection();
			client.Dispatcher.RegisterHandler(clientSerivce);
			client.Open();
			return client;
		}

		protected abstract IClientChannel CreateClientChannel();
	}
}