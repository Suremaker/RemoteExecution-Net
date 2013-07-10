namespace StatefulServices.Contracts
{
	public interface IRegistrationService
	{
		void Register(string name);
		string GetUserName();
	}
}