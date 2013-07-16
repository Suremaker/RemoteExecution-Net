using System;
using System.Threading;
using NUnit.Framework;
using RemoteExecution.Channels;
using RemoteExecution.Handlers;
using RemoteExecution.Messages;
using Rhino.Mocks;

namespace RemoteExecution.UT.Handlers
{
	[TestFixture]
	public class ResponseHandlerTests
	{
		private ResponseHandler _subject;
		private IMessageChannel _channel;

		[SetUp]
		public void SetUp()
		{
			_channel = MockRepository.GenerateMock<IMessageChannel>();
			_subject = new ResponseHandler(_channel);
		}

		[Test]
		public void Should_set_channel_as_target_channel()
		{
			Assert.That(_subject.TargetChannel, Is.SameAs(_channel));
		}

		[Test]
		public void Should_generate_handler_id()
		{
			Assert.That(_subject.Id, Is.Not.EqualTo(Guid.Empty));
		}

		[Test]
		public void Should_wait_for_response_return_when_message_is_handled()
		{
			var waitingThread = new Thread(() => _subject.WaitForResponse());
			waitingThread.Start();

			_subject.Handle(new Response(), null);

			if (!waitingThread.Join(TimeSpan.FromMilliseconds(200)))
			{
				waitingThread.Abort();
				Assert.Fail("WaitForResponse did not finished");
			}
		}

		[Test]
		public void Should_return_value_andled_message()
		{
			const string expectedValue = "test";
			_subject.Handle(new Response(null, expectedValue), null);
			Assert.That(_subject.GetValue(), Is.EqualTo(expectedValue));
		}
	}
}