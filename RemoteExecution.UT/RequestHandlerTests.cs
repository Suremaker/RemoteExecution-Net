using System;
using NUnit.Framework;
using RemoteExecution.Channels;
using RemoteExecution.Handlers;
using RemoteExecution.Messages;
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
		[TestCase(true)]
		[TestCase(false)]
		public void Should_call_handler(bool isResponseExpected)
		{
			var reqest = new Request(Guid.NewGuid().ToString(), "group", "Foo", new object[0], isResponseExpected);
			_subject.Handle(reqest, _messageChannel);
			_handler.AssertWasCalled(h => h.Foo());
		}

		[Test]
		public void Should_send_response_if_is_expected()
		{
			var correlationId = Guid.NewGuid().ToString();
			var reqest = new Request(correlationId, "group", "Foo", new object[0], true);
			_subject.Handle(reqest, _messageChannel);
			_messageChannel.AssertWasCalled(ch => ch.Send(Arg<Response>.Matches(r => r.CorrelationId == correlationId)));
		}

		[Test]
		public void Should_send_response_for_exception_if_response_is_expected()
		{
			_handler.Stub(h => h.Foo()).Throw(new Exception());

			var correlationId = Guid.NewGuid().ToString();
			var reqest = new Request(correlationId, "group", "Foo", new object[0], true);
			_subject.Handle(reqest, _messageChannel);
			_messageChannel.AssertWasCalled(ch => ch.Send(Arg<ExceptionResponse>.Matches(r => r.CorrelationId == correlationId)));
		}

		[Test]
		public void Should_not_send_response_if_is_not_expected()
		{
			var reqest = new Request(Guid.NewGuid().ToString(), "group", "Foo", new object[0], false);
			_subject.Handle(reqest, _messageChannel);
			_messageChannel.AssertWasNotCalled(ch => ch.Send(Arg<IResponse>.Is.Anything));
		}

		[Test]
		public void Should_not_send_response_for_exception_if_is_not_expected()
		{
			_handler.Stub(h => h.Foo()).Throw(new Exception());
			var reqest = new Request(Guid.NewGuid().ToString(), "group", "Foo", new object[0], false);
			_subject.Handle(reqest, _messageChannel);
			_messageChannel.AssertWasNotCalled(ch => ch.Send(Arg<IResponse>.Is.Anything));
		}
	}
}
