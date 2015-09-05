using System;

using IllyumL2T.Core.FieldsSplit.FieldsSplit;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IllyumL2T.Core.FieldsSplit.UnitTests
{
    [TestClass]
    public class FixedWidthValuesFieldsSplitterTests
    {
        [TestMethod, TestCategory("FixedWidth"), ExpectedException(typeof(ArgumentNullException))]
        public void FixedWidthSplit_InvalidInput()
        {
            // Arrange
            string line = null;
            var splitter = new FixedWidthValuesFieldsSplitter<Record>();

            // Act
            string[] values = splitter.Split(line);

            // Assert
        }

        [TestMethod, TestCategory("FixedWidth")]
        public void FixedWidthSplit_IncompleteInput()
        {
            // Arrange
            string type = "00";
            string category = "2";
            string line = $"{type}{category}";

            // Act
            var splitter = new FixedWidthValuesFieldsSplitter<Record>();
            string[] actual = splitter.Split(line);

            // Assert
            var expectedLength = 2;
            Assert.AreEqual<int>(expectedLength, actual.Length);

            var expectedTypeValue = type;
            var actualTypeValue = actual[0];
            Assert.AreEqual<string>(expectedTypeValue, actualTypeValue);

            var expectedCategoryValue = category.ToString();
            var actualCategoryValue = actual[1];
            Assert.AreEqual<string>(expectedCategoryValue, actualCategoryValue);
        }
    }
}