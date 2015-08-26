using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using IllyumL2T.Core.Interfaces;

namespace IllyumL2T.Core
{
  public class FileParser<T> where T : class, new()
  {
    //protected ILineParser<T> _lineParser; // this could be better design but ParseFileWithErrorsTest fails with it.

    //public FileParser()
    //{
    //    _lineParser = new LineParser<T>();
    //}

    public IEnumerable<ParseResult<T>> Read(StreamReader reader, char delimiter, bool includeHeaders)
    {
      if (reader == null)
      {
        throw new ArgumentNullException("StreamReader reader");
      }

      // If the file contains headers in the first row, simply skip those...
      if (includeHeaders)
      {
        reader.ReadLine();
      }

      while (true)
      {
        var line = reader.ReadLine();
        if (line == null) //This is the condition to finish the iteration. Based on http://msdn.microsoft.com/en-us/library/system.io.streamreader.readline(v=vs.110).aspx
        {
          yield break;
        }
        if (String.IsNullOrWhiteSpace(line)) //An empty line should mean a non-existing application object, not the end of the iteration.
        {
          continue;
        }
        var lineParser = new LineParser<T>();
        var parseResult = lineParser.Parse(line, delimiter);
        //var parseResult = _lineParser.Parse(line, delimiter); // this could be better design but ParseFileWithErrorsTest fails with it.
        yield return parseResult;
      }
    }

    public IEnumerable<ParseResult<T>> Read(System.IO.BinaryReader reader)
    {
      if (reader == null)
      {
        throw new ArgumentNullException("BinaryReader reader");
      }

      IPacketReader input = new PacketReader(reader);
      var parser = new FixedWidthBinaryParser<T>();
      foreach (var result in parser.Parse(input))
      {
        yield return result;
      }
    }
  }


  //Temporally here. Once evaluated, then they could go at the proper location.

  public class FixedWidthBinaryParser<T> where T : class, new()
  {
    private Type SchemaToRead;
    private int message_size;

    public FixedWidthBinaryParser()
    {
      SchemaToRead = typeof(T);
      message_size = CalculateTotalDeclaredSize(SchemaToRead);
    }

    public IEnumerable<ParseResult<T>> Parse(IPacketReader packetReader)
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

    /// <summary>
    /// A first try to link with the existing design.
    /// </summary>
    /// <param name="message">Data message</param>
    /// <returns>Values to be parsed by a LineParser-derived class.</returns>
    public string[] Parse(byte[] message)
    {
      var result = new List<string>();
      var buffer = new CircularBuffer((uint)message.Length);
      buffer.Add(message);

      foreach (int bytes_to_read in GetPropertyLengths(SchemaToRead))
      {
        string dataitem_as_string = string.Empty;
        if (bytes_to_read > 0)
        {
          byte[] dataitem_bytes = buffer.ReadBytes((uint)bytes_to_read);
          if (dataitem_bytes == null)
          {
            var exception_message = new System.Text.StringBuilder();
            exception_message.AppendFormat("No {0} bytes found for dataitem.", bytes_to_read);
            exception_message.AppendFormat("\r\nMessage Length: {0}", message.Length);
            throw new Exception(exception_message.ToString());
          }
          dataitem_as_string = Provisional_ForExploratoryPurposes_DefaultToUTF8String(dataitem_bytes);
        }
        result.Add(dataitem_as_string);
      }
      return result.ToArray();
    }
    private int CalculateTotalDeclaredSize(Type type) { return type.GetProperties().Select(p => (IllyumL2T.Core.ParseBehaviorAttribute)p.GetCustomAttributes(typeof(IllyumL2T.Core.ParseBehaviorAttribute), true).First()).Where(attr => attr.Length > 0).Sum(a => a.Length); }
    private IEnumerable<int> GetPropertyLengths(Type type) { return type.GetProperties().Select(p => (IllyumL2T.Core.ParseBehaviorAttribute)p.GetCustomAttributes(typeof(IllyumL2T.Core.ParseBehaviorAttribute), true).First()).Select(attr => attr.Length); }
    private string Provisional_ForExploratoryPurposes_DefaultToUTF8String(byte[] dataitem)
    {
      //TODO: check the possible requirements for binary vs text declarations.
      return System.Text.Encoding.UTF8.GetString(dataitem);
    }
  }

  /// <summary>
  /// This inheritance relationship is a key part of the actual link to the existing design.
  /// The link between a line-oriented parsing approach and the packet/message-oriented parsing approach.
  /// </summary>
  /// <typeparam name="T">Target application object type.</typeparam>
  public class MessageParser<T> : LineParser<T> where T : class, new()
  {
    public MessageParser() : base() { }

    public ParseResult<T> Parse(string[] values, byte[] message)
    {
      var parseResult = new ParseResult<T>()
      {
        Message = message,
        Instance = new T(),
      };

      ParseValues(values, parseResult);

      return parseResult;
    }
  }

  public interface IPacketReader
  {
    IEnumerable<byte[]> ReadNextPacket();
  }
  public interface IMessageReader
  {
    IEnumerable<byte[]> ReadNextMessageFrom(byte[] packet);
  }
  public class PacketReader : IPacketReader
  {
    private System.IO.BinaryReader source;

