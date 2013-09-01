using System;
using RemoteExecution.Dispatchers;
using RemoteExecution.Executors;

namespace RemoteExecution.Connections
{
	/// <summary>
	/// Remote connection interface allowing to execute operations remotely or configure handlers for operations incoming from remote end.
	/// </summary>
	public interface IRemoteConnection : IDisposable
	{
		/// <summary>
		/// Fires when connection is closed on this or remote end.
		/// </summary>
		event Action Closed;
		/// <summary>
		/// Returns true if connection is opened, otherwise false.
		/// </summary>
		bool IsOpen { get; }
		/// <summary>
		/// Returns operation dispatcher used for handling incoming operations.
		/// </summary>
		IOperationDispatcher OperationDispatcher { get; }
		/// <summary>
		/// Returns remote executor used for executing operations on remote end.
		/// </summary>
		IRemoteExecutor RemoteExecutor { get; }
	}
}
