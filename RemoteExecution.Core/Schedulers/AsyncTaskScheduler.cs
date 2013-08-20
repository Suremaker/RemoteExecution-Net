using System;
using System.Threading.Tasks;

namespace RemoteExecution.Core.Schedulers
{
	/// <summary>
	/// Class for scheduling task execution in background threads.
	/// </summary>
	public class AsyncTaskScheduler : ITaskScheduler
	{
		#region ITaskScheduler Members

		/// <summary>
		/// Schedules task asynchronous execution in background threads.
		/// </summary>
		/// <param name="task">Task to execute.</param>
		public void Execute(Action task)
		{
			Task.Factory.StartNew(task);
		}

		#endregion
	}
}