    public PacketReader(System.IO.BinaryReader reader)
    {
      source = reader;
    }

    /// <summary>
    /// From a given binary source, it provides all contained data packets from that binary source.
    /// A raw data packet could contain zero o more application messages or a fragment of an application message at the end of a data packet.
    /// </summary>
    /// <returns>Iterator for all raw data packets from a given binary source.</returns>
    public IEnumerable<byte[]> ReadNextPacket()
    {
      const int buffer_size = 0x400; //Question: What if the incoming packet is larger than this value? Answer: By the FixedWidthBinaryParser<T>.Parse method, as currently designed, such larger packet will be processed the same as a smaller one: each contained message will be processed and any fragmented message at the end of the packet would be completed with the completing fragment at the start of the next read packet. Moreover, in the case of a larger message inside a larger packet, the same applies, in principle, thanks to the adjusting size of CircularBuffer. Of course, execution-based evidence is needed to backup just that.
      byte[] buffer = new byte[buffer_size];
      do
      {
        int read = source.Read(buffer, 0, buffer_size);
        if (read == 0)
        {
          yield break;
        }
        byte[] packet = new byte[read];
        Array.Copy(buffer, packet, read);
        yield return packet;
      } while (true);
    }
  }

  public class MessageReader : IMessageReader
  {
    private CircularBuffer buffer;
    private uint message_size;

    public MessageReader(uint message_size) : this(new CircularBuffer(), message_size) { }

    public MessageReader(CircularBuffer buffer, uint message_size)
    {
      Reset(buffer, message_size);
    }

    public IEnumerable<byte[]> ReadNextMessageFrom(byte[] packet)
    {
      buffer.Add(packet);
      do //to fetch all messages from the packet
      {
        uint next_message_size = message_size;
        byte[] rawMessage = buffer.ReadBytes(next_message_size);
        if (rawMessage == null)
        {
          yield break;
        }
        yield return rawMessage;
      } while (true);
    }

    private void Reset(CircularBuffer buffer, uint message_size)
    {
      this.buffer = buffer;
      this.message_size = message_size;
    }
  }

  public class CircularBuffer
  {
    public uint CurrentMaxSize;
    public uint CurrentSize;
    private byte[] buffer;
    private uint write_index;
    private uint read_index;

    public CircularBuffer(uint initialMaxSize = 0x4800)
    {
      CurrentMaxSize = initialMaxSize;
      buffer = new byte[CurrentMaxSize];
      write_index = 0;
      read_index = 0;
      CurrentSize = 0;
    }

    public void Add(byte[] data)
    {
      if (data == null || data.Length <= 0)
      {
        return;
      }
      uint free_bytes = CurrentMaxSize - CurrentSize;
      if (data.Length > free_bytes)
      {
        uint newCurrentMaxSize = CurrentMaxSize + ((uint)data.Length - free_bytes);
        byte[] newbuffer = new byte[newCurrentMaxSize];
        if (CurrentSize > 0)
        {
          byte[] current_content = PreviewBytes(CurrentSize);
          Array.Copy(current_content, 0, newbuffer, 0, CurrentSize);
        }
        read_index = 0;
        write_index = CurrentSize;
        buffer = newbuffer;
        CurrentMaxSize = newCurrentMaxSize;
      }
      if (write_index + data.Length <= CurrentMaxSize)
      {
        Array.Copy(data, 0, buffer, write_index, data.Length);
      }
      else
      {
        uint this_cycle_remain = CurrentMaxSize - write_index;
        uint next_cycle_fragment = (uint)data.Length - this_cycle_remain;
        Array.Copy(data, 0, buffer, write_index, this_cycle_remain);
        Array.Copy(data, this_cycle_remain, buffer, 0, next_cycle_fragment);
      }
      CurrentSize += (uint)data.Length;
      write_index = (write_index + (uint)data.Length) % CurrentMaxSize;
    }

    public byte[] PreviewBytes(uint size)
    {
      byte[] result = null;
      if (size > 0 && size <= CurrentSize)
      {
        result = GetPreviewBytes(size);
      }
      return result;
    }

    private byte[] GetPreviewBytes(uint size)
    {
      byte[] result = null;
      if (size > 0)
      {
        result = new byte[size];
        if (read_index + size <= CurrentMaxSize)
        {
          Array.Copy(buffer, read_index, result, 0, size);
        }
        else
        {
          uint this_cycle_remain = CurrentMaxSize - read_index;
          uint next_cycle_fragment = size - this_cycle_remain;
          Array.Copy(buffer, read_index, result, 0, this_cycle_remain);
          Array.Copy(buffer, 0, result, this_cycle_remain, next_cycle_fragment);
        }
      }
      return result;
    }

    public byte[] ReadBytes(uint size)
    {
      byte[] result = null;
      if (size > 0)
      {
        result = PreviewBytes(size);
        if (result != null)
        {
          CurrentSize -= size;
          read_index = (read_index + size) % CurrentMaxSize;
        }
      }
      return result;
    }
  }
}