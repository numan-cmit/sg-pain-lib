namespace SplitGrid.PainLib.Interop
{
    public class Payment
    {
        public string Id { get; set; }
        public TransactionInfo[] Transactions { get; set; }
    }
}