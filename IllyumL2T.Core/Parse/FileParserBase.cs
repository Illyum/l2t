using System;
using System.IO;
using System.Collections.Generic;
using IllyumL2T.Core.Interfaces;

namespace IllyumL2T.Core.FieldsSplit.Parse
{
  public class FileParserBase<T> where T : class, new()
  {
    protected IEnumerable<ParseResult<T>> Read(TextReader reader, ILineParser<T> lineParser, bool includeHeaders, bool skipEmptyLines)
    {
      if (reader == null)
      {
        throw new ArgumentNullException("TextReader reader");
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
        if (String.IsNullOrWhiteSpace(line))
        {
          if (skipEmptyLines)
          {
            continue;
          }
        }
        var parseResult = lineParser.Parse(line);
        yield return parseResult;
      }
    }
  }
}