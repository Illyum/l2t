using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using IllyumL2T.Core.Interfaces;
using IllyumL2T.Core.Parse;
using IllyumL2T.Core.FieldsSplit.FieldsSplit;

namespace IllyumL2T.Core.FieldsSplit.Marshal
{
  public class BinaryFileMarshaler<T> where T : class, new()
  {
    public IEnumerable<ParseResult<T>> Read(BinaryReader reader, char delimiter, bool includeHeaders, bool skipEmptyLines = true)
    {
      if (reader == null)
      {
        throw new ArgumentNullException("BinaryReader reader");
      }

      var lineParser = new LineParser<T>(new FixedWidthValuesFieldsSplitter<T>());
      //var parseResult = lineParser.Parse(line);
      //yield return parseResult;

      //reader.ReadChars() ->PacketReader

      //IPacketReader input = new PacketReader(reader);
      //var marshaler = new BinaryFixedWidthMarshaler<T>();
      //foreach (var result in marshaler.Read(input))
      //{
      //  yield return result;
      //}

      /*while (true)
      {
        var line = marshaler.ReadLine();
        if (line == null)
        {
          yield break;
        }
        if (String.IsNullOrWhiteSpace(line))
        {
          if (skipEmptyLines)
          {
            continue;
          }
        }
        var parseResult = lineParser.Parse(line);
        yield return parseResult;
      }*/

      return null;
    }
  }
/*
  //Temporally here. Once evaluated, then they could go at the proper location.
  //https://github.com/Illyum/l2t/blob/76a1e0823682737be672b18baf16cfa1a3b06c83/IllyumL2T.Core/FileParser.cs
  public class BinaryFixedWidthMarshaler<T> where T : class, new()
  {
    private Type SchemaToRead;
    private int message_size;


    public BinaryFixedWidthMarshaler()
    {
      SchemaToRead = typeof(T);
      message_size = CalculateTotalDeclaredSize(SchemaToRead);
    }

    public IEnumerable<ParseResult<T>> Read(IPacketReader packetReader)
    {
      IMessageReader messageReader = new MessageReader((uint)message_size);


      foreach (byte[] packet in packetReader.ReadNextPacket())
      {
        foreach (byte[] message in messageReader.ReadNextMessageFrom(packet))
        {
          string[] values = Parse(message);
          var messageParser = new MessageParser<T>();
          yield return messageParser.Parse(values, message);
        }
      }
    }

    private int CalculateTotalDeclaredSize(Type type) { return type.GetProperties().Select(p => (IllyumL2T.Core.ParseBehaviorAttribute)p.GetCustomAttributes(typeof(IllyumL2T.Core.ParseBehaviorAttribute), true).First()).Where(attr => attr.Length > 0).Sum(a => a.Length); }
    private IEnumerable<int> GetPropertyLengths(Type type) { return type.GetProperties().Select(p => (IllyumL2T.Core.ParseBehaviorAttribute)p.GetCustomAttributes(typeof(IllyumL2T.Core.ParseBehaviorAttribute), true).First()).Select(attr => attr.Length); } 
  }
*/
}