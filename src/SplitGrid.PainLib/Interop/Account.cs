namespace SplitGrid.PainLib.Interop
{
    /// <summary>
    /// Account, either Creditor/Debtor
    /// </summary>
    public class Account
    {
        public string Name { get; set; }
        public string IBanOrAccntNumber { get; set; }
        public string AccountUsageType { get; set; }
        public string BankerClearanceCode { get; set; }
        public string BankerClearanceId { get; set; }
        public string BankerBusinessIdentifierCode { get; set; }
        
    }

}