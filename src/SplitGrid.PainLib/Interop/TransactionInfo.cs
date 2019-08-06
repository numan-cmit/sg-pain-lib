using System;

namespace SplitGrid.PainLib.Interop
{
    public class TransactionInfo
    {
        public string OriginialInstructionId { get; set; }
        public string OriginialEndToEndId { get; set; }
        public Account Creditor { get; set; }
        public Account Debtor { get; set; }
        public PaymentStatus Status { get; set; }
        public decimal Amount { get; set; }
        public string[] ErrorMessages { get; set; }
        public DateTime RequestExecutionDate { get; set; }
        public RemittanceInfo Remittance { get; set; }
    }
}
