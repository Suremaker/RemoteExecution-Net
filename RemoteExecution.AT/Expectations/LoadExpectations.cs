using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using RemoteExecution.AT.Helpers.Contracts;
using RemoteExecution.Connections;

namespace RemoteExecution.AT.Expectations
{
	class TaskData
	{
		private readonly Func<IClientConnection> _connectionCreator;
		private readonly int _times;
		public IClientConnection Connection { get; private set; }
		public int CurrentResult { get; private set; }
		public Exception Exception { get; set; }

		public TaskData(Func<IClientConnection> connectionCreator, int times)
		{
			_connectionCreator = connectionCreator;
			_times = times;
		}

		public void ExecuteTask()
		{
			Connection = _connectionCreator();
			var calculator = Connection.RemoteExecutor.Create<ICalculator>();
			try
			{
				for (int i = 0; i < _times; i++)
					CurrentResult = calculator.Add(CurrentResult, 1);
			}
			catch (Exception e)
			{
				Exception = e;
			}
		}
	}

	public abstract partial class BehaviorExpectations
	{
		[Test]
		[TestCase(1, 100)]
		[TestCase(10, 100)]
		[TestCase(100, 100)]
		[TestCase(200, 100)]
		public void Should_perform_all_operations_with_one_connection_under_load(int threadsCount, int times)
		{
			using (StartServer())
			using (var client = OpenClientConnection())
			{
				var tasks = new List<TaskData>();
				for (int i = 0; i < threadsCount; i++)
					tasks.Add(new TaskData(() => client, times));

				PerformTasks(tasks);
				Assert.That(tasks.Count(t => t.CurrentResult == times), Is.EqualTo(threadsCount));
			}
		}

		[Test]
		[TestCase(1, 100)]
		[TestCase(10, 100)]
		[TestCase(30, 100)]
		[TestCase(200, 100)]
		public void Should_perform_all_operations_with_own_connection_under_load(int threadsCount, int times)
		{
			var tasks = new List<TaskData>();
			try
			{
				using (StartServer(threadsCount))
				{
					for (int i = 0; i < threadsCount; i++)
						tasks.Add(new TaskData(OpenClientConnection, times));

					PerformTasks(tasks);
					Assert.That(tasks.Count(t => t.CurrentResult == times), Is.EqualTo(threadsCount));
				}
			}
			finally
			{
				Parallel.ForEach(tasks, t => t.Connection.Dispose());
			}
		}

		private void PerformTasks(IEnumerable<TaskData> tasks)
		{
			Console.WriteLine("Performing tasks");
			WaitForThreads(StartThreads(tasks));
		}

		private IEnumerable<Thread> StartThreads(IEnumerable<TaskData> tasks)
		{
			var threads = tasks.Select(t => new Thread(t.ExecuteTask)).ToList();

			foreach (Thread thread in threads)
				thread.Start();

			return threads;
		}

		private void WaitForThreads(IEnumerable<Thread> threads)
		{
			foreach (Thread thread in threads)
				thread.Join();
		}
	}
}