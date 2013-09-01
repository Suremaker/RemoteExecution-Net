namespace RemoteExecution.Dispatchers.Messages
{
	/// <summary>
	/// Response message interface containing response value for correlated request message.
	/// </summary>
	public interface IResponseMessage : IMessage
	{
		/// <summary>
		/// Returns response value.
		/// </summary>
		object Value { get; }
	}
}