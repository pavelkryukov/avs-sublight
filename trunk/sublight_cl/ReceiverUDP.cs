using System;
using System.Net;
using System.Net.Sockets;

namespace sublight_cl
{
    internal class ReceiverUdp : Receiver
    {
        private const int Timeout = 100;

        internal override void Receive(byte[] data)
        {
            try
            {
                _mysocket.ReceiveFrom(data, 4, SocketFlags.None, ref _remote);
            }
            catch (SocketException)
            {
                throw new ReceiverException();
            }
        }

        internal override void Send(byte[] data)
        {
            _mysocket.SendTo(data, 4, SocketFlags.None, _remote);
        }

        private readonly Socket _mysocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private EndPoint _remote;

        internal ReceiverUdp(UInt16 port, Side side) : base(side)
        {
            _mysocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
            _mysocket.Bind(new IPEndPoint(IPAddress.Any, port));
            _mysocket.ReceiveTimeout = Timeout;

            _remote = new IPEndPoint(IPAddress.Any, 0);
        }

        public void KillSocket()
        {
            _mysocket.Close();
        }

        ~ReceiverUdp()
        {
            KillSocket();
        }
    } 
}
