using System.IO;
using System.Collections.Generic;
using IllyumL2T.Core.FieldsSplit.FieldsSplit;
using IllyumL2T.Core.Parse;

namespace IllyumL2T.Core.FieldsSplit.Parse
{
  public class FixedWidthValuesFileParser<T> : FileParserBase<T> where T : class, new()
  {
    public IEnumerable<ParseResult<T>> Read(TextReader reader, bool includeHeaders, bool skipEmptyLines = true, bool trimInput = true)
    {
      var fieldsSplitter = new FixedWidthValuesFieldsSplitter<T>(trimInput);
      var lineParser = new LineParser<T>(fieldsSplitter);
      return base.Read(reader, lineParser, includeHeaders, skipEmptyLines);
    }
  }
}