namespace RemoteExecution.IT.Services
{
	internal class BroadcastService : IBroadcastService
	{
		public int ReceivedNumber { get; private set; }

		#region IBroadcastService Members

		public void SetNumber(int number)
		{
			ReceivedNumber = number;
		}

		#endregion
	}
}