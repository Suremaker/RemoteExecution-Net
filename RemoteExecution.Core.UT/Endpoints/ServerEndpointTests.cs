using System;
using System.Linq;
using NUnit.Framework;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Endpoints;
using RemoteExecution.Core.Endpoints.Listeners;
using RemoteExecution.Core.Executors;
using RemoteExecution.Core.Schedulers;
using Rhino.Mocks;

namespace RemoteExecution.Core.UT.Endpoints
{
	[TestFixture]
	public class ServerEndpointTests
	{
		class TestableServerEndpoint : ServerEndpoint
		{
			private readonly IOperationDispatcher _operationDispatcher;

			public TestableServerEndpoint(IServerListener listener, IServerEndpointConfig config, IOperationDispatcher operationDispatcher)
				: base(listener, config)
			{
				_operationDispatcher = operationDispatcher;
			}

			protected override IOperationDispatcher GetOperationDispatcher()
			{
				return _operationDispatcher;
			}
		}

		private IServerEndpoint _subject;
		private IServerListener _listener;
		private IServerEndpointConfig _config;
		private IRemoteExecutorFactory _remoteExecutorFactory;
		private ITaskScheduler _taskScheduler;
		private IOperationDispatcher _operationDispatcher;

		[SetUp]
		public void SetUp()
		{
			_listener = MockRepository.GenerateMock<IServerListener>();
			_remoteExecutorFactory = MockRepository.GenerateMock<IRemoteExecutorFactory>();
			_taskScheduler = MockRepository.GenerateMock<ITaskScheduler>();
			_config = MockRepository.GenerateMock<IServerEndpointConfig>();
			_config.Stub(c => c.MaxConnections).Return(10);
			_config.Stub(c => c.RemoteExecutorFactory).Return(_remoteExecutorFactory);
			_config.Stub(c => c.TaskScheduler).Return(_taskScheduler);
			_operationDispatcher = MockRepository.GenerateMock<IOperationDispatcher>();
			_operationDispatcher.Stub(d => d.MessageDispatcher).Return(MockRepository.GenerateMock<IMessageDispatcher>());
			_subject = new TestableServerEndpoint(_listener, _config, _operationDispatcher);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void Should_is_listening_reflect_listener_state(bool expected)
		{
			_listener.Stub(l => l.IsListening).Return(expected);
			Assert.That(_subject.IsListening, Is.EqualTo(expected));
		}

		[Test]
		public void Should_start_listening()
		{
			_subject.StartListening();
			_listener.AssertWasCalled(l => l.StartListening());
		}

		[Test]
		public void Dispose_should_dispose_listener()
		{
			_subject.Dispose();
			_listener.AssertWasCalled(l => l.Dispose());
		}

		[Test]
		public void Dispose_should_dispose_all_connections()
		{
			var channel1 = OpenChannel();
			var channel2 = OpenChannel();

			_subject.Dispose();
			channel1.AssertWasCalled(c => c.Dispose());
			channel2.AssertWasCalled(c => c.Dispose());
			Assert.That(_subject.ActiveConnections, Is.Empty);
		}

		[Test]
		public void Should_create_new_connection_for_opened_channel()
		{
			var channel = GenerateChannel();
			var remoteExecutor = MockRepository.GenerateMock<IRemoteExecutor>();
			_remoteExecutorFactory.Stub(f => f.CreateRemoteExecutor(Arg<IDuplexChannel>.Is.Equal(channel), Arg<IMessageDispatcher>.Is.Anything)).Return(remoteExecutor);

			_listener.Raise(l => l.OnChannelOpen += null, channel);

			var connection = _subject.ActiveConnections.SingleOrDefault();
			Assert.That(connection, Is.Not.Null);
			Assert.That(connection.Dispatcher, Is.EqualTo(_operationDispatcher));
			Assert.That(connection.Executor, Is.EqualTo(remoteExecutor));
		}

		[Test]
		public void Should_remove_closed_connections_from_list()
		{
			var channel1 = OpenChannel();

			Assert.That(_subject.ActiveConnections.Count(), Is.EqualTo(1));
			var connection1 = _subject.ActiveConnections.Single();

			var channel2 = OpenChannel();

			Assert.That(_subject.ActiveConnections.Count(), Is.EqualTo(2));
			var connection2 = _subject.ActiveConnections.Single(c => c != connection1);

			channel1.Raise(c => c.ChannelClosed += null);
			Assert.That(_subject.ActiveConnections.Count(), Is.EqualTo(1));
			Assert.That(_subject.ActiveConnections.Single(), Is.SameAs(connection2));

			channel2.Raise(c => c.ChannelClosed += null);
			Assert.That(_subject.ActiveConnections, Is.Empty);
		}

		private IDuplexChannel OpenChannel()
		{
			var channel1 = GenerateChannel();
			_listener.Raise(l => l.OnChannelOpen += null, channel1);
			return channel1;
		}

		private static IDuplexChannel GenerateChannel()
		{
			var channel = MockRepository.GenerateMock<IDuplexChannel>();
			channel.Stub(c => c.Id).Return(Guid.NewGuid());
			channel.Stub(c => c.Dispose()).WhenCalled(m => channel.Raise(c => c.ChannelClosed += null));
			return channel;
		}
	}
}
