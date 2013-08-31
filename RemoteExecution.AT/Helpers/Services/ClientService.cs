using RemoteExecution.AT.Helpers.Contracts;

namespace RemoteExecution.AT.Helpers.Services
{
	class ClientService : IClientService
	{
		#region IClientService Members

		public string GetHexValue(int value)
		{
			return value.ToString("X");
		}

		#endregion
	}
}