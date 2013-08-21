using System;
using NUnit.Framework;
using RemoteExecution.AT.Helpers.Contracts;
using RemoteExecution.Core.Channels;

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
	}
}