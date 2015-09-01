using IllyumL2T.Core.FieldsSplit.FieldsSplit;
using IllyumL2T.Core.FieldsSplit.UnitTests.Classes_for_Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

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
      Assert.Fail();
    }

    [TestMethod, TestCategory("FixedWidth")]
    public void FixedWidthSplit_IncompleteInput()
    {
      // Arrange
      ushort type = 0;
      byte category = 2;
      string line = $"{type:00}{category}";
      var splitter = new FixedWidthValuesFieldsSplitter<Record>();

      // Act
      string[] values = splitter.Split(line);

      // Assert
      Assert.AreEqual<int>(2, values.Length);
      Assert.AreEqual<string>($"{type:00}",values[0]);
      Assert.AreEqual<string>($"{category}", values[1]);
    }
  }
}