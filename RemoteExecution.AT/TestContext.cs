using NUnit.Framework;
using RemoteExecution.AT.Helpers.Contracts;
using RemoteExecution.AT.Helpers.Services;
using RemoteExecution.Core.Config;
using RemoteExecution.Core.Connections;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Endpoints;
using RemoteExecution.Core.TransportLayer;
using RemoteExecution.TransportLayer.Lidgren;

namespace RemoteExecution.AT
{
	public abstract class TestContext
	{
		protected abstract string ClientChannelUri { get; }
		protected abstract string ServerConnectionListenerUri { get; }

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			TransportLayerResolver.Register(new LidgrenProvider());
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
			client.Dispatcher.RegisterHandler(clientSerivce);
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
			clientConnection.Dispatcher
			                .RegisterHandler<ICalculator>(new Calculator())
			                .RegisterHandler<IGreeter>(new Greeter())
			                .RegisterHandler<IRemoteService>(new RemoteService(clientConnection, endpoint.BroadcastExecutor));
		}
	}
}