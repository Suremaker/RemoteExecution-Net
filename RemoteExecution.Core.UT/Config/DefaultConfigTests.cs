using NUnit.Framework;
using RemoteExecution.Core.Config;

namespace RemoteExecution.Core.UT.Config
{
	[TestFixture]
	public class DefaultConfigTests
	{
		[Test]
		public void Should_defaults_be_specified()
		{
			Assert.That(DefaultConfig.RemoteExecutorFactory, Is.Not.Null);
			Assert.That(DefaultConfig.TaskScheduler, Is.Not.Null);
		}
	}
}