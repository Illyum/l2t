using IllyumL2T.Core.FieldsSplit.FieldsSplit;
using IllyumL2T.Core.FieldsSplit.UnitTests.Classes_for_Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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

      // Act
      var splitter = new FixedWidthValuesFieldsSplitter<Record>();
      string[] values = splitter.Split(line);

      // Assert
      Assert.Fail();
    }
  }
}