using System;
using System.IO;
using System.Collections.Generic;

using IllyumL2T.Core.FieldsSplit;

namespace IllyumL2T.Core.Parse
{
  public class PositionalValuesFileParser<T> where T : class, new()
  {
    public IEnumerable<ParseResult<T>> Read(StreamReader reader, bool includeHeaders)
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

      var fieldsSplitter = new PositionalValuesFieldsSplitter<T>();
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
  }
}