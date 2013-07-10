using StatefulServices.Contracts;

namespace StatefulServices.Server
{
	internal class RegistrationService : IRegistrationService
	{
		private readonly UserContext _userContext;

		public RegistrationService(UserContext userContext)
		{
			_userContext = userContext;
		}

		public void Register(string name)
		{
			_userContext.Name = name;
		}

		public string GetUserName()
		{
			return _userContext.Name;
		}
	}
}