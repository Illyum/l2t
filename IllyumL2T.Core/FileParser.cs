using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using IllyumL2T.Core.Interfaces;

namespace IllyumL2T.Core
{
  public class FileParser<T> where T : class, new()
  {
    protected ILineParser<T> _lineParser;

    public FileParser()
    {
      _lineParser = new LineParser<T>();
    }

    public IEnumerable<ParseResult<T>> Read(StreamReader reader, char delimiter, bool includeHeaders)
    {
      if(reader == null)
      {
        throw new ArgumentNullException("StreamReader reader");
      }

      // If the file contains headers in the first row, simply skip those...
      if(includeHeaders)
      {
        reader.ReadLine();
      }

      while(true)
      {
        var line = reader.ReadLine();
        if(String.IsNullOrEmpty(line) == false)
        {
          var lineParser = new LineParser<T>();
          var parseResult = lineParser.Parse(line, delimiter);
          yield return parseResult;
        }
        else
        {
          yield break;
        }
      }
    }
  }
}
