using System;
using System.Collections.Generic;

namespace IllyumL2T.Core.Interfaces
{
  public interface ILineParser<T>
  {
    IEnumerable<IFieldParser> FieldParsers { get; }

    ParseResult<T> Parse(string line);
  }
}