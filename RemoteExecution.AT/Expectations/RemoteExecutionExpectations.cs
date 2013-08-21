using System;
using NUnit.Framework;
using RemoteExecution.AT.Helpers.Contracts;

namespace RemoteExecution.AT.Expectations
{
	public abstract partial class BehaviorExpectations
	{
		[Test]
		public void Should_successfuly_execute_remote_operation()
		{
			using (StartServer())
			using (var client = OpenClientConnection())
			{
				var calculator = client.Executor.Create<ICalculator>();
				var greeter = client.Executor.Create<IGreeter>();

				Assert.That(calculator.Add(3, 5), Is.EqualTo(8));
				Assert.That(greeter.Hello("Josh"), Is.EqualTo("Hello Josh!"));
			}
		}

		[Test]
		public void Should_pass_server_side_exceptions()
		{
			using (StartServer())
			using (var client = OpenClientConnection())
			{
				var calculator = client.Executor.Create<ICalculator>();

				Assert.Throws<DivideByZeroException>(() => calculator.Divide(4, 0));
			}
		}
	}
}
