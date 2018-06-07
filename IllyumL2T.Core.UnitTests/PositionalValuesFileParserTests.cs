using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using IllyumL2T.Core.Parse;

namespace IllyumL2T.Core.FieldsSplit.UnitTests
{
  [TestClass]
  public class PositionalValuesFileParserTests
  {
    static IEnumerable<SimpleOrder> _orders;

    [ClassInitialize]
    public static void InitializeClass(TestContext context)
    {
      var shipAddress = "Address A";
      var dateTime = new DateTime(2010, 10, 10);

      _orders = Enumerable.Range(1, 10)
                          .Select(counter => new SimpleOrder()
                          {
                            OrderId = (short)counter,
                            Freight = 1.1m,
                            ShipAddress = shipAddress,
                            DeliveryDate = dateTime
                          });

      var ordersFilePath = Path.Combine(context.DeploymentDirectory, "FixedWidthOrders.csv");
      using (var writer = new StreamWriter(ordersFilePath))
      {
        foreach (var order in _orders)
        {
          //The abstract positional idea of an order line based on the SimpleOrder class layout:
          //[  123][   123.45][A ship address.                                   ][25/12/2016]
          writer.WriteLine("{0,5}{1,9:#0.00}{2,-50}{3,10:dd/MM/yyyy}",
                           order.OrderId,
                           order.Freight,
                           order.ShipAddress,
                           order.DeliveryDate);
        }
      }

      var ordersWithErrorsFilePath = Path.Combine(context.DeploymentDirectory, "FixedWidthOrdersWithErrors.csv");
      using (var writer = new StreamWriter(ordersWithErrorsFilePath))
      {
        writer.WriteLine("{0,5}{1,9:#0.00}{2,-50}{3,10:dd/MM/yyyy}", "A100A", "____", "ShipNowhere", "99/99/9999");  // 3 parsing errors
        writer.WriteLine("{0,5}{1,9:#0.00}{2,-50}{3,10:dd/MM/yyyy}", "10000", "(89)", "ShipNowhere", "InvalidDate"); // 2 parsing errors
        writer.WriteLine("{0,5}{1,9:#0.00}{2,-50}{3,10:dd/MM/yyyy}", "A100A", "####", "ShipNowhere", "99/99/9999");  // 3 parsing errors
      }
    }

    public TestContext TestContext { get; set; }

    [TestMethod]
    public void ParseFileTest()
    {
      // Arrange
      var ordersFilePath = Path.Combine(TestContext.DeploymentDirectory, "FixedWidthOrders.csv");
      using (var reader = new StreamReader(ordersFilePath))
      {
        var fileParser = new PositionalValuesFileParser<SimpleOrder>();

        // Act
        var parseResults = fileParser.Read(reader, includeHeaders: false);

        // Assert
        Assert.IsTrue(_orders.SequenceEqual(parseResults.Select(parseResult => parseResult.Instance)));
      }
    }

    [TestMethod]
    public void ParseFileWithErrorsTest()
    {
      // Arrange
      var ordersWithErrorsFilePath = Path.Combine(TestContext.DeploymentDirectory, "FixedWidthOrdersWithErrors.csv");
      using (var reader = new StreamReader(ordersWithErrorsFilePath))
      {
        var fileParser = new PositionalValuesFileParser<SimpleOrder>();

        // Act
        var parseResults = fileParser.Read(reader, includeHeaders: false);

        // Assert
        var actualLinesWithError = default(int);
        var actualFieldsWithError = default(int);

        foreach (var parseResult in parseResults)
        {
          Assert.IsNull(parseResult.Instance);

          if (parseResult.Errors.Any())
          {
            actualLinesWithError++;
            actualFieldsWithError += parseResult.Errors.Count();
          }
        }

        // In the FixedWidthOrdersWithErrors.csv file created when the test class is initialized
        // There are three lines with errors so the logger must be called three times
        var expectedLinesWithErrors = 3;
        Assert.AreEqual<int>(expectedLinesWithErrors, actualLinesWithError);

        // And there must be eight errors in the fields being parsed
        var expectedErrorsInFields = 8;
        Assert.AreEqual<int>(expectedErrorsInFields, actualFieldsWithError);
      }
    }
  }
}