using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IllyumL2T.Core.FieldsSplit.UnitTests
{
  [TestClass]
  public class DelimiterSeparatedValuesFieldsSplitterTests
  {
    [TestMethod]
    public void SetupTest()
    {
      // Arrange
      var properties = typeof(Order).GetProperties();
      var expected = properties.Length;

      // Act
      var fieldsSplitter = new DelimiterSeparatedValuesFieldsSplitter<Order>(delimiter: ',');
      var actual = fieldsSplitter.FieldParsers.Length;

      // Assert
      Assert.AreEqual<int>(expected, actual);
    }

    [TestMethod]
    public void SplitTest()
    {
      // Arrange
      var delimiter = ',';
      var line = String.Format("10248{0} 1.10{0} Address X{0} 10/10/2010", delimiter);

      // Act
      var fieldsSplitter = new DelimiterSeparatedValuesFieldsSplitter<Order>(delimiter);
      var actual = fieldsSplitter.Split(line);

      // Assert
      var expected = new string[] { "10248", "1.10", "Address X", "10/10/2010" };
      Assert.IsTrue(expected.SequenceEqual(actual));
    }

    [TestMethod]
    public void SplitDoubleQuotedTextTest()
    {
      // Arrange
      var delimiter = '|';
      var line = String.Format("10248{0}1.0{0}\"Address between double quotes\"{0}10/10/2010", delimiter);

      // Act
      var fieldsSplitter = new DelimiterSeparatedValuesFieldsSplitter<Order>(delimiter);
      var actual = fieldsSplitter.Split(line);
      
      // Assert
      var expected = new string[] { "10248", "1.0", "Address between double quotes", "10/10/2010" };
      Assert.IsTrue(expected.SequenceEqual(actual));
    }
  }
}
