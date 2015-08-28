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

    public IEnumerable<ParseResult<T>> Read(StreamReader reader, char delimiter, bool includeHeaders, bool skipEmptyLines = true)
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
      _lineParser = new LineParser<T>(fieldsSplitter);

      while(true)
      {
        var line = reader.ReadLine();
        if (line == null) //This is the condition to finish the iteration. Based on http://msdn.microsoft.com/en-us/library/system.io.streamreader.readline(v=vs.110).aspx
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
        var parseResult = _lineParser.Parse(line);
        yield return parseResult;
      }
    }
  }
}