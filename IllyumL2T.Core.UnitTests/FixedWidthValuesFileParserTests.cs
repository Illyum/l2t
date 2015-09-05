using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using IllyumL2T.Core.FieldsSplit.Parse;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IllyumL2T.Core.FieldsSplit.UnitTests
{
    [TestClass]
    public class FixedWidthValuesFileParserTests
    {
        [TestMethod, TestCategory("FixedWidth")]
        public void ParseOneFixedWidthLine()
        {
            // Arrange
            ushort type = 19;
            byte category = 5;
            uint id = 26;
            string label = "ABC";
            string input = $"{type}{category}{id:0000}{label}";

            TextReader reader = new StringReader(input);

            // Act
            var fileParser = new FixedWidthValuesFileParser<Record>();
            var parseResults = fileParser.Read(reader, includeHeaders: false);
            var actual = parseResults.Select(parseResult => parseResult.Instance)
                                     .SingleOrDefault();

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual<ushort>(type, actual.Type);
            Assert.AreEqual<byte>(category, actual.Category);
            Assert.AreEqual<uint>(id, actual.ID);
            Assert.AreEqual<string>("ABC", actual.Label);
        }

        [TestMethod, TestCategory("FixedWidth")]
        public void ParseOneFixedWidthLineWithTrimInput()
        {
            // Arrange
            TextReader reader = new StringReader("195  26ABC");
            var fileParser = new FixedWidthValuesFileParser<Record>();

            // Act
            var parseResults = fileParser.Read(reader, includeHeaders: false);
            var parsedObjects = new List<Record>(parseResults.Select(parseResult => parseResult.Instance));

            // Assert
            Assert.IsNotNull(parsedObjects);
            Assert.AreEqual<int>(1, parsedObjects.Count());
            Assert.IsNotNull(parsedObjects[0]);
            Assert.AreEqual<ushort>(19, parsedObjects[0].Type);
            Assert.AreEqual<byte>(5, parsedObjects[0].Category);
            Assert.AreEqual<uint>(26, parsedObjects[0].ID);
            Assert.AreEqual<string>("ABC", parsedObjects[0].Label);
        }

        [TestMethod, TestCategory("FixedWidth")]
        public void ParseTwoFixedWidthLines()
        {
            // Arrange
            var lines = new StringBuilder();
            lines.AppendLine("0015432 XY");
            lines.AppendLine("1950026ABC");
            TextReader reader = new StringReader(lines.ToString());
            var fileParser = new FixedWidthValuesFileParser<Record>();

            // Act
            var parseResults = fileParser.Read(reader, includeHeaders: false, trimInput: false);
            var parsedObjects = new List<Record>(parseResults.Select(parseResult => parseResult.Instance));

            // Assert
            Assert.IsNotNull(parsedObjects);
            Assert.AreEqual<int>(2, parsedObjects.Count());

            Assert.IsNotNull(parsedObjects[0]);
            Assert.AreEqual<ushort>(0, parsedObjects[0].Type);
            Assert.AreEqual<byte>(1, parsedObjects[0].Category);
            Assert.AreEqual<uint>(5432, parsedObjects[0].ID);
            Assert.AreEqual<string>(" XY", parsedObjects[0].Label);

            Assert.IsNotNull(parsedObjects[1]);
            Assert.AreEqual<ushort>(19, parsedObjects[1].Type);
            Assert.AreEqual<byte>(5, parsedObjects[1].Category);
            Assert.AreEqual<uint>(26, parsedObjects[1].ID);
            Assert.AreEqual<string>("ABC", parsedObjects[1].Label);
        }

        [TestMethod, TestCategory("FixedWidth")]
        public void ParseIncompleteFixedWidthLine()
        {
            // Arrange
            ushort type = 0;
            byte category = 2;
            string line = $"{type:00}{category}";
            var reader = new StringReader(line);
            var fileParser = new FixedWidthValuesFileParser<Record>();

            // Act
            var parseResults = fileParser.Read(reader, includeHeaders: false, trimInput: false);
            var parsedObjects = new List<Record>(parseResults.Select(parseResult => parseResult.Instance));

            // Assert
            Assert.AreEqual<int>(1, parsedObjects.Count);
            Assert.AreEqual<ushort>(type, parsedObjects[0].Type);
            Assert.AreEqual<byte>(category, parsedObjects[0].Category);
            Assert.AreEqual<uint>(0, parsedObjects[0].ID);
            Assert.AreEqual<string>(null, parsedObjects[0].Label);
        }
    }
}