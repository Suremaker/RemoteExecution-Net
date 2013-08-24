using NUnit.Framework;
using RemoteExecution.AT.Expectations;

namespace RemoteExecution.AT.TransportLayer
{
	[TestFixture]
	public class LidgrenProviderTests : BehaviorExpectations
	{
		protected override string ServerConnectionListenerUri
		{
			get { return "net://0.0.0.0:3251/test_app_id"; }
		}

		protected override string ClientChannelUri
		{
			get { return "net://localhost:3251/test_app_id"; }
		}
	}
}