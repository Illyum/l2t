using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace IllyumL2T.Core.UnitTests
{
  [TestClass]
  public class FileParserTests_BinaryFixedWidth
  {
    [TestMethod, TestCategory("BinaryFixedWidth")]
    public void ParseStringRepresentations()
    {
      // Arrange
      var frame = new System.IO.MemoryStream(new byte[] { 0x31, 0x39, 0x35, 0x20, 0x20, 0x32, 0x36, 0x41, 0x42, 0x43 }); //For now, each value represented as a string.

      // Act
      List<Record> parsed_objects;
      using (var reader = new System.IO.BinaryReader(frame))
      {
        var fileParser = new FileParser<Record>();
        var parseResults = fileParser.Read(reader);
        parsed_objects = parseResults.Aggregate(new List<Record>(), (whole, next) => { whole.Add(next.Instance); return whole; });
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

    [TestMethod, TestCategory("BinaryFixedWidth")]
    public void ParseStringAndBinaryRepresentations()
    {
      // Arrange
      var frame = new System.IO.MemoryStream(new byte[] { 0x00, 0x13, 0x05, 0x00, 0x00, 0x00, 0x1A, 0x41, 0x42, 0x43 }); //The same values as the previous expectation specification/test, now just not all represented as strings.

      // Act
      List<Record> parsed_objects;
      using (var reader = new System.IO.BinaryReader(frame, Encoding.ASCII))
      {
        var fileParser = new FileParser<Record>();
        var parseResults = fileParser.Read(reader);
        parsed_objects = parseResults.Aggregate(new List<Record>(), (whole, next) => { whole.Add(next.Instance); return whole; });
      }

      // Assert
      Assert.IsNotNull(parsed_objects);
      Assert.AreEqual<int>(1, parsed_objects.Count());
      Assert.IsNotNull(parsed_objects[0]); /*TODO: Currently, the test fails here because of the following parsing errors. But a binary parsing should process these values correctly. Don't you think?
        "Type: Unparsable System.UInt16 >>> \0\u0013"
        "Category: Unparsable System.Byte >>> \u0005"
        "ID: Unparsable System.UInt32 >>> \0\0\0\u001a"
      */
      Assert.AreEqual<ushort>(19, parsed_objects[0].Type);
      Assert.AreEqual<byte>(5, parsed_objects[0].Category);
      Assert.AreEqual<uint>(26, parsed_objects[0].ID);
      Assert.AreEqual<string>("ABC", parsed_objects[0].Label);
    }
  }


  /* Provisional place for the following declarations */


  #region Input case
  /// <summary>
  /// Application-level objects serialized as raw data packets for TCP transmission or for binary storage.
  /// </summary>
  public class Record
  {
    [IllyumL2T.Core.ParseBehavior(Length = 2)] public ushort Type { get; set; }
    [IllyumL2T.Core.ParseBehavior(Length = 1)] public byte Category { get; set; }
    [IllyumL2T.Core.ParseBehavior(Length = 4)] public uint ID { get; set; }
    [IllyumL2T.Core.ParseBehavior(Length = 3)] public string Label { get; set; }
  }

  #endregion
}