using RemoteExecution.AT.Helpers.Contracts;

namespace RemoteExecution.AT.Helpers.Services
{
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