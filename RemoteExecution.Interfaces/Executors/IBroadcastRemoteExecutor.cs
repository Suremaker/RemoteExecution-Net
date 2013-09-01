namespace RemoteExecution.Executors
{
	/// <summary>
	/// Broadcast remote executor interface allowing to create proxy objects, that would execute all of its methods remotely on all clients.
	/// </summary>
	public interface IBroadcastRemoteExecutor
	{
		/// <summary>
		/// Creates proxy object for interface <c>T</c>, that would execute all of its methods remotely on all clients.
		/// All interface methods have to have be methods returning void, and all would be treated as one way methods.
		/// </summary>
		/// <typeparam name="T">Type of proxy to create.</typeparam>
		/// <returns>Proxy object.</returns>
		T Create<T>();
	}
}