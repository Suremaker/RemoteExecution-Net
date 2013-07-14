namespace StatefulServices.Server
{
	internal class ClientContext
	{
		public string Name { get; set; }
		public bool IsRegistered { get { return !string.IsNullOrEmpty(Name); } }
	}
}