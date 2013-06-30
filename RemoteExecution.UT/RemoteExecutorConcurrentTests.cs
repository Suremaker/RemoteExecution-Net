using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using RemoteExecution.Dispatching;
using RemoteExecution.Messaging;
using RemoteExecution.UT.Helpers;

namespace RemoteExecution.UT
{
	[TestFixture]
	public class RemoteExecutorConcurrentTests
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_operationDispatcher = new OperationDispatcher();
			_connection = new MockNetworkConnection(_operationDispatcher);
			_remoteExecutor = new RemoteExecutor(_connection);
			_subject = _remoteExecutor.Create<ICalculator>();
		}

		#endregion

		private OperationDispatcher _operationDispatcher;
		private MockNetworkConnection _connection;
		private ICalculator _subject;
		private RemoteExecutor _remoteExecutor;

		[Test]
		public void ShouldSupportConcurrentOperations()
		{
			var requests = new ConcurrentStack<Request>();
			_connection.OnMessageSend = r => requests.Push((Request)r);

			int validResults = 0;

			var tasks = new List<Thread>();
			for (int i = 0; i < 10; ++i)
			{
				var thread = new Thread(o =>
					{
						string add = _subject.Add((int)o, 0);
						if (Equals(add, o))
							Interlocked.Increment(ref validResults);
					});
				tasks.Add(thread);
				thread.Start(i);
			}

			Thread.Sleep(500);
			foreach (Request request in requests)
				_operationDispatcher.Dispatch(new Response(request.CorrelationId, request.Args[0]), null);

			foreach (Thread thread in tasks)
				thread.Join();

			Assert.That(validResults, Is.EqualTo(tasks.Count));
		}
	}
}