namespace RemoteExecution.Channels
{
	/// <summary>
	/// Duplex channel interface allowing to send and receive messages.
	/// </summary>
	public interface IDuplexChannel : IInputChannel, IOutputChannel { }
}