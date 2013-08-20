using System;
using NUnit.Framework;
using RemoteExecution.Core.Dispatchers;
using Rhino.Mocks;

namespace RemoteExecution.Core.UT.Dispatchers
{
	[TestFixture]
	public class MessageDispatcherTests
	{
		private IMessageDispatcher _subject;
		private IMessageHandler _defaultHandler;

		private static IMessageHandler CreateHandler(string messageType)
		{
			return CreateHandler(messageType, Guid.NewGuid());
		}

		private static IMessageHandler CreateHandler(string messageType, Guid handlerGroupId)
		{
			var messageHandler = MockRepository.GenerateMock<IMessageHandler>();
			messageHandler.Stub(h => h.HandledMessageType).Return(messageType);
			messageHandler.Stub(h => h.HandlerGroupId).Return(handlerGroupId);
			return messageHandler;
		}

		private static IMessage CreateMessage(string messageType)
		{
			var message = MockRepository.GenerateMock<IMessage>();
			message.Stub(m => m.MessageType).Return(messageType);
			return message;
		}

		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_subject = new MessageDispatcher();
			_defaultHandler = CreateHandler(null);
			_subject.DefaultHandler = _defaultHandler;
		}

		#endregion

		[Test]
		public void Should_dispatch_message_to_default_handler_if_there_is_no_registered_handlers()
		{
			IMessage message = CreateMessage("unregisteredType");
			_subject.Dispatch(message);
			_defaultHandler.AssertWasCalled(h => h.Handle(message));
		}

		[Test]
		public void Should_dispatch_message_to_proper_handlers()
		{
			const string type1 = "type1";
			const string type2 = "type2";
			var handler1 = CreateHandler(type1);
			var handler2 = CreateHandler(type2);

			var message1 = CreateMessage(type1);
			var message2 = CreateMessage(type2);

			_subject.Register(handler1);
			_subject.Register(handler2);

			_subject.Dispatch(message1);
			_subject.Dispatch(message2);

			handler1.AssertWasCalled(h => h.Handle(message1));
			handler2.AssertWasCalled(h => h.Handle(message2));

			handler1.AssertWasNotCalled(h => h.Handle(message2));
			handler2.AssertWasNotCalled(h => h.Handle(message1));
		}

		[Test]
		public void Should_dispatch_message_to_registered_handler()
		{
			const string messageType = "messageType";

			IMessageHandler messageHandler = CreateHandler(messageType);
			IMessage message = CreateMessage(messageType);

			_subject.Register(messageHandler);
			_subject.Dispatch(message);

			messageHandler.AssertWasCalled(h => h.Handle(message));
			_defaultHandler.AssertWasNotCalled(h => h.Handle(Arg<IMessage>.Is.Anything));
		}

		[Test]
		public void Should_dispatch_throw_if_no_default_handler_is_specified_and_message_cannot_be_handled_by_any_handler()
		{
			_subject.DefaultHandler = null;
			const string messageType = "someType";
			var expectedMessage = string.Format("Unable to dispatch message of type '{0}': no suitable handlers were found.", messageType);

			var ex = Assert.Throws<InvalidOperationException>(() => _subject.Dispatch(CreateMessage(messageType)));
			Assert.That(ex.Message, Is.EqualTo(expectedMessage));
		}

		[Test]
		public void Should_group_dispatch()
		{
			var group = Guid.NewGuid();
			var handler1 = CreateHandler("type1", group);
			var handler2 = CreateHandler("type2", group);
			var handler3 = CreateHandler("type3", Guid.NewGuid());

			_subject.Register(handler1);
			_subject.Register(handler2);
			_subject.Register(handler3);

			var message = CreateMessage(null);
			_subject.GroupDispatch(group, message);

			handler1.AssertWasCalled(h => h.Handle(message));
			handler2.AssertWasCalled(h => h.Handle(message));
			handler3.AssertWasNotCalled(h => h.Handle(message));

			_defaultHandler.AssertWasNotCalled(h => h.Handle(message));
		}

