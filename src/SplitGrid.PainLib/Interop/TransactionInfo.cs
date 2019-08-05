namespace SplitGrid.PainLib.Interop
{
    public class TransactionInfo
    {
        public Account Creditor { get; set; }
        public Account Debtor { get; set; }
        public TransactionStatus Status { get; set; }
    }
}