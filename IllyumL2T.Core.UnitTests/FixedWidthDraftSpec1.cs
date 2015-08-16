using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace IllyumL2T.Core.UnitTests
{
    [TestClass]
    public class FixedWidthDraftSpec1
    {
        [TestMethod, TestCategory("BinaryFixedWidth"), Description("As of August 15, 2015: this case is a false start because is yet too broad. We need other baby-steps previous cases.")]
        public void VerySimpleFirst_Proposed_Case_To_Get_The_Feeling_Of_This_FixedWidthParsingApproach()
        {
            // Arrange
            var frame0 = new System.IO.MemoryStream(new byte[] { 0x30, 0x39, 0xEA, 0x00, 0x00, 0x02, 0xA6, 0x41, 0x42, 0x43 }); //The incoming bytes, maybe from the network or from network frames captured in a file.
            var raw_data = new List<System.IO.Stream> { frame0 };
            var source = new SimpleTestRawDataReader(raw_data); //Stub
            var metadata_provider = new CLRTypeBasedMetadata<Record>(); //Stub
            var reader = new BinaryFixedWidthReader(metadata_provider); //SUT

            // Act
            var parsed_objects = reader.GetNextObjectFrom<Record>(source).Aggregate(new List<Record>(), (whole, next) => { whole.Add(next); return whole; });

            // Assert
            Assert.IsNotNull(parsed_objects);
            Assert.AreEqual<int>(1, parsed_objects.Count);
            Assert.IsNotNull(parsed_objects[0]);
            Assert.AreEqual<ushort>(12345, parsed_objects[0].Type);
            Assert.AreEqual<byte>(234, parsed_objects[0].Category);
            Assert.AreEqual<uint>(678, parsed_objects[0].ID);
            Assert.AreEqual<string>("ABC", parsed_objects[0].Label);
        }
    }

    [TestClass]
    public class DecoupledSourceSpec
    {
        [TestMethod, TestCategory("BinaryFixedWidth"), Description("A proposal for a decoupled binary data source. As a first step to decouple the data source from the parsing.")]
        public void Decoupled_BinarySource()
        {
            // Arrange
            var frame0 = new System.IO.MemoryStream(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09 }); //The incoming bytes, maybe from the network or from network frames captured in a file.
            var frame1 = new System.IO.MemoryStream(new byte[] { 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10  });
            var raw_data = new List<System.IO.Stream> { frame0, frame1 };
            var source = new SimpleTestRawDataReader(raw_data); //Stub

            // Act
            var checksum = source.GetNextRawPacket().Sum(frame => frame.Sum(b => b));

            // Assert
            Assert.AreEqual<int>(136, checksum);
        }
    }

    [TestClass]
    public class FileParserTests_BinaryFixedWidth
    {
        [TestMethod, TestCategory("BinaryFixedWidth")]
        public void ParseBinaryFixedWidthFileSpec()
        {
            // Arrange
            var frame = new System.IO.MemoryStream(new byte[] { 0x30, 0x39, 0xEA, 0x00, 0x00, 0x02, 0xA6, 0x41, 0x42, 0x43 }); //The incoming bytes, maybe from the network or from network frames captured in a file.
            var fixedWidthStream = new FixedWidthStream<Record>(frame);

            // Act
            List<Record> parsed_objects;
            using (var reader = new System.IO.StreamReader(fixedWidthStream))
            {
                var fileParser = new FileParser<Record>();
                var parseResults = fileParser.Read(reader, delimiter: '|', includeHeaders: false);
                parsed_objects = parseResults.Aggregate(new List<Record>(), (whole, next) => { whole.Add(next.Instance); return whole; });
            }

            // Assert
            Assert.IsNotNull(parsed_objects);
            Assert.AreEqual<int>(1, parsed_objects.Count());
            Assert.IsNotNull(parsed_objects[0]);
            Assert.AreEqual<ushort>(12345, parsed_objects[0].Type);
            Assert.AreEqual<byte>(234, parsed_objects[0].Category);
            Assert.AreEqual<uint>(678, parsed_objects[0].ID);
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

    #region Abstractions
    /// <summary>
    /// For the case of a CLS-based schema source, this attribute helps to declare the layout of sequential, fixed width, fields.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SequentialLayoutAttribute : Attribute
    {
        public uint Length;
    }

    /// <summary>
    /// Provide a sequence of raw data packets; where packet is, maybe, a series of captured payloads from TCP transmitted frames.
    /// </summary>
    public interface IRawDataReader
    {
        /// <summary>
        /// From a given binary source, it provides all contained raw data packets from that binary source.
        /// A raw data packet could contain zero o more application messages or a fragment of an application message at the end of a raw data packet.
        /// </summary>
        /// <returns>Iterator for all raw data packets from a given binary source.</returns>
        IEnumerable<byte[]> GetNextRawPacket();
    }

    /// <summary>
    /// Abstracts the source ot the layout schema; that is, the schema data can be stored in .NET CLS or in XSD or a custom storage.
    /// </summary>
    public interface ISchemaProvider
    {
        IDictionary<string, KeyValuePair<uint, Type>> GetTargetSchemaFor(Type type);
        uint GetTargetRawMessageSizeFor(Type type);
    }
    #endregion

    #region System under test/specification (SUTs)

    public class FixedWidthStream<T> : System.IO.Stream
    {
        private System.IO.Stream source;
        private bool closed;

        public FixedWidthStream(System.IO.Stream input)
        {
            source = input;
            closed = false;
        }

        public override bool CanRead { get { return !closed; } }
        public override bool CanSeek { get { throw new NotImplementedException(); } }
        public override bool CanWrite { get { throw new NotImplementedException(); } }
        public override long Length { get { throw new NotImplementedException(); } }

        public override long Position
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public override void Close()
        {
            if (!closed)
            {
                closed = true;
                base.Close();
            }
        }
        public override void Flush()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method does not work and ended up as very bad idea.
        /// TODO: Discard this FixedWidthStream<T> alternative.
        /// </summary>
        public override int Read(byte[] buffer, int offset, int count)
        {
            //what if count/read > "line" size (where "line" is an application object)?
            //what if count/read < "line" size (where "line" is an application object)?
            int read = source.Read(buffer, offset, count);

            var readline = new StringBuilder();
            var reader = new BinaryReader(new MemoryStream(buffer));
            //reader. -> The proposed ParseBehaviorAttribute.Length property was supposed to be used here to build a “line” out of the read buffer.

            //what if encoded bytes > count?

            byte[] encoded_bytes = Encoding.UTF8.GetBytes(readline.ToString());
            encoded_bytes.CopyTo(buffer, 0);
            return read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                Close();
                base.Dispose(disposing);
            }
        }
    }

    public class BinaryFixedWidthReader
    {
        private ISchemaProvider metadata;
        public BinaryFixedWidthReader(ISchemaProvider metadata)
        {
            this.metadata = metadata;
        }

        public IEnumerable<T> GetNextObjectFrom<T>(IRawDataReader input)
        {
            throw new NotImplementedException();
        }
    }

#endregion

#region Stubs
    /// <summary>
    /// Simple stub implementations for testing support.
    /// </summary>
    class SimpleTestRawDataReader : IRawDataReader
    {
        private IEnumerable<System.IO.Stream> source;
        public SimpleTestRawDataReader(IEnumerable<System.IO.Stream> input)
        {
            source = input;
        }
        public IEnumerable<byte[]> GetNextRawPacket()
        {
            foreach (System.IO.Stream frame in source)
            {
                int buffer_size = (int)frame.Length;
                var buffer = new byte[buffer_size];
                frame.Read(buffer, 0, buffer_size);
                yield return buffer;
            }
        }
    }

    class CLRTypeBasedMetadata<T> : ISchemaProvider
    {
        public IDictionary<string, KeyValuePair<uint, Type>> GetTargetSchemaFor(Type type)
        {
            throw new NotImplementedException();
        }

        public uint GetTargetRawMessageSizeFor(Type type)
        {
            throw new NotImplementedException();
        }
    }
#endregion
}