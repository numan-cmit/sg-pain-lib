using System;

namespace SplitGrid.PainLib.Interop
{
    public class PaymentInfo
    {
        public string FileName { get; set; }
        public Payment[] Payments { get; set; }
        public PaymentStatus Status { get; set; }
        public string[] ErrorMessages{ get; set; }
        public DateTime CreationDate { get; set; }

    }
}