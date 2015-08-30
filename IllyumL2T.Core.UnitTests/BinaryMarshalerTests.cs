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