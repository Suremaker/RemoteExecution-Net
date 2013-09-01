using System;
using NUnit.Framework;
using RemoteExecution.Channels;
using RemoteExecution.Connections;
using RemoteExecution.Dispatchers;
using RemoteExecution.Dispatchers.Messages;
using RemoteExecution.Executors;
using RemoteExecution.Schedulers;
using Rhino.Mocks;

namespace RemoteExecution.Core.UT.Connections
{
	public abstract class ConnectionTestBase<TConnection, TChannel>
		where TConnection : IRemoteConnection
		where TChannel : class, IDuplexChannel
	{
		protected TChannel Channel;
		protected IMessageDispatcher MessageDispatcher;
		protected IOperationDispatcher OperationDispatcher;
		protected IRemoteExecutor RemoteExecutor;
		protected IRemoteExecutorFactory RemoteExecutorFactory;
		protected ITaskScheduler Scheduler;
		protected TConnection Subject;

		[SetUp]
		public void BaseSetUp()
		{
			MessageDispatcher = MockRepository.GenerateMock<IMessageDispatcher>();
			OperationDispatcher = MockRepository.GenerateMock<IOperationDispatcher>();
			RemoteExecutorFactory = MockRepository.GenerateMock<IRemoteExecutorFactory>();
			Channel = MockRepository.GenerateMock<TChannel>();
			RemoteExecutor = MockRepository.GenerateMock<IRemoteExecutor>();
			Scheduler = MockRepository.GenerateMock<ITaskScheduler>();

			Scheduler.Stub(s => s.Execute(Arg<Action>.Is.Anything)).WhenCalled(a => ((Action)a.Arguments[0]).Invoke());
			OperationDispatcher.Stub(d => d.MessageDispatcher).Return(MessageDispatcher);
			RemoteExecutorFactory.Stub(f => f.CreateRemoteExecutor(Arg<IDuplexChannel>.Is.Anything, Arg<IMessageDispatcher>.Is.Anything)).Return(RemoteExecutor);
			Subject = CreateSubject();
		}

		[Test]
		public void Should_abort_all_pending_operations_on_channel_close_and_raise_connection_closed_event()
		{
			bool wasCloseEventRaised = false;
			Subject.Closed += () => wasCloseEventRaised = true;
			Channel.Stub(c => c.Id).Return(Guid.NewGuid());
			Channel.Raise(c => c.Closed += null);

			MessageDispatcher.AssertWasCalled(d => d.GroupDispatch(
				Arg<Guid>.Is.Equal(Channel.Id),
				Arg<ExceptionResponseMessage>.Matches(m =>
				                                      m.ExceptionType == typeof(OperationAbortedException).AssemblyQualifiedName &&
				                                      m.Message == "Connection has been closed." &&
				                                      m.CorrelationId == string.Empty)));

			Assert.That(wasCloseEventRaised, Is.True);
		}

		[Test]
		public void Should_bind_input_channel_to_dispatcher()
		{
			var message = MockRepository.GenerateMock<IMessage>();
			Channel.Raise(c => c.Received += null, message);
			MessageDispatcher.AssertWasCalled(d => d.Dispatch(message));
		}

		[Test]
		public void Should_create_remote_executor()
		{
			RemoteExecutorFactory.AssertWasCalled(f => f.CreateRemoteExecutor(Channel, MessageDispatcher));
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
		public void Should_initialize_operation_dispatcher()
		{
			Assert.That(Subject.OperationDispatcher, Is.SameAs(OperationDispatcher));
		}

		[Test]
		public void Should_initialize_remote_executor()
		{
			Assert.That(Subject.RemoteExecutor, Is.SameAs(RemoteExecutor));
		}

		[Test]
		public void Should_use_scheduler_for_incoming_messages()
		{
			var message = MockRepository.GenerateMock<IMessage>();
			Channel.Raise(c => c.Received += null, message);
			Scheduler.AssertWasCalled(s => s.Execute(Arg<Action>.Is.Anything));
		}

		protected abstract TConnection CreateSubject();
	}
}