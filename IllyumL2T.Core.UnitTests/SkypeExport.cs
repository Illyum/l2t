using System.Linq;
using System.IO;
using IllyumL2T.Core.Parse;
using IllyumL2T.Core.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IllyumL2T.Core.UnitTests
{
  [TestClass]
  public class SkypeExport
  {
    [TestMethod]
    public void Skype_conversation()
    {
      // Arrange
      var skype_export_file = @"filex.csv";
      using (var reader = new StreamReader(skype_export_file))
      {
        var fileParser = new DelimiterSeparatedValuesFileParser<SkypeLine>();

        // Act
        var parseResults = fileParser.Read(reader, delimiter: ',', includeHeaders: false);
        var itr=parseResults.GetEnumerator();
        Assert.IsTrue(itr.MoveNext());
        Assert.IsNotNull(itr.Current.Instance);
        Assert.AreEqual<string>("TimestampMs", itr.Current.Instance.TimestampMs);

        Assert.IsTrue(itr.MoveNext());
        Assert.IsNotNull(itr.Current.Instance);
      }
    }
  }
}