using SplitGrid.PainLib.Interop;
using System;
using System.Linq;

namespace SplitGrid.PainLib
{
    public class Reader
    {
        public PaymentInfo[] GetPaymentsData(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath)) return Enumerable.Empty<PaymentInfo>().ToArray();
            throw new NotImplementedException();
        }
    }
}