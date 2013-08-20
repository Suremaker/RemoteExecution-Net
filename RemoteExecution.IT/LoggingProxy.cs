using System;
using System.Diagnostics;
using AopAlliance.Intercept;
using Spring.Aop.Framework;

namespace RemoteExecution.IT
{
	public class LoggingProxy
	{
		public static T For<T>(T target, string id)
		{
			var factory = new ProxyFactory(typeof(T), new ConsoleLoggingInterceptor(id)) { Target = target };
			return (T)factory.GetProxy();
		}
	}

	public class ConsoleLoggingInterceptor : IMethodInterceptor
	{
		private readonly string _id;

		public ConsoleLoggingInterceptor(string id)
		{
			_id = id;
		}

		#region IMethodInterceptor Members

		public object Invoke(IMethodInvocation invocation)
		{
			var watch = new Stopwatch();
			Console.WriteLine("{0} calling {1}", _id, invocation.Method.Name);
			try
			{
				watch.Start();
				var result = invocation.Proceed();
				watch.Stop();
				Console.WriteLine("{0} {1} ended in {2} ms", _id, invocation.Method.Name, watch.ElapsedMilliseconds);
				return result;
			}
			catch (Exception)
			{
				watch.Stop();
				Console.WriteLine("{0} {1} ended with exception in {2} ms", _id, invocation.Method.Name, watch.ElapsedMilliseconds);
				throw;
			}
		}

		#endregion
	}
}