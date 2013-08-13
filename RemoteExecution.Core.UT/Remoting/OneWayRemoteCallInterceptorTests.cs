﻿using System.Linq;
using NUnit.Framework;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Dispatchers.Messages;
using RemoteExecution.Core.Remoting;
using Rhino.Mocks;
using Spring.Aop.Framework;

namespace RemoteExecution.Core.UT.Remoting
{
	[TestFixture]
	public class OneWayRemoteCallInterceptorTests
	{
		public interface ITestInterface
		{
			string Hello(int x);
			void Notify(string text);
		}

		private IChannelProvider _channelProvider;
		private MockRepository _repository;
		private IOutgoingMessageChannel _channel;
		private const string _interfaceName = "testInterface";

		[SetUp]
		public void SetUp()
		{
			_repository = new MockRepository();
			_channelProvider = _repository.DynamicMock<IChannelProvider>();
			_channel = _repository.DynamicMock<IOutgoingMessageChannel>();
			_channelProvider.Stub(p => p.GetOutgoingChannel()).Return(_channel);
		}

		private ITestInterface GetInvocationHelper()
		{
			var subject = new OneWayRemoteCallInterceptor(_channelProvider, _interfaceName);
			return (ITestInterface)new ProxyFactory(typeof(ITestInterface), subject).GetProxy();
		}

		[Test]
		public void Should_send_request_message_with_no_response_expected()
		{
			_repository.ReplayAll();
			GetInvocationHelper().Notify("text");

			_channel.AssertWasCalled(ch => ch.Send(Arg<RequestMessage>.Matches(r => !r.IsResponseExpected)));
		}

		[Test]
		public void Should_send_message_with_method_details()
		{
			_repository.ReplayAll();

			const int methodArg = 5;
			GetInvocationHelper().Hello(methodArg);

			_channel.AssertWasCalled(ch => ch.Send(Arg<RequestMessage>.Matches(m =>
				m.Args.SequenceEqual(new object[] { methodArg }) &&
				m.MethodName == "Hello" &&
				m.MessageType == _interfaceName)));
		}
	}
}