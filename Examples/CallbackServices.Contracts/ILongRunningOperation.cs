namespace CallbackServices.Contracts
{
	public interface ILongRunningOperation
	{
		void Perform(int steps);
	}
}