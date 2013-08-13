using System;
using System.Threading;
using NUnit.Framework;
using RemoteExecution.Core.Dispatchers.Handlers;
using RemoteExecution.Core.Dispatchers.Messages;

namespace RemoteExecution.Core.UT.Dispatchers.Handlers
{
	[TestFixture]
	public class ResponseHandlerTests
	{
		private ResponseHandler _subject;
		private readonly Guid _groupId = Guid.NewGuid();

		[SetUp]
		public void SetUp()
		{
			_subject = new ResponseHandler(_groupId);
		}

		[Test]
		public void Should_set_handler_group_id()
		{
			Assert.That(_subject.HandlerGroupId, Is.EqualTo(_groupId.ToString()));
		}

		[Test]
		public void Should_generate_handled_message_type()
		{
			Assert.That(Guid.Parse(_subject.HandledMessageType), Is.Not.EqualTo(Guid.Empty));
		}

		[Test]
		public void Should_wait_for_response_return_when_message_is_handled()
		{
			var waitingThread = new Thread(() => _subject.WaitForResponse());
			waitingThread.Start();

			_subject.Handle(new ResponseMessage());

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
			_subject.Handle(new ResponseMessage(null, expectedValue));
			Assert.That(_subject.GetValue(), Is.EqualTo(expectedValue));
		}
	}
}