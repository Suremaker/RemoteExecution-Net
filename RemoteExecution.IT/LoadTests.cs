using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
			private readonly Func<ClientEndpoint> _endpointCreator;
			private readonly int _times;
			public int CurrentResult { get; private set; }
			public Exception Exception { get; set; }

			public TaskData(Func<ClientEndpoint> endpointCreator, int times)
			{
				_endpointCreator = endpointCreator;
				_times = times;
			}

			public void ExecuteTask()
			{
				var endpoint = _endpointCreator();
				var calculator = endpoint.RemoteExecutor.Create<ICalculatorService>();
				try
				{
					for (int i = 0; i < _times; i++)
						CurrentResult = calculator.Add(CurrentResult, 1);
					Console.WriteLine("Finished.");
				}
				catch (Exception e)
				{
					Exception = e;
				}
			}
		}

		private TestableServerEndpoint _server;
		private ConcurrentBag<ClientEndpoint> _clients;
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
			_clients = new ConcurrentBag<ClientEndpoint>();
		}

		[TearDown]
		public void TearDown()
		{
			Console.WriteLine("Closing {0} client connections", _clients.Count);
			Parallel.ForEach(_clients, c => c.Dispose());
			SyncHelper.WaitUntil(() => !_server.ActiveConnections.Any(), 250);
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
				tasks.Add(new TaskData(() => client, times));

			PerformTasks(tasks);
			Assert.That(tasks.Count(t => t.CurrentResult == times), Is.EqualTo(threadsCount));
		}

		[Test]
		[TestCase(1, 100)]
		[TestCase(10, 100)]
		[TestCase(30, 100)]
		[TestCase(200, 100)]
		public void ShouldPerformAllOperationsWithOwnConnection(int threadsCount, int times)
		{
			var tasks = new List<TaskData>();
			for (int i = 0; i < threadsCount; i++)
				tasks.Add(new TaskData(CreateClientConnection, times));

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
