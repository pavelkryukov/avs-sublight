using System;
using System.Linq;

namespace sublight_cl
{
    internal abstract class Receiver
    {
        internal sealed class ReceiverException : Exception
        {
        };

        private readonly byte[] _chk;
        private readonly byte[] _chkAns;

        public readonly Lamp4 Lamp;

        private readonly Side _side;

        private readonly Func<byte, bool> _checkIfMine;

        internal abstract void Receive(byte[] value);
        internal abstract void Send(byte[] value);

        internal Receiver(Side side)
        {
            Lamp = new Lamp4(side) {IsOn = true};
            Lamp.Show();
            _side = side;

            switch (_side)
            {
                case Side.Left:
                    _chk =    new byte[] { 0x00, 0xFF, 0xFF, 0xFF };
                    _chkAns = new byte[] { 0x04, 0xAA, 0xAA, 0xAA };
                    _checkIfMine = data => (data & 0xCC) == 0x0C;

                    break;
                case Side.Right:
                    _chk    = new byte[] { 0xC0, 0xFF, 0xFF, 0xFF };
                    _chkAns = new byte[] { 0xC4, 0xAA, 0xAA, 0xAA };
                    _checkIfMine = data => (data & 0xCC) == 0xCC;

                    break;
                case Side.Top:
                    _chk = new byte[] { 0x40, 0xFF, 0xFF, 0xFF };
                    _chkAns = new byte[] { 0x44, 0xAA, 0xAA, 0xAA };
                    _checkIfMine = data => (data & 0x3C) == 0x0C;

                    break;
                case Side.Bottom:
                    _chk = new byte[] { 0x70, 0xFF, 0xFF, 0xFF };
                    _chkAns = new byte[] { 0x74, 0xAA, 0xAA, 0xAA };
                    _checkIfMine = data => (data & 0x3C) == 0x3C;

                    break;
            }
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
                Lamp.DoEvents();
                var data = new byte[4];
                try
                {
                    Receive(data);
                }
                catch (ReceiverException)
                {
                    if (!Lamp.IsOn)
                    {
                        break;
                    }
                    continue;
                }

                if (data.SequenceEqual(_chk))
                {
                    Send(_chkAns);
                }

                else if (_checkIfMine(data[0]) && Crc(data))
                {
                    Lamp.SetColor(data);
                }
            }
        }
    }
}
