using NUnit.Framework;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Dispatchers.Messages;
using RemoteExecution.Core.Serializers;
using Rhino.Mocks;

namespace RemoteExecution.Core.UT.Channels
{
	public class TestableDuplexChannel : DuplexChannel, ITestableOutputChannel
	{
		private bool _isOpen;

		public TestableDuplexChannel(IMessageSerializer serializer)
			: base(serializer)
		{
			Received += msg => MessageReceived = msg;
		}

		public IMessage MessageReceived { get; private set; }

		protected override void Close()
		{
			_isOpen = false;
			FireChannelClosed();
		}

		public override bool IsOpen
		{
			get { return _isOpen; }
		}

		protected override void SendData(byte[] data)
		{
			SentData = data;
		}

		public byte[] SentData { get; private set; }

		public new void OnReceive(byte[] data)
		{
			base.OnReceive(data);
		}
	}

	[TestFixture]
	public class DuplexChannelTests : ChannelTestsBase<TestableDuplexChannel>
	{
		protected override TestableDuplexChannel CreateSubject()
		{
			return new TestableDuplexChannel(MessageSerializer);
		}

		[Test]
		public void Should_on_received_deserialize_and_fire_message()
		{
			var message = new ResponseMessage();
			MessageSerializer.Stub(s => s.Deserialize(Arg<byte[]>.Is.Anything)).Return(message);

			Subject.OnReceive(SerializedData);
			MessageSerializer.AssertWasCalled(s => s.Deserialize(SerializedData));
			Assert.That(Subject.MessageReceived, Is.EqualTo(message));
		}

		[Test]
		public void Should_assign_itself_to_request_message()
		{
			var message = new RequestMessage();
			MessageSerializer.Stub(s => s.Deserialize(Arg<byte[]>.Is.Anything)).Return(message);

			Subject.OnReceive(SerializedData);
			var actualMessage = ((RequestMessage)Subject.MessageReceived);
			Assert.That(actualMessage.Channel, Is.EqualTo(Subject));
		}
	}
}
