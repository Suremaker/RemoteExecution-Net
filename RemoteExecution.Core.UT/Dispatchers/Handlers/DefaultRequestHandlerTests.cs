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
	public class DefaultRequestHandlerTests
	{
		private DefaultRequestHandler _subject;
		private IChannelProvider _channelProvider;
		private IOutgoingMessageChannel _outgoingChannel;

		[SetUp]
		public void SetUp()
		{
			_subject = new DefaultRequestHandler();
			_channelProvider = MockRepository.GenerateMock<IChannelProvider>();
			_outgoingChannel = MockRepository.GenerateMock<IOutgoingMessageChannel>();
			_channelProvider.Stub(p => p.GetOutgoingChannel()).Return(_outgoingChannel);
		}

		[Test]
		[TestCase(typeof(ExceptionResponseMessage))]
		[TestCase(typeof(ResponseMessage))]
		public void Should_ignore_messages_other_than_requests(Type messageType)
		{
			Assert.DoesNotThrow(() => _subject.Handle((IMessage)Activator.CreateInstance(messageType)));
		}

		[Test]
		public void Should_send_exception_response_for_given_request_if_response_is_expected()
		{
			const string correlationId = "correlation";
			const string interfaceName = "interface";

			_subject.Handle(CreateRequest(correlationId, interfaceName, true));

			_outgoingChannel.AssertWasCalled(c => c.Send(Arg<ExceptionResponseMessage>.Matches(m =>
				m.CorrelationId == correlationId &&
				m.ExceptionType == typeof(InvalidOperationException).AssemblyQualifiedName &&
				m.Message == string.Format("No handler is defined for {0} type.", interfaceName))));
		}

		[Test]
		public void Should_not_send_any_response_for_given_request_if_response_is_not_expected()
		{
			_subject.Handle(CreateRequest("correlation", "interface", false));
			_outgoingChannel.AssertWasNotCalled(c => c.Send(Arg<IMessage>.Is.Anything));
		}

		[Test]
		[Ignore("Not implemented yet")]
		public void Should_ignore_errors_on_send()
		{
			_outgoingChannel.Stub(c => c.Send(Arg<IMessage>.Is.Anything)).Throw(new InvalidOperationException());
			Assert.DoesNotThrow(() => _subject.Handle(CreateRequest("correlation", "interface", true)));
		}

		private IRequestMessage CreateRequest(string correlationId, string interfaceName, bool isResponseExpected)
		{
			return new RequestMessage(correlationId, interfaceName, "method", new object[0], isResponseExpected) { ChannelProvider = _channelProvider };
		}
	}
}