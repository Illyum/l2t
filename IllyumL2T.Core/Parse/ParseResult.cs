using System;
using System.Collections.Generic;

namespace IllyumL2T.Core
{
  public class ParseResult<T>
  {
    public T Instance { get; set; }

    public string Line { get; set; }
    public byte[] Bytes { get; set; }

    public IEnumerable<string> Errors { get; set; }
  }
}