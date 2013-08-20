namespace BroadcastServices.Contracts
{
	public interface IRegistrationService
	{
		string GetUserName();
		void Register(string name);
	}
}