using System.ComponentModel.Design;

namespace DataWindow.DesignerInternal
{
    internal class DesignerTransactionImpl : DesignerTransaction
    {
        private readonly DesignerHost _host;

        public DesignerTransactionImpl(DesignerHost host)
        {
            _host = host;
        }

        public DesignerTransactionImpl(DesignerHost host, string name) : base(name)
        {
            _host = host;
        }

        protected override void Dispose(bool disposing)
        {
            if (!Committed && !Canceled) Cancel();
            base.Dispose(disposing);
        }

        protected override void OnCommit()
        {
            _host.TransactionCommiting(this);
        }

        protected override void OnCancel()
        {
            _host.TransactionCanceling(this);
        }
    }
}