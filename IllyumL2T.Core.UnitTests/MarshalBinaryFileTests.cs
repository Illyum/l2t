using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IllyumL2T.Core.FieldsSplit.Marshal;
using IllyumL2T.Core.FieldsSplit.UnitTests.Classes_for_Testing;
using System;

namespace IllyumL2T.Core.FieldsSplit.UnitTests
{
  [TestClass]
  public class MarshalBinaryFileTests
  {
    [TestMethod, TestCategory("BinaryMarshaling")]
    public void BinaryFileMarshalerTests()
    {
      // Arrange
      var frame = new System.IO.MemoryStream(new byte[] { 0x01, 0x09, 0x35, 0x20, 0x20, 0x32, 0x36, 0x41, 0x42, 0x43 }); //For now, each value represented as a string. 

      // Act
      List<Record> parsed_objects;
      using (var reader = new System.IO.BinaryReader(frame))
      {
        Type type = typeof(Record);
        int size= type.GetProperties().Select(p => (IllyumL2T.Core.ParseBehaviorAttribute)p.GetCustomAttributes(typeof(IllyumL2T.Core.ParseBehaviorAttribute), true).First()).Where(attr => attr.Length > 0).Sum(a => a.Length);
        //char[] chars = reader.ReadChars(size);
        //Assert.AreEqual<string>("195  26ABC", new string(chars));
        byte[] bytes = reader.ReadBytes(size);
        Assert.AreEqual<int>(size, bytes.Length);
        Assert.IsTrue(frame.ToArray().SequenceEqual(bytes));
        return;

        var fileMarshaler = new BinaryFileMarshaler<Record>();
        var parseResults = fileMarshaler.Read(reader, delimiter: ',', includeHeaders: false);
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
  }
}