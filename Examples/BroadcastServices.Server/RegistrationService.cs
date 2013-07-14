using BroadcastServices.Contracts;

namespace BroadcastServices.Server
{
	internal class RegistrationService : IRegistrationService
	{
		private readonly ClientContext _clientContext;
		private readonly IBroadcastService _broadcastService;

		public RegistrationService(ClientContext clientContext, IBroadcastService broadcastService)
		{
			_clientContext = clientContext;
			_broadcastService = broadcastService;
		}

		public void Register(string name)
		{
			_clientContext.Name = name;
			_broadcastService.UserRegistered(name);
		}

		public string GetUserName()
		{
			return _clientContext.Name;
		}
	}
}