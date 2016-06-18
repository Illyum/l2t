using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace IllyumL2T.Core.Parse
{
  public class PositionalValuesFileParser<T> where T : class, new()
  {
    public IEnumerable<ParseResult<T>> Read(StreamReader reader, bool includeHeaders)
    {
      return null;
    }
  }
}