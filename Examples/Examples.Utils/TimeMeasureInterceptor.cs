﻿using System;
using System.Diagnostics;
using System.Linq;
using AopAlliance.Intercept;

namespace Examples.Utils
{
	internal class TimeMeasureInterceptor : IMethodInterceptor
	{
		private readonly ConsoleColor _color;

		public TimeMeasureInterceptor(ConsoleColor color)
		{
			_color = color;
		}

		#region IMethodInterceptor Members

		public object Invoke(IMethodInvocation invocation)
		{
			var methodName = string.Format("{0}({1})", invocation.Method.Name, string.Join(", ", invocation.Arguments ?? Enumerable.Empty<object>()));
			var watch = new Stopwatch();

			try
			{
				Console.ForegroundColor = _color;
				Console.WriteLine("{0} --> {1}: started", DateTime.UtcNow.ToString("HH:mm:ss.ffff"), methodName);
				Console.ResetColor();
				watch.Start();
				return invocation.Proceed();
			}
			finally
			{
				watch.Stop();
				Console.ForegroundColor = _color;
				Console.WriteLine("{0} <-- {1}: finished in {2}ms", DateTime.UtcNow.ToString("HH:mm:ss.ffff"), methodName, watch.ElapsedMilliseconds);
				Console.ResetColor();
			}
		}

		#endregion
	}
}