namespace SplitGrid.PainLib.Interop
{
    public class StructuredInfo
    {
        public ReferredDocument[] ReferredDocuments { get; set; }
        public decimal? CreditAmount { get; set; }
        public decimal? RemittedAmount { get; set; }
        public string CreditorReferenceCode { get; set; }
        public string CreditorReferenceIssuer { get; set; }
        public string CreditorReference { get; set; }
        public string[] AdditionalRemittanceInfo { get; set; }
    }
}