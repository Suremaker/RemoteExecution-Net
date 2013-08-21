using RemoteExecution.AT.Helpers.Contracts;

namespace RemoteExecution.AT.Helpers.Services
{
	class Greeter : IGreeter
	{
		#region IGreeter Members

		public string Hello(string name)
		{
			return string.Format("Hello {0}!", name);
		}

		#endregion
	}
}