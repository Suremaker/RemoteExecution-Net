using StatelessServices.Contracts;

namespace StatelessServices.Server
{
	internal class GreeterService : IGreeter
	{
		public Message Greet(Person person)
		{
			return new Message(
				string.Format("Personal greeting for Mr./Mrs. {0}", person.LastName),
				string.Format("Hello {0}!", person.FirstName));
		}
	}
}