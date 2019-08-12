
using SplitGrid.PainLib.Interop;
using SplitGrid.PainLib.Schema.pain_002_001_03;
using System;

namespace SplitGrid.PainLib.Extensions
{
    public static class StatusExtension
    {
        public static TransactionStatus ToTransactionStatus(this TransactionIndividualStatus3Code source)
        {
            switch (source)
            {
                case TransactionIndividualStatus3Code.ACTC:
                    return TransactionStatus.TransactionAccepted;
                case TransactionIndividualStatus3Code.ACCP:
                    return TransactionStatus.Accepted;
                case TransactionIndividualStatus3Code.RJCT:
                    return TransactionStatus.Rejected;
                case TransactionIndividualStatus3Code.PDNG:
                    return TransactionStatus.Pending;
                case TransactionIndividualStatus3Code.ACSP:
                    return TransactionStatus.AcceptedByTheClearingSystem;
                case TransactionIndividualStatus3Code.ACSC:
                    return TransactionStatus.AcceptedSettlementCompleted;
                case TransactionIndividualStatus3Code.ACWC:
                    return TransactionStatus.TransactionAcceptedWithChange;
                default:
                    throw new ArgumentOutOfRangeException($"{source}  status not found");
            }
        }

        public static PaymentStatus ToTransactionStatus(this TransactionGroupStatus3Code source)
        {
            switch (source)
            {
                case TransactionGroupStatus3Code.ACTC: return PaymentStatus.TransactionAccepted;
                case TransactionGroupStatus3Code.ACSP: return PaymentStatus.AcceptedByTheClearingSystem;
                case TransactionGroupStatus3Code.ACSC: return PaymentStatus.AcceptedSettlementCompleted;
                case TransactionGroupStatus3Code.ACWC: return PaymentStatus.TransactionAcceptedWithChange;
                case TransactionGroupStatus3Code.ACCP: return PaymentStatus.Accepted;
                case TransactionGroupStatus3Code.PART: return PaymentStatus.PartiallyAccepted;
                case TransactionGroupStatus3Code.RJCT: return PaymentStatus.Rejected;
                case TransactionGroupStatus3Code.PDNG: return PaymentStatus.Pending;
                case TransactionGroupStatus3Code.RCVD: return PaymentStatus.Received;
                default:
                    throw new ArgumentOutOfRangeException($"{source}  status not found");
            }
        }
    }
}
