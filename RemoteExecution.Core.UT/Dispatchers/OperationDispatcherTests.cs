using System;
using NUnit.Framework;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Dispatchers.Handlers;
using Rhino.Mocks;

namespace RemoteExecution.Core.UT.Dispatchers
{
	[TestFixture]
	public class OperationDispatcherTests
	{
		class TestableOperationDispatcher : OperationDispatcher
		{
			public TestableOperationDispatcher(IMessageDispatcher messageDispatcher)
				: base(messageDispatcher)
			{
			}
		}

		public interface IFoo { }
		public class Foo : IFoo { }

		private IOperationDispatcher _subject;
		private IMessageDispatcher _messageDispatcher;

		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_messageDispatcher = MockRepository.GenerateMock<IMessageDispatcher>();
			_subject = new TestableOperationDispatcher(_messageDispatcher);
		}

		#endregion

		[Test]
		public void Should_instantiate_message_dispatcher()
		{
			Assert.That(new OperationDispatcher().MessageDispatcher, Is.InstanceOf<MessageDispatcher>());
		}

		[Test]
		public void Should_register_default_handler()
		{
			_messageDispatcher.AssertWasCalled(d => d.DefaultHandler = Arg<DefaultRequestHandler>.Is.TypeOf);
		}

		[Test]
		public void Should_register_request_handler()
		{
			var foo = MockRepository.GenerateMock<IFoo>();
			_subject.RegisterHandler(typeof(IFoo), foo);

			_messageDispatcher.AssertWasCalled(m => m.Register(Arg<RequestHandler>.Matches(h =>
				h.InterfaceType == typeof(IFoo) &&
				h.Handler == foo &&
				h.HandledMessageType == typeof(IFoo).Name &&
				h.HandlerGroupId == typeof(IFoo).GUID)));
		}

		[Test]
		public void Should_register_request_handler_with_generic_method()
		{
			var foo = MockRepository.GenerateMock<IFoo>();
			_subject.RegisterHandler(foo);

			_messageDispatcher.AssertWasCalled(m => m.Register(Arg<RequestHandler>.Matches(h =>
				h.InterfaceType == typeof(IFoo) &&
				h.Handler == foo &&
				h.HandledMessageType == typeof(IFoo).Name &&
				h.HandlerGroupId == typeof(IFoo).GUID)));
		}

		[Test]
		public void Should_register_throw_if_handler_does_not_implement_specified_interface()
		{
			var ex = Assert.Throws<ArgumentException>(() => _subject.RegisterHandler(typeof(IFoo), new object()));
			Assert.That(ex.Message, Is.StringStarting("Unable to register 'Object' handler: it does not implement 'IFoo' interface."));
		}

		[Test]
		public void Should_register_throw_if_handler_is_not_of_interface_type()
		{
			var ex = Assert.Throws<ArgumentException>(() => _subject.RegisterHandler(typeof(Foo), new Foo()));
			Assert.That(ex.Message, Is.StringStarting("Unable to register handler: 'Foo' type is not an interface."));
		}

		[Test]
		public void Should_register_throw_if_handler_is_null()
		{
			Assert.Throws<ArgumentNullException>(() => _subject.RegisterHandler(typeof(IFoo), null));
		}

		[Test]
		public void Should_unregister_handler()
		{
			_subject.UnregisterHandler(typeof(IFoo));
			_messageDispatcher.AssertWasCalled(m => m.Unregister(typeof(IFoo).Name));
		}

		[Test]
		public void Should_unregister_handler_with_generic_method()
		{
			_subject.UnregisterHandler<IFoo>();
			_messageDispatcher.AssertWasCalled(m => m.Unregister(typeof(IFoo).Name));
		}

		[Test]
		public void Should_return_itself_in_order_to_chain_operations()
		{
			var foo = MockRepository.GenerateMock<IFoo>();
			var dispatcher = _subject.RegisterHandler(foo)
					.UnregisterHandler<IFoo>()
					.RegisterHandler(typeof(IFoo), foo)
					.UnregisterHandler(typeof(IFoo));

			Assert.That(dispatcher, Is.SameAs(_subject));
		}
	}
}