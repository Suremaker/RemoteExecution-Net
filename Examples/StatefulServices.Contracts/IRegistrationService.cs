namespace StatefulServices.Contracts
{
	public interface IRegistrationService
	{
		string GetUserName();
		void Register(string name);
	}
}