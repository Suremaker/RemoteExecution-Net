using AopAlliance.Intercept;
using NUnit.Framework;
using RemoteExecution.Core.Executors;
using RemoteExecution.Core.Remoting;
using Rhino.Mocks;
using Spring.Aop.Framework;

namespace RemoteExecution.Core.UT.Remoting
{
	[TestFixture]
	public class RemoteCallInterceptorTests
	{
		public interface ITestInterface
		{
			string Hello(int x);
			void Notify(string text);
		}

		private IMethodInterceptor _oneWayInterceptor;
		private IMethodInterceptor _twoWayInterceptor;

		[SetUp]
		public void SetUp()
		{
			_oneWayInterceptor = MockRepository.GenerateMock<IMethodInterceptor>();
			_twoWayInterceptor = MockRepository.GenerateMock<IMethodInterceptor>();
		}

		private ITestInterface GetInvocationHelper(NoResultMethodExecution noResultMethodExecution = NoResultMethodExecution.TwoWay)
		{
			var subject = new RemoteCallInterceptor(_oneWayInterceptor, _twoWayInterceptor, noResultMethodExecution);
			return (ITestInterface)new ProxyFactory(typeof(ITestInterface), subject).GetProxy();
		}

		[Test]
		[TestCase(NoResultMethodExecution.OneWay)]
		[TestCase(NoResultMethodExecution.TwoWay)]
		public void Should_always_execute_operations_returning_result_as_two_way(NoResultMethodExecution executionMode)
		{
			GetInvocationHelper(executionMode).Hello(5);
			_twoWayInterceptor.AssertWasCalled(i => i.Invoke(Arg<IMethodInvocation>.Is.Anything));
			_oneWayInterceptor.AssertWasNotCalled(i => i.Invoke(Arg<IMethodInvocation>.Is.Anything));
		}

		[Test]
		public void Should_execute_no_result_returning_operation_as_one_way()
		{
			GetInvocationHelper(NoResultMethodExecution.OneWay).Notify("test");
			_oneWayInterceptor.AssertWasCalled(i => i.Invoke(Arg<IMethodInvocation>.Is.Anything));
			_twoWayInterceptor.AssertWasNotCalled(i => i.Invoke(Arg<IMethodInvocation>.Is.Anything));
		}

		[Test]
		public void Should_wait_for_response_if_one_way_method_is_called_in_sync_mode()
		{
			GetInvocationHelper().Notify("test");
			_twoWayInterceptor.AssertWasCalled(i => i.Invoke(Arg<IMethodInvocation>.Is.Anything));
			_oneWayInterceptor.AssertWasNotCalled(i => i.Invoke(Arg<IMethodInvocation>.Is.Anything));
		}
	}
}
