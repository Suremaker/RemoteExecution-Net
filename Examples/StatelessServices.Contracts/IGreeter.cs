namespace StatelessServices.Contracts
{
	public class Person
	{
		public Person(string firstName, string lastName)
		{
			LastName = lastName;
			FirstName = firstName;
		}

		public string FirstName { get; private set; }
		public string LastName { get; private set; }
	}

	public class Message
	{
		public Message(string title, string text)
		{
			Text = text;
			Title = title;
		}

		public string Text { get; private set; }
		public string Title { get; private set; }

		public override string ToString()
		{
			return string.Format("{0}: {1}", Title, Text);
		}
	}

	public interface IGreeter
	{
		Message Greet(Person person);
	}
}
