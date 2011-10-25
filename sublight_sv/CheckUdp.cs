using System.Net;
using System.Net.Sockets;

namespace sublight_sv
{
    internal class CheckUdp : Check
    {
        private readonly Socket _mysocket; 
        private readonly IPEndPoint _sender;

        internal override bool IsAvailable()
        {
            return _mysocket.Available != 0;
        }

        internal override void Send(byte[] data)
        {
 	        _mysocket.SendTo(data, 4, SocketFlags.None, _sender);
        }

        internal override byte[] Receive()
        {
            var remote = (EndPoint) _sender;
            var data = new byte[4];
            _mysocket.ReceiveFrom(data, ref remote);
            return data;
        }

        internal CheckUdp(MainForm mainForm, System.UInt16 port) : base(mainForm)
        {
            _mysocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _mysocket.Bind(new IPEndPoint(IPAddress.Any, 12051));
            _mysocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
            _mysocket.Blocking = false;

            _sender = new IPEndPoint(IPAddress.Broadcast, port);
        }

        public void KillSocket()
        {
            _mysocket.Close();
        }
    }
}
