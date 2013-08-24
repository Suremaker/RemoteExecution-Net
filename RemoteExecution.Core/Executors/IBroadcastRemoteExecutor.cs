namespace RemoteExecution.Core.Executors
{
	public interface IBroadcastRemoteExecutor
	{
		T Create<T>();
	}
}