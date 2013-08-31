using NUnit.Framework;
using RemoteExecution.Core.Config;

namespace RemoteExecution.Core.UT.Config
{
	[TestFixture]
	public class ClientConfigTests
	{
		private IClientConfig _subject;

		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_subject = new ClientConfig();
		}

		#endregion

		[Test]
		public void Should_set_default_remote_executor_factory()
		{
			Assert.That(_subject.RemoteExecutorFactory, Is.SameAs(DefaultConfig.RemoteExecutorFactory));
		}

		[Test]
		public void Should_set_default_task_scheduler()
		{
			Assert.That(_subject.TaskScheduler, Is.SameAs(DefaultConfig.TaskScheduler));
		}
	}
}
