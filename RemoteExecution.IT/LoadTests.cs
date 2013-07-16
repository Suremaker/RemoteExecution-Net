using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using RemoteExecution.Endpoints;
using RemoteExecution.IT.Services;

namespace RemoteExecution.IT
{
	[TestFixture]
	public class LoadTests
	{
		class TaskData
		{
			private readonly ICalculatorService _calculator;
			private readonly int _times;
			public int CurrentResult { get; private set; }
			public Exception Exception { get; set; }

			public TaskData(ClientEndpoint endpoint, int times)
			{
				_times = times;
				_calculator = endpoint.Connection.RemoteExecutor.Create<ICalculatorService>();
			}

			public void ExecuteTask()
			{
				try
				{
					for (int i = 0; i < _times; i++)
					{
						CurrentResult = _calculator.Add(CurrentResult, 1);
						Console.WriteLine("{0}: {1}", Thread.CurrentThread.ManagedThreadId, CurrentResult);
					}
				}
				catch (Exception e)
				{
					Exception = e;
				}
			}
		}

		private ServerEndpoint _server;
		private List<ClientEndpoint> _clients;
		private const ushort _port = 3232;
		private const string _appId = "test";

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{			
			_server = new TestableServerEndpoint(_appId, 200, _port);
			_server.StartListening();
		}

		[SetUp]
		public void SetUp()
		{
			_clients = new List<ClientEndpoint>();
		}

		[TearDown]
		public void TearDown()
		{
			Console.WriteLine("Closing client connections");
			foreach (var client in _clients)
				client.Dispose();
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			_server.Dispose();
		}

		[Test]
		[TestCase(1, 100)]
		[TestCase(10, 100)]
		[TestCase(100, 100)]
		[TestCase(200, 100)]
		public void ShouldPerformAllOperationsWithOneConnection(int threadsCount, int times)
		{
			var client = CreateClientConnection();
			var tasks = new List<TaskData>();
			for (int i = 0; i < threadsCount; i++)
				tasks.Add(new TaskData(client, times));

			PerformTasks(tasks);
			Assert.That(tasks.Count(t => t.CurrentResult == times), Is.EqualTo(threadsCount));
		}

		[Test]
		[TestCase(1, 100)]
		[TestCase(10, 100)]
		[TestCase(30, 100)]
		[TestCase(30, 200)]
		public void ShouldPerformAllOperationsWithOwnConnection(int threadsCount, int times)
		{
			var tasks = new List<TaskData>();
			for (int i = 0; i < threadsCount; i++)
				tasks.Add(new TaskData(CreateClientConnection(), times));

			PerformTasks(tasks);
			Assert.That(tasks.Count(t => t.CurrentResult == times), Is.EqualTo(threadsCount));
		}

		private ClientEndpoint CreateClientConnection()
		{
			var client = new ClientEndpoint(_appId);
			client.ConnectTo("localhost", _port);
			_clients.Add(client);
			return client;
		}

		private void PerformTasks(IEnumerable<TaskData> tasks)
		{
			Console.WriteLine("Performing tasks");
			WaitForThreads(StartThreads(tasks));
		}

		private void WaitForThreads(IEnumerable<Thread> threads)
		{
			foreach (Thread thread in threads)
				thread.Join();
		}

		private IEnumerable<Thread> StartThreads(IEnumerable<TaskData> tasks)
		{
			var threads = tasks.Select(t => new Thread(t.ExecuteTask)).ToList();

			foreach (Thread thread in threads)
				thread.Start();

			return threads;
		}
	}
}
