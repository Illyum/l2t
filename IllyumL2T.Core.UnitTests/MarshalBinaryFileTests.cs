using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IllyumL2T.Core.FieldsSplit.Marshal;

namespace IllyumL2T.Core.FieldsSplit.UnitTests
{
  [TestClass]
  public class MarshalBinaryFileTests
  {
    [TestMethod, TestCategory("BinaryMarshaling")]
    public void BinaryFileMarshalerTests()
    {
      // Arrange
      var frame = new System.IO.MemoryStream(new byte[] { 0x31, 0x39, 0x35, 0x20, 0x20, 0x32, 0x36, 0x41, 0x42, 0x43 }); //For now, each value represented as a string. 

      // Act
      List<Record> parsed_objects;
      using (var reader = new System.IO.BinaryReader(frame))
      {
        var fileMarshaler = new BinaryFileMarshaler<Record>();
        var parseResults = fileMarshaler.Read(reader, delimiter: ',', includeHeaders: false);
        parsed_objects = new List<Record>(parseResults.Select(result => result.Instance));
        //parsed_objects = parseResults.Aggregate(new List<Record>(), (whole, next) => { whole.Add(next.Instance); return whole; });
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