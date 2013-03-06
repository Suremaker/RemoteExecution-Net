namespace RemoteExecution
{
	public interface IRemoteExecutor
	{
		T Create<T>();
	}
}