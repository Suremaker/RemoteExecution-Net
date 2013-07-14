using AopAlliance.Intercept;
using Spring.Aop.Framework;

namespace Examples.Utils
{
	public class Aspects
	{
		private static readonly IMethodInterceptor _timeMeasureInterceptor = new TimeMeasureInterceptor();

		public static TInterface WithTimeMeasure<TInterface>(TInterface target)
		{
			var proxyFactory = new ProxyFactory(target);
			proxyFactory.AddAdvice(_timeMeasureInterceptor);
			return (TInterface)proxyFactory.GetProxy();
		}
	}
}
