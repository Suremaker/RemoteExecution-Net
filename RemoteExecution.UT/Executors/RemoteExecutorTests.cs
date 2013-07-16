using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using RemoteExecution.Dispatchers;
using RemoteExecution.Executors;
using RemoteExecution.Handlers;
using RemoteExecution.Messages;
using RemoteExecution.UT.Helpers;
using Rhino.Mocks;

namespace RemoteExecution.UT.Executors
{
	[TestFixture]
	public class RemoteExecutorTests
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_operationDispatcher = MockRepository.GenerateMock<IOperationDispatcher>();
			_channel = new MockMessageChannel();
			_remoteExecutor = new RemoteExecutor(_operationDispatcher, _channel);
			_subject = _remoteExecutor.Create<ICalculator>();
			_currentHandler = null;
		}

		#endregion

		private ICalculator _subject;
		private IOperationDispatcher _operationDispatcher;
		private MockMessageChannel _channel;
		private IResponseHandler _currentHandler;
		private RemoteExecutor _remoteExecutor;

		private void AsyncTest(Action action)
		{
			Exception testException = null;
			var thread = new Thread(() =>
			{
				try
				{
					action();
				}
				catch (Exception e)
				{
					testException = e;
				}
			});
			thread.Start();
			if (!thread.Join(250))
				Assert.Fail("Operation timed out!");
			else if (testException != null)
				throw testException;
		}

		private Request EnsureRequest()
		{
			Assert.That(_channel.SentMessages.Count, Is.EqualTo(1));
			var request = (Request)_channel.SentMessages.Single();
			return request;
		}

		private void BindMessageLoopback(object value)
		{
			_operationDispatcher.Stub(d => d.RegisterResponseHandler(null)).IgnoreArguments().WhenCalled(UpdateCurrentHandler);

			_channel.OnMessageSend = m => _currentHandler.Handle(new Response(m.CorrelationId, value), _channel);
		}

		private void BindThrowMessageLoopback(Exception value)
		{
			_operationDispatcher.Stub(d => d.RegisterResponseHandler(null)).IgnoreArguments().WhenCalled(UpdateCurrentHandler);

			_channel.OnMessageSend = m => _currentHandler.Handle(new ExceptionResponse(m.CorrelationId, value.GetType(), value.Message), _channel);
		}

		private void UpdateCurrentHandler(MethodInvocation a)
		{
			_currentHandler = (IResponseHandler)a.Arguments[0];
		}

		[Test]
		public void ShouldGenerateClient()
		{
			Assert.That(_subject, Is.Not.Null);
		}

		[Test]
		public void ShouldRegisterAndUnregisterHandlerWithMessageGroupId()
		{
			BindMessageLoopback("8");
			AsyncTest(() =>
			{
				_subject.Add(3, 5);

				Assert.That(_currentHandler, Is.Not.Null);
				Request request = EnsureRequest();
				Assert.That(_currentHandler.Id, Is.EqualTo(request.CorrelationId));

				_operationDispatcher.AssertWasCalled(d => d.UnregisterResponseHandler(_currentHandler));
			});
		}

		[Test]
		public void ShouldReturnResponseValue()
		{
			BindMessageLoopback("8");
			AsyncTest(() => Assert.That(_subject.Add(3, 5), Is.EqualTo("8")));
		}

		[Test]
		public void ShouldSendOperationRequest()
		{
			BindMessageLoopback("8");
			AsyncTest(() =>
				{
					_subject.Add(3, 5);

					Request request = EnsureRequest();

					Assert.That(request, Is.Not.Null);
					Assert.That(request.GroupId, Is.EqualTo("ICalculator"));
					Assert.That(request.OperationName, Is.EqualTo("Add"));
					Assert.That(request.Args, Is.EqualTo(new object[] { 3, 5 }));
				});
		}

		[Test]
		public void ShouldThrowIfResponseIsException()
		{
			BindThrowMessageLoopback(new Exception("test"));
			AsyncTest(() =>
				{
					var ex = Assert.Throws<Exception>(() => _subject.Add(2, 3));
					Assert.That(ex.Message, Is.EqualTo("test"));
				});
		}
	}
}
