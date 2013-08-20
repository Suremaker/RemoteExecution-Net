using StatelessServices.Contracts;

namespace StatelessServices.Server
{
	internal class CalculatorService : ICalculator
	{
		#region ICalculator Members

		public int Add(int x, int y)
		{
			return x + y;
		}

		public int Subtract(int x, int y)
		{
			return x - y;
		}

		public int Multiply(int x, int y)
		{
			return x * y;
		}

		public int Divide(int x, int y)
		{
			return x / y;
		}

		#endregion
	}
}