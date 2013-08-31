using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using RemoteExecution.Core.IT.Helpers;
using RemoteExecution.Dispatchers;
using RemoteExecution.Dispatchers.Messages;
using RemoteExecution.Executors;

namespace RemoteExecution.Core.IT.Executors
{
	[TestFixture]
	public class RemoteExecutorTests
	{
		private IRemoteExecutor _subject;
		private MessageDispatcher _messageDispatcher;
		private MockDuplexChannel _channel;
		private ICalculator _calculator;

		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_messageDispatcher = new MessageDispatcher();
			_channel = new MockDuplexChannel();
			_subject = new RemoteExecutor(_channel, _messageDispatcher);
			_calculator = _subject.Create<ICalculator>();
		}

		#endregion

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
		public void Should_support_concurrent_operations()
		{
			var requests = new ConcurrentStack<RequestMessage>();
			_channel.OnSend += r => requests.Push((RequestMessage)r);

			int validResults = 0;

			var tasks = new List<Thread>();
			for (int i = 0; i < 10; ++i)
			{
				var thread = new Thread(o =>
				{
					int add = _calculator.Add((int)o, 0);
					if (Equals(add, o))
						Interlocked.Increment(ref validResults);
				});
				tasks.Add(thread);
				thread.Start(i);
			}

			Thread.Sleep(500);
			foreach (RequestMessage request in requests)
				_messageDispatcher.Dispatch(new ResponseMessage(request.CorrelationId, request.Args[0]));

			foreach (Thread thread in tasks)
				thread.Join();

			Assert.That(validResults, Is.EqualTo(tasks.Count));
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
