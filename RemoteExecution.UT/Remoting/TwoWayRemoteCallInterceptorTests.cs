using System.Linq;
using NUnit.Framework;
using RemoteExecution.Channels;
using RemoteExecution.Dispatchers;
using RemoteExecution.Handlers;
using RemoteExecution.Messages;
using RemoteExecution.Remoting;
using Rhino.Mocks;
using Spring.Aop.Framework;

namespace RemoteExecution.UT.Remoting
{
	[TestFixture]
	public class TwoWayRemoteCallInterceptorTests
	{
		class TestableTwoWayRemoteCallInterceptor : TwoWayRemoteCallInterceptor
		{
			private readonly IResponseHandler _responseHandler;

			public TestableTwoWayRemoteCallInterceptor(IOperationDispatcher operationDispatcher, IMessageChannel channel, IResponseHandler responseHandler, string interfaceName)
				: base(operationDispatcher, channel, interfaceName)
			{
				_responseHandler = responseHandler;
			}

			protected override IResponseHandler CreateResponseHandler()
			{
				return _responseHandler;
			}
		}

		public interface ITestInterface
		{
			string Hello(int x);
			void Notify(string text);
		}

		private IOperationDispatcher _dispatcher;
		private IMessageChannel _channel;
		private IResponseHandler _responseHandler;
		private MockRepository _repository;
		private const string _handlerId = "handlerID";
		private const string _interfaceName = "testInterface";

		[SetUp]
		public void SetUp()
		{
			_repository = new MockRepository();
			_dispatcher = _repository.DynamicMock<IOperationDispatcher>();
			_channel = _repository.DynamicMock<IMessageChannel>();
			_responseHandler = _repository.DynamicMock<IResponseHandler>();
		}

		private ITestInterface GetInvocationHelper()
		{
			var subject = new TestableTwoWayRemoteCallInterceptor(_dispatcher, _channel, _responseHandler, _interfaceName);
			return (ITestInterface)new ProxyFactory(typeof(ITestInterface), subject).GetProxy();
		}

		[Test]
		public void Should_execute_operations_in_order()
		{
			using (_repository.Ordered())
			{
				Expect.Call(() => _dispatcher.RegisterResponseHandler(_responseHandler));
				Expect.Call(() => _channel.Send(Arg<IMessage>.Is.Anything));
				Expect.Call(() => _responseHandler.WaitForResponse());
				Expect.Call(() => _dispatcher.UnregisterResponseHandler(_responseHandler));
				Expect.Call(() => _responseHandler.GetValue());
			}
			_repository.ReplayAll();
			GetInvocationHelper().Hello(5);
			_repository.VerifyAll();
		}

		[Test]
		public void Should_wait_for_response_even_if_method_returns_void()
		{
			_repository.ReplayAll();
			GetInvocationHelper().Notify("text");

			_channel.AssertWasCalled(ch => ch.Send(Arg<Request>.Matches(r => r.IsResponseExpected)));
			_dispatcher.AssertWasCalled(d => d.RegisterResponseHandler(_responseHandler));
			_responseHandler.AssertWasCalled(h => h.WaitForResponse());
		}

		[Test]
		public void Should_send_message_with_handler_id()
		{
			Expect.Call(_responseHandler.Id).Return(_handlerId);
			_repository.ReplayAll();
			GetInvocationHelper().Hello(5);
			_channel.AssertWasCalled(ch => ch.Send(Arg<IMessage>.Matches(m => m.CorrelationId == _handlerId)));
		}

		[Test]
		public void Should_send_message_with_method_details()
		{
			Expect.Call(_responseHandler.Id).Return(_handlerId);
			_repository.ReplayAll();

			const int methodArg = 5;
			GetInvocationHelper().Hello(methodArg);

			_channel.AssertWasCalled(ch => ch.Send(Arg<Request>.Matches(m =>
				m.Args.SequenceEqual(new object[] { methodArg }) &&
				m.OperationName == "Hello" &&
				m.GroupId == _interfaceName)));
		}
	}
}