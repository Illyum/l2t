using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using IllyumL2T.Core.FieldsSplit;
using IllyumL2T.Core.Parse;

namespace IllyumL2T.Core.FieldsSplit.UnitTests
{
  [TestClass]
  public class FieldParserTests
  {
    [TestMethod]
    public void FieldParserCreationTest()
    {
      // Arrange
      var propertyName = "StringProperty";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);

      // Act
      var fieldParser = new FieldParser(propertyInfo);

      // Assert
      var expected = propertyName;
      var actual = fieldParser.FieldName;
      Assert.AreEqual<string>(expected, actual);
    }

    [TestMethod]
    public void FieldParserWithPatternTest()
    {
      // Arrange
      var propertyName = "StringPropertyWithPattern";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      var actual = fieldParser.Parse("user@domain.com");

      // Assert
      var expected = "user@domain.com";
      Assert.AreEqual(expected, actual);

      Assert.IsFalse(fieldParser.Errors.Any());
    }

    [TestMethod]
    public void FieldParserWithPatternFailedTest()
    {
      // Arrange
      var propertyName = "StringPropertyWithPattern";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      var actual = fieldParser.Parse("****");

      // Assert
      Assert.IsNull(actual);

      var expectedErrorCount = 1;
      var actualErrorCount = fieldParser.Errors.Count();
      Assert.AreEqual<int>(expectedErrorCount, actualErrorCount);

      var expectedErrorMessage = String.Format("{0}: {1} does not match pattern >>> {2}",
                                               fieldParser.FieldName,
                                               fieldParser.FieldInput,
                                               fieldParser.ParseBehavior.Pattern);
      var actualErrorMessage = fieldParser.Errors.Single();
      Assert.AreEqual<string>(expectedErrorMessage, actualErrorMessage);
    }

    [TestMethod]
    public void FieldParserEmptyWithPatternTest()
    {
      // Arrange
      var propertyName = "StringPropertyWithPattern";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      var actual = fieldParser.Parse(String.Empty);

      // Assert
      Assert.IsNull(actual);

      var expectedErrorCount = 1;
      var actualErrorCount = fieldParser.Errors.Count();
      Assert.AreEqual<int>(expectedErrorCount, actualErrorCount);

      var expectedErrorMessage = String.Format("{0}: {1} does not match pattern >>> {2}",
                                               fieldParser.FieldName,
                                               fieldParser.FieldInput,
                                               fieldParser.ParseBehavior.Pattern);
      var actualErrorMessage = fieldParser.Errors.Single();
      Assert.AreEqual<string>(expectedErrorMessage, actualErrorMessage);
    }

    [TestMethod]
    public void ParseDateTimeTest()
    {
      // Arrange
      var propertyName = "DateTimeProperty";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      var actual = fieldParser.Parse("24/12/1900");

      // Assert
      var expected = new DateTime(1900, 12, 24);
      Assert.AreEqual(expected, actual);
      Assert.IsFalse(fieldParser.Errors.Any());
    }

    [TestMethod]
    public void ParseDateTimeUnparsableTest()
    {
      // Arrange
      var propertyName = "DateTimeProperty";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      var actual = fieldParser.Parse("ABCD");

      // Assert
      Assert.IsNull(actual);

      var expectedErrorCount = 1;
      var actualErrorCount = fieldParser.Errors.Count();
      Assert.AreEqual<int>(expectedErrorCount, actualErrorCount);

      var expectedErrorMessage = String.Format("{0}: Unparsable {1} >>> {2}",
                                               fieldParser.FieldName,
                                               fieldParser.FieldType,
                                               fieldParser.FieldInput);
      var actualErrorMessage = fieldParser.Errors.Single();
      Assert.AreEqual<string>(expectedErrorMessage, actualErrorMessage);
    }

    [TestMethod]
    public void ParseNullableDateTimeTest()
    {
      // Arrange
      var propertyName = "NullableDateTimeProperty";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      var actual = fieldParser.Parse("ABCD");

      // Assert
      Assert.IsNull(actual);
      Assert.IsFalse(fieldParser.Errors.Any());
    }

    [TestMethod]
    public void ParseDateTimeOffsetTest()
    {
      // Arrange
      var propertyName = "DateTimeOffsetProperty";
      var propertyInfo = typeof(Bar).GetProperties().Single(p => p.Name == propertyName);

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      var actual = fieldParser.Parse("24/12/1900 +1:00");

      // Assert
      var expected = new DateTimeOffset(new DateTime(1900, 12, 24), new TimeSpan(1, 0, 0));
      Assert.AreEqual(expected, actual);
      Assert.IsFalse(fieldParser.Errors.Any());
    }

