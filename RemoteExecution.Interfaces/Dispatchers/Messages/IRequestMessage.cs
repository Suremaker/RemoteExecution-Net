using RemoteExecution.Channels;

namespace RemoteExecution.Dispatchers.Messages
{
	/// <summary>
	/// Request message interface containing all information required to perform operation on remote end.
	/// </summary>
	public interface IRequestMessage : IMessage
	{
		/// <summary>
		/// Returns method arguments.
		/// </summary>
		object[] Args { get; }
		/// <summary>
		/// Returns channel where response should be sent to.
		/// </summary>
		IOutputChannel Channel { get; set; }
		/// <summary>
		/// Returns true if response is expected, otherwise false.
		/// </summary>
		bool IsResponseExpected { get; }
		/// <summary>
		/// Returns name of method to execute.
		/// </summary>
		string MethodName { get; }
	}
}