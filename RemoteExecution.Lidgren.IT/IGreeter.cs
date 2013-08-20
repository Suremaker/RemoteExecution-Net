namespace RemoteExecution.Lidgren.IT
{
	public interface IGreeter
	{
		string Hello(string name);
	}

	class Greeter : IGreeter
	{
		public string Hello(string name)
		{
			return string.Format("Hello {0}!", name);
		}
	}
}