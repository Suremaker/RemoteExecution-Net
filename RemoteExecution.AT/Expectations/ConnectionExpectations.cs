using System.Linq;
using NUnit.Framework;
using RemoteExecution.AT.Helpers;
using RemoteExecution.AT.Helpers.Contracts;

// ReSharper disable AccessToDisposedClosure

namespace RemoteExecution.AT.Expectations
{
	public abstract partial class BehaviorExpectations : TestContext
	{
		[Test]
		public void Should_reconnect_client_with_same_executor()
		{
			using (StartServer())
			using (var client = OpenClientConnection())
			{
				var calculator = client.Executor.Create<ICalculator>();

				Assert.That(calculator.Divide(8, 2), Is.EqualTo(4));

				client.Dispose();
				Assert.That(client.IsOpen, Is.False);
				client.Open();

				Assert.That(calculator.Divide(6, 2), Is.EqualTo(3));
			}
		}

		[Test]
		public void Should_server_close_client_connection()
		{
			using (var server = StartServer())
			using (var client = OpenClientConnection())
			{
				server.ActiveConnections.Single().Dispose();
				SyncHelper.WaitUntil(() => !client.IsOpen);
				SyncHelper.WaitUntil(() => !server.ActiveConnections.Any());
			}
		}

		[Test]
		public void Should_server_shutdown_close_all_client_connections()
		{
			using (var server = StartServer())
			using (var client1 = OpenClientConnection())
			using (var client2 = OpenClientConnection())
			{
				server.Dispose();
				SyncHelper.WaitUntil(() => !client1.IsOpen);
				SyncHelper.WaitUntil(() => !client2.IsOpen);
			}
		}

		[Test]
		public void Should_successfuly_connect_and_disconnect_client()
		{
			using (var server = StartServer())
			{
				using (var client = CreateClientConnection())
				{
					client.Open();

					Assert.That(client.IsOpen);
					Assert.That(server.ActiveConnections.Count(), Is.EqualTo(1));
				}
				SyncHelper.WaitUntil(() => !server.ActiveConnections.Any());
			}
		}
	}
}
// ReSharper restore AccessToDisposedClosure