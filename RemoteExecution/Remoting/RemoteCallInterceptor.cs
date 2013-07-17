using AopAlliance.Intercept;
using RemoteExecution.Executors;

namespace RemoteExecution.Remoting
{
	internal class RemoteCallInterceptor : IMethodInterceptor
	{
		private readonly NoResultMethodExecution _noResultMethodExecution;
		private readonly IMethodInterceptor _oneWayInterceptor;
		private readonly IMethodInterceptor _twoWayInterceptor;

		public RemoteCallInterceptor(IMethodInterceptor oneWayInterceptor, IMethodInterceptor twoWayInterceptor, NoResultMethodExecution noResultMethodExecution)
		{
			_oneWayInterceptor = oneWayInterceptor;
			_twoWayInterceptor = twoWayInterceptor;
			_noResultMethodExecution = noResultMethodExecution;
		}

		#region IMethodInterceptor Members

		public object Invoke(IMethodInvocation invocation)
		{
			if (_noResultMethodExecution == NoResultMethodExecution.OneWay && invocation.Method.ReturnType == typeof(void))
				return _oneWayInterceptor.Invoke(invocation);
			return _twoWayInterceptor.Invoke(invocation);
		}

		#endregion
	}
}