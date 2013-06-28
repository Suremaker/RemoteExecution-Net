using System;
using NUnit.Framework;
using RemoteExecution.Handling;
using RemoteExecution.Messaging;
using Rhino.Mocks;

namespace RemoteExecution.UT
{
	[TestFixture]
	public class RequestHandlerTests
	{
		private RequestHandler _subject;
		private IMock _handler;
		private IMessageChannel _messageChannel;

		public interface IMock
		{
			void Foo();
		}

		[SetUp]
		public void SetUp()
		{
			_handler = MockRepository.GenerateMock<IMock>();
			_messageChannel = MockRepository.GenerateMock<IMessageChannel>();
			_subject = new RequestHandler(typeof(IMock), _handler);
		}

		[Test]
		[TestCase("some id")]
		[TestCase(null)]
		public void Should_call_handler(string correlationId)
		{
			var reqest = new Request { CorrelationId = correlationId, Args = new object[0], GroupId = "group", OperationName = "Foo" };
			_subject.Handle(reqest, _messageChannel);
			_handler.AssertWasCalled(h => h.Foo());
		}

		[Test]
		public void Should_send_response_if_correlation_id_is_specified()
		{
			const string expectedId = "some id";
			var reqest = new Request { CorrelationId = expectedId, Args = new object[0], GroupId = "group", OperationName = "Foo" };
			_subject.Handle(reqest, _messageChannel);
			_messageChannel.AssertWasCalled(ch => ch.Send(Arg<Response>.Matches(r => r.CorrelationId == expectedId)));
		}

		[Test]
		public void Should_send_response_for_exception_if_correlation_id_is_specified()
		{
			_handler.Stub(h => h.Foo()).Throw(new Exception());

			const string expectedId = "some id";
			var reqest = new Request { CorrelationId = expectedId, Args = new object[0], GroupId = "group", OperationName = "Foo" };
			_subject.Handle(reqest, _messageChannel);
			_messageChannel.AssertWasCalled(ch => ch.Send(Arg<ExceptionResponse>.Matches(r => r.CorrelationId == expectedId)));
		}

		[Test]
		public void Should_not_send_response_if_correlation_id_is_not_specified()
		{
			var reqest = new Request { CorrelationId = null, Args = new object[0], GroupId = "group", OperationName = "Foo" };
			_subject.Handle(reqest, _messageChannel);
			_messageChannel.AssertWasNotCalled(ch => ch.Send(Arg<IResponse>.Is.Anything));
		}

		[Test]
		public void Should_not_send_response_for_exception_if_correlation_id_is_not_specified()
		{
			_handler.Stub(h => h.Foo()).Throw(new Exception());
			var reqest = new Request { CorrelationId = null, Args = new object[0], GroupId = "group", OperationName = "Foo" };
			_subject.Handle(reqest, _messageChannel);
			_messageChannel.AssertWasNotCalled(ch => ch.Send(Arg<IResponse>.Is.Anything));
		}
	}
}