    [TestMethod]
    public void ParseByteTest()
    {
      // Arrange
      var propertyName = "ByteProperty";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      var actual = fieldParser.Parse("123");

      // Assert
      var expected = (byte) 123;
      Assert.AreEqual(expected, actual);

      Assert.IsFalse(fieldParser.Errors.Any());
    }

    [TestMethod]
    public void ParseByteFailedTest()
    {
      // Arrange
      var propertyName = "ByteProperty";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      var actual = fieldParser.Parse("ABCD");

      // Assert
      Assert.IsNull(actual);

      var expectedErrorCount = 1;
      var actualErrorCount = fieldParser.Errors.Count();
      Assert.AreEqual<int>(expectedErrorCount, actualErrorCount);

      var expectedErrorMessage = String.Format("{0}: Unparsable {1} >>> {2}",
                                               fieldParser.FieldName,
                                               fieldParser.FieldType,
                                               fieldParser.FieldInput );
      var actualErrorMessage = fieldParser.Errors.Single();
      Assert.AreEqual<string>(expectedErrorMessage, actualErrorMessage);
    }

    [TestMethod]
    public void ParseByteOverflowTest()
    {
      // Arrange
      var propertyName = "ByteProperty";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);

      // Deliberate concatenation, not addition...
      var input = Byte.MaxValue.ToString() + "0";

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      var actual = fieldParser.Parse(input);

      // Assert
      Assert.IsNull(actual);

      var expectedErrorCount = 1;
      var actualErrorCount = fieldParser.Errors.Count();
      Assert.AreEqual<int>(expectedErrorCount, actualErrorCount);

