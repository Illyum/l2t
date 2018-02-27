using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using IllyumL2T.Core.FieldsSplit;
using IllyumL2T.Core.Parse;

namespace IllyumL2T.Core.FieldsSplit.UnitTests
{
  [TestClass]
  public class LineParserTests
  {
    [TestMethod]
    public void LineParserCreateTest()
    {
      // Arrange
      var properties = typeof(Foo).GetProperties();
      var expected = properties.Count();

      // Act
      var lineParser = new LineParser<Foo>();
      var actual = lineParser.FieldParsers.Count();

      // Assert
      // There has to be as many FieldParsers as Properties in the target type...
      Assert.AreEqual<int>(expected, actual);

      // ...and there has to be one single field for every property in ClassForTesting...
      foreach (var field in lineParser.FieldParsers)
      {
        Assert.IsNotNull(properties.Single(p => p.Name.Equals(field.FieldName)));
      }
    }

    [TestMethod]
    public void LineParseTest()
    {
      // Arrange
      var line = @"Jane Doe,jane@domain.com,F,,128,,1234,,98765,,1234567890,,1.2,,3.4,,3.1416,,25/12/2007,";

      // Act
      var lineParser = new LineParser<Foo>();
      var parseResult = lineParser.Parse(line);

      // Assert
      Assert.IsNull(parseResult.Errors);

      var actual = parseResult.Instance;
      var expected = new Foo()
      {
        StringProperty = "Jane Doe",
        StringPropertyWithPattern = "jane@domain.com",
        CharProperty = 'F',
        NullableCharProperty = null,
        ByteProperty = 128,
        NullableByteProperty = null,
        Int16Property = 1234,
        NullableInt16Property = null,
        Int32Property = 98765,
        NullableInt32Property = null,
        Int64Property = 1234567890,
        NullableInt64Property = null,
        SingleProperty = 1.2F,
        NullableSingleProperty = null,
        DoubleProperty = 3.4,
        NullableDoubleProperty = null,
        DecimalProperty = 3.1416m,
        NullableDecimalProperty = null,
        DateTimeProperty = new DateTime(2007, 12, 25),
        NullableDateTimeProperty = null,
      };

      Assert.AreEqual<Foo>(expected, actual);
    }

    [TestMethod]
    public void LineParsePositionalTest()
    {
      // Arrange
      string address = "jane@domain.com";
      var line = $" 1234   3.1416{address,-50}25/12/2007";

      // Act
      var lineParser = new LineParser<SimpleOrder>(new PositionalValuesFieldsSplitter<SimpleOrder>());
      var parseResult = lineParser.Parse(line);

      // Assert
      Assert.IsNull(parseResult.Errors);
      //Assert.IsNull(parseResult.Errors, parseResult.Errors.Aggregate(new System.Text.StringBuilder(), (w, n) => w.AppendFormat("{0} |", n)).ToString());

        var actual = parseResult.Instance;
      Assert.IsNotNull(actual);
      var expected = new SimpleOrder()
      {
        OrderId = 1234,
        Freight = 3.1416m,
        ShipAddress = address,
        DeliveryDate = new DateTime(2007, 12, 25),
      };

      Assert.AreEqual<SimpleOrder>(expected, actual);
    }

    class B { public int A { get; set; } public string b { get; set; } }
    class A : B { public int C { get; set; } public int D { get; set; } }
    [TestMethod, Description("Case experiment")]
    public void layout_order_of_inherited_properties()
    {
      // Arrange
      var typeProperties = typeof(A).GetProperties();

      // Act
      var list = $"{typeProperties.Aggregate(new System.Text.StringBuilder(),(w,n)=>w.AppendFormat("{0}|",n.Name))}";

      // Assert
      Assert.AreEqual<string>("C|D|A|b|", list);
    }

    [TestMethod]
    public void LineParsePositionalTest_Fragment1()
    {
      // Arrange
      string address = "jane@domain.com";
      var line = $" 1234   3.1416{address,-50}25/12/2007";
      var expected = new MessageXHead()
      {
        OrderId = 1234,
        Freight = 3.1416m
        //ShipAddress = address,
        //DeliveryDate = new DateTime(2007, 12, 25)
      };

      // Act
      var lineParser = new LineParser<MessageX>(new PositionalValuesFieldsSplitter<MessageX>());
      var parseResult = lineParser.Parse(line);

      // Assert
      //Assert.IsNull(parseResult.Errors);
      Assert.IsNull(parseResult.Errors, $"{parseResult.Errors.Aggregate(new System.Text.StringBuilder(), (w, n) => w.AppendFormat("{0} |", n))}");
      var actual = parseResult.Instance;
      Assert.IsNotNull(actual);
      Assert.AreEqual<MessageX>(expected, actual);
    }

