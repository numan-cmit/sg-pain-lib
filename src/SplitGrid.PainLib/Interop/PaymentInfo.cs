﻿namespace SplitGrid.PainLib.Interop
{
    public class PaymentInfo
    {
        public string FileName { get; set; }
        public Payment[] Payments { get; set; }
        public PaymentStatus Status { get; set; }
    }
}