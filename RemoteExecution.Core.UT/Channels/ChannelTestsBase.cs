using NUnit.Framework;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Dispatchers.Messages;
using RemoteExecution.Core.Serializers;
using Rhino.Mocks;

namespace RemoteExecution.Core.UT.Channels
{
	public abstract class ChannelTestsBase<TChannel> where TChannel : ITestableOutputChannel
	{
		protected readonly byte[] SerializedData = new byte[] { 1, 2, 3 };
		protected IMessageSerializer MessageSerializer;
		protected TChannel Subject;

		[SetUp]
		public void SetUp()
		{
			MessageSerializer = MockRepository.GenerateMock<IMessageSerializer>();
			Subject = CreateSubject();
		}

		[Test]
		public void Should_dispose_close_channel()
		{
			Subject.Dispose();
			Assert.That(Subject.IsOpen, Is.False);
		}

		[Test]
		public void Should_serialize_request_and_pass_to_implementation()
		{
			MessageSerializer.Stub(s => s.Serialize(Arg<IMessage>.Is.Anything)).Return(SerializedData);
			var requestMessage = new RequestMessage("1", "abc", "def", new object[] { 1, 2, 3 }, true);
			Subject.Send(requestMessage);
			MessageSerializer.AssertWasCalled(s => s.Serialize(requestMessage));
			Assert.That(Subject.SentData, Is.EqualTo(SerializedData));
		}

		[Test]
		public void Should_throw_not_connected_exception_on_send_if_connection_is_closed()
		{
			Subject.Dispose();
			var ex = Assert.Throws<NotConnectedException>(() => Subject.Send(new ResponseMessage()));
			Assert.That(ex.Message, Is.EqualTo("Channel is closed."));
		}

		protected abstract TChannel CreateSubject();
	}
}