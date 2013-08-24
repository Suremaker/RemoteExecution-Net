using System;
using NUnit.Framework;
using RemoteExecution.AT.Helpers.Contracts;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Endpoints;

namespace RemoteExecution.AT.Expectations
{
	public abstract partial class BehaviorExpectations
	{
		[Test]
		public void Should_executor_throw_if_remote_operation_is_performed_on_closed_connection()
		{
			using (var client = CreateClientConnection())
			{
				Assert.Throws<NotConnectedException>(() => client.Executor.Create<IGreeter>().Hello("John"));
			}
		}

		[Test]
		public void Should_broadcast_executor_throw_if_remote_operation_is_performed_on_closed_server()
		{
			using (var server = CreateServer())
			{
				Assert.Throws<NotConnectedException>(() => server.BroadcastExecutor.Create<IBroadcastService>().SetValue(32));
			}
		}

		[Test]
		public void Should_client_throw_if_unable_to_connect_to_server()
		{
			using (var client = CreateClientConnection())
			{
				var ex = Assert.Throws<ConnectionException>(client.Open);
				Assert.That(ex.Message, Is.EqualTo("Connection closed."));
			}
		}

		[Test]
		public void Should_opening_client_connection_throw_if_already_opened()
		{
			using (StartServer())
			using (var client = CreateClientConnection())
			{
				client.Open();
				var ex = Assert.Throws<InvalidOperationException>(client.Open);
				Assert.That(ex.Message, Is.EqualTo("Channel already opened."));
			}
		}

		[Test]
		public void Should_starting_server_throw_if_already_started()
		{
			using (var server = StartServer())
			{
				var ex = Assert.Throws<InvalidOperationException>(server.Start);
				Assert.That(ex.Message, Is.EqualTo("Server already started."));
			}
		}

		[Test]
		public void Should_start_throw_if_server_cannot_be_started()
		{
			using (StartServer())
			{
				var ex = Assert.Throws<ServerStartException>(() => StartServer());
				Assert.That(ex.Message, Is.StringStarting("Unable to start server: "));
			}
		}

		[Test]
		public void Should_allow_to_dispose_client_many_times()
		{
			var client = CreateClientConnection();
			client.Dispose();
			Assert.DoesNotThrow(client.Dispose);
		}

		[Test]
		public void Should_allow_to_dispose_server_many_times()
		{
			var server = StartServer();
			server.Dispose();
			Assert.DoesNotThrow(server.Dispose);
		}

		[Test]
		public void Should_allow_to_restart_server()
		{
			using (var server = StartServer())
			{
				server.Dispose();
				server.Start();
				using (var client = OpenClientConnection())
				{
					Assert.That(client.IsOpen);
				}
			}
		}
	}
}