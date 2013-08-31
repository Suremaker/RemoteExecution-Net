namespace RemoteExecution.Channels
{
	public interface IClientChannel : IDuplexChannel
	{
		void Open();
	}
}