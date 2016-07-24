using System;

using IllyumL2T.Core.FieldsSplit;

namespace IllyumL2T.Core.Parse
{
  public class PositionalValuesFileParser<T> : ValuesFileParser<T> where T : class, new()
  {
    public override FieldsSplitterBase<T> CreateValuesFieldsSplitter(char? delimiter = null)
    {
      var fieldsSplitter = new PositionalValuesFieldsSplitter<T>();
      return fieldsSplitter;
    }
  }
}