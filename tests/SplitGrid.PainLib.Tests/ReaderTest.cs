using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}
