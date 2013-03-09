using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using RemoteExecution.Dispatching;
using RemoteExecution.Handling;
using RemoteExecution.Messaging;
using RemoteExecution.UT.Helpers;
using Rhino.Mocks;

namespace RemoteExecution.UT
{
	[TestFixture]
	public class RemotingClientTests
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_operationDispatcher = MockRepository.GenerateMock<IOperationDispatcher>();
			_connection = new MockNetworkConnection(_operationDispatcher);
			_remoteExecutor = new RemoteExecutor(_connection);
			_subject = _remoteExecutor.Create<ICalculator>();
			_currentHandler = null;
		}

		#endregion

		private ICalculator _subject;
		private IOperationDispatcher _operationDispatcher;
		private MockNetworkConnection _connection;
		private IHandler _currentHandler;
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
			Assert.That(_connection.SentMessages.Count, Is.EqualTo(1));
			var request = (Request)_connection.SentMessages.Single();
			return request;
		}

		private void BindMessageLoopback(object value)
		{
			_operationDispatcher.Stub(d => d.AddHandler(null)).IgnoreArguments().WhenCalled(UpdateCurrentHandler);

			_connection.OnMessageSend = m => _currentHandler.Handle(new Response(m.CorrelationId, value), _connection);
		}

		private void UpdateCurrentHandler(MethodInvocation a)
		{
			_currentHandler = (IHandler)a.Arguments[0];
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

					_operationDispatcher.AssertWasCalled(d => d.RemoveHandler(_currentHandler));
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
			BindMessageLoopback(new Exception("test"));
			AsyncTest(() =>
				{
					var ex = Assert.Throws<Exception>(() => _subject.Add(2, 3));
					Assert.That(ex.Message, Is.EqualTo("test"));
				});
		}
	}
}