    [TestMethod]
    public void ShortLineParsePositionalTest()
    {
      // Arrange
      string address = "jane@domain.com";
      var line = $" 1234   3.1416{address,-30}";//input line length is less than declared total length.

      // Act
      var lineParser = new LineParser<SimpleOrder>(new PositionalValuesFieldsSplitter<SimpleOrder>());
      var parseResult = lineParser.Parse(line);

      // Assert
      Assert.IsNull(parseResult.Errors);

      var actual = parseResult.Instance;
      Assert.IsNotNull(actual);
      var expected = new SimpleOrder()
      {
        OrderId = 1234,
        Freight = 3.1416m,
        ShipAddress = address,
        DeliveryDate = DateTime.MinValue,
      };

      Assert.AreEqual<SimpleOrder>(expected, actual);
    }

    [TestMethod]
    public void EmptyLineParsePositionalTest()
    {
      // Arrange
      var line = "";

      // Act
      var lineParser = new LineParser<SimpleOrder>(new PositionalValuesFieldsSplitter<SimpleOrder>());
      var parseResult = lineParser.Parse(line);

      // Assert
      Assert.IsNull(parseResult.Errors);

      var actual = parseResult.Instance;
      Assert.IsNotNull(actual);
      var expected = new SimpleOrder()
      {
        OrderId = 0,
        Freight = 0m,
        ShipAddress = null,
        DeliveryDate = DateTime.MinValue,
      };

      Assert.AreEqual<SimpleOrder>(expected, actual);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void NullLineParsePositionalTest()
    {
      // Arrange
      string line = null;

      // Act
      var lineParser = new LineParser<SimpleOrder>(new PositionalValuesFieldsSplitter<SimpleOrder>());
      var parseResult = lineParser.Parse(line);

      // Assert
      Assert.Fail("Null input line must throw an exception.");
    }

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void NegativeLengthFieldParsePositionalTest()
    {
      // Arrange
      var line = $" 1234ABCDE";//input line length is invalid field length.

      // Act
      var lineParser = new LineParser<BadRecord>(new PositionalValuesFieldsSplitter<BadRecord>());
      var parseResult = lineParser.Parse(line);

      // Assert
      Assert.Fail("Positional field with non-positive length must throw an exception.");
    }

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void ZeroLengthFieldParsePositionalTest()
    {
      // Arrange
      var line = $" 1234ABCDE";//input line length is invalid field length.

      // Act
      var lineParser = new LineParser<BadRecord2>(new PositionalValuesFieldsSplitter<BadRecord2>());
      var parseResult = lineParser.Parse(line);

      // Assert
      Assert.Fail("Positional field with non-positive length must throw an exception.");
    }

    [TestMethod]
    public void LineParseWithErrorsTest()
    {
      // Arrange
      var line = @"Jane Doe,jane@domain.com,F,,128,,ABCD,,98765,,1234567890,,*,,3.4,,3.1416,,99/99/9999,";

      // Act
      var lineParser = new LineParser<Foo>();
      var parseResult = lineParser.Parse(line);

      // Assert
      Assert.IsNotNull(parseResult.Errors);

      var expectedErrorCount = 3;
      var actualErrorCount = parseResult.Errors.Count();
      Assert.AreEqual<int>(expectedErrorCount, actualErrorCount);

      var expectedErrorMessages = new string[]
      {
        "Int16Property: Unparsable System.Int16 >>> ABCD",
        "SingleProperty: Unparsable System.Single >>> *",
        "DateTimeProperty: Unparsable System.DateTime >>> 99/99/9999"
      };
      var actualErrorMessages = parseResult.Errors;
      Assert.IsTrue(expectedErrorMessages.SequenceEqual(actualErrorMessages));
    }

    [TestMethod]
    public void LineParseNullsTest()
    {
      // Arrange
      var line = @"Jane Doe,jane@domain.com,F,,128,,1234,,98765,,1234567890,,1.2,,3.4,,3.1416,,27/10/2007,";

      // Act
      var lineParser = new LineParser<Foo>();
      var parseResult = lineParser.Parse(line);

      // Assert
      Assert.IsNotNull(parseResult);

      Assert.IsNull(parseResult.Errors);

      var actual = parseResult.Instance;
      var expected = new Foo()
      {
        StringProperty = "Jane Doe",
        StringPropertyWithPattern = "jane@domain.com",
        CharProperty = 'F',
        NullableCharProperty = null,
        ByteProperty = 128,
        NullableByteProperty = null,
        Int16Property = 1234,
        NullableInt16Property = null,
        Int32Property = 98765,
        NullableInt32Property = null,
        Int64Property = 1234567890,
        NullableInt64Property = null,
        SingleProperty = 1.2F,
        NullableSingleProperty = null,
        DoubleProperty = 3.4,
        NullableDoubleProperty = null,
        DecimalProperty = 3.1416m,
        NullableDecimalProperty = null,
        DateTimeProperty = new DateTime(2007, 10, 27),
        NullableDateTimeProperty = null,
      };

      Assert.AreEqual<Foo>(expected, actual);
    }

    [TestMethod]
    public void LineParseWithDelimiterWithinTheFieldTest()
    {
      // Arrange
      var line = "\"Doe, Jane\",jane@domain.com,F,,128,,1234,,98765,,1234567890,,1.2,,3.4,,3.1416,,25/12/2007,";

      // Act
      var lineParser = new LineParser<Foo>();
      var parseResult = lineParser.Parse(line);

      // Assert
      Assert.IsNull(parseResult.Errors);

      var actual = parseResult.Instance;
      var expected = new Foo()
      {
        StringProperty = "Doe, Jane",
        StringPropertyWithPattern = "jane@domain.com",
        CharProperty = 'F',
        NullableCharProperty = null,
        ByteProperty = 128,
        NullableByteProperty = null,
        Int16Property = 1234,
        NullableInt16Property = null,
        Int32Property = 98765,
        NullableInt32Property = null,
        Int64Property = 1234567890,
        NullableInt64Property = null,
        SingleProperty = 1.2F,
        NullableSingleProperty = null,
        DoubleProperty = 3.4,
        NullableDoubleProperty = null,
        DecimalProperty = 3.1416m,
        NullableDecimalProperty = null,
        DateTimeProperty = new DateTime(2007, 12, 25),
        NullableDateTimeProperty = null,
      };

      Assert.AreEqual<Foo>(expected, actual);
    }

    [TestMethod]
    public void LineParseUsingPipeAsDelimiterTest()
    {
      // Arrange
      var line = "Jane Doe|jane@domain.com|F||128||1234||98765||1234567890||1.2||3.4||3.1416||25/12/2007|";

      // Act
      var fieldsSplitter = new DelimiterSeparatedValuesFieldsSplitter<Foo>(delimiter: '|');
      var lineParser = new LineParser<Foo>(fieldsSplitter);
      var parseResult = lineParser.Parse(line);

      // Assert
      Assert.IsNull(parseResult.Errors);

      var actual = parseResult.Instance;
      var expected = new Foo()
      {
        StringProperty = "Jane Doe",
        StringPropertyWithPattern = "jane@domain.com",
        CharProperty = 'F',
        NullableCharProperty = null,
        ByteProperty = 128,
        NullableByteProperty = null,
        Int16Property = 1234,
        NullableInt16Property = null,
        Int32Property = 98765,
        NullableInt32Property = null,
        Int64Property = 1234567890,
        NullableInt64Property = null,
        SingleProperty = 1.2F,
        NullableSingleProperty = null,
        DoubleProperty = 3.4,
        NullableDoubleProperty = null,
        DecimalProperty = 3.1416m,
        NullableDecimalProperty = null,
        DateTimeProperty = new DateTime(2007, 12, 25),
        NullableDateTimeProperty = null,
      };

      Assert.AreEqual<Foo>(expected, actual);
    }

    [TestMethod]
    public void LineParseUsingTabsAsDelimiterTest()
    {
      // Arrange
      var line = "Jane Doe\tjane@domain.com\tF\t\t128\t\t1234\t\t98765\t\t1234567890\t\t1.2\t\t3.4\t\t3.1416\t\t25/12/2007\t";

      // Act
      var fieldsSplitter = new DelimiterSeparatedValuesFieldsSplitter<Foo>(delimiter: '\t');
      var lineParser = new LineParser<Foo>(fieldsSplitter);
      var parseResult = lineParser.Parse(line);

      // Assert
      Assert.IsNull(parseResult.Errors);

      var actual = parseResult.Instance;
      var expected = new Foo()
      {
        StringProperty = "Jane Doe",
        StringPropertyWithPattern = "jane@domain.com",
        CharProperty = 'F',
        NullableCharProperty = null,
        ByteProperty = 128,
        NullableByteProperty = null,
        Int16Property = 1234,
        NullableInt16Property = null,
        Int32Property = 98765,
        NullableInt32Property = null,
        Int64Property = 1234567890,
        NullableInt64Property = null,
        SingleProperty = 1.2F,
        NullableSingleProperty = null,
        DoubleProperty = 3.4,
        NullableDoubleProperty = null,
        DecimalProperty = 3.1416m,
        NullableDecimalProperty = null,
        DateTimeProperty = new DateTime(2007, 12, 25),
        NullableDateTimeProperty = null,
      };

      Assert.AreEqual<Foo>(expected, actual);
    }
  }
}
