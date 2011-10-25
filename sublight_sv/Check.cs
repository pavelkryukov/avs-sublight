using System.Linq;
using System.Timers;

namespace sublight_sv
{
    abstract internal class Check
    {
        private static readonly byte[] ChkL = { 0x00, 0xFF, 0xFF, 0xFF };
        private static readonly byte[] ChkLAns = { 0x04, 0xAA, 0xAA, 0xAA };

        private static readonly byte[] ChkR = { 0xC0, 0xFF, 0xFF, 0xFF };
        private static readonly byte[] ChkRAns = { 0xC4, 0xAA, 0xAA, 0xAA };

        internal abstract void Send(byte[] data);
        internal abstract byte[] Receive();
        internal abstract bool IsAvailable();

        private readonly Timer _timer;
        private int _i;

        private readonly ChkDialog _chkDialog;


        internal Check(MainForm parent)
        {
            _timer = new Timer
                         {
                             Enabled = true, 
                             Interval = 100
                         };
            _timer.Elapsed += ((sender, e) => _i++);

            _chkDialog = new ChkDialog(parent);
        }

        private bool SendAndReceiveData(byte[] req, byte[] ans)
        {
            Send(req);

            byte[] rdata = null;
            _timer.Start();
            _i = 0;

            while (_i <= 3)//Wait for ansver 3 timer ticks
            {
                _chkDialog.ReDraw();
                if (IsAvailable())
                {
                    rdata = Receive();
                }
            }
          
            _timer.Stop();
            return rdata != null && ans.SequenceEqual(rdata);
        }

        public void StartSending()
        {
            _chkDialog.ClearProgressBar();

            _chkDialog.SetLeft  = SendAndReceiveData(ChkL, ChkLAns);
            _chkDialog.AddProgressBar();
            _chkDialog.ReDraw();

            _chkDialog.SetRight = SendAndReceiveData(ChkR, ChkRAns);
            _chkDialog.AddProgressBar();
            _chkDialog.ReDraw();
        }
    }
}
