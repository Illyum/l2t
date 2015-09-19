using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using IllyumL2T.Core.Parse;

namespace IllyumL2T.Core.FieldsSplit.UnitTests
{
  [TestClass]
  public class EmptyLinesIssueWithFileParser
  {
    const int HowManyOrders = 10;
    const string OrdersFile = "OrdersWithEmptyLine.csv";

    static IEnumerable<Order> _orders;

    [ClassInitialize]
    public static void InitializeClass(TestContext context)
    {
      var shipAddress = "Address A";
      var dateTime = new DateTime(2010, 10, 10);

      _orders = Enumerable.Range(1, HowManyOrders)
                          .Select(counter => new Order()
                          {
                            OrderId = (short)counter,
                            Freight = 1.1m,
                            ShipAddress = shipAddress,
                            DeliveryDate = dateTime
                          });

      var writeOrder = new Action<Order, TextWriter>((order,writer)=>
      {
        writer.WriteLine("{0}, {1:#0.00}, {2}, {3:dd/MM/yyyy}",
                         order.OrderId,
                         order.Freight,
                         order.ShipAddress,
                         order.DeliveryDate);
      });

      var ordersFilePath = Path.Combine(context.DeploymentDirectory, OrdersFile);
      using (var writer = new StreamWriter(ordersFilePath))
      {
        foreach (var order in _orders.TakeWhile(order => order.OrderId <= HowManyOrders / 2))
        {
          writeOrder(order, writer);
        }
        writer.WriteLine(); //Empty line inserted in the middle of the order lines.
        foreach (var order in _orders.TakeWhile(order => order.OrderId > HowManyOrders / 2))
        {
          writeOrder(order, writer);
        }
      }
    }

    public TestContext TestContext { get; set; }

    [TestMethod]
    public void ParseFileWithEmptyLineTest()
    {
      // Arrange
      var ordersFilePath = Path.Combine(TestContext.DeploymentDirectory, OrdersFile);
      using (var reader = new StreamReader(ordersFilePath))
      {
        var fileParser = new DelimiterSeparatedValuesFileParser<Order>();

        // Act
        var parseResults = fileParser.Read(reader, delimiter: ',', includeHeaders: false);

        // Assert
        Assert.IsTrue(_orders.SequenceEqual(parseResults.Select(parseResult => parseResult.Instance)));
      }
    }
  }
}