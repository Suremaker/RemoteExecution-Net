using System;

namespace RemoteExecution.IT.Services
{
	public class ClientService : IClientService
	{
		private readonly int _value;
		public TimeSpan TimeSpan { get; set; }

		public ClientService(int value)
		{
			_value = value;
		}

		#region IClientService Members

		public int GetClientValue()
		{
			return _value;
		}

		public void Callback(TimeSpan timeSpan)
		{
			TimeSpan = timeSpan;
		}

		#endregion
	}
}