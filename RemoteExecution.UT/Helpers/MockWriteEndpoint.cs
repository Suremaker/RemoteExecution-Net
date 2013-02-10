using System;
using System.Collections.Generic;
using RemoteExecution.Endpoints;

namespace RemoteExecution.UT.Helpers
{
    class MockWriteEndpoint : IWriteEndpoint
    {
        public Action<IMessage> OnMessageSend { get; set; }
        public List<IMessage> SentMessages { get; private set; }

        public MockWriteEndpoint()
        {
            SentMessages = new List<IMessage>();
            OnMessageSend = m => { };
        }

        #region IWriteEndpoint Members

        public void Send(IMessage message)
        {
            SentMessages.Add(message);
            OnMessageSend(message);
        }

        #endregion
    }
}