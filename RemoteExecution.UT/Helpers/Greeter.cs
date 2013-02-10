namespace RemoteExecution.UT.Helpers
{
    public class Greeter : IGreeter
    {
        #region IGreeter Members

        public string Hello(string name)
        {
            return "Hello " + name;
        }

        #endregion
    }
}