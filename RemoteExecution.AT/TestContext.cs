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
		protected IServerEndpoint StartServer()
		{
			var server = new GenericServerEndpoint(CreateServerListener(), new ServerEndpointConfig(), () => new OperationDispatcher(), ConfigureConnection);
			server.Start();
			return server;
		}

		private void ConfigureConnection(IRemoteConnection clientConnection)
		{
			clientConnection.Dispatcher
				.RegisterHandler<ICalculator>(new Calculator())
				.RegisterHandler<IGreeter>(new Greeter())
				.RegisterHandler<IRemoteService>(new RemoteService(clientConnection));
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

		protected IClientConnection OpenClientConnectionWithCallback()
		{
			var client = CreateClientConnection();
			client.Dispatcher.RegisterHandler<IClientService>(new ClientService());
			client.Open();
			return client;
		}

		protected abstract IServerListener CreateServerListener();
		protected abstract IClientChannel CreateClientChannel();
	}
}