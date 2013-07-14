namespace OneWayMethodServices.Contracts
{
	public interface ILongRunningOperation
	{
		string Repeat(string text, int times);
		void RepeatWithCallback(string text, int times);
		void RepeatWithOneWayCallback(string text, int times);
	}
}