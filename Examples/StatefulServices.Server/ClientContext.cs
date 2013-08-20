namespace StatefulServices.Server
{
	internal class ClientContext
	{
		public bool IsRegistered { get { return !string.IsNullOrEmpty(Name); } }
		public string Name { get; set; }
	}
}