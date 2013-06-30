using System.Linq;
using NUnit.Framework;
using RemoteExecution.Messaging;
using RemoteExecution.Remoting;
using Rhino.Mocks;
using Spring.Aop.Framework;

namespace RemoteExecution.UT
{
	[TestFixture]
	public class OneWayRemoteCallInterceptorTests
	{
		public interface ITestInterface
		{
			string Hello(int x);
			void Notify(string text);
		}

		private IMessageChannel _channel;
		private MockRepository _repository;
		private const string _interfaceName = "testInterface";

		[SetUp]
		public void SetUp()
		{
			_repository = new MockRepository();
			_channel = _repository.DynamicMock<IMessageChannel>();
		}

		private ITestInterface GetInvocationHelper()
		{
			var subject = new OneWayRemoteCallInterceptor(_channel, _interfaceName);
			return (ITestInterface)new ProxyFactory(typeof(ITestInterface), subject).GetProxy();
		}

		[Test]
		public void Should_send_request_message_with_no_response_expected()
		{
			_repository.ReplayAll();
			GetInvocationHelper().Notify("text");

			_channel.AssertWasCalled(ch => ch.Send(Arg<Request>.Matches(r => !r.IsResponseExpected)));
		}

		[Test]
		public void Should_send_message_with_method_details()
		{
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