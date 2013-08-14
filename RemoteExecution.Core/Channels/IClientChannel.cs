namespace RemoteExecution.Core.Channels
{
	public interface IClientChannel : IDuplexChannel
	{
		void Open();
	}
}