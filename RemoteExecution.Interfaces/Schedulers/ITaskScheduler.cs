using System;

namespace RemoteExecution.Schedulers
{
	/// <summary>
	/// Interface for scheduling task execution.
	/// </summary>
	public interface ITaskScheduler
	{
		/// <summary>
		/// Schedules task execution.
		/// </summary>
		/// <param name="task">Task to execute.</param>
		void Execute(Action task);
	}
}
