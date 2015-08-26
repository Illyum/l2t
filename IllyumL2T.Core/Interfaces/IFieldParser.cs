using System;
using System.Collections.Generic;

namespace IllyumL2T.Core.Interfaces
{
  public interface IFieldParser
  {
    string FieldName { get; }

    Type FieldType { get; }

    object FieldValue { get; }

    byte[] FieldInput { get; }

    ParseBehaviorAttribute ParseBehavior { get; }

    IEnumerable<string> Errors { get; }

    //object Parse(string input);
    object Parse(byte[] input);
  }
}