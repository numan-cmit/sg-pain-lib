using SplitGrid.PainLib.Interop;
using SplitGrid.PainLib.Schema.pain_002_001_03;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace SplitGrid.PainLib
{
    public class Reader
    {
        public PaymentInfo[] GetPaymentsData(string folderPath, string filePattern = "*.xml")
        {
            if (string.IsNullOrEmpty(folderPath)) return Enumerable.Empty<PaymentInfo>().ToArray();
            if (!Directory.Exists(folderPath)) return Enumerable.Empty<PaymentInfo>().ToArray();

            var files = Directory.EnumerateFiles(folderPath, filePattern);

            if (!files.Any()) return Enumerable.Empty<PaymentInfo>().ToArray();

            return ProcessFiles(files);
        }

        private PaymentInfo[] ProcessFiles(IEnumerable<string> files)
        {
            var result = new List<PaymentInfo>();
            foreach (var file in files)
            {
                var info = GetPaymentInfo(file);
                if (info != null) result.Add(info);
            }
            return result.ToArray();
        }

        private PaymentInfo GetPaymentInfo(string file)
        {
            using (var stream = File.OpenRead(file))
            {
                try
                {
                    var serializer = new XmlSerializer(typeof(Document));
                    var doc = (Document)serializer.Deserialize(stream);
                    return BuildPaymentInfoFromDocument(doc, file);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        private PaymentInfo BuildPaymentInfoFromDocument(Document doc, string fileName)
        {
            PaymentInfo paymentInfo = new PaymentInfo();
            try
            {
                var originalGrpInfoAndSts = doc.CstmrPmtStsRpt.OrgnlGrpInfAndSts;
                List<string> groupStatusReason = new List<string>();
                paymentInfo.FileName = fileName;
                paymentInfo.ErrorMessages = GetErrorMessages(originalGrpInfoAndSts.StsRsnInf);
                paymentInfo.Status = GetStatus(originalGrpInfoAndSts.GrpSts.ToString());
                paymentInfo.Payments = GetPayments(doc.CstmrPmtStsRpt.OrgnlPmtInfAndSts);
                paymentInfo.CreationDate = doc.CstmrPmtStsRpt.GrpHdr.CreDtTm;

            }
            catch (Exception ex)
            {
                return null;
            }
            return paymentInfo;
        }

        private Payment[] GetPayments(OriginalPaymentInformation1[] orgnlPmtInfAndSts)
        {
            List<Payment> payments = new List<Payment>();
            try
            {
                if (orgnlPmtInfAndSts?.Count() > 0)
                {
                    foreach (var pmtInfo in orgnlPmtInfAndSts)
                    {
                        Payment payment = new Payment();
                        payment.Id = pmtInfo.OrgnlPmtInfId;
                        if (pmtInfo.TxInfAndSts?.Count() > 0) payment.Transactions = GetPaymentTransactions(pmtInfo.TxInfAndSts);
                        payments.Add(payment);
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return payments.ToArray();
        }

        private TransactionInfo[] GetPaymentTransactions(PaymentTransactionInformation25[] txInfAndSts)
        {
            List<TransactionInfo> transactions = new List<TransactionInfo>();
            try
            {
                foreach (var txnInfo in txInfAndSts)
                {
                    TransactionInfo transaction = new TransactionInfo();
                    transaction.Status = GetStatus(txnInfo.TxSts.ToString());
                    if (txnInfo.OrgnlTxRef != null)
                    {
                        transaction.OriginialInstructionId = txnInfo.OrgnlInstrId;
                        transaction.OriginialEndToEndId = txnInfo.OrgnlEndToEndId;
                        transaction.Amount = GetAmount(txnInfo.OrgnlTxRef.Amt?.Item);
                        transaction.RequestExecutionDate = txnInfo.OrgnlTxRef.ReqdExctnDt;
                        transaction.Creditor = GetAccountData(txnInfo.OrgnlTxRef, true);
                        transaction.Debtor = GetAccountData(txnInfo.OrgnlTxRef, false);
                        transaction.Remittance = GetRemittanceInfo(txnInfo.OrgnlTxRef.RmtInf);
                    }
                    transactions.Add(transaction);
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return transactions.ToArray();
        }

        private RemittanceInfo GetRemittanceInfo(RemittanceInformation5 rmtInf)
        {
            RemittanceInfo remittance = new RemittanceInfo();
            if (rmtInf != null)
            {
                remittance.UnStructuredMessage = rmtInf.Ustrd;
                if (rmtInf.Strd?.Count() > 0) remittance.StructuredInfos = GetReferredDocumentInfo(rmtInf.Strd);
            }
            return remittance;
        }

        private StructuredInfo[] GetReferredDocumentInfo(StructuredRemittanceInformation7[] structuredRemittances)
        {
            List<StructuredInfo> structuredInfos = new List<StructuredInfo>();
            List<ReferredDocument> referredDocuments = new List<ReferredDocument>();
            try
            {
                if (structuredRemittances?.Count() > 0)
                {
                    var structuredInfo = new StructuredInfo();
                    foreach (var structuredRemittance in structuredRemittances)
                    {
                        var referdDocInfotypes = structuredRemittance.RfrdDocInf;
                        if (referdDocInfotypes?.Count() > 0)
                        {
                            var referredDoc = new ReferredDocument();
                            foreach (var rfrdDoc in referdDocInfotypes)
                            {
                                referredDoc.InformationType = rfrdDoc.Tp?.CdOrPrtry?.ToString();
                                referredDoc.Issuer = rfrdDoc.Tp?.Issr;
                                referredDoc.InvoiceNumber = rfrdDoc.Nb;
                                referredDocuments.Add(referredDoc);
                            }
                        }
                        structuredInfo.ReferredDocuments = referredDocuments.ToArray();
                        structuredInfo.CreditAmount = structuredRemittance.RfrdDocAmt?.CdtNoteAmt?.Value;
                        structuredInfo.RemittedAmount = structuredRemittance.RfrdDocAmt?.RmtdAmt?.Value;
                        structuredInfo.CreditorReferenceCode = structuredRemittance?.CdtrRefInf?.Tp?.CdOrPrtry?.Item.ToString();
                        structuredInfo.CreditorReferenceIssuer = structuredRemittance?.CdtrRefInf?.Tp?.Issr;
                        structuredInfo.CreditorReference = structuredRemittance?.CdtrRefInf?.Ref;
                        structuredInfo.AdditionalRemittanceInfo = structuredRemittance?.AddtlRmtInf;
                        structuredInfos.Add(structuredInfo);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return structuredInfos.ToArray();
        }
        private Account GetAccountData(OriginalTransactionReference13 orgnlTxRef, bool isCreditor)
        {
            Account account = new Account();
            AccountIdentification4Choice accountInfo = new AccountIdentification4Choice();
            FinancialInstitutionIdentification7 financialInstnId = new FinancialInstitutionIdentification7();
            try
            {
                if (isCreditor)
                {
                    account.Name = orgnlTxRef.Cdtr?.Nm;
                    if (orgnlTxRef.CdtrAcct != null)
                        accountInfo = orgnlTxRef.CdtrAcct?.Id;

                    if (orgnlTxRef.CdtrAgt != null)
                        financialInstnId = orgnlTxRef.CdtrAgt.FinInstnId;
                }
                else
                {
                    account.Name = orgnlTxRef.DbtrAcct?.Nm;
                    if (orgnlTxRef.CdtrAcct != null)
                        accountInfo = orgnlTxRef.DbtrAcct?.Id;

                    if (orgnlTxRef.CdtrAgt != null)
                        financialInstnId = orgnlTxRef.DbtrAgt.FinInstnId;
                }
                if (financialInstnId != null)
                {
                    account.BankerBusinessIdentifierCode = financialInstnId?.BIC;
                    account.BankerClearanceId = financialInstnId.ClrSysMmbId?.MmbId;
                    account.BankerClearanceCode = financialInstnId.ClrSysMmbId?.ClrSysId?.Item;
                }
                if (accountInfo != null)
                {
                    if (accountInfo?.Item.GetType() == typeof(GenericAccountIdentification1))
                    {
                        var cdtrAccount = (GenericAccountIdentification1)accountInfo.Item;
                        account.IBanOrAccntNumber = cdtrAccount.Id;
                        account.AccountUsageType = cdtrAccount.SchmeNm?.Item;
                    }
                    else
                        account.IBanOrAccntNumber = accountInfo.Item?.ToString();
                }
            }
            catch (Exception ex)
            {

            }
            return account;

        }

        private decimal GetAmount(object item)
        {
            decimal amount = default(decimal);
            if (item?.GetType() == typeof(EquivalentAmount2))
            {
                var equivalentAmount = (EquivalentAmount2)item;
                amount = equivalentAmount.Amt.Value;
            }
            else if (item?.GetType() == typeof(ActiveOrHistoricCurrencyAndAmount))
            {
                var currencyAndAmount = (ActiveOrHistoricCurrencyAndAmount)item;
                amount = currencyAndAmount.Value;
            }
            return amount;
        }

        private string[] GetErrorMessages(StatusReasonInformation8[] statusReasons)
        {
            List<string> errMessages = new List<string>();

            if (statusReasons != null)
            {
                foreach (var statusReason in statusReasons)
                {
                    var statusCode = statusReason.Rsn?.Item;
                    var additionalInfo = statusReason.AddtlInf;
                    errMessages.Add(statusCode);

                    if (additionalInfo?.Count() > 0) errMessages.AddRange(additionalInfo);
                }
            }
            return errMessages.ToArray();
        }

        private PaymentStatus GetStatus(string statusType)
        {
            PaymentStatus status = default(PaymentStatus);
            switch (statusType)
            {
                case "ACCP":
                    status = PaymentStatus.Accepted;
                    break;
                case "PART":
                    status = PaymentStatus.PartiallyAccepted;
                    break;
                case "RJCT":
                    status = PaymentStatus.Rejected;
                    break;
                case "PDNG":
                    status = PaymentStatus.Pending;
                    break;
                default:
                    break;
            }
            return status;
        }
    }
}