using System;
using System.Collections.Generic;

using IllyumL2T.Core.Interfaces;
using IllyumL2T.Core.Parse;

namespace IllyumL2T.Core.FieldsSplit
{
  public abstract class FieldsSplitterBase<T> where T : class, new()
  {
    public IFieldParser[] FieldParsers { get; private set; }

    public FieldsSplitterBase()
    {
      var typeProperties = typeof(T).GetProperties();
      FieldParsers = new IFieldParser[typeProperties.Length]; //IFieldParser maybe should be renamed to IPropertyParser, as only .NET Properties are supported, not .NET Fields.
      for(int i = 0; i < typeProperties.Length; i++)
      {
        FieldParsers[i] = new FieldParser(typeProperties[i]);
      }
    }

    public abstract string[] Split(string line);
  }
}
