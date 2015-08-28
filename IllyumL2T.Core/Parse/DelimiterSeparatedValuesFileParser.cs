using System.IO;
using System.Collections.Generic;
using IllyumL2T.Core.FieldsSplit;
using IllyumL2T.Core.FieldsSplit.Parse;

namespace IllyumL2T.Core.Parse
{
  public class DelimiterSeparatedValuesFileParser<T> : FileParserBase<T> where T : class, new()
  {
    public IEnumerable<ParseResult<T>> Read(TextReader reader, char delimiter, bool includeHeaders, bool skipEmptyLines = true)
    {
      var fieldsSplitter = new DelimiterSeparatedValuesFieldsSplitter<T>(delimiter);
      var lineParser = new LineParser<T>(fieldsSplitter);
      return base.Read(reader, lineParser, includeHeaders, skipEmptyLines);
    }
  }
}