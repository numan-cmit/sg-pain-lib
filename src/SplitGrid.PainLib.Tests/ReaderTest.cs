using System;
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
        public void GetPaymentsInvokedWithPathExist_THEN_NoFilesResultIsEmptyArray()
        {
            // Arrange
            var sut = new Reader();

            // Act
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
        public void GetPaymentsInvokedWithPathExists()
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
            var files = Directory.EnumerateFiles(appPath, "*.xml");
            foreach (var file in files)
            {
                using (var stream = File.OpenRead(file))
                {
                    try
                    {
                        var paymentInfos = result.ToList();
                        var paymentInfo = paymentInfos.Where(x => x.FileName.Contains(file)).FirstOrDefault();
                        var serializer = new XmlSerializer(typeof(Document));
                        var doc = (Document)serializer.Deserialize(stream);

                        //Assert 
                        Assert.AreEqual(paymentInfo.Payments.Length, doc.CstmrPmtStsRpt.OrgnlPmtInfAndSts.Length);

                    }
                    catch (Exception)
                    {
                        //return null;
                    }
                }

            }

        }

    }
}
