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
  }
}
