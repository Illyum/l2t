using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using IllyumL2T.Core.FieldsSplit;
using IllyumL2T.Core.Parse;

namespace IllyumL2T.Core.FieldsSplit.UnitTests
{
  [TestClass]
  public class DelimiterSeparatedValuesFileParserTests
  {
    static IEnumerable<Order> _orders;

    [ClassInitialize]
    public static void InitializeClass(TestContext context)
    {
      var shipAddress = "Address A";
      var dateTime = new DateTime(2010, 10, 10);

      _orders = Enumerable.Range(1, 10)
                          .Select(counter => new Order()
                          {
                            OrderId = (short) counter,
                            Freight = 1.1m,
                            ShipAddress = shipAddress,
                            DeliveryDate = dateTime
                          });

      var ordersFilePath = Path.Combine(context.DeploymentDirectory, "Orders.csv");
      using(var writer = new StreamWriter(ordersFilePath))
      {
        foreach(var order in _orders)
        {
          writer.WriteLine("{0}, {1:#0.00}, {2}, {3:dd/MM/yyyy}",
                           order.OrderId,
                           order.Freight,
                           order.ShipAddress,
                           order.DeliveryDate);
        }
      }

      var ordersWithErrorsFilePath = Path.Combine(context.DeploymentDirectory, "OrdersWithErrors.csv");
      using(var writer = new StreamWriter(ordersWithErrorsFilePath))
      {
        writer.WriteLine("A100A, ____, ShipNowhere, 99/99/9999");  // 3 parsing errors
        writer.WriteLine("10000, (89), ShipNowhere, InvalidDate"); // 2 parsing errors
        writer.WriteLine("A100A, ####, ShipNowhere, 99/99/9999");  // 3 parsing errors
      }
    }

    public TestContext TestContext { get; set; }

    [TestMethod]
    public void ParseFileTest()
    {
      // Arrange
      var ordersFilePath = Path.Combine(TestContext.DeploymentDirectory, "Orders.csv");
      using(var reader = new StreamReader(ordersFilePath))
      {
        var fileParser = new DelimiterSeparatedValuesFileParser<Order>();

        // Act
        var parseResults = fileParser.Read(reader, delimiter: ',', includeHeaders: false);

        // Assert
        Assert.IsTrue(_orders.SequenceEqual(parseResults.Select(parseResult => parseResult.Instance)));
      }
    }

    [TestMethod]
    public void ParseFileWithErrorsTest()
    {
      // Arrange
      var ordersWithErrorsFilePath = Path.Combine(TestContext.DeploymentDirectory, "OrdersWithErrors.csv");
      using(var reader = new StreamReader(ordersWithErrorsFilePath))
      {
        var fileParser = new DelimiterSeparatedValuesFileParser<Order>();

        // Act
        var parseResults = fileParser.Read(reader, delimiter: ',', includeHeaders: false);

        // Assert
        var actualLinesWithError = default(int);
        var actualFieldsWithError = default(int);

        foreach(var parseResult in parseResults)
        {
          Assert.IsNull(parseResult.Instance);

          if(parseResult.Errors.Any())
          {
            actualLinesWithError++;
            actualFieldsWithError += parseResult.Errors.Count();
          }
        }

        // In the OrdersWithErrors.csv file created when the test class is initialized
        // There are three lines with errors so the logger must be called three times
        var expectedLinesWithErrors = 3;
        Assert.AreEqual<int>(expectedLinesWithErrors, actualLinesWithError);

        // And there must be eight errors in the fields being parsed
        var expectedErrorsInFields = 8;
        Assert.AreEqual<int>(expectedErrorsInFields, actualFieldsWithError);
      }
    }

    //TODO:Temporally here. Once evaluated, then they could move at a proper location.

    [TestMethod]
    public void MemoryStreamAsDelimitedTextTest()
    {
      // Arrange
      var text_lines = new System.IO.MemoryStream(new byte[]
      {
        0x31, 0x09, 0x32, 0x09, 0x41, 0x09, 0x33, 0x31, 0x2F, 0x31, 0x32, 0x2F, 0x32, 0x30, 0x31, 0x35, 0x0D, 0x0A,
        0x33, 0x09, 0x34, 0x09, 0x42, 0x09, 0x30, 0x33, 0x2F, 0x30, 0x32, 0x2F, 0x32, 0x30, 0x31, 0x36, 0x0D, 0x0A
      });

      IEnumerable<Order> orders = new List<Order>
      {
        new Order { OrderId = 1, Freight = 2M, ShipAddress = "A", DeliveryDate = new DateTime(2015, 12, 31) },
        new Order { OrderId = 3, Freight = 4M, ShipAddress = "B", DeliveryDate = new DateTime(2016, 2, 3) }
      };

      using (var reader = new System.IO.StreamReader(text_lines))
      {
        var parser = new DelimiterSeparatedValuesFileParser<Order>();

        // Act
        var parseResults = parser.Read(reader, delimiter: '\t', includeHeaders: false);

        // Assert
        Assert.IsTrue(orders.SequenceEqual(parseResults.Select(parseResult => parseResult.Instance)));
      }
    }

