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
        //var results = new List<ParseResult<SimpleOrder>>(parseResults);
        Assert.IsTrue(_orders.SequenceEqual(parseResults.Select(parseResult => parseResult.Instance)));

        //int k = 0;
        //foreach (var r in parseResults)
        //{
        //  Assert.IsNotNull(_orders.ElementAt(k));
        //  Assert.IsNotNull(r.Instance);
        //  Assert.AreEqual<short>(_orders.ElementAt(k).OrderId, r.Instance.OrderId);
        //  Assert.AreEqual<decimal>(_orders.ElementAt(k).Freight, r.Instance.Freight);
        //  Assert.AreEqual<DateTime>(_orders.ElementAt(k).DeliveryDate, r.Instance.DeliveryDate);
        //  Assert.AreEqual<string>(_orders.ElementAt(k).ShipAddress, r.Instance.ShipAddress);
        //  ++k;
        //}

        //System.Diagnostics.Trace.WriteLine("All results: " + results.Count());
        //System.Diagnostics.Trace.WriteLine("Instances: " + results.Count(r => r.Instance != null));
        //System.Diagnostics.Trace.WriteLine("With errors: " + results.Sum(r => r.Errors?.Count()));

        //System.Diagnostics.Trace.WriteLine(results.Aggregate(new System.Text.StringBuilder(), (whole, next) => whole.Append($"\nOrderId: {next.Instance.OrderId} | Errors: {(next.Errors != null ? next.Errors.Count().ToString() : "-")}")).ToString());
        //        Assert.IsTrue(false,results.Aggregate(new System.Text.StringBuilder(), (whole, next) => whole.AppendFormat("[{0}]", next.Errors.Aggregate(new System.Text.StringBuilder(), (whole2, next2) => whole2.AppendFormat(">{0}<", next2)).ToString())).ToString());
        //Assert.IsTrue(false);
      }
    }
  }
}