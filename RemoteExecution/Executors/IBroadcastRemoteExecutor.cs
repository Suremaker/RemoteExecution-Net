namespace RemoteExecution.Executors
{
	public interface IBroadcastRemoteExecutor
	{
		T Create<T>();
	}
}