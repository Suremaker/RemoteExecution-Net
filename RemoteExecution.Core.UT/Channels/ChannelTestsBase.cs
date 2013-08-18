using NUnit.Framework;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Dispatchers.Messages;
using RemoteExecution.Core.Serializers;
using Rhino.Mocks;

namespace RemoteExecution.Core.UT.Channels
{
	public abstract class ChannelTestsBase<TChannel> where TChannel : ITestableOutputChannel
	{
		protected TChannel Subject;
		protected IMessageSerializer MessageSerializer;
		protected readonly byte[] SerializedData = new byte[] { 1, 2, 3 };

		[SetUp]
		public void SetUp()
		{
			MessageSerializer = MockRepository.GenerateMock<IMessageSerializer>();			
			Subject = CreateSubject();
		}

		protected abstract TChannel CreateSubject();

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
	}
}