      var expectedErrorMessage = String.Format("{0}: Unparsable {1} >>> {2}",
                                               fieldParser.FieldName,
                                               fieldParser.FieldType,
                                               fieldParser.FieldInput);
      var actualErrorMessage = fieldParser.Errors.Single();
      Assert.AreEqual<string>(expectedErrorMessage, actualErrorMessage);
    }

    [TestMethod]
    public void ParseNullableByteTest()
    {
      // Arrange
      var propertyName = "NullableByteProperty";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      var actual = fieldParser.Parse(String.Empty);

      // Assert
      Assert.IsNull(actual);
      Assert.IsFalse(fieldParser.Errors.Any());
    }

    [TestMethod]
    public void ParseCharTest()
    {
      // Arrange
      var propertyName = "CharProperty";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      var actual = fieldParser.Parse("A");

      // Assert
      Assert.IsNotNull(actual);
      Assert.IsFalse(fieldParser.Errors.Any());
    }

    [TestMethod]
    public void ParseCharFailedTest()
    {
      // Arrange
      var propertyName = "CharProperty";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      var actual = fieldParser.Parse("AB");

      // Assert
      Assert.IsNull(actual);

      var expectedErrorCount = 1;
      var actualErrorCount = fieldParser.Errors.Count();
      Assert.AreEqual<int>(expectedErrorCount, actualErrorCount);

      var expectedErrorMessage = String.Format("{0}: Unparsable {1} >>> {2}",
                                               fieldParser.FieldName,
                                               fieldParser.FieldType,
                                               fieldParser.FieldInput);
      var actualErrorMessage = fieldParser.Errors.Single();
      Assert.AreEqual<string>(expectedErrorMessage, actualErrorMessage);
    }

    [TestMethod]
    public void ParseNullableCharTest()
    {
      // Arrange
      var propertyName = "NullableCharProperty";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      var actual = fieldParser.Parse(String.Empty);

      // Assert
      Assert.IsNull(actual);
      Assert.IsFalse(fieldParser.Errors.Any());
    }

    [TestMethod]
    public void ParseInt16Test()
    {
      // Arrange
      var propertyName = "Int16Property";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      var actual = fieldParser.Parse("12345");

      // Assert
      Assert.IsNotNull(actual);
      Assert.IsFalse(fieldParser.Errors.Any());
    }

    [TestMethod]
    public void ParseInt16UnparsableTest()
    {
      // Arrange
      var propertyName = "Int16Property";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      var actual = fieldParser.Parse("ABCD");

      // Assert
      Assert.IsNull(actual);

      var expectedErrorCount = 1;
      var actualErrorCount = fieldParser.Errors.Count();
      Assert.AreEqual<int>(expectedErrorCount, actualErrorCount);

      var expectedErrorMessage = String.Format("{0}: Unparsable {1} >>> {2}",
                                               fieldParser.FieldName,
                                               fieldParser.FieldType,
                                               fieldParser.FieldInput);
      var actualErrorMessage = fieldParser.Errors.Single();
      Assert.AreEqual<string>(expectedErrorMessage, actualErrorMessage);
    }

    [TestMethod]
    public void ParseInt16OverflowTest()
    {
      // Arrange
      var propertyName = "Int16Property";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);

      // Deliberate concatenation, not an addition...
      var input = Int16.MaxValue.ToString() + "0";

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      var actual = fieldParser.Parse(input);

      // Assert
      Assert.IsNull(actual);

      var expectedErrorCount = 1;
      var actualErrorCount = fieldParser.Errors.Count();
      Assert.AreEqual<int>(expectedErrorCount, actualErrorCount);

      var expectedErrorMessage = String.Format("{0}: Unparsable {1} >>> {2}",
                                               fieldParser.FieldName,
                                               fieldParser.FieldType,
                                               fieldParser.FieldInput);
      var actualErrorMessage = fieldParser.Errors.Single();
      Assert.AreEqual<string>(expectedErrorMessage, actualErrorMessage);
    }

    [TestMethod]
    public void ParseNullableInt16Test()
    {
      // Arrange
      var propertyName = "NullableInt16Property";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      var actual = fieldParser.Parse(String.Empty);

      // Assert
      Assert.IsNull(actual);
      Assert.IsFalse(fieldParser.Errors.Any());
    }

    [TestMethod]
    public void ParseInt32Test()
    {
      // Arrange
      var propertyName = "Int32Property";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);
      var input = Int32.MaxValue.ToString();

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      fieldParser.Parse(input);

      // Assert
      var actual = fieldParser.FieldValue;
      Assert.IsNotNull(actual);

      Assert.IsFalse(fieldParser.Errors.Any());
    }

    [TestMethod]
    public void ParseInt32UnparsableTest()
    {
      // Arrange
      var propertyName = "Int32Property";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      fieldParser.Parse("ABCD");

      // Assert
      var actual = fieldParser.FieldValue;
      Assert.IsNull(actual);

      var expectedErrorCount = 1;
      var actualErrorCount = fieldParser.Errors.Count();
      Assert.AreEqual<int>(expectedErrorCount, actualErrorCount);

      var expectedErrorMessage = String.Format("{0}: Unparsable {1} >>> {2}",
                                               fieldParser.FieldName,
                                               fieldParser.FieldType,
                                               fieldParser.FieldInput);
      var actualErrorMessage = fieldParser.Errors.Single();
      Assert.AreEqual<string>(expectedErrorMessage, actualErrorMessage);
    }

    [TestMethod]
    public void ParseInt32OverflowTest()
    {
      // Arrange
      var propertyName = "Int32Property";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);

      // Deliberate concatenation, not an addition...
      var input = Int32.MaxValue.ToString() + "0";

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      var actual = fieldParser.Parse(input);

      // Assert
      Assert.IsNull(actual);

      var expectedErrorCount = 1;
      var actualErrorCount = fieldParser.Errors.Count();
      Assert.AreEqual<int>(expectedErrorCount, actualErrorCount);

      var expectedErrorMessage = String.Format("{0}: Unparsable {1} >>> {2}",
                                               fieldParser.FieldName,
                                               fieldParser.FieldType,
                                               fieldParser.FieldInput);
      var actualErrorMessage = fieldParser.Errors.Single();
      Assert.AreEqual<string>(expectedErrorMessage, actualErrorMessage);
    }

    [TestMethod]
    public void ParseNullableInt32Test()
    {
      // Arrange
      var propertyName = "NullableInt32Property";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      var actual = fieldParser.Parse(String.Empty);

      // Assert
      Assert.IsNull(actual);
      Assert.IsFalse(fieldParser.Errors.Any());
    }

    [TestMethod]
    public void ParseInt64Test()
    {
      // Arrange
      var propertyName = "Int64Property";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);
      var input = (Int64.MaxValue).ToString();

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      var actual = fieldParser.Parse(input);

      // Assert
      Assert.IsNotNull(actual);
      Assert.IsFalse(fieldParser.Errors.Any());
    }

    [TestMethod]
    public void ParseInt64UnparsableTest()
    {
      // Arrange
      var propertyName = "Int64Property";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      var actual = fieldParser.Parse("ABCD");

      // Assert
      Assert.IsNull(actual);

      var expectedErrorCount = 1;
      var actualErrorCount = fieldParser.Errors.Count();
      Assert.AreEqual<int>(expectedErrorCount, actualErrorCount);

      var expectedErrorMessage = String.Format("{0}: Unparsable {1} >>> {2}",
                                               fieldParser.FieldName,
                                               fieldParser.FieldType,
                                               fieldParser.FieldInput);
      var actualErrorMessage = fieldParser.Errors.Single();
      Assert.AreEqual<string>(expectedErrorMessage, actualErrorMessage);
    }

    [TestMethod]
    public void ParseInt64OverflowTest()
    {
      // Arrange
      var propertyName = "Int64Property";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);

      // Deliberate concatenation, not a addition...
      var input = Int64.MaxValue.ToString() + "0";

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      var actual = fieldParser.Parse(input);

      // Assert
      Assert.IsNull(actual);

      var expectedErrorCount = 1;
      var actualErrorCount = fieldParser.Errors.Count();
      Assert.AreEqual<int>(expectedErrorCount, actualErrorCount);

      var expectedErrorMessage = String.Format("{0}: Unparsable {1} >>> {2}",
                                               fieldParser.FieldName,
                                               fieldParser.FieldType,
                                               fieldParser.FieldInput);
      var actualErrorMessage = fieldParser.Errors.Single();
      Assert.AreEqual<string>(expectedErrorMessage, actualErrorMessage);
    }

    [TestMethod]
    public void ParseDecimalTest()
    {
      // Arrange
      var propertyName = "DecimalProperty";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      var actual = fieldParser.Parse("12.34");

      // Assert
      Assert.IsNotNull(actual);
      Assert.IsFalse(fieldParser.Errors.Any());
    }

    [TestMethod]
    public void ParseDecimalUnparsableTest()
    {
      // Arrange
      var propertyName = "DecimalProperty";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      var actual = fieldParser.Parse("ABCD");

      // Assert
      Assert.IsNull(actual);

      var expectedErrorCount = 1;
      var actualErrorCount = fieldParser.Errors.Count();
      Assert.AreEqual<int>(expectedErrorCount, actualErrorCount);

      var expectedErrorMessage = String.Format("{0}: Unparsable {1} >>> {2}",
                                               fieldParser.FieldName,
                                               fieldParser.FieldType,
                                               fieldParser.FieldInput);
      var actualErrorMessage = fieldParser.Errors.Single();
      Assert.AreEqual<string>(expectedErrorMessage, actualErrorMessage);
    }

    [TestMethod]
    public void ParseNullableDecimalTest()
    {
      // Arrange
      var propertyName = "NullableDecimalProperty";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      var actual = fieldParser.Parse("ABCD");

      // Assert
      Assert.IsNull(actual);
      Assert.IsFalse(fieldParser.Errors.Any());
    }

    [TestMethod]
    public void ParseSingleTest()
    {
      // Arrange
      var propertyName = "SingleProperty";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      var actual = fieldParser.Parse("9.87");

      // Assert
      Assert.IsNotNull(actual);
      Assert.IsFalse(fieldParser.Errors.Any());
    }

    [TestMethod]
    public void ParseSingleUnparsableTest()
    {
      // Arrange
      var propertyName = "SingleProperty";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      var actual = fieldParser.Parse("ABCD");

      // Assert
      Assert.IsNull(actual);

      var expectedErrorCount = 1;
      var actualErrorCount = fieldParser.Errors.Count();
      Assert.AreEqual<int>(expectedErrorCount, actualErrorCount);

      var expectedErrorMessage = String.Format("{0}: Unparsable System.Single >>> {1}", fieldParser.FieldName, fieldParser.FieldInput);
      var actualErrorMessage = fieldParser.Errors.Single();
      Assert.AreEqual<string>(expectedErrorMessage, actualErrorMessage);
    }

    [TestMethod]
    public void ParseNullableSingleTest()
    {
      // Arrange
      var propertyName = "NullableSingleProperty";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      var actual = fieldParser.Parse(String.Empty);

      // Assert
      Assert.IsNull(actual);
      Assert.IsFalse(fieldParser.Errors.Any());
    }

    [TestMethod]
    public void ParseDoubleTest()
    {
      // Arrange
      var propertyName = "DoubleProperty";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      var actual = fieldParser.Parse("87.65");

      // Assert
      Assert.IsNotNull(actual);
      Assert.IsFalse(fieldParser.Errors.Any());
    }

    [TestMethod]
    public void ParseDoubleUnparsableTest()
    {
      // Arrange
      var propertyName = "DoubleProperty";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      var actual = fieldParser.Parse("ABCD");

      // Assert
      Assert.IsNull(actual);

      var expectedErrorCount = 1;
      var actualErrorCount = fieldParser.Errors.Count();
      Assert.AreEqual<int>(expectedErrorCount, actualErrorCount);

      var expectedErrorMessage = String.Format("{0}: Unparsable {1} >>> {2}",
                                               fieldParser.FieldName,
                                               fieldParser.FieldType,
                                               fieldParser.FieldInput);
      var actualErrorMessage = fieldParser.Errors.Single();
      Assert.AreEqual<string>(expectedErrorMessage, actualErrorMessage);
    }

    [TestMethod]
    public void ParseNullableDoubleTest()
    {
      // Arrange
      var propertyName = "NullableDoubleProperty";
      var propertyInfo = typeof(Foo).GetProperties().Single(p => p.Name == propertyName);

      // Act
      var fieldParser = new FieldParser(propertyInfo);
      var actual = fieldParser.Parse(String.Empty);

      // Assert
      Assert.IsNull(actual);
      Assert.IsFalse(fieldParser.Errors.Any());
    }
  }
}
