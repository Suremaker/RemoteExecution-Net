using System;
using RemoteExecution.Endpoints;
using StatelessServices.Contracts;

namespace StatelessServices.Client
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var client = new ClientEndpoint(Protocol.Id))
			{
				client.ConnectTo("localhost", 3131);

				GreetSomePeople(client.RemoteExecutor.Create<IGreeter>());
				PerformSomeCalculations(client.RemoteExecutor.Create<ICalculator>());

				Console.WriteLine("Done. Press enter to exit.");
				Console.ReadLine();
			}
		}

		private static void GreetSomePeople(IGreeter greeter)
		{
			Console.WriteLine(greeter.Greet(new Person("John", "Smith")));
			Console.WriteLine(greeter.Greet(new Person("Kitty", "Johnson")));
		}

		private static void PerformSomeCalculations(ICalculator calculator)
		{
			Console.WriteLine("Lets add 3 and 5:");
			Console.WriteLine(calculator.Add(3, 5));

			Console.WriteLine("Lets subtract 5 and 7:");
			Console.WriteLine(calculator.Subtract(5, 7));

			Console.WriteLine("Lets multiply 3 by 2:");
			Console.WriteLine(calculator.Multiply(3, 2));

			Console.WriteLine("Lets divide 3 by 0:");

			try
			{
				Console.WriteLine(calculator.Divide(3, 0));
			}
			catch (Exception e)
			{
				Console.WriteLine("{0}: {1}", e.GetType().Name, e.Message);
			}
		}
	}
}
