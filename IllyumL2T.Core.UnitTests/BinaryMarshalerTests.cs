using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IllyumL2T.Core.FieldsSplit.Marshal;
using IllyumL2T.Core.FieldsSplit.UnitTests.Classes_for_Testing;
using System;

namespace IllyumL2T.Core.FieldsSplit.UnitTests
{
  [TestClass]
  public class BinaryDeserializerTests
  {
    [TestMethod, TestCategory("BinaryUnmarshaling")]
    public void SimpleUnmarshaling1()
    {
      // Arrange
      var frame = new System.IO.MemoryStream(new byte[] { 0x01, 0x09, 0x35, 0x20, 0x20, 0x32, 0x36, 0x41, 0x42, 0x43 }); //For now, each value represented as a string. 

      /*
      System.IO.Stream
        Microsoft.JScript.COMCharStream
        System.Data.OracleClient.OracleBFile
        System.Data.OracleClient.OracleLob
        System.Data.SqlTypes.SqlFileStream
        System.IO.BufferedStream
        System.IO.Compression.DeflateStream
        System.IO.Compression.GZipStream
        System.IO.FileStream
        System.IO.MemoryStream
        System.IO.Pipes.PipeStream
        System.IO.UnmanagedMemoryStream
        System.Net.Security.AuthenticatedStream
        System.Net.Sockets.NetworkStream
        System.Printing.PrintQueueStream
        System.Security.Cryptography.CryptoStream

      System.IO.Stream stream1 = new System.IO.MemoryStream();
      System.IO.Stream stream2 = new System.Net.Sockets.NetworkStream(socket: null);

      System.IO.TextReader x1 = new System.IO.StreamReader(stream1);
      System.IO.TextReader x2 = new System.IO.StreamReader(stream2);
      System.IO.BinaryReader x3 = new System.IO.BinaryReader(stream2);
      */

      // Act
      List<Record> parsed_objects;
      using (var reader = new System.IO.BinaryReader(frame))
      {
        var fileMarshaler = new BinaryDeserializer<Record>();
        var parseResults = fileMarshaler.Deserialize(reader);
        parsed_objects = new List<Record>(parseResults.Select(result => result.Instance));
      }

      // Assert
      Assert.IsNotNull(parsed_objects);
      Assert.AreEqual<int>(1, parsed_objects.Count());
      Assert.IsNotNull(parsed_objects[0]);
      Assert.AreEqual<ushort>(19, parsed_objects[0].Type);
      Assert.AreEqual<byte>(5, parsed_objects[0].Category);
      Assert.AreEqual<uint>(26, parsed_objects[0].ID);
      Assert.AreEqual<string>("ABC", parsed_objects[0].Label);
    }

    [TestMethod, TestCategory("BinaryUnmarshaling")]
    public void Bytes_vs_Chars_DifferenceAwareness_SimpleCase1()
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