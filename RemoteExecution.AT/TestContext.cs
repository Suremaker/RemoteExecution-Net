using NUnit.Framework;
using RemoteExecution.AT.Helpers.Contracts;
using RemoteExecution.AT.Helpers.Services;
using RemoteExecution.Core.Config;
using RemoteExecution.Core.Connections;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Endpoints;

namespace RemoteExecution.AT
{
	public abstract class TestContext
	{
		protected abstract string ClientChannelUri { get; }
		protected abstract string ServerConnectionListenerUri { get; }

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			Configurator.Configure();
		}

		protected IClientConnection CreateClientConnection()
		{
			return new ClientConnection(ClientChannelUri);
		}

		protected GenericServerEndpoint CreateServer(int maxConnections = 128)
		{
			return new GenericServerEndpoint(ServerConnectionListenerUri, new ServerConfig { MaxConnections = maxConnections }, () => new OperationDispatcher(), ConfigureConnection);
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
			client.OperationDispatcher.RegisterHandler(clientSerivce);
			client.Open();
			return client;
		}

		protected IServerEndpoint StartServer(int maxConnections = 128)
		{
			var server = CreateServer(maxConnections);
			server.Start();
			return server;
		}

		private void ConfigureConnection(IServerEndpoint endpoint, IRemoteConnection clientConnection)
		{
			clientConnection.OperationDispatcher
			                .RegisterHandler<ICalculator>(new Calculator())
			                .RegisterHandler<IGreeter>(new Greeter())
			                .RegisterHandler<IRemoteService>(new RemoteService(clientConnection, endpoint.BroadcastRemoteExecutor));
		}
	}
}