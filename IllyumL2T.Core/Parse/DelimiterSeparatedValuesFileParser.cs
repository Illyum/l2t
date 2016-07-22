using System;

using IllyumL2T.Core.FieldsSplit;

namespace IllyumL2T.Core.Parse
{
  
  public class DelimiterSeparatedValuesFileParser<T> : ValuesFileParser<T> where T : class, new()
  {
    public override FieldsSplitterBase<T> CreateValuesFieldsSplitter(char? delimiter = null)
    {
      if (!delimiter.HasValue)
      {
        throw new ArgumentNullException(nameof(delimiter));
      }
      var fieldsSplitter = new DelimiterSeparatedValuesFieldsSplitter<T>(delimiter.Value);
      return fieldsSplitter;
    }
  }
}