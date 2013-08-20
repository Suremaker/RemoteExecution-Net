using System;
using System.Text;
using System.Threading;
using OneWayMethodServices.Contracts;

namespace OneWayMethodServices.Server
{
	internal class LongRunningOperation : ILongRunningOperation
	{
		private readonly IClientCallback _oneWayCallback;
		private readonly IClientCallback _twoWayCallback;

		public LongRunningOperation(IClientCallback twoWayCallback, IClientCallback oneWayCallback)
		{
			_twoWayCallback = twoWayCallback;
			_oneWayCallback = oneWayCallback;
		}

		#region ILongRunningOperation Members

		public string Repeat(string text, int times)
		{
			return RepeatText(text, times);
		}

		public void RepeatWithCallback(string text, int times)
		{
			_twoWayCallback.FinishRepetition(RepeatText(text, times));
		}

		public void RepeatWithOneWayCallback(string text, int times)
		{
			_oneWayCallback.FinishRepetition(RepeatText(text, times));
		}

		#endregion

		private string RepeatText(string text, int times)
		{
			var builder = new StringBuilder();
			for (int i = 0; i < times; i++)
			{
				builder.Append(text);
				Thread.Sleep(TimeSpan.FromSeconds(1));
			}
			return builder.ToString();
		}
	}
}