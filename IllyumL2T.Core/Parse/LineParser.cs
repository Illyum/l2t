using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using IllyumL2T.Core.Interfaces;
using IllyumL2T.Core.FieldsSplit;

namespace IllyumL2T.Core.Parse
{
  public class LineParser<T> : ILineParser<T> where T : class, new()
  {
    FieldsSplitterBase<T> _fieldsSplitter;
    List<string> _parseErrors;

    public LineParser()
      : this(new DelimiterSeparatedValuesFieldsSplitter<T>(delimiter: ','))
    {
    }

    public LineParser(FieldsSplitterBase<T> fieldsSplitter)
    {
      _fieldsSplitter = fieldsSplitter;
    }

    #region ILineParser implementation

    public IEnumerable<IFieldParser> FieldParsers
    {
      get { return _fieldsSplitter.FieldParsers; }
    }

    public ParseResult<T> Parse(string line)
    {
      //
      // This is a new parse so reset everything...
      //
      var parseResult = new ParseResult<T>()
      {
        Line = line,
        Instance = new T(),
      };

      _parseErrors = new List<string>();

      //
      // Get the values to parse and set into the instance
      //
      var values = _fieldsSplitter.Split(line);
      for(int i = 0; i < values.Count(); ++i)
      {
        var fieldProcessor = _fieldsSplitter.FieldParsers[i];
        var property = parseResult.Instance.GetType().GetProperty(fieldProcessor.FieldName);
        var propertyValue = fieldProcessor.Parse(values[i]);

        if(propertyValue != null)
        {
          property.SetValue(parseResult.Instance, propertyValue, null);
        }
        else
        {
          _parseErrors.AddRange(fieldProcessor.Errors.Select(e => e));
        }
      }

      if(_parseErrors.Any())
      {
        parseResult.Instance = null;
        parseResult.Errors = _parseErrors;
      }

      return parseResult;
    }

    #endregion
  }
}
