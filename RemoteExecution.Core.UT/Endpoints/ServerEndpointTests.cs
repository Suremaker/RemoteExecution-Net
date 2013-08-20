using System;
using System.Collections.Generic;
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
		private IServerEndpoint _subject;
		private IServerListener _listener;
		private IServerEndpointConfig _config;
		private IRemoteExecutorFactory _remoteExecutorFactory;
		private ITaskScheduler _taskScheduler;
		private IOperationDispatcher _operationDispatcher;

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

		#region Setup/Teardown

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
			_subject = new GenericServerEndpoint(_listener, _config, () => _operationDispatcher);
		}

		#endregion

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
		public void Dispose_should_dispose_listener()
		{
			_subject.Dispose();
			_listener.AssertWasCalled(l => l.Dispose());
		}

		[Test]
		public void Should_configure_opened_connection()
		{
			var wasConfigured = false;
			_subject = new GenericServerEndpoint(_listener, _config, () => _operationDispatcher, c => wasConfigured = true);
			OpenChannel();

			Assert.That(wasConfigured, Is.True);
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
		public void Should_fire_connection_closed_in_nonblocking_way()
		{
			var wasEventFired = false;
			_subject.ConnectionClosed += c => wasEventFired = true;

			//closing channel will effect with no event raise, because mock scheduler is not stubbed
			OpenChannel().Dispose();
			Assert.That(wasEventFired, Is.False);

			//after scheduler is stubbed, closing channel will effect with raising an event
			_taskScheduler.Stub(s => s.Execute(Arg<Action>.Is.Anything)).WhenCalled(m => ((Action)m.Arguments[0]).Invoke());
			OpenChannel().Dispose();
			Assert.That(wasEventFired, Is.True);
		}

		[Test]
		public void Should_fire_connection_opened_in_nonblocking_way()
		{
			var wasEventFired = false;
			_subject.ConnectionOpened += c => wasEventFired = true;

			//opening channel will effect with calling task scheduler, but event will be not raised because mock scheduler is not stubbed
			OpenChannel();
			_taskScheduler.AssertWasCalled(s => s.Execute(Arg<Action>.Is.Anything));
			Assert.That(wasEventFired, Is.False);

			//after scheduler is stubbed, opening channel will effect with raising an event
			_taskScheduler.Stub(s => s.Execute(Arg<Action>.Is.Anything)).WhenCalled(m => ((Action)m.Arguments[0]).Invoke());
			OpenChannel();
			Assert.That(wasEventFired, Is.True);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void Should_is_running_reflect_listener_state(bool expected)
		{
			_listener.Stub(l => l.IsListening).Return(expected);
			Assert.That(_subject.IsRunning, Is.EqualTo(expected));
		}

		[Test]
		public void Should_reject_new_channels_by_disposing_them_if_max_connection_count_is_reached()
		{
			var openedChannels = new List<IDuplexChannel>();
			for (int i = 0; i < _config.MaxConnections; ++i)
				openedChannels.Add(OpenChannel());

			var rejectedChannels = new List<IDuplexChannel>();
			for (int i = 0; i < 5; ++i)
				rejectedChannels.Add(OpenChannel());

			foreach (var channel in openedChannels)
				channel.AssertWasNotCalled(c => c.Dispose());

			foreach (var channel in rejectedChannels)
				channel.AssertWasCalled(c => c.Dispose());
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

		[Test]
		public void Should_start_endpoint()
		{
			_subject.Start();
			_listener.AssertWasCalled(l => l.StartListening());
		}
	}
}
