namespace BroadcastServices.Contracts
{
	public interface IBroadcastService
	{
		void UserRegistered(string name);
		void UserLeft(string name);
	}
}