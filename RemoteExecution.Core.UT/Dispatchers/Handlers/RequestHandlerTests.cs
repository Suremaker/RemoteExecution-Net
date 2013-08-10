using System;
using NUnit.Framework;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Dispatchers.Handlers;
using RemoteExecution.Core.Dispatchers.Messages;
using Rhino.Mocks;

namespace RemoteExecution.Core.UT.Dispatchers.Handlers
{
	[TestFixture]
	public class RequestHandlerTests
	{
		private RequestHandler _subject;
		private IMock _handler;
		private IChannelProvider _channelProvider;
		private IOutgoingMessageChannel _outgoingChannel;

		public interface IMock
		{
			void Foo();
		}

		[SetUp]
		public void SetUp()
		{
			_handler = MockRepository.GenerateMock<IMock>();
			_channelProvider = MockRepository.GenerateMock<IChannelProvider>();
			_outgoingChannel = MockRepository.GenerateMock<IOutgoingMessageChannel>();
			_channelProvider.Stub(p => p.GetOutgoingChannel()).Return(_outgoingChannel);
			_subject = new RequestHandler(typeof(IMock), _handler);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void Should_call_handler(bool isResponseExpected)
		{
			var reqest = new RequestMessage(Guid.NewGuid().ToString(), "group", "Foo", new object[0], isResponseExpected) { ChannelProvider = _channelProvider };
			_subject.Handle(reqest);
			_handler.AssertWasCalled(h => h.Foo());
		}

		[Test]
		public void Should_send_response_if_is_expected()
		{
			var correlationId = Guid.NewGuid().ToString();
			var reqest = new RequestMessage(correlationId, "group", "Foo", new object[0], true) { ChannelProvider = _channelProvider };
			_subject.Handle(reqest);
			_outgoingChannel.AssertWasCalled(ch => ch.Send(Arg<ResponseMessage>.Matches(r => r.CorrelationId == correlationId)));
		}

		[Test]
		public void Should_send_response_for_exception_if_response_is_expected()
		{
			_handler.Stub(h => h.Foo()).Throw(new Exception());

			var correlationId = Guid.NewGuid().ToString();
			var reqest = new RequestMessage(correlationId, "group", "Foo", new object[0], true) { ChannelProvider = _channelProvider };
			_subject.Handle(reqest);
			_outgoingChannel.AssertWasCalled(ch => ch.Send(Arg<ExceptionResponseMessage>.Matches(r => r.CorrelationId == correlationId)));
		}

		[Test]
		public void Should_not_send_response_if_is_not_expected()
		{
			var reqest = new RequestMessage(Guid.NewGuid().ToString(), "group", "Foo", new object[0], false) { ChannelProvider = _channelProvider };
			_subject.Handle(reqest);
			_outgoingChannel.AssertWasNotCalled(ch => ch.Send(Arg<IMessage>.Is.Anything));
		}

		[Test]
		public void Should_not_send_response_for_exception_if_is_not_expected()
		{
			_handler.Stub(h => h.Foo()).Throw(new Exception());
			var reqest = new RequestMessage(Guid.NewGuid().ToString(), "group", "Foo", new object[0], false) { ChannelProvider = _channelProvider };
			_subject.Handle(reqest);
			_outgoingChannel.AssertWasNotCalled(ch => ch.Send(Arg<IMessage>.Is.Anything));
		}

		[Test]
		[Ignore("Not implemented yet")]
		public void Should_handle_channel_failure()
		{
			_outgoingChannel.Stub(ch => ch.Send(Arg<IMessage>.Is.Anything)).Throw(new InvalidOperationException());

			var reqest = new RequestMessage(Guid.NewGuid().ToString(), "group", "Foo", new object[0], true) { ChannelProvider = _channelProvider };
			Assert.DoesNotThrow(() => _subject.Handle(reqest));
		}
	}
}
