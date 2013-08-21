using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace RemoteExecution.AT.Expectations
{
	public abstract partial class BehaviorExpectations
	{
		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void Should_client_connection_notify_once_about_connection_close(bool shouldCloseOnServerSide)
		{
			var closeCount = 0;
			using (var server = StartServer())
			using (var client = OpenClientConnection())
			{
				if (shouldCloseOnServerSide)
					server.ActiveConnections.Single().Dispose();

				client.Closed += () => Interlocked.Increment(ref closeCount);
			}

			Assert.That(closeCount, Is.EqualTo(1));
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void Should_server_notify_once_about_connection_close(bool shouldCloseOnServerSide)
		{
			var closeCount = 0;
			using (var server = StartServer())
			{
				server.ConnectionClosed += connection => Interlocked.Increment(ref closeCount);
				using (OpenClientConnection())
				{
					if (shouldCloseOnServerSide)
						server.ActiveConnections.Single().Dispose();
				}
			}

			Assert.That(closeCount, Is.EqualTo(1));
		}

		[Test]
		public void Should_server_notify_once_about_connection_open()
		{
			var openCount = 0;
			using (var server = StartServer())
			{
				server.ConnectionOpened += connection => Interlocked.Increment(ref openCount);
				using (OpenClientConnection())
				{
				}
			}

			Assert.That(openCount, Is.EqualTo(1));
		}
	}
}