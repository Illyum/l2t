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
    static IEnumerable<ShipmentNulled> _ordersWithNullInstances;
    static IEnumerable<Shipment> _ordersAsShipments;
    static IEnumerable<ShipmentSkipBlanks> _ordersAsShipments2;
    static IEnumerable<AssociativeArrayEntry> _map;

    [ClassInitialize]
    public static void InitializeClass(TestContext context)
    {
      var shipAddress = "Address A";
      var dateTime = new DateTime(2010, 10, 10);

      _orders = Enumerable.Range(1, 10)
                          .Select(counter => new Order()
                          {
                            OrderId = (short) counter,
                            Freight = 1.1M,
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

      var ordersWithBlankLinesFilePath = Path.Combine(context.DeploymentDirectory, "OrdersWithBlankLines.csv");
      using (var writer = new StreamWriter(ordersWithBlankLinesFilePath))
      {
        _ordersWithNullInstances = _orders.Aggregate(new List<ShipmentNulled>(), (whole, next) =>
         {
           writer.WriteLine("{0}, {1:#0.00}, {2}, {3:dd/MM/yyyy}",
               next.OrderId,
               next.Freight,
               next.ShipAddress,
               next.DeliveryDate);
           whole.Add(new ShipmentNulled { OrderId = next.OrderId, Freight = next.Freight, ShipAddress = next.ShipAddress, DeliveryDate = next.DeliveryDate });

           writer.WriteLine("");
           whole.Add(null);

           return whole;
         });
      }
      _ordersAsShipments = _ordersWithNullInstances.TakeWhile(order => order != null).Aggregate(new List<Shipment>(), (whole, next) =>
      {
        whole.Add(new Shipment { OrderId = next.OrderId, Freight = next.Freight, ShipAddress = next.ShipAddress, DeliveryDate = next.DeliveryDate });
        return whole;
      });

      _ordersAsShipments2 = _ordersWithNullInstances.Where(order => order != null).Aggregate(new List<ShipmentSkipBlanks>(), (whole, next) =>
      {
        whole.Add(new ShipmentSkipBlanks { OrderId = next.OrderId, Freight = next.Freight, ShipAddress = next.ShipAddress, DeliveryDate = next.DeliveryDate });
        return whole;
      });

      _map = Enumerable.Range(1, 5).Select(counter => new AssociativeArrayEntry() { Id = (short)counter, Name = $"Name_{counter}" });
      var mapFilePath = Path.Combine(context.DeploymentDirectory, "Map.csv");
      using (var writer = new StreamWriter(mapFilePath))
      {
        foreach (var pair in _map)
        {
          writer.WriteLine("{0}, {1}", pair.Id, pair.Name);
        }
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

    [TestMethod]
    public void DefaultParseFileWithBlankLinesTest()
    {
      // Arrange
      var ordersFilePath = Path.Combine(TestContext.DeploymentDirectory, "OrdersWithBlankLines.csv");
      using (var reader = new StreamReader(ordersFilePath))
      {
        var fileParser = new DelimiterSeparatedValuesFileParser<Shipment>();

        // Act
        var parseResults = fileParser.Read(reader, delimiter: ',', includeHeaders: false);

        // Assert
        Assert.IsTrue(_ordersAsShipments.SequenceEqual(parseResults.Select(parseResult => parseResult.Instance)));
      }
    }

    [TestMethod]
    public void NulledParseFileWithBlankLinesTest()
    {
      // Arrange
      var ordersFilePath = Path.Combine(TestContext.DeploymentDirectory, "OrdersWithBlankLines.csv");
      using (var reader = new StreamReader(ordersFilePath))
      {
        var fileParser = new DelimiterSeparatedValuesFileParser<ShipmentNulled>();

        // Act
        var parseResults = fileParser.Read(reader, delimiter: ',', includeHeaders: false);

        // Assert
        Assert.IsTrue(_ordersWithNullInstances.SequenceEqual(parseResults.Select(parseResult => parseResult.Instance)));
      }
    }

    [TestMethod]
    public void SkipParseFileWithBlankLinesTest()
    {
      // Arrange
      var ordersFilePath = Path.Combine(TestContext.DeploymentDirectory, "OrdersWithBlankLines.csv");
      using (var reader = new StreamReader(ordersFilePath))
      {
        var fileParser = new DelimiterSeparatedValuesFileParser<ShipmentSkipBlanks>();

        // Act
        var parseResults = fileParser.Read(reader, delimiter: ',', includeHeaders: false);

        // Assert
        Assert.IsTrue(_ordersAsShipments2.SequenceEqual(parseResults.Select(parseResult => parseResult.Instance)));
      }
    }

    [TestMethod]
    public void ParseMapFileTest()
    {
      // Arrange
      var mapFilePath = Path.Combine(TestContext.DeploymentDirectory, "Map.csv");
      using (var reader = new StreamReader(mapFilePath))
      {
        var fileParser = new DelimiterSeparatedValuesFileParser<AssociativeArrayEntry>();

        // Act
        var parseResults = fileParser.Read(reader, delimiter: ',', includeHeaders: false);
        var dictionary = parseResults.Aggregate(new Dictionary<short, string>(), (whole, next) => { whole[next.Instance.Id] = next.Instance.Name; return whole; });

        // Assert
        Assert.AreEqual("(1,Name_1)(2,Name_2)(3,Name_3)(4,Name_4)(5,Name_5)", $"{dictionary.Aggregate(new System.Text.StringBuilder(),(w,n)=>w.AppendFormat("({0},{1})",n.Key,n.Value))}");
      }
    }
  }
}