using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SplitGrid.PainLib.Schema.pain_002_001_03;

namespace SplitGrid.PainLib.Tests
{
    [TestClass]
    public class ReaderTest
    {
        [TestMethod]
        public void WHEN_GetPaymentsInvokedWithNullPath_THEN_ResultIsEmptyArray()
        {
            // Arrange
            var sut = new Reader();

            // Act
            var result = sut.GetPaymentsData(null);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void WHEN_GetPaymentsInvokedWithPathWhichDoesNotExist_THEN_ResultIsEmptyArray()
        {
            // Arrange
            var sut = new Reader();

            // Act
            var result = sut.GetPaymentsData(@"c:\a-path-or-folder-which-does-not-exist-on-file-system");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void WHEN_GetPaymentsInvokedWithValidPathNoFiles_THEN_ResultIsEmptyArray()
        {
            // Arrange
            var sut = new Reader();

            // Act -create dynamically one empty folder and remove 

            string exePath = System.Environment.CurrentDirectory.ToString();
            if (exePath.Contains(@"\bin\Debug"))
            {
                exePath = exePath.Remove((exePath.Length - (@"\bin\Debug").Length));
            }
            string appPath = exePath + "\\EmptyFolder";
            var result = sut.GetPaymentsData(appPath);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void WHEN_GetPaymentsInvokedWithValidPathContainSingleFile_THEN_ResultIsOnlyOneArrayItem()
        {
            // Arrange
            var sut = new Reader();

            // Act
            string exePath = System.Environment.CurrentDirectory.ToString();
            if (exePath.Contains(@"\bin\Debug"))
            {
                exePath = exePath.Remove((exePath.Length - (@"\bin\Debug").Length));
            }
            string appPath = exePath + "\\Files";
            var result = sut.GetPaymentsData(appPath);
            // Assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(new DateTime(2016, 03, 16, 11, 43, 55), result.FirstOrDefault().CreationDate);
            Assert.AreEqual("PartiallyAccepted", result.FirstOrDefault().Status.ToString());
            Assert.AreEqual(5, result.FirstOrDefault().Payments.Length);
            Assert.AreEqual(0, result.FirstOrDefault().ErrorMessages.Length);

        }

        [TestMethod]
        public void WHEN_GetPaymentsWithOnePaymentInfoContainsOnlyDebitorInfo_THEN_ResultIsOnlyOneSinglePaymentData()
        {
            // Arrange
            var sut = new Reader();

            // Act
            var originalPaymentInformations = new List<OriginalPaymentInformation1> {
                 new OriginalPaymentInformation1 { OrgnlPmtInfId = "FilA20160212TC01",
                                                 TxInfAndSts = new List<PaymentTransactionInformation25>{
                                                  new PaymentTransactionInformation25 { OrgnlInstrId = "0000000008", OrgnlEndToEndId= "Own reference 1", TxSts= TransactionIndividualStatus3Code.ACCP
                                                  , OrgnlTxRef= new OriginalTransactionReference13{  Amt = new AmountType3Choice{ Item = new EquivalentAmount2 { Amt = new ActiveOrHistoricCurrencyAndAmount { Value = 25.99m} } },ReqdExctnDt = new DateTime(2016,03,16),
                                                                  Dbtr = new PartyIdentification32 { Nm= "Debtor AB" },DbtrAcct = new CashAccount16 {Id= new AccountIdentification4Choice {Item  = new GenericAccountIdentification1 {  Id = "44444001", SchmeNm =new AccountSchemeName1Choice { Item = "BBAN" } } } },
                                                                  DbtrAgt = new BranchAndFinancialInstitutionIdentification4{ FinInstnId = new FinancialInstitutionIdentification7 { BIC= "HANDGB20" } },RmtInf = new RemittanceInformation5{ Ustrd  = new List<string> { "Message to beneficairy" }.ToArray() }  } } }.ToArray() }
            }; 

            // assign the data over here and verify the payment info
            var result = sut.GetPayments(originalPaymentInformations.ToArray());
            // Assert
            var paymentInfo = result.FirstOrDefault();
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("FilA20160212TC01", paymentInfo.Id);
            Assert.AreEqual(1, paymentInfo.Transactions.Length);
            Assert.AreEqual("0000000008", paymentInfo.Transactions[0].OriginialInstructionId);
            Assert.AreEqual("Own reference 1", paymentInfo.Transactions[0].OriginialEndToEndId);
            Assert.AreEqual("Accepted", paymentInfo.Transactions[0].Status.ToString());
            Assert.AreEqual(25.99m, paymentInfo.Transactions[0].Amount);
            Assert.AreEqual(new DateTime(2016, 03, 16), paymentInfo.Transactions[0].RequestExecutionDate);
            Assert.AreEqual("Debtor AB", paymentInfo.Transactions[0].Debtor.Name);
            Assert.AreEqual("44444001", paymentInfo.Transactions[0].Debtor.IBanOrAccntNumber);
            Assert.AreEqual("BBAN", paymentInfo.Transactions[0].Debtor.AccountUsageType);
            Assert.AreEqual("HANDGB20", paymentInfo.Transactions[0].Debtor.BankerBusinessIdentifierCode);
            Assert.AreEqual("Message to beneficairy", paymentInfo.Transactions[0].Remittance.UnStructuredMessage[0]);
        }
    }
}
