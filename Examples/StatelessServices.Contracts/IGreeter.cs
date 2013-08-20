namespace StatelessServices.Contracts
{
	public class Person
	{
		public string FirstName { get; private set; }
		public string LastName { get; private set; }

		public Person(string firstName, string lastName)
		{
			LastName = lastName;
			FirstName = firstName;
		}
	}

	public class Message
	{
		public string Text { get; private set; }
		public string Title { get; private set; }

		public Message(string title, string text)
		{
			Text = text;
			Title = title;
		}

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
