﻿using System;
using NUnit.Framework;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Connections;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Dispatchers.Messages;
using RemoteExecution.Core.Executors;
using Rhino.Mocks;

namespace RemoteExecution.Core.UT.Connections
{
	public abstract class ConnectionTestBase<TConnection, TChannel>
		where TConnection : IRemoteConnection
		where TChannel : class, IDuplexChannel
	{
		protected IOperationDispatcher OperationDispatcher;
		protected IRemoteExecutorFactory RemoteExecutorFactory;
		protected TChannel Channel;
		protected IMessageDispatcher MessageDispatcher;
		protected IRemoteExecutor RemoteExecutor;
		protected TConnection Subject;

		[SetUp]
		public void BaseSetUp()
		{
			MessageDispatcher = MockRepository.GenerateMock<IMessageDispatcher>();
			OperationDispatcher = MockRepository.GenerateMock<IOperationDispatcher>();
			RemoteExecutorFactory = MockRepository.GenerateMock<IRemoteExecutorFactory>();
			Channel = MockRepository.GenerateMock<TChannel>();
			RemoteExecutor = MockRepository.GenerateMock<IRemoteExecutor>();
			OperationDispatcher.Stub(d => d.MessageDispatcher).Return(MessageDispatcher);
			RemoteExecutorFactory.Stub(f => f.CreateRemoteExecutor(Arg<IDuplexChannel>.Is.Anything, Arg<IMessageDispatcher>.Is.Anything)).Return(RemoteExecutor);
			Subject = CreateSubject();
		}

		protected abstract TConnection CreateSubject();

		[Test]
		public void Should_create_remote_executor()
		{
			RemoteExecutorFactory.AssertWasCalled(f => f.CreateRemoteExecutor(Channel, MessageDispatcher));
		}

		[Test]
		public void Should_initialize_remote_executor()
		{
			Assert.That(Subject.Executor, Is.SameAs(RemoteExecutor));
		}

		[Test]
		public void Should_initialize_operation_dispatcher()
		{
			Assert.That(Subject.Dispatcher, Is.SameAs(OperationDispatcher));
		}

		[Test]
		public void Should_delegate_is_open_to_channel([Values(true, false)]bool isOpen)
		{
			Channel.Stub(c => c.IsOpen).Return(isOpen);
			Assert.That(Subject.IsOpen, Is.EqualTo(isOpen));
		}

		[Test]
		public void Should_dispose_dispose_channel()
		{
			Subject.Dispose();
			Channel.AssertWasCalled(c => c.Dispose());
		}

		[Test]
		public void Should_bind_input_channel_to_dispatcher()
		{
			var message = MockRepository.GenerateMock<IMessage>();
			Channel.Raise(c => c.Received += null, message);
			MessageDispatcher.AssertWasCalled(d => d.Dispatch(message));
		}

		[Test]
		public void Should_abort_all_pending_operations_on_channel_close()
		{
			Channel.Stub(c => c.Id).Return(Guid.NewGuid());
			Channel.Raise(c => c.ChannelClosed += null);

			MessageDispatcher.AssertWasCalled(d => d.GroupDispatch(
				Arg<Guid>.Is.Equal(Channel.Id),
				Arg<ExceptionResponseMessage>.Matches(m =>
					m.ExceptionType == typeof(OperationAbortedException).AssemblyQualifiedName &&
					m.Message == "Connection has been closed." &&
					m.CorrelationId == string.Empty)));
		}
	}
}