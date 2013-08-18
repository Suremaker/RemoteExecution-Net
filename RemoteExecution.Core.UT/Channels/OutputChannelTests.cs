using NUnit.Framework;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Serializers;

namespace RemoteExecution.Core.UT.Channels
{
	public interface ITestableOutputChannel : IOutputChannel
	{
		byte[] SentData { get; }
	}

	public class TestableOutputChannel : OutputChannel, ITestableOutputChannel
	{
		private bool _isOpen = true;

		public TestableOutputChannel(IMessageSerializer serializer)
			: base(serializer)
		{
		}

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
	}

	[TestFixture]
	public class OutputChannelTests : ChannelTestsBase<TestableOutputChannel>
	{
		protected override TestableOutputChannel CreateSubject()
		{
			return new TestableOutputChannel(MessageSerializer);
		}
	}
}
