using System;
using System.IO;
using System.Collections.Generic;

using IllyumL2T.Core.Interfaces;
using IllyumL2T.Core.FieldsSplit;

namespace IllyumL2T.Core.Parse
{
  public abstract class ValuesFileParser<T> where T : class, new()
  {
    protected ILineParser<T> _lineParser;

    public ValuesFileParser()
    {
      _lineParser = new LineParser<T>();
    }

    public IEnumerable<ParseResult<T>> Read(StreamReader reader, char delimiter, bool includeHeaders)
    {
      return Read(reader, includeHeaders: includeHeaders, delimiter: delimiter);
    }
    public IEnumerable<ParseResult<T>> Read(StreamReader reader, bool includeHeaders)
    {
      return Read(reader, includeHeaders: includeHeaders);
    }
    protected IEnumerable<ParseResult<T>> Read(StreamReader reader, bool includeHeaders, char? delimiter = null)
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

      FieldsSplitterBase<T> fieldsSplitter = CreateValuesFieldsSplitter(delimiter);
      var lineParser = new LineParser<T>(fieldsSplitter);

      while (true)
      {
        var line = reader.ReadLine();
        if (String.IsNullOrEmpty(line) == false)
        {
          var parseResult = lineParser.Parse(line);
          yield return parseResult;
        }
        else
        {
          yield break;
        }
      }
    }

    public abstract FieldsSplitterBase<T> CreateValuesFieldsSplitter(char? delimiter = null);
  }
}