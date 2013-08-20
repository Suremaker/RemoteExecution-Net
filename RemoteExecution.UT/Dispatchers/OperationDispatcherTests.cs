using System;
using System.Linq;
using NUnit.Framework;
using RemoteExecution.Channels;
using RemoteExecution.Dispatchers;
using RemoteExecution.Handlers;
using RemoteExecution.Messages;
using RemoteExecution.UT.Helpers;
using Rhino.Mocks;

namespace RemoteExecution.UT.Dispatchers
{
	[TestFixture]
	public class OperationDispatcherTests
	{
		private OperationDispatcher _subject;
		private MockMessageChannel _channel;

		private T GetResult<T>(string id)
		{
			return _channel.SentMessages.OfType<IResponse>().Where(r => r.CorrelationId == id).Select(r => r.Value).OfType<T>().SingleOrDefault();
		}

		private IResponseHandler RegisterResponseHandlerFor(IMessageChannel channel)
		{
			var handler = MockRepository.GenerateMock<IResponseHandler>();
			handler.Stub(x => x.Id).Return(Guid.NewGuid().ToString());
			handler.Stub(x => x.TargetChannel).Return(channel);

			_subject.RegisterResponseHandler(handler);
			return handler;
		}

		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_subject = new OperationDispatcher();
			_channel = new MockMessageChannel();
		}

		#endregion

		[Test]
		public void ShouldDispatchOperations()
		{
			_subject.RegisterRequestHandler(typeof(IGreeter), new Greeter());
			_subject.RegisterRequestHandler(typeof(ICalculator), new Calculator());

			_subject.Dispatch(new Request("1", "ICalculator", "Add", new object[] { 2, 3 }, true), _channel);
			_subject.Dispatch(new Request("2", "IGreeter", "Hello", new object[] { "Jack" }, true), _channel);
			Assert.That(GetResult<string>("1"), Is.EqualTo("5"));
			Assert.That(GetResult<string>("2"), Is.EqualTo("Hello Jack"));
		}

		[Test]
		public void ShouldDispatchResponses()
		{
			const string id = "some id";

			var handler = MockRepository.GenerateMock<IResponseHandler>();
			handler.Stub(x => x.Id).Return(id);
			_subject.RegisterResponseHandler(handler);

			var response = new Response(id, "text");
			_subject.Dispatch(response, _channel);

			handler.AssertWasCalled(h => h.Handle(response, _channel));
		}

		[Test]
		public void ShouldNotAllowToRegisterHandlerIfItDoesNotImplementSpecifiedInterface()
		{
			var ex = Assert.Throws<ArgumentException>(() => _subject.RegisterRequestHandler(typeof(ICalculator), new Greeter()));
			Assert.That(ex.Message, Is.StringStarting("Unable to register Greeter handler: it does not implement ICalculator interface."));
		}

		[Test]
		public void ShouldNotAllowToRegisterHandlerIfTheSpecifiedHandlerObjectIsNotOfInterfaceType()
		{
			var ex = Assert.Throws<ArgumentException>(() => _subject.RegisterRequestHandler(new Calculator()));
			Assert.That(ex.Message, Is.StringStarting("Unable to register handler: Calculator type is not an interface."));
		}

		[Test]
		public void ShouldNotAllowToRegisterHandlerIfTheSpecifiedTypeIsNotAnInterface()
		{
			var ex = Assert.Throws<ArgumentException>(() => _subject.RegisterRequestHandler(typeof(Calculator), new Calculator()));
			Assert.That(ex.Message, Is.StringStarting("Unable to register handler: Calculator type is not an interface."));
		}

		[Test]
		public void ShouldNotSendErrorResponseIfThereIsNoRegisteredResponseHandlerForGivenResponseMessage()
		{
			const string id = "test";
			_subject.Dispatch(new Response(id, "text"), _channel);
			Assert.That(GetResult<IResponse>(id), Is.Null);
		}

		[Test]
		public void ShouldPassHandlerExceptions()
		{
			_subject.RegisterRequestHandler(typeof(ICalculator), new Calculator());

			_subject.Dispatch(new Request("1", "ICalculator", "Subtract", new object[] { 3, 2 }, true), _channel);
			var ex = Assert.Throws<ArgumentException>(() => GetResult<Exception>("1"));
			Assert.That(ex.Message, Is.EqualTo("test"));
		}

		[Test]
		public void ShouldPassInvalidOperationExceptionIfNoHandlerIsRegistered()
		{
			_subject.Dispatch(new Request("1", "ICalculator", "Add", new object[] { 0, 1 }, true), _channel);
			var ex = Assert.Throws<InvalidOperationException>(() => GetResult<Exception>("1"));
			Assert.That(ex.Message, Is.EqualTo("No handler is defined for ICalculator type."));
		}

		[Test]
		public void ShouldPassInvalidOperationExceptionIfNoOperationIsDefinedInHandler()
		{
			_subject.RegisterRequestHandler(typeof(ICalculator), new Calculator());
			_subject.Dispatch(new Request("1", "ICalculator", "Add", new object[] { null, 3.14, "test" }, true), _channel);

			var ex = Assert.Throws<InvalidOperationException>(() => GetResult<Exception>("1"));
			Assert.That(ex.Message, Is.EqualTo("Unable to call Add(null,Double,String) method on ICalculator handler: no matching method was found."));
		}

		[Test]
		public void ShouldSendAbortResponse()
		{
			var responseHandler = RegisterResponseHandlerFor(_channel);
			const string message = "some text";
			_subject.DispatchAbortResponsesFor(_channel, message);

			responseHandler.AssertWasCalled(h => h.Handle(Arg<ExceptionResponse>.Matches(m => m.Message == message && m.ExceptionType == typeof(OperationAbortedException).AssemblyQualifiedName), Arg.Is(_channel)));
		}

		[Test]
		public void ShouldSendAbortResponsesToAllResponseHandlersListeningOnGivenChannel()
		{
			var handlers = new IResponseHandler[5];
			for (int i = 0; i < handlers.Length; i++)
				handlers[i] = RegisterResponseHandlerFor(_channel);
			var otherHandler = RegisterResponseHandlerFor(MockRepository.GenerateMock<IMessageChannel>());

			_subject.DispatchAbortResponsesFor(_channel, "some text");

			foreach (var responseHandler in handlers)
				responseHandler.AssertWasCalled(h => h.Handle(Arg<ExceptionResponse>.Is.Anything, Arg.Is(_channel)));

			otherHandler.AssertWasNotCalled(h => h.Handle(Arg<ExceptionResponse>.Is.Anything, Arg.Is(_channel)));
		}

		[Test]
		public void ShouldUnregisterResponseHandler()
		{
			const string id = "some id";
			var handler = MockRepository.GenerateMock<IResponseHandler>();
			handler.Stub(x => x.Id).Return(id);
			_subject.RegisterResponseHandler(handler);

			_subject.UnregisterResponseHandler(handler);

			_subject.Dispatch(new Response(id, "text"), _channel);

			handler.AssertWasNotCalled(h => h.Handle(Arg<IMessage>.Is.Anything, Arg<IMessageChannel>.Is.Anything));
		}
	}
}