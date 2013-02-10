using System;
using System.Threading;
using RemoteExecution.Endpoints;
using RemoteExecution.Messages;

namespace RemoteExecution.Handling
{
    internal class ResponseHandler : IHandler
    {
        private readonly ManualResetEventSlim _resetEvent = new ManualResetEventSlim(false);
        private object _value;

        public ResponseHandler()
        {
            Id = Guid.NewGuid().ToString();
        }

        #region IHandler Members

        public string Id { get; private set; }

        public void Handle(IMessage msg, IWriteEndpoint writeEndpoint)
        {
            _value = ((Response)msg).Value;
            _resetEvent.Set();
        }

        #endregion

        public object GetValue()
        {
            if (_value is Exception)
                throw (Exception)_value;
            return _value;
        }

        public void Wait()
        {
            _resetEvent.Wait();
        }
    }
}