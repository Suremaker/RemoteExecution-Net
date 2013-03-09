using System;
using AopAlliance.Intercept;
using Spring.Aop.Framework;

namespace RemoteExecution.IT
{
	public class LoggingProxy
	{
		public static T For<T>(T target)
		{
			var factory = new ProxyFactory(typeof(T), new ConsoleLoggingInterceptor()) { Target = target };
			return (T)factory.GetProxy();
		}
	}

	public class ConsoleLoggingInterceptor : IMethodInterceptor
	{
		public object Invoke(IMethodInvocation invocation)
		{
			Console.WriteLine("Calling " + invocation.Method.Name);
			var result = invocation.Proceed();
			Console.WriteLine("Ending " + invocation.Method.Name);
			return result;
		}
	}
}