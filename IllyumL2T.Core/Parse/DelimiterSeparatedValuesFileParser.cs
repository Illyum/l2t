using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using IllyumL2T.Core.Interfaces;
using IllyumL2T.Core.FieldsSplit;

namespace IllyumL2T.Core.Parse
{
  public class DelimiterSeparatedValuesFileParser<T> where T : class, new()
  {
    protected ILineParser<T> _lineParser;

    public DelimiterSeparatedValuesFileParser()
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

      var fieldsSplitter = new DelimiterSeparatedValuesFieldsSplitter<T>(delimiter);
      var lineParser = new LineParser<T>(fieldsSplitter);
      
      while(true)
      {
        var line = reader.ReadLine();
        if(String.IsNullOrEmpty(line) == false)
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
  }
}
