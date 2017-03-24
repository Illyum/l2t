using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using IllyumL2T.Core.Parse;
using System.IO;

namespace IllyumL2T.Core.FieldsSplit.UnitTests
{
  [TestClass]
  public class LineWriteTests
  {
    [TestMethod]
    public void LineWriteTest()
    {
      // Arrange
      short orderId = 321;
      decimal charge = 12.5M;
      string address = "Address 1";
      var when = new DateTime(2015, 11, 28);

      var lineParser = new LineParser<Shipment>();
      var instance = new Shipment() { OrderId = orderId, Freight = charge, ShipAddress = address, DeliveryDate = when };

      var writer = new StringWriter();

      // Act
      lineParser.Write(writer, instance);

      // Assert
      Assert.AreEqual<string>($"{orderId},{charge},{address},{when.ToString("d/M/yyyy")}", $"{writer.GetStringBuilder()}");
    }
  }
}