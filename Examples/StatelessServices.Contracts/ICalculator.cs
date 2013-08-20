namespace StatelessServices.Contracts
{
	public interface ICalculator
	{
		int Add(int x, int y);
		int Divide(int x, int y);
		int Multiply(int x, int y);
		int Subtract(int x, int y);
	}
}