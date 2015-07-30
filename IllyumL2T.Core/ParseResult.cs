using System;
using System.Collections.Generic;

using IllyumL2T.Core.Interfaces;

namespace IllyumL2T.Core
{
  public class ParseResult<T>
  {
    public T Instance { get; set; }

    public string Line { get; set; }

    public IEnumerable<string> Errors { get; set; }
  }
}
