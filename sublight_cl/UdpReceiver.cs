using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace sublight_cl
{
    class UdpReceiver
    {
        private readonly byte[] _chk;
        private readonly byte[] _chkAns;
        private readonly byte _mask;

        public readonly Lamp Lamp;

        private readonly Side _side;

        private readonly Socket _mysocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private EndPoint _remote;

        private const int Timeout = 100;

        internal UdpReceiver(UInt16 port, Side side)
        {
            Lamp = new Lamp(side);
            Lamp.IsOn = true;
            Lamp.Show();
            _side = side;

            switch (_side)
            {
                case Side.Left:
                    _chk =    new byte[] { 0x00, 0xFF, 0xFF, 0xFF };
                    _chkAns = new byte[] { 0x04, 0xAA, 0xAA, 0xAA };
                    _mask = 0x0C;
                    
                    break;
                case Side.Right:
                    _chk    = new byte[] { 0xC0, 0xFF, 0xFF, 0xFF };
                    _chkAns = new byte[] { 0xC4, 0xAA, 0xAA, 0xAA };
                    _mask = 0xCC;
                    break;
            }

            _mysocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
            _mysocket.Bind(new IPEndPoint(IPAddress.Any, port));
            _mysocket.ReceiveTimeout = Timeout;

            _remote = new IPEndPoint(IPAddress.Any,0);
        }

        public void KillSocket()
        {
            _mysocket.Close();
        }

        ~UdpReceiver()
        {
            KillSocket();
        }

        private static byte ControlSumByte(byte source)
        {
            byte sum = 0;
            for (; source > 0; source >>= 1)
            {
                sum += (byte)(source & 1);
            }
            return sum;
        }

        private static bool Crc(byte[] check)
        {
            return (check[0] & 0x3) == ((ControlSumByte((byte)(check[0] & ~0x3)) +
                                         ControlSumByte(check[1]) +
                                         ControlSumByte(check[2]) +
                                         ControlSumByte(check[3])
                                        ) & 3);
        }

        public void Start()
        {
            while(true)
            {
                var data = new byte[4];
                try
                {
                    _mysocket.ReceiveFrom(data, 4, SocketFlags.None, ref _remote);
                }
                catch (SocketException)
                {
                    Lamp.DoEvents();
                    if (!Lamp.IsOn)
                    {
                        break;
                    }
                    continue;
                }

                if (data.SequenceEqual(_chk))
                {
                    _mysocket.SendTo(_chkAns, 4, SocketFlags.None, _remote);
                    Lamp.DoEvents();
                }

                else if ((data[0] & 0xCC) == _mask && Crc(data))
                {
                    Lamp.SetColor(data);
                }

            }
        }
    }
}
