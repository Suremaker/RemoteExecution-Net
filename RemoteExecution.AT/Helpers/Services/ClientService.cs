using RemoteExecution.AT.Helpers.Contracts;

namespace RemoteExecution.AT.Helpers.Services
{
	class ClientService : IClientService
	{
		public string GetHexValue(int value)
		{
			return value.ToString("X");
		}
	}
}