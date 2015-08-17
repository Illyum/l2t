using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text.RegularExpressions;

using IllyumL2T.Core.Interfaces;

namespace IllyumL2T.Core
{
  public class LineParser<T> : ILineParser<T> where T : class, new()
  {
    List<string> _parseErrors;

    public LineParser()
    {
      _fieldParsers = new List<IFieldParser>();
      foreach(var propertyInfo in typeof(T).GetProperties())
      {
        _fieldParsers.Add(new FieldParser(propertyInfo));
      }
    }

    #region ILineParser implementation

    protected List<IFieldParser> _fieldParsers;
    public IEnumerable<IFieldParser> FieldParsers
    {
      get { return _fieldParsers; }
    }

    public ParseResult<T> Parse(string line, char delimiter)
    {
      var parseResult = new ParseResult<T>()
      {
        Line = line,
        Instance = new T(),
      };

      // Get the values to parse and set into the instance
      var pattern = GetRegexPatternToSplitFields(delimiter);
      var values = Regex.Split(line, pattern, RegexOptions.Singleline | RegexOptions.Compiled);

      ParseValues(values, parseResult);

      return parseResult;
    }

    #endregion

    #region Support methods

    protected void ParseValues(string[] values, ParseResult<T> parseResult)
    {
      if (values.Length != _fieldParsers.Count)
      {
        throw new InvalidOperationException("Values mismatch fields definition");
      }

      for (var i = 0; i < values.Length; ++i)
      {
        // Remove leading and trailing spaces and quotes. A future version could make the characters set configurable
        var value = values[i].Trim('"', ' ');
        var fieldParser = _fieldParsers[i];
        var property = parseResult.Instance.GetType().GetProperty(fieldParser.FieldName);
        var propertyValue = fieldParser.Parse(value);

        if (propertyValue != null)
        {
          property.SetValue(parseResult.Instance, propertyValue, null);
        }
        else
        {
          foreach (var error in fieldParser.Errors)
          {
            _parseErrors = _parseErrors ?? new List<string>();
            _parseErrors.Add(error);
          }
        }
      }

      if (_parseErrors != null)
      {
        parseResult.Instance = null;
        parseResult.Errors = _parseErrors;
      }
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

    #endregion
  }
}