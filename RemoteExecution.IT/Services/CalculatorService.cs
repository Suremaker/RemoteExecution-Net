namespace RemoteExecution.IT.Services
{
	public class CalculatorService : ICalculatorService
	{
		#region ICalculatorService Members

		public int Add(int x, int y)
		{
			return x + y;
		}

		#endregion
	}
}