using StatefulServices.Contracts;

namespace StatefulServices.Server
{
	internal class RegistrationService : IRegistrationService
	{
		private readonly ClientContext _clientContext;

		public RegistrationService(ClientContext clientContext)
		{
			_clientContext = clientContext;
		}

		public void Register(string name)
		{
			_clientContext.Name = name;
		}

		public string GetUserName()
		{
			return _clientContext.Name;
		}
	}
}