namespace BroadcastServices.Server
{
	internal class UserContext
	{
		public string Name { get; set; }
		public bool IsRegistered { get { return !string.IsNullOrEmpty(Name); } }
	}
}