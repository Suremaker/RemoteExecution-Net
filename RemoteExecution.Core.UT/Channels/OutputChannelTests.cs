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

		#region ITestableOutputChannel Members

		public override bool IsOpen
		{
			get { return _isOpen; }
		}

		public byte[] SentData { get; private set; }

		#endregion

		protected override void Close()
		{
			_isOpen = false;
			FireChannelClosed();
		}

		protected override void SendData(byte[] data)
		{
			SentData = data;
		}
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
