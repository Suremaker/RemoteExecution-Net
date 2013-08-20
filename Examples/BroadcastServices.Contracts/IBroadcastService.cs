namespace BroadcastServices.Contracts
{
	public interface IBroadcastService
	{
		void UserLeft(string name);
		void UserRegistered(string name);
	}
}