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
			IOperationDispatcher dispatcher = new OperationDispatcher();
			dispatcher.RegisterHandler<ICalculator>(new Calculator());
			dispatcher.RegisterHandler<IGreeter>(new Greeter());
			dispatcher.RegisterHandler<IRemoteService>(new RemoteService());

			var server = new GenericServerEndpoint(CreateServerListener(), new ServerEndpointConfig(), () => dispatcher);
			server.Start();
			return server;
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

		protected abstract IServerListener CreateServerListener();
		protected abstract IClientChannel CreateClientChannel();
	}
}