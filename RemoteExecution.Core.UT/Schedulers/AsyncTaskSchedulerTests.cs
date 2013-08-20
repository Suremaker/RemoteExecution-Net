using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using RemoteExecution.Core.Schedulers;

namespace RemoteExecution.Core.UT.Schedulers
{
	[TestFixture]
	public class AsyncTaskSchedulerTests
	{
		private ITaskScheduler _subject;

		private static IEnumerable<Action> PrepareTasks(int taskCount, int sleepTime, SemaphoreSlim semaphore)
		{
			for (int i = 0; i < taskCount; ++i)
				yield return () => { Thread.Sleep(sleepTime); semaphore.Release(); };
		}

		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_subject = new AsyncTaskScheduler();
		}

		#endregion

		[Test]
		public void Should_execute_tasks_in_background()
		{
			const int sleepTime = 10;
			const int taskCount = 100;

			var semaphore = new SemaphoreSlim(0);

			var tasks = PrepareTasks(taskCount, sleepTime, semaphore).ToArray();

			var watch = new Stopwatch();
			watch.Start();
			foreach (var task in tasks)
				_subject.Execute(task);
			var scheduleTime = watch.Elapsed;


			for (int i = 0; i < taskCount; i++)
				Assert.That(semaphore.Wait(TimeSpan.FromMilliseconds(500)), Is.True, "Failed during wait for task: " + i);
			watch.Stop();

			const int totalSleepTime = sleepTime*taskCount;
			Console.WriteLine("Time for schedule: {0},\nTime spent: {1},\nTotal sleep Time: {2}", scheduleTime.TotalMilliseconds, watch.ElapsedMilliseconds, totalSleepTime);
			Assert.That(scheduleTime.TotalMilliseconds, Is.LessThan(totalSleepTime));
			Assert.That(watch.ElapsedMilliseconds, Is.LessThan(totalSleepTime));
		}
	}
}
