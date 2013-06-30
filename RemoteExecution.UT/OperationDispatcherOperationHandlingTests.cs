using System;
using System.Linq;
using NUnit.Framework;
using RemoteExecution.Dispatching;
using RemoteExecution.Messaging;
using RemoteExecution.UT.Helpers;

namespace RemoteExecution.UT
{
	[TestFixture]
	public class OperationDispatcherOperationHandlingTests
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_subject = new OperationDispatcher();
			_networkConnection = new MockNetworkConnection(_subject);
		}

		#endregion

		private OperationDispatcher _subject;
		private MockNetworkConnection _networkConnection;

		private T GetResult<T>(string id)
		{
			return _networkConnection.SentMessages.OfType<IResponse>().Where(r => r.CorrelationId == id).Select(r => r.Value).OfType<T>().SingleOrDefault();
		}

		[Test]
		public void ShouldDispatchOperations()
		{
			_subject.RegisterRequestHandler(typeof(IGreeter), new Greeter());
			_subject.RegisterRequestHandler(typeof(ICalculator), new Calculator());

			_subject.Dispatch(new Request("1", "ICalculator", "Add", new object[] { 2, 3 }, true), _networkConnection);
			_subject.Dispatch(new Request("2", "IGreeter", "Hello", new object[] { "Jack" }, true), _networkConnection);
			Assert.That(GetResult<string>("1"), Is.EqualTo("5"));
			Assert.That(GetResult<string>("2"), Is.EqualTo("Hello Jack"));
		}

		[Test]
		public void ShouldNotAllowToRegisterHandlerIfItDoesNotImplementSpecifiedInterface()
		{
			var ex = Assert.Throws<ArgumentException>(() => _subject.RegisterRequestHandler(typeof(ICalculator), new Greeter()));
			Assert.That(ex.Message, Is.StringStarting("Unable to register Greeter handler: it does not implement ICalculator interface."));
		}

		[Test]
		public void ShouldPassHandlerExceptions()
		{
			_subject.RegisterRequestHandler(typeof(ICalculator), new Calculator());

			_subject.Dispatch(new Request("1", "ICalculator", "Subtract", new object[] { 3, 2 }, true), _networkConnection);
			var ex = Assert.Throws<ArgumentException>(() => GetResult<Exception>("1"));
			Assert.That(ex.Message, Is.EqualTo("test"));
		}

		[Test]
		public void ShouldPassInvalidOperationExceptionIfNoHandlerIsRegistered()
		{
			_subject.Dispatch(new Request("1", "ICalculator", "Add", new object[] { 0, 1 }, true), _networkConnection);
			var ex = Assert.Throws<InvalidOperationException>(() => GetResult<Exception>("1"));
			Assert.That(ex.Message, Is.EqualTo("No handler is defined for ICalculator type."));
		}

		[Test]
		public void ShouldPassInvalidOperationExceptionIfNoOperationIsDefinedInHandler()
		{
			_subject.RegisterRequestHandler(typeof(ICalculator), new Calculator());
			_subject.Dispatch(new Request("1", "ICalculator", "Add", new object[] { null, 3.14, "test" }, true), _networkConnection);

			var ex = Assert.Throws<InvalidOperationException>(() => GetResult<Exception>("1"));
			Assert.That(ex.Message, Is.EqualTo("Unable to call Add(null,Double,String) method on ICalculator handler: no matching method was found."));
		}
	}
}