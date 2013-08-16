using System;
using NUnit.Framework;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Connections;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Dispatchers.Messages;
using RemoteExecution.Core.Executors;
using Rhino.Mocks;

namespace RemoteExecution.Core.UT.Connections
{
	[TestFixture]
	public class RemoteConnectionTests
	{
		private IRemoteConnection _subject;
		private IOperationDispatcher _operationDispatcher;
		private IRemoteExecutorFactory _remoteExecutorFactory;
		private IDuplexChannel _channel;
		private IMessageDispatcher _messageDispatcher;
		private IRemoteExecutor _remoteExecutor;

		[SetUp]
		public void SetUp()
		{
			_messageDispatcher = MockRepository.GenerateMock<IMessageDispatcher>();
			_operationDispatcher = MockRepository.GenerateMock<IOperationDispatcher>();
			_remoteExecutorFactory = MockRepository.GenerateMock<IRemoteExecutorFactory>();
			_channel = MockRepository.GenerateMock<IDuplexChannel>();
			_remoteExecutor = MockRepository.GenerateMock<IRemoteExecutor>();
			_operationDispatcher.Stub(d => d.MessageDispatcher).Return(_messageDispatcher);
			_remoteExecutorFactory.Stub(f => f.CreateRemoteExecutor(Arg<IDuplexChannel>.Is.Anything, Arg<IMessageDispatcher>.Is.Anything)).Return(_remoteExecutor);

			_subject = new RemoteConnection(_channel, _remoteExecutorFactory, _operationDispatcher);
		}

		[Test]
		public void Should_create_remote_executor()
		{
			_remoteExecutorFactory.AssertWasCalled(f => f.CreateRemoteExecutor(_channel, _messageDispatcher));
		}

		[Test]
		public void Should_initialize_remote_executor()
		{
			Assert.That(_subject.Executor, Is.SameAs(_remoteExecutor));
		}

		[Test]
		public void Should_initialize_operation_dispatcher()
		{
			Assert.That(_subject.Dispatcher, Is.SameAs(_operationDispatcher));
		}

		[Test]
		public void Should_delegate_is_open_to_channel([Values(true, false)]bool isOpen)
		{
			_channel.Stub(c => c.IsOpen).Return(isOpen);
			Assert.That(_subject.IsOpen, Is.EqualTo(isOpen));
		}

		[Test]
		public void Should_dispose_dispose_channel()
		{
			_subject.Dispose();
			_channel.AssertWasCalled(c => c.Dispose());
		}

		[Test]
		public void Should_bind_input_channel_to_dispatcher()
		{
			var message = MockRepository.GenerateMock<IMessage>();
			_channel.Raise(c => c.Received += null, message);
			_messageDispatcher.AssertWasCalled(d => d.Dispatch(message));
		}

		[Test]
		public void Should_abort_all_pending_operations_on_channel_close()
		{
			_channel.Stub(c => c.Id).Return(Guid.NewGuid());
			_channel.Raise(c => c.ChannelClosed += null);

			_messageDispatcher.AssertWasCalled(d => d.GroupDispatch(
				Arg<Guid>.Is.Equal(_channel.Id),
				Arg<ExceptionResponseMessage>.Matches(m =>
					m.ExceptionType == typeof(OperationAbortedException).AssemblyQualifiedName &&
					m.Message == "Connection has been closed." &&
					m.CorrelationId == string.Empty)));
		}
	}
}
