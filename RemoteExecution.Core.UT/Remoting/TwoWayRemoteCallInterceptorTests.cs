using System.Linq;
using NUnit.Framework;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Dispatchers.Handlers;
using RemoteExecution.Core.Dispatchers.Messages;
using RemoteExecution.Core.Remoting;
using Rhino.Mocks;
using Spring.Aop.Framework;

namespace RemoteExecution.Core.UT.Remoting
{
	[TestFixture]
	public class TwoWayRemoteCallInterceptorTests
	{
		class TestableTwoWayRemoteCallInterceptor : TwoWayRemoteCallInterceptor
		{
			private readonly IResponseHandler _responseHandler;

			public TestableTwoWayRemoteCallInterceptor(IOutputChannel channel, IMessageDispatcher dispatcher, IResponseHandler responseHandler, string interfaceName)
				: base(channel, dispatcher, interfaceName)
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

		private IMessageDispatcher _messageDispatcher;
		private IOutputChannel _channel;
		private IResponseHandler _responseHandler;
		private MockRepository _repository;
		private const string _handlerId = "handlerID";
		private const string _interfaceName = "testInterface";

		private ITestInterface GetInvocationHelper()
		{
			var subject = new TestableTwoWayRemoteCallInterceptor(_channel, _messageDispatcher, _responseHandler, _interfaceName);
			return (ITestInterface)new ProxyFactory(typeof(ITestInterface), subject).GetProxy();
		}

		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_repository = new MockRepository();
			_messageDispatcher = _repository.DynamicMock<IMessageDispatcher>();
			_channel = _repository.DynamicMock<IOutputChannel>();
			_responseHandler = _repository.DynamicMock<IResponseHandler>();
		}

		#endregion

		[Test]
		public void Should_execute_operations_in_order()
		{
			using (_repository.Ordered())
			{
				Expect.Call(() => _messageDispatcher.Register(_responseHandler));

				Expect.Call(() => _channel.Send(Arg<IMessage>.Is.Anything));
				Expect.Call(() => _responseHandler.WaitForResponse());

				Expect.Call(_responseHandler.HandledMessageType).Return(_handlerId);
				Expect.Call(() => _messageDispatcher.Unregister(_handlerId));
				Expect.Call(_responseHandler.GetValue());
			}
			_repository.ReplayAll();
			GetInvocationHelper().Hello(5);
			_repository.VerifyAll();
		}

		[Test]
		public void Should_send_message_with_handler_id()
		{
			Expect.Call(_responseHandler.HandledMessageType).Return(_handlerId);
			_repository.ReplayAll();
			GetInvocationHelper().Hello(5);
			_channel.AssertWasCalled(ch => ch.Send(Arg<IMessage>.Matches(m => m.CorrelationId == _handlerId)));
		}

		[Test]
		public void Should_send_message_with_method_details()
		{
			Expect.Call(_responseHandler.HandledMessageType).Return(_handlerId);
			_repository.ReplayAll();

			const int methodArg = 5;
			GetInvocationHelper().Hello(methodArg);

			_channel.AssertWasCalled(ch => ch.Send(Arg<RequestMessage>.Matches(m =>
				m.Args.SequenceEqual(new object[] { methodArg }) &&
				m.MethodName == "Hello" &&
				m.MessageType == _interfaceName)));
		}

		[Test]
		public void Should_wait_for_response_even_if_method_returns_void()
		{
			_repository.ReplayAll();
			GetInvocationHelper().Notify("text");

			_channel.AssertWasCalled(ch => ch.Send(Arg<RequestMessage>.Matches(r => r.IsResponseExpected)));
			_messageDispatcher.AssertWasCalled(d => d.Register(_responseHandler));
			_responseHandler.AssertWasCalled(h => h.WaitForResponse());
		}
	}
}