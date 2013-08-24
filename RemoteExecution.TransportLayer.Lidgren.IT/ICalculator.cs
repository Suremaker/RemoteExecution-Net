namespace RemoteExecution.TransportLayer.Lidgren.IT
{
	public interface ICalculator
	{
		int Add(int x, int y);
		int Divide(int x, int y);
	}

	class Calculator : ICalculator
	{
		#region ICalculator Members

		public int Add(int x, int y)
		{
			return x + y;
		}

		public int Divide(int x, int y)
		{
			return x / y;
		}

		#endregion
	}
}