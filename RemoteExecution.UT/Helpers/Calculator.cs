using System;
using System.Globalization;

namespace RemoteExecution.UT.Helpers
{
    public class Calculator : ICalculator
    {
	    #region ICalculator Members

	    public string Add(int x, int y)
        {
            return (x + y).ToString(CultureInfo.InvariantCulture);
        }

        public string Subtract(int x, int y)
        {
            throw new ArgumentException("test");
        }

	    #endregion
    }
}