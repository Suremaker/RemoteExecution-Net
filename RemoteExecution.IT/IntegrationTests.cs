using NUnit.Framework;
using RemoteExecution.Dispatching;
using RemoteExecution.Endpoints;
using RemoteExecution.Networking;

namespace RemoteExecution.IT
{
    [TestFixture]
    public class IntegrationTests
    {
        #region Setup/Teardown

        [SetUp]
        public void FixtureSetUp()
        {
            const string appId = "test_app";
            _serverToClientEndpoint = null;
            _serverDispatcher = new OperationDispatcher();
            _server = new Server(appId, _serverDispatcher, 5, 3232);
            _server.Start();

            _clientDispatcher = new OperationDispatcher();
            _clientDispatcher.RegisterFor<IVersionProvider>(new VersionProvider());
            _client = new Client(appId, _clientDispatcher);
            _server.OnNewConnection = endpoint => { _serverToClientEndpoint = endpoint; };
            _client.Start();
        }

        [TearDown]
        public void FixtureTearDown()
        {
            _client.Stop();
            _server.Stop();
        }

        #endregion

        private Server _server;
        private IOperationDispatcher _serverDispatcher;
        private IOperationDispatcher _clientDispatcher;
        private Client _client;
        private IWriteEndpoint _serverToClientEndpoint;

        private void ConnectClient()
        {
            _client.Connect("localhost", 3232);
        }

        [Test]
        public void ShouldGameServerCheckClientVersion()
        {
            ConnectClient();
            var versionProvider = RemoteExecutor.Create<IVersionProvider>(_serverDispatcher, _serverToClientEndpoint);
            Assert.That(versionProvider.GetVersion(), Is.EqualTo(5));
        }

        [Test]
        public void ShouldGameServerReceiveNewConnection()
        {
            ConnectClient();
            Assert.That(_serverToClientEndpoint, Is.Not.Null);
        }
    }
}
