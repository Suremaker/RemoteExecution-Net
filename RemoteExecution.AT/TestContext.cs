using NUnit.Framework;
using RemoteExecution.AT.Helpers.Contracts;
using RemoteExecution.AT.Helpers.Services;
using RemoteExecution.Core.Connections;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Endpoints;
using RemoteExecution.Core.Executors;
using RemoteExecution.Core.Schedulers;
using RemoteExecution.Core.TransportLayer;
using RemoteExecution.TransportLayer.Lidgren;

namespace RemoteExecution.AT
{
	public abstract class TestContext
	{
		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			TransportLayerResolver.Register(new LidgrenProvider());
		}

		protected IServerEndpoint StartServer(int maxConnections = 128)
		{
			var server = CreateServer(maxConnections);
			server.Start();
			return server;
		}

		protected GenericServerEndpoint CreateServer(int maxConnections = 128)
		{
			return new GenericServerEndpoint(ServerConnectionListenerUri, new ServerEndpointConfig { MaxConnections = maxConnections }, () => new OperationDispatcher(), ConfigureConnection);
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
			return new ClientConnection(ClientChannelUri, new RemoteExecutorFactory(), new OperationDispatcher(), new AsyncTaskScheduler());
		}

		protected IClientConnection OpenClientConnection()
		{
			var client = CreateClientConnection();
			client.Open();
			return client;
		}

		protected IClientConnection OpenClientConnectionWithCallback<TInterface>(TInterface clientSerivce)
		{
			var client = CreateClientConnection();
			client.Dispatcher.RegisterHandler(clientSerivce);
			client.Open();
			return client;
		}

		protected abstract string ServerConnectionListenerUri { get; }
		protected abstract string ClientChannelUri { get; }
	}
}