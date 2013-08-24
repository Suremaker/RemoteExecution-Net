using System;
using NUnit.Framework;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Executors;
using Rhino.Mocks;

namespace RemoteExecution.Core.UT.Executors
{
	[TestFixture]
	public class BroadcastRemoteExecutorTests
	{
		private BroadcastRemoteExecutor _subject;
		private IBroadcastChannel _broadcastChannel;

		public interface ICalculator
		{
			int Add(int x, int y);
		}

		public interface IMyInterface : ICalculator
		{
			void VoidMethod(string text);
		}

		public interface INotifier
		{
			void Notify(string test);
		}

		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_broadcastChannel = MockRepository.GenerateMock<IBroadcastChannel>();
			_subject = new BroadcastRemoteExecutor(_broadcastChannel);
		}

		#endregion

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

		[Test]
		public void Should_send_message_on_channel()
		{
			_subject.Create<INotifier>().Notify("test");
			_broadcastChannel.AssertWasCalled(ch => ch.Send(Arg<IMessage>.Is.Anything));
		}
	}
}