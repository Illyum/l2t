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

    /// <summary>
    /// This can also be seen as Delimited packets (implicitly) & Delimited messages & Positional values.
    /// </summary>
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
    [TestMethod]
    public void MemoryStreamAsPositionalBinarySocketsTest()
    {
      IEnumerable<ParseResult<SimpleOrder>> parseResults = null;

      var frames = new List<byte[]>
      {
        new byte[] { 0x20, 0x20, 0x20, 0x20, 0x31 },
        new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x32 },
        new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x41 },
        new byte[] { 0x33, 0x31, 0x2F, 0x31, 0x32, 0x2F, 0x32, 0x30, 0x31, 0x35 },
        new byte[] { 0x20, 0x20, 0x20, 0x20, 0x33 },
        new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x34 },
        new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x42 },
        new byte[] { 0x30, 0x33, 0x2F, 0x30, 0x32, 0x2F, 0x32, 0x30, 0x31, 0x36 }
      };

      IEnumerable<SimpleOrder> orders = new List<SimpleOrder>
      {
        new SimpleOrder { OrderId = 1, Freight = 2M, ShipAddress = "A", DeliveryDate = new DateTime(2015, 12, 31) },
        new SimpleOrder { OrderId = 3, Freight = 4M, ShipAddress = "B", DeliveryDate = new DateTime(2016, 2, 3) }
      };

      var sync = new System.Threading.ManualResetEvent(false);
      int port = 13001;
      System.Net.Sockets.TcpListener server = null;
      try
      {
        var listen = new Action(() =>
        {
          server = System.Net.Sockets.TcpListener.Create(port);
          server.Start();
          using (var serverside_client = server.AcceptTcpClient())
          using (var serverside_stream = serverside_client.GetStream())
          {
            foreach (var frame in frames)
            {
              serverside_stream.Write(frame, 0, frame.Length);
              System.Threading.Thread.Sleep(500);
            }
          }
          sync.WaitOne();
          server.Stop();
        });
        var listen_task = System.Threading.Tasks.Task.Run(listen);

        var received = new System.Text.StringBuilder();
        using (var client = new System.Net.Sockets.TcpClient())
        {
          client.Connect(Environment.MachineName, port);
System.Diagnostics.Trace.WriteLine($"client.Connected:{client.Connected}");
//          Assert.IsTrue(client.Connected);
          using (var stream = client.GetStream())
          using (var reader = new System.IO.BinaryReader(stream))
          {
            const int buffer_size = 0x400;
            byte[] buffer = new byte[buffer_size];
            do
            {
              int read = reader.Read(buffer, 0, buffer_size);
              System.Diagnostics.Trace.WriteLine($"read:{read}");
              if (read == 0)
              {
                break;
              }
              byte[] packet = new byte[read];
              Array.Copy(buffer, packet, read);
              received.Append(System.Text.Encoding.UTF8.GetString(packet));
            } while (true);

            //var parser = new PositionalValuesFileParser<SimpleOrder>();

            // Act
            //parseResults = parser.Read(reader);
          }
        }

        sync.Set();
        listen_task.Wait(5000);

        // Assert
        Assert.IsTrue(received.ToString().Length > 0);
        //Assert.IsTrue(orders.SequenceEqual(parseResults.Select(parseResult => parseResult.Instance)));
      }
      finally
      {
        server?.Stop();
        server = null;
      }
      Assert.IsNull(server);
    }

    [TestMethod]
    public void test_executable_model_with_sockets()
    {
      var frames = new List<byte[]>
      {
        new byte[] { 0x20, 0x20, 0x20, 0x20, 0x31 },
        new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x32 },
        new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x41 },
        new byte[] { 0x33, 0x31, 0x2F, 0x31, 0x32, 0x2F, 0x32, 0x30, 0x31, 0x35 },
        new byte[] { 0x20, 0x20, 0x20, 0x20, 0x33 },
        new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x34 },
        new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x42 },
        new byte[] { 0x30, 0x33, 0x2F, 0x30, 0x32, 0x2F, 0x32, 0x30, 0x31, 0x36 }
      };

      var sync = new System.Threading.ManualResetEvent(false);
      int port = 13002;
      System.Net.Sockets.TcpListener server = null;
      try
      {
        var listen = new Action(() =>
        {
          server = System.Net.Sockets.TcpListener.Create(port);
          server.Start();
          using (var serverside_client = server.AcceptTcpClient())
          using (var serverside_stream = serverside_client.GetStream())
          {
            foreach (var frame in frames)
            {
              serverside_stream.Write(frame, 0, frame.Length);
              System.Threading.Thread.Sleep(500);
            }
          }
          sync.WaitOne();
          server.Stop();
        });
        var listen_task = System.Threading.Tasks.Task.Run(listen);

        var received = new System.Text.StringBuilder();
        using (var client = new System.Net.Sockets.TcpClient())
        {
          client.Connect(Environment.MachineName, port);
          Assert.IsTrue(client.Connected);
          using (var stream = client.GetStream())
          using (var reader = new System.IO.BinaryReader(stream))
          {
            const int buffer_size = 0x400;
            byte[] buffer = new byte[buffer_size];
            do
            {
              int read = reader.Read(buffer, 0, buffer_size);
              System.Diagnostics.Trace.WriteLine($"read:{read}");
              if (read == 0)
              {
                break;
              }
              byte[] packet = new byte[read];
              Array.Copy(buffer, packet, read);
              received.Append(System.Text.Encoding.UTF8.GetString(packet));
            } while (true);
          }
        }

        sync.Set();
        listen_task.Wait(5000);
        Assert.IsTrue(received.ToString().Length > 0);
      }
      finally
      {
        server?.Stop();
        server = null;
      }
      Assert.IsNull(server);
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

    /// <summary>
    /// Delimited packets (implicitly) & Delimited messages & Positional values; that is, packets, messages have separators and values are positional.
    /// </summary>
    [TestMethod, Ignore]
    public void MemoryStreamAsMixedPositionalDelimitedBinaryTest()
    {
      // Arrange
      var frame = new System.IO.MemoryStream(new byte[]
      {
        0x20, 0x20, 0x20, 0x20, 0x31,
        0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x32,
        0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x41,
        0x33, 0x31, 0x2F, 0x31, 0x32, 0x2F, 0x32, 0x30, 0x31, 0x35,
        0x0A,
        0x20, 0x20, 0x20, 0x20, 0x33,
        0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x34,
        0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x42,
        0x30, 0x33, 0x2F, 0x30, 0x32, 0x2F, 0x32, 0x30, 0x31, 0x36,
        0x0A
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
        var parseResults = parser.Read(reader, group_separator: null, record_separator: 0x0A, unit_separator: null);

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