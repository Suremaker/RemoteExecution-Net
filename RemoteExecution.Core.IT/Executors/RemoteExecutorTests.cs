using System;
using NUnit.Framework;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Dispatchers.Messages;
using RemoteExecution.Core.Executors;
using RemoteExecution.Core.IT.Helpers;

namespace RemoteExecution.Core.IT.Executors
{
	[TestFixture]
	public class RemoteExecutorTests
	{
		private IRemoteExecutor _subject;
		private MessageDispatcher _messageDispatcher;
		private MockDuplexChannel _channel;
		private ICalculator _calculator;

		[SetUp]
		public void SetUp()
		{
			_messageDispatcher = new MessageDispatcher();
			_channel = new MockDuplexChannel();
			_subject = new RemoteExecutor(_channel, _messageDispatcher);
			_calculator = _subject.Create<ICalculator>();
		}

		[Test]
		public void Should_generate_proxy()
		{
			Assert.That(_calculator, Is.Not.Null);
		}

		[Test]
		public void Should_return_value()
		{
			const int value = 33;
			_channel.OnSend += msg => _messageDispatcher.Dispatch(new ResponseMessage(msg.CorrelationId, value));

			Assert.That(_calculator.Add(30, 3), Is.EqualTo(value));
		}

		[Test]
		public void Should_throw_exception()
		{
			const string message = "message";
			_channel.OnSend += msg => _messageDispatcher.Dispatch(new ExceptionResponseMessage(msg.CorrelationId, typeof(InvalidOperationException), message));

			var ex = Assert.Throws<InvalidOperationException>(() => _calculator.Add(3, 2));
			Assert.That(ex.Message, Is.EqualTo(message));
		}
	}
}
