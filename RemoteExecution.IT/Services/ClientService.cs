using System;

namespace RemoteExecution.IT.Services
{
	public class ClientService : IClientService
	{
		private readonly int _value;

		public ClientService(int value)
		{
			_value = value;
		}

		public int GetClientValue()
		{
			return _value;
		}
	}
}