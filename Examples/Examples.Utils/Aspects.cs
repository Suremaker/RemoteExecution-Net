using System;
using Spring.Aop.Framework;

namespace Examples.Utils
{
	public class Aspects
	{
		public static TInterface WithTimeMeasure<TInterface>(TInterface target, ConsoleColor color = ConsoleColor.Blue)
		{
			var proxyFactory = new ProxyFactory(target);
			proxyFactory.AddAdvice(new TimeMeasureInterceptor(color));
			return (TInterface)proxyFactory.GetProxy();
		}
	}
}