		[Test]
		public void Should_group_dispatch_throw_if_no_default_handler_is_specified_and_message_cannot_be_handled_by_any_handler()
		{
			_subject.DefaultHandler = null;
			var handlerGroupId = Guid.NewGuid();

			var expectedMessage = string.Format("Unable to dispatch message to group '{0}': no suitable handlers were found.", handlerGroupId);

			var ex = Assert.Throws<InvalidOperationException>(() => _subject.GroupDispatch(handlerGroupId, CreateMessage(null)));
			Assert.That(ex.Message, Is.EqualTo(expectedMessage));
		}

		[Test]
		public void Should_group_dispatch_use_default_handler_if_group_is_empty()
		{
			var handlerGroupId = Guid.NewGuid();

			var handler = CreateHandler("type", handlerGroupId);
			_subject.Register(handler);
			_subject.Unregister(handler.HandledMessageType);

			var message = CreateMessage(null);
			_subject.GroupDispatch(handlerGroupId, message);

			handler.AssertWasNotCalled(h => h.Handle(message));
			_defaultHandler.AssertWasCalled(h => h.Handle(message));
		}

		[Test]
		public void Should_group_dispatch_use_default_handler_if_group_is_not_defined()
		{
			var someGroup = Guid.NewGuid();
			var otherGroup = Guid.NewGuid();

			var handler = CreateHandler("type", someGroup);
			_subject.Register(handler);

			var message = CreateMessage(null);
			_subject.GroupDispatch(otherGroup, message);

			handler.AssertWasNotCalled(h => h.Handle(message));
			_defaultHandler.AssertWasCalled(h => h.Handle(message));
		}

		[Test]
		public void Should_register_throw_if_handler_does_not_have_handler_group_id_specified()
		{
			var ex = Assert.Throws<ArgumentException>(() => _subject.Register(CreateHandler("type", Guid.Empty)));
			Assert.That(ex.Message, Is.StringStarting("Handler does not have HandlerGroupId specified."));
		}

		[Test]
		public void Should_register_throw_if_handler_does_not_have_message_type_specified()
		{
			var ex = Assert.Throws<ArgumentException>(() => _subject.Register(CreateHandler(null)));
			Assert.That(ex.Message, Is.StringStarting("Handler does not have HandledMessageType specified."));
		}

		[Test]
		public void Should_register_throw_if_there_is_already_a_handler_specified_for_given_message_type()
		{
			const string type = "someType";
			var expectedMessage = string.Format("Unable to register handler for message type '{0}': only one handler could be registered for given message type.", type);

			_subject.Register(CreateHandler(type));
			var ex = Assert.Throws<ArgumentException>(() => _subject.Register(CreateHandler(type)));
			Assert.That(ex.Message, Is.StringStarting(expectedMessage));
		}

		[Test]
		public void Should_unregister_from_handler_group()
		{
			var group = Guid.NewGuid();
			var handler1 = CreateHandler("type1", group);
			var handler2 = CreateHandler("type2", group);

			_subject.Register(handler1);
			_subject.Register(handler2);
			_subject.Unregister(handler2.HandledMessageType);

			var message = CreateMessage(null);
			_subject.GroupDispatch(group, message);

			handler1.AssertWasCalled(h => h.Handle(message));
			handler2.AssertWasNotCalled(h => h.Handle(message));
		}

		[Test]
		public void Should_unregister_handler()
		{
			const string messageType = "type";
			var messageHandler = CreateHandler(messageType);

			_subject.Register(messageHandler);
			_subject.Unregister(messageType);

			var message = CreateMessage(messageType);
			_subject.Dispatch(message);

			messageHandler.AssertWasNotCalled(h => h.Handle(message));
			_defaultHandler.AssertWasCalled(h => h.Handle(message));
		}
	}
}
