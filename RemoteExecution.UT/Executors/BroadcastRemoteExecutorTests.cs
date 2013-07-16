using System;
using NUnit.Framework;
using RemoteExecution.Channels;
using RemoteExecution.Executors;
using RemoteExecution.Messages;
using RemoteExecution.UT.Helpers;
using Rhino.Mocks;

namespace RemoteExecution.UT.Executors
{
	[TestFixture]
	public class BroadcastRemoteExecutorTests
	{
		private BroadcastRemoteExecutor _subject;
		private IBroadcastChannel _broadcastChannel;

		public interface IMyInterface : ICalculator
		{
			void VoidMethod(string text);
		}

		[SetUp]
		public void SetUp()
		{
			_broadcastChannel = MockRepository.GenerateMock<IBroadcastChannel>();
			_subject = new BroadcastRemoteExecutor(_broadcastChannel);
		}

		[Test]
		public void Should_send_message_on_channel()
		{
			_subject.Create<INotifier>().Notify("test");
			_broadcastChannel.AssertWasCalled(ch => ch.Send(Arg<IMessage>.Is.Anything));
		}

		[Test]
		public void Should_not_allow_to_generate_proxy_if_interface_have_two_way_methods()
		{
			var ex = Assert.Throws<InvalidOperationException>(() => _subject.Create<ICalculator>());
			Assert.That(ex.Message, Is.EqualTo("ICalculator interface cannot be used for broadcasting because some of its methods returns result."));
		}

		[Test]
		public void Should_not_allow_to_generate_proxy_if_interface_inherits_two_way_methods()
		{
			var ex = Assert.Throws<InvalidOperationException>(() => _subject.Create<IMyInterface>());
			Assert.That(ex.Message, Is.EqualTo("IMyInterface interface cannot be used for broadcasting because some of its methods returns result."));
		}
	}
}