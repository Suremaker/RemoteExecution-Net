namespace RemoteExecution.Core.Executors
{
	public interface IRemoteExecutor
	{
		T Create<T>();
		T Create<T>(NoResultMethodExecution noResultMethodExecution);
	}
}