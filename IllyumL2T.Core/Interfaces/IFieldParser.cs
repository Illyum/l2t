using System;
using System.Collections.Generic;

namespace IllyumL2T.Core.Interfaces
{
  public interface IFieldParser
  {
    string FieldName { get; }

    Type FieldType { get; }

    object FieldValue { get; }

    string FieldInput { get; }

    ParseBehaviorAttribute ParseBehavior { get; }

    IEnumerable<string> Errors { get; }

    object Parse(string input);
  }
}