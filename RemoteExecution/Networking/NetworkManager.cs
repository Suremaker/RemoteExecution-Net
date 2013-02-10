using System;
using System.Threading;
using Lidgren.Network;

namespace RemoteExecution.Networking
{
    public abstract class NetworkManager : IDisposable
    {
        private readonly Thread _thread;
        private bool _isRunning;
        protected NetPeer Peer { get; private set; }

        protected NetworkManager(NetPeer netPeer)
        {
            Peer = netPeer;
            _thread = new Thread(Run) { Name = "Networking Thread" };
        }

        #region IDisposable Members

        public void Dispose()
        {
            Stop();
        }

        #endregion

        public void Start()
        {
            if (_isRunning)
                return;
            _isRunning = true;
            Peer.Start();
            _thread.Start();
        }

        public void Stop()
        {
            if (!_isRunning)
                return;
            _isRunning = false;
            Peer.Shutdown("Gracefull shutdown");
            _thread.Join();
        }

        protected abstract void HandleClosedConnection(NetConnection connection);
        protected abstract void HandleData(NetIncomingMessage message);
        protected abstract void HandleNewConnection(NetConnection connection);

        private void HandleMessage(NetIncomingMessage msg)
        {
            switch (msg.MessageType)
            {
                case NetIncomingMessageType.Data:
                    HandleData(msg);
                    break;
                case NetIncomingMessageType.StatusChanged:
                    HandleStatusChange(msg);
                    break;
            }
        }

        private void HandleStatusChange(NetIncomingMessage msg)
        {
            switch ((NetConnectionStatus)msg.ReadByte())
            {
                case NetConnectionStatus.Connected:
                    HandleNewConnection(msg.SenderConnection);
                    break;
                case NetConnectionStatus.Disconnected:
                    HandleClosedConnection(msg.SenderConnection);
                    break;
            }

        }

        private void Run()
        {
            while (_isRunning)
            {
                NetIncomingMessage msg = Peer.ReadMessage();
                if (msg != null)
                    HandleMessage(msg);
                else
                    Peer.MessageReceivedEvent.WaitOne(250);
            }
        }
    }
}
