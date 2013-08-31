using System;
using NUnit.Framework;
using RemoteExecution.Channels;
using RemoteExecution.Dispatchers.Handlers;
using RemoteExecution.Dispatchers.Messages;
using Rhino.Mocks;

namespace RemoteExecution.Core.UT.Dispatchers.Handlers
{
	[TestFixture]
	public class DefaultRequestHandlerTests
	{
		private DefaultRequestHandler _subject;
		private IDuplexChannel _channel;

		private IRequestMessage CreateRequest(string correlationId, string interfaceName, bool isResponseExpected)
		{
			return new RequestMessage(correlationId, interfaceName, "method", new object[0], isResponseExpected) { Channel = _channel};
		}

		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_subject = new DefaultRequestHandler();
			_channel = MockRepository.GenerateMock<IDuplexChannel>();
		}

		#endregion

		[Test]
		[Ignore("Not implemented yet")]
		public void Should_ignore_errors_on_send()
		{
			_channel.Stub(c => c.Send(Arg<IMessage>.Is.Anything)).Throw(new InvalidOperationException());
			Assert.DoesNotThrow(() => _subject.Handle(CreateRequest("correlation", "interface", true)));
		}

		[Test]
		[TestCase(typeof(ExceptionResponseMessage))]
		[TestCase(typeof(ResponseMessage))]
		public void Should_ignore_messages_other_than_requests(Type messageType)
		{
			Assert.DoesNotThrow(() => _subject.Handle((IMessage)Activator.CreateInstance(messageType)));
		}

		[Test]
		public void Should_not_send_any_response_for_given_request_if_response_is_not_expected()
		{
			_subject.Handle(CreateRequest("correlation", "interface", false));
			_channel.AssertWasNotCalled(c => c.Send(Arg<IMessage>.Is.Anything));
		}

		[Test]
		public void Should_send_exception_response_for_given_request_if_response_is_expected()
		{
			const string correlationId = "correlation";
			const string interfaceName = "interface";

			_subject.Handle(CreateRequest(correlationId, interfaceName, true));

			_channel.AssertWasCalled(c => c.Send(Arg<ExceptionResponseMessage>.Matches(m =>
			                                                                           m.CorrelationId == correlationId &&
			                                                                           m.ExceptionType == typeof(InvalidOperationException).AssemblyQualifiedName &&
			                                                                           m.Message == string.Format("No handler is defined for {0} type.", interfaceName))));
		}
	}
}