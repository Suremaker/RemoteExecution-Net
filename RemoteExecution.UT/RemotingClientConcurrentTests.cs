using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using RemoteExecution.Dispatching;
using RemoteExecution.Messages;
using RemoteExecution.UT.Helpers;

namespace RemoteExecution.UT
{
    [TestFixture]
    public class RemotingClientConcurrentTests
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _operationDispatcher = new OperationDispatcher();
            _endpoint = new MockWriteEndpoint();
            _subject = RemoteExecutor.Create<ICalculator>(_operationDispatcher, _endpoint);
        }

        #endregion

        private OperationDispatcher _operationDispatcher;
        private MockWriteEndpoint _endpoint;
        private ICalculator _subject;

        [Test]
        public void ShouldSupportConcurrentOperations()
        {
            var requests = new ConcurrentStack<Request>();
            _endpoint.OnMessageSend = r => requests.Push((Request)r);

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