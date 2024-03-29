﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Text.RegularExpressions;

namespace IllyumL2T.Core.FieldsSplit
{
  public class DelimiterSeparatedValuesFieldsSplitter<T> : FieldsSplitterBase<T> where T : class, new()
  {
    public string Pattern { get; private set; }

    public DelimiterSeparatedValuesFieldsSplitter(char delimiter)
    {
      Pattern = GetRegexPatternToSplitFields(delimiter);
    }

    public override string[] Split(string input)
    {
      var options = RegexOptions.Singleline | RegexOptions.Compiled;
      var values = Regex.Split(input, Pattern, options);

      if(values.Count() != FieldParsers.Count())
      {
        throw new InvalidOperationException($"Values mismatch fields definition ({values.Count()} values vs {FieldParsers.Count()} defined properties).");
      }

      // Remove leading and trailing spaces and quotes. A future version could make the characters set configurable
      return values.Select(v => v.Trim('"', ' ')).ToArray();
    }

    string GetRegexPatternToSplitFields(char delimiter)
    {
      var pattern = ConfigurationManager.AppSettings["SplitByDelimiterRegexPattern"];

      if(String.IsNullOrEmpty(pattern))
      {
        pattern = @"(?=(?:[^\""]*\""[^\""]*\"")*(?![^\""]*\""))";
      }

      return Regex.Escape(delimiter.ToString()) + pattern;
    }
  }
}
