using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using IllyumL2T.Core.Parse;

namespace IllyumL2T.Core.FieldsSplit.UnitTests
{
  [TestClass]
  public class MemoryStreamBinaryTests
  {
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
      // Arrange
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
      IEnumerable<ParseResult<SimpleOrder>> parsed_orders = null;
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
              System.Threading.Thread.Sleep(100);
            }
          }
          sync.WaitOne();
          server.Stop();
        });
        var listen_task = System.Threading.Tasks.Task.Run(listen);

        using (var client = new System.Net.Sockets.TcpClient())
        {
          client.Connect(Environment.MachineName, port);
          using (var stream = client.GetStream())
          using (var reader = new System.IO.BinaryReader(stream))
          {
            var parser = new PositionalValuesFileParser<SimpleOrder>();

            // Act
            var parseResults = parser.Read(reader);
            parsed_orders = parseResults.ToList();
          }
        }

        sync.Set();
        listen_task.Wait(500);
      }
      finally
      {
        server?.Stop();
        server = null;
        sync.Dispose();
      }

      // Assert
      Assert.IsTrue(orders.SequenceEqual(parsed_orders.Select(parseResult => parseResult.Instance)));
    }

    /// <summary>
    /// Delimited packets & Delimited messages & Delimited values; that is, packets, messages and values have separators.
    /// </summary>
    [TestMethod]
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
        var parseResults = parser.Read(reader, group_separator: (char)0x1D, record_separator: (char)0x1E, unit_separator: (char)0x09);

        // Assert
        Assert.IsTrue(orders.SequenceEqual(parseResults.Select(parseResult => parseResult.Instance)));
      }
    }

    /// <summary>
    /// Delimited packets & Delimited messages & Delimited values; that is, packets, messages and values have separators.
    /// This time with two groups of records.
    /// </summary>
    [TestMethod]
    public void MemoryStreamAsDelimitedBinaryTest2()
    {
      // Arrange
      IEnumerable<Order> orders = new List<Order>
      {
        new Order { OrderId = 1, Freight = 2M, ShipAddress = "A", DeliveryDate = new DateTime(2015, 12, 31) },
        new Order { OrderId = 3, Freight = 4M, ShipAddress = "B", DeliveryDate = new DateTime(2016, 2, 3) },
        new Order { OrderId = 3, Freight = 4M, ShipAddress = "B", DeliveryDate = new DateTime(2016, 2, 3) },
        new Order { OrderId = 1, Freight = 2M, ShipAddress = "A", DeliveryDate = new DateTime(2015, 12, 31) }
      };

      var frame = new System.IO.MemoryStream(new byte[]
      {
        0x31, 0x09, 0x32, 0x09, 0x41, 0x09, 0x33, 0x31, 0x2F, 0x31, 0x32, 0x2F, 0x32, 0x30, 0x31, 0x35, 0x1E,
        0x33, 0x09, 0x34, 0x09, 0x42, 0x09, 0x30, 0x33, 0x2F, 0x30, 0x32, 0x2F, 0x32, 0x30, 0x31, 0x36, 0x1E,
        0x1D,
        0x33, 0x09, 0x34, 0x09, 0x42, 0x09, 0x30, 0x33, 0x2F, 0x30, 0x32, 0x2F, 0x32, 0x30, 0x31, 0x36, 0x1E,
        0x31, 0x09, 0x32, 0x09, 0x41, 0x09, 0x33, 0x31, 0x2F, 0x31, 0x32, 0x2F, 0x32, 0x30, 0x31, 0x35, 0x1E
      });

      using (var reader = new System.IO.BinaryReader(frame))
      {
        var parser = new DelimiterSeparatedValuesFileParser<Order>();

        // Act
        var parseResults = parser.Read(reader, group_separator: (char)0x1D, record_separator: (char)0x1E, unit_separator: (char)0x09);

        // Assert
        Assert.IsTrue(orders.SequenceEqual(parseResults.Select(parseResult => parseResult.Instance)));
      }
    }

    /// <summary>
    /// Delimited packets (implicitly) & Delimited messages & Positional values; that is, packets, messages have separators and values are positional.
    /// </summary>
    [TestMethod]
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
        var parseResults = parser.Read(reader, group_separator: null, record_separator: (char)0x0A, unit_separator: null);

        // Assert
        Assert.IsTrue(orders.SequenceEqual(parseResults.Select(parseResult => parseResult.Instance)));
      }
    }

    /// <summary>
    /// Delimited packets (implicitly) & Delimited messages & Positional values; that is, packets, messages have separators and values are positional.
    /// This time for U+0002 Start of Text(STX) and U+0003 End of Text(ETX) as separators case.
    /// </summary>
    [TestMethod]
    public void MemoryStreamAsMixedPositionalDelimitedBinaryTest2()
    {
      // Arrange
      var frame = new System.IO.MemoryStream(new byte[]
      {
        0x02,
        0x20, 0x20, 0x20, 0x20, 0x31,
        0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x32,
        0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x41,
        0x33, 0x31, 0x2F, 0x31, 0x32, 0x2F, 0x32, 0x30, 0x31, 0x35,
        0x03,
        0x02,
        0x20, 0x20, 0x20, 0x20, 0x33,
        0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x34,
        0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x42,
        0x30, 0x33, 0x2F, 0x30, 0x32, 0x2F, 0x32, 0x30, 0x31, 0x36,
        0x03
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
        var parseResults = parser.Read(reader, group_separator: (char)0x02, record_separator: (char)0x03, unit_separator: null);

        // Assert
        Assert.IsTrue(orders.SequenceEqual(parseResults.Select(parseResult => parseResult.Instance)));
      }
    }

    /// <summary>
    /// Delimited packets (implicitly) & Delimited messages & Positional values; that is, packets, messages have separators and values are positional.
    /// This time with incomplete record at the start of the byte frame.
    /// </summary>
    [TestMethod]
    public void MemoryStreamAsMixedPositionalDelimitedBinaryTest3()
    {
      // Arrange
      var frame = new System.IO.MemoryStream(new byte[]
      {
        0x30, 0x33, 0x2F, 0x30, 0x32, 0x2F, 0x32, 0x30, 0x31, 0x36,
        0x03,
        0x02,
        0x20, 0x20, 0x20, 0x20, 0x31,
        0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x32,
        0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x41,
        0x33, 0x31, 0x2F, 0x31, 0x32, 0x2F, 0x32, 0x30, 0x31, 0x35,
        0x03,
        0x02,
        0x20, 0x20, 0x20, 0x20, 0x33,
        0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x34,
        0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x42,
        0x30, 0x33, 0x2F, 0x30, 0x32, 0x2F, 0x32, 0x30, 0x31, 0x36,
        0x03
      });

      IEnumerable<SimpleOrder> orders = new List<SimpleOrder>
      {
        null,
        new SimpleOrder { OrderId = 1, Freight = 2M, ShipAddress = "A", DeliveryDate = new DateTime(2015, 12, 31) },
        new SimpleOrder { OrderId = 3, Freight = 4M, ShipAddress = "B", DeliveryDate = new DateTime(2016, 2, 3) }
      };

      using (var reader = new System.IO.BinaryReader(frame))
      {
        var parser = new PositionalValuesFileParser<SimpleOrder>();

        // Act
        var parseResults = parser.Read(reader, group_separator: (char)0x02, record_separator: (char)0x03, unit_separator: null);

        // Assert
        Assert.IsTrue(orders.SequenceEqual(parseResults.Select(parseResult => parseResult.Instance)));
      }
    }

    [TestMethod]
    public void ByteReaderTest1()
    {
      // Arrange
      var frame = new System.IO.MemoryStream(new byte[]
      {
        0x02,
        0x20, 0x21, 0x22, 0x23, 0x24,
        0x25, 0x26, 0x27, 0x28, 0x29,
        0x03,
        0x02,
        0x30, 0x31, 0x32, 0x33, 0x34,
        0x03,
      });

      using (var reader = new System.IO.BinaryReader(frame))
      {
        var bytes = new ByteReader();

        // Act
        var results = new List<byte[]>(bytes.Read(reader, start_of_text: 0x02, end_of_text: 0x03));

        // Assert
        Assert.AreEqual<int>(2, results.Count());
        CollectionAssert.AreEqual(new byte[] { 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29 }, results[0]);
        CollectionAssert.AreEqual(new byte[] { 0x30, 0x31, 0x32, 0x33, 0x34 }, results[1]);
      }
    }

    #region Bytes vs Chars difference awareness
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

    [TestMethod]
    public void Bytes_vs_Chars_DifferenceAwareness_SampleCase2()
    {
      // Arrange
      string lastname = "López";
      System.Text.Encoding latin1 = System.Text.Encoding.GetEncoding("ISO-8859-1");
      System.Text.Encoding utf8 = System.Text.Encoding.UTF8;

      var bytes1 = new System.IO.MemoryStream();
      var writer1 = new System.IO.BinaryWriter(bytes1, latin1);
      writer1.Write(lastname);
      bytes1.Position = 0;

      var bytes2 = new System.IO.MemoryStream();
      var writer2 = new System.IO.BinaryWriter(bytes2, utf8);
      writer2.Write(lastname);
      bytes2.Position = 0;

      var reader1 = new System.IO.BinaryReader(bytes1, latin1);
      var reader2 = new System.IO.BinaryReader(bytes2, utf8);

      // Act
      char[] chars1 = reader1.ReadChars(0xFF);
      char[] chars2 = reader2.ReadChars(0xFF);

      // Assert
      Assert.AreEqual<int>(6, chars1.Length);
      Assert.AreEqual<int>(6, chars2.Length);
    }

    [TestMethod]
    public void Bytes_vs_Chars_DifferenceAwareness_SampleCase3()
    {
      // Arrange
      string lastname = "López";
      System.Text.Encoding latin1 = System.Text.Encoding.GetEncoding("ISO-8859-1");
      System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
      System.Text.Encoding unicode = System.Text.Encoding.Unicode;

      var bytes1 = new System.IO.MemoryStream();
      var writer1 = new System.IO.BinaryWriter(bytes1, latin1);
      writer1.Write(lastname);
      bytes1.Position = 0;

      var bytes2 = new System.IO.MemoryStream();
      var writer2 = new System.IO.BinaryWriter(bytes2, utf8);
      writer2.Write(lastname);
      bytes2.Position = 0;

      var bytes3 = new System.IO.MemoryStream();
      var writer3 = new System.IO.BinaryWriter(bytes3, unicode);
      writer3.Write(lastname);
      bytes3.Position = 0;

      var reader1 = new System.IO.BinaryReader(bytes1, latin1);
      var reader2 = new System.IO.BinaryReader(bytes2, utf8);
      var reader3 = new System.IO.BinaryReader(bytes3, unicode);

      // Act
      char[] chars1 = reader1.ReadChars(5);
      char[] chars2 = reader2.ReadChars(5);
      char[] chars3 = reader3.ReadChars(5);

      byte[] expected_char1 = BitConverter.GetBytes(lastname[1]);
      byte[] char0 = BitConverter.GetBytes(chars2[0]);
      byte[] char1 = BitConverter.GetBytes(chars2[1]);
      byte[] char2 = BitConverter.GetBytes(chars2[2]);

      // Assert
      Assert.AreEqual<int>(5, chars1.Length);
      Assert.AreEqual<int>(5, chars2.Length);
      Assert.AreEqual<string>("Lópe", new string(chars1));
      Assert.AreEqual<string>("Lópe", new string(chars2));
      Assert.AreEqual<string>("䰊瀀攀稀", new string(chars3));

      Assert.AreEqual<int>(2, char0.Length);
      Assert.AreEqual<byte>(6, char0[0]);
      Assert.AreEqual<byte>(0, char0[1]);

      Assert.AreEqual<int>(2, char1.Length);
      Assert.AreEqual<byte>(76, char1[0]);
      Assert.AreEqual<byte>(0, char1[1]);

      Assert.AreEqual<int>(expected_char1.Length, char1.Length);
      CollectionAssert.AreNotEqual(expected_char1, char1);
      Assert.AreNotEqual<byte>(expected_char1[0], char1[0]);
      Assert.AreEqual<byte>(expected_char1[1], char1[1]);
    }
    #endregion
    #region CircularButter.ReadBytesUpTo tests
    //TODO:Temporally here. Once evaluated, then they could move at a proper location.
    [TestMethod, TestCategory("CircularBuffer")]
    public void ReadBytesUpTo_basic0()
    {
      // Arrange
      var buffer = new CircularBuffer();
      buffer.Add(new char[] { (char)0x01, (char)0x09 });

      // Act
      char[] bytes = buffer.ReadBytesUpTo((char)0x09);

      // Assert
      Assert.IsNotNull(bytes);
      Assert.AreEqual<int>(1, bytes.Length);
      Assert.AreEqual<char>((char)0x01, bytes[0]);
    }

    [TestMethod, TestCategory("CircularBuffer")]
    public void ReadBytesUpTo_basic1()
    {
      // Arrange
      var buffer = new CircularBuffer();
      buffer.Add(new char[] { (char)0x01, (char)0x0D });

      // Act
      char[] bytes = buffer.ReadBytesUpTo((char)0x09, (char)0x0D);

      // Assert
      Assert.IsNotNull(bytes);
      Assert.AreEqual<int>(1, bytes.Length);
      Assert.AreEqual<char>((char)0x01, bytes[0]);
    }

    [TestMethod, TestCategory("CircularBuffer")]
    public void ReadBytesUpTo_basic2()
    {
      // Arrange
      var buffer = new CircularBuffer();
      buffer.Add(new char[] { });

      // Act
      char[] bytes = buffer.ReadBytesUpTo((char)0x09);

      // Assert
      Assert.IsNull(bytes);
    }

    [TestMethod, TestCategory("CircularBuffer")]
    public void ReadBytesUpTo_basic3()
    {
      // Arrange
      var buffer = new CircularBuffer();
      buffer.Add(new char[] { (char)0x01, (char)0x09 });

      // Act
      char[] bytes = buffer.ReadBytesUpTo((char)0x0D, (char)0x1E);

      // Assert
      Assert.IsNull(bytes);
    }

    [TestMethod, TestCategory("CircularBuffer")]
    public void ReadBytesUpTo_basic4()
    {
      // Arrange
      var buffer = new CircularBuffer();
      buffer.Add(new char[] { (char)0x01, (char)0x09, (char)0x02, (char)0x0D });

      // Act
      char[] bytes1 = buffer.ReadBytesUpTo((char)0x09, (char)0x0D);
      char[] bytes2 = buffer.ReadBytesUpTo((char)0x09, (char)0x0D);

      // Assert
      Assert.IsNotNull(bytes1);
      Assert.AreEqual<int>(1, bytes1.Length);
      Assert.AreEqual<char>((char)0x01, bytes1[0]);

      Assert.IsNotNull(bytes2);
      Assert.AreEqual<int>(1, bytes2.Length);
      Assert.AreEqual<char>((char)0x02, bytes2[0]);
    }

    [TestMethod, TestCategory("CircularBuffer")]
    public void ReadBytesUpTo_basic5()
    {
      // Arrange
      var buffer = new CircularBuffer();
      buffer.Add(new char[] { (char)0x0D });

      // Act
      char[] bytes = buffer.ReadBytesUpTo((char)0x09, (char)0x0D);

      // Assert
      Assert.IsNull(bytes);
    }

    [TestMethod, TestCategory("CircularBuffer")]
    public void ReadBytesUpTo_basic6()
    {
      // Arrange
      var buffer = new CircularBuffer();
      buffer.Add(new char[] { (char)0x0D, (char)0x09, (char)0x0D });

      // Act
      char[] bytes1 = buffer.ReadBytesUpTo((char)0x09, (char)0x0D);
      char[] bytes2 = buffer.ReadBytesUpTo((char)0x09, (char)0x0D);
      char[] bytes3 = buffer.ReadBytesUpTo((char)0x09, (char)0x0D);

      // Assert
      Assert.IsNull(bytes1);
      Assert.IsNull(bytes2);
      Assert.IsNull(bytes3);
    }

    [TestMethod, TestCategory("CircularBuffer")]
    public void ReadBytesUpTo_basic7()
    {
      // Arrange
      var buffer = new CircularBuffer();
      buffer.Add(new char[] { (char)0x0D, (char)0x01, (char)0x09, (char)0x0D, (char)0x02 });

      // Act
      char[] bytes1 = buffer.ReadBytesUpTo((char)0x09, (char)0x0D);
      char[] bytes2 = buffer.ReadBytesUpTo((char)0x09, (char)0x0D);
      char[] bytes3 = buffer.ReadBytesUpTo((char)0x09, (char)0x0D);
      char[] bytes4 = buffer.ReadBytesUpTo((char)0x09, (char)0x0D);

      // Assert
      Assert.IsNotNull(bytes1);
      Assert.AreEqual<int>(1, bytes1.Length);
      Assert.AreEqual<char>((char)0x01, bytes1[0]);
      Assert.IsNull(bytes3);
      Assert.IsNull(bytes4);
    }

    [TestMethod, TestCategory("CircularBuffer")]
    public void ReadBytesUpTo_basic8()
    {
      // Arrange
      var buffer = new CircularBuffer();
      buffer.Add(new char[] { (char)0x01, (char)0x1E, (char)0x1D, (char)0x02, (char)0x1E });

      // Act
      char[] bytes1 = buffer.ReadBytesUpTo((char)0x1E, (char)0x1D);
      char[] bytes2 = buffer.ReadBytesUpTo((char)0x1E, (char)0x1D);
      char[] bytes3 = buffer.ReadBytesUpTo((char)0x1E, (char)0x1D);
      char[] bytes4 = buffer.ReadBytesUpTo((char)0x1E, (char)0x1D);
      char[] bytes5 = buffer.ReadBytesUpTo((char)0x1E, (char)0x1D);

      // Assert
      Assert.IsNotNull(bytes1);
      Assert.AreEqual<int>(1, bytes1.Length);
      Assert.AreEqual<char>((char)0x01, bytes1[0]);

      Assert.IsNotNull(bytes2);
      Assert.AreEqual<int>(1, bytes2.Length);
      Assert.AreEqual<char>((char)0x02, bytes2[0]);

      Assert.IsNull(bytes4);
    }

    [TestMethod, TestCategory("CircularBuffer")]
    public void ReadBytesUpTo_basic9()
    {
      // Arrange
      var buffer = new CircularBuffer();
      buffer.Add(new char[] { (char)0x02, (char)0x20, (char)0x03, (char)0x02, (char)0x21, (char)0x03 });

      // Act
      char[] bytes1 = buffer.ReadBytesUpTo((char)0x02, (char)0x03);
      char[] bytes2 = buffer.ReadBytesUpTo((char)0x02, (char)0x03);
      char[] bytes3 = buffer.ReadBytesUpTo((char)0x02, (char)0x03);
      char[] bytes4 = buffer.ReadBytesUpTo((char)0x02, (char)0x03);

      // Assert
      Assert.IsNotNull(bytes1);
      Assert.AreEqual<int>(1, bytes1.Length);
      Assert.AreEqual<char>((char)0x20, bytes1[0]);

      Assert.IsNotNull(bytes2);
      Assert.AreEqual<int>(1, bytes2.Length);
      Assert.AreEqual<char>((char)0x21, bytes2[0]);

      Assert.IsNull(bytes3);
      Assert.IsNull(bytes4);
    }
    #endregion
  }
}