    [TestMethod]
    public void MemoryStreamAsPositionalTextTest()
    {
      // Arrange
      var text_lines = new System.IO.MemoryStream(new byte[]
      {
        0x20, 0x20, 0x20, 0x20, 0x31,
        0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x32,
        0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x41,
        0x33, 0x31, 0x2F, 0x31, 0x32, 0x2F, 0x32, 0x30, 0x31, 0x35,
        0x0D, 0x0A,
        0x20, 0x20, 0x20, 0x20, 0x33,
        0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x34,
        0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x42,
        0x30, 0x33, 0x2F, 0x30, 0x32, 0x2F, 0x32, 0x30, 0x31, 0x36,
        0x0D, 0x0A
      });

      IEnumerable<SimpleOrder> orders = new List<SimpleOrder>
      {
        new SimpleOrder { OrderId = 1, Freight = 2M, ShipAddress = "A", DeliveryDate = new DateTime(2015, 12, 31) },
        new SimpleOrder { OrderId = 3, Freight = 4M, ShipAddress = "B", DeliveryDate = new DateTime(2016, 2, 3) }
      };

      using (var reader = new System.IO.StreamReader(text_lines))
      {
        var parser = new PositionalValuesFileParser<SimpleOrder>();

        // Act
        var parseResults = parser.Read(reader, includeHeaders: false);

        // Assert
        Assert.IsTrue(orders.SequenceEqual(parseResults.Select(parseResult => parseResult.Instance)));
      }
    }

    /// <summary>
    /// Positional packets & Positional messages & Positional values; that is, no separators.
    /// Simplest case where byte[] (message) is interpreted as a text line.
    /// </summary>
    [TestMethod]
    public void MemoryStreamAsPositionalBinaryTest()
    {
      // Arrange
      var frame = new System.IO.MemoryStream(new byte[]
      {
        0x20, 0x20, 0x20, 0x20, 0x31,
        0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x32,
        0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x41,
        0x33, 0x31, 0x2F, 0x31, 0x32, 0x2F, 0x32, 0x30, 0x31, 0x35,
        0x20, 0x20, 0x20, 0x20, 0x33,
        0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x34,
        0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x42,
        0x30, 0x33, 0x2F, 0x30, 0x32, 0x2F, 0x32, 0x30, 0x31, 0x36,
      });

      IEnumerable<SimpleOrder> orders = new List<SimpleOrder>
      {
        new SimpleOrder { OrderId = 1, Freight = 2M, ShipAddress = "A", DeliveryDate = new DateTime(2015, 12, 31) },
        new SimpleOrder { OrderId = 3, Freight = 4M, ShipAddress = "B", DeliveryDate = new DateTime(2016, 2, 3) }
      };

      using (var reader = new System.IO.BinaryReader(frame))
      {
        var parser = new PositionalValuesFileParser<SimpleOrder>();

        // Act
        var parseResults = parser.Read(reader);

        // Assert
        Assert.IsTrue(orders.SequenceEqual(parseResults.Select(parseResult => parseResult.Instance)));
      }
    }

    /// <summary>
    /// Delimited packets & Delimited messages && Delimited values; that is, packets, messages and values have separators.
    /// </summary>
    [TestMethod, Ignore]
    public void MemoryStreamAsDelimitedBinaryTest()
    {
      // Arrange
      IEnumerable<Order> orders = new List<Order>
      {
        new Order { OrderId = 1, Freight = 2M, ShipAddress = "A", DeliveryDate = new DateTime(2015, 12, 31) },
        new Order { OrderId = 3, Freight = 4M, ShipAddress = "B", DeliveryDate = new DateTime(2016, 2, 3) }
      };

      var frame = new System.IO.MemoryStream(new byte[]
      {
        0x31, 0x09, 0x32, 0x09, 0x41, 0x09, 0x33, 0x31, 0x2F, 0x31, 0x32, 0x2F, 0x32, 0x30, 0x31, 0x35, 0x1E,
        0x33, 0x09, 0x34, 0x09, 0x42, 0x09, 0x30, 0x33, 0x2F, 0x30, 0x32, 0x2F, 0x32, 0x30, 0x31, 0x36, 0x1E,
        0x1D
      });

      using (var reader = new System.IO.BinaryReader(frame))
      {
        var parser = new DelimiterSeparatedValuesFileParser<Order>();

        // Act
        var parseResults = parser.Read(reader, group_separator: 0x1D, record_separator: 0x1E, unit_separator: 0x09);

        // Assert
        Assert.IsTrue(orders.SequenceEqual(parseResults.Select(parseResult => parseResult.Instance)));
      }
    }

    [TestMethod]
    public void Bytes_vs_Chars_DifferenceAwareness_SampleCase1()
    {
      // Arrange
      string lastname = "López";
      System.Text.Encoding latin1 = System.Text.Encoding.GetEncoding("ISO-8859-1");
      System.Text.Encoding utf8 = System.Text.Encoding.UTF8;

      // Act
      var latin1_bytes = latin1.GetBytes(lastname);
      var utf8_bytes = utf8.GetBytes(lastname);

      // Assert
      Assert.AreEqual<int>(5, latin1_bytes.Length);
      Assert.AreEqual<int>(6, utf8_bytes.Length);
    }
  }
}