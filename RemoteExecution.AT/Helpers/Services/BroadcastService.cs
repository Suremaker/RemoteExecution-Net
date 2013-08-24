using RemoteExecution.AT.Helpers.Contracts;

namespace RemoteExecution.AT.Helpers.Services
{
	class BroadcastService : IBroadcastService
	{
		public int ReceivedValue { get; private set; }
		public void SetValue(int value)
		{
			ReceivedValue = value;
		}
	}
}