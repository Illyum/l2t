using System;
using System.IO;
using System.Collections.Generic;

using IllyumL2T.Core.Interfaces;
using IllyumL2T.Core.FieldsSplit;

namespace IllyumL2T.Core.Parse
{
  public abstract class ValuesFileParser<T> where T : class, new()
  {
    protected ILineParser<T> _lineParser;

    public ValuesFileParser()
    {
      _lineParser = new LineParser<T>();
    }

    public IEnumerable<ParseResult<T>> Read(BinaryReader reader, byte group_separator, byte record_separator, byte unit_separator)
    {
      return ReadBinaryAsTemplateMethod(reader, group_separator, record_separator, unit_separator);
    }

    protected IEnumerable<ParseResult<T>> ReadBinaryAsTemplateMethod(BinaryReader reader, byte group_separator, byte record_separator, byte unit_separator)
    {
      if (reader == null)
      {
        throw new ArgumentNullException($"{nameof(BinaryReader)} reader");
      }

      yield break; //on construction...

      //FieldsSplitterBase<T> fieldsSplitter = CreateValuesFieldsSplitter((char)unit_separator);
      //var lineParser = new LineParser<T>(fieldsSplitter);

      //while (true)
      //{
      //  var record = reader.ReadLine(); //<-- here is the abstraction layer clash point.
      //  if (String.IsNullOrEmpty(record) == false)
      //  {
      //    var parseResult = lineParser.Parse(record);
      //    yield return parseResult;
      //  }
      //  else
      //  {
      //    yield break;
      //  }
      //}
    }

    /// <summary>
    /// Parses delimiter-separated values from text lines.
    /// </summary>
    /// <param name="reader">Input reader of text lines.</param>
    /// <param name="delimiter">Delimites values in text lines.</param>
    /// <param name="includeHeaders">Are there headers in the first input line?</param>
    /// <returns>Iterator with the parsed results.</returns>
    public IEnumerable<ParseResult<T>> Read(TextReader reader, char delimiter, bool includeHeaders)
    {
      return ReadTextAsTemplateMethod(reader, includeHeaders: includeHeaders, delimiter: delimiter);
    }

    /// <summary>
    /// Parses positional/fixed-width values from text lines.
    /// </summary>
    /// <param name="reader">Input reader of text lines.</param>
    /// <param name="includeHeaders">Are there headers in the first input line?</param>
    /// <returns>Iterator with the parsed results.</returns>
    public IEnumerable<ParseResult<T>> Read(TextReader reader, bool includeHeaders)
    {
      return ReadTextAsTemplateMethod(reader, includeHeaders: includeHeaders);
    }

    /// <summary>
    /// Defines the abstract (basic) structure of the textline-based parsing algorithm. The specific field splitting policy is deferred to subclasses.
    /// By Template Method we mean, quote: Define the skeleton of an algorithm in an operation, deferring some steps to subclasses. Template Method lets subclasses redefine certain steps of an algorithm without changing the algorithm's structure.
    /// http://www.dofactory.com/net/template-method-design-pattern
    /// </summary>
    /// <param name="reader">Input reader of text lines.</param>
    /// <param name="includeHeaders">Are there headers in the first input line?</param>
    /// <param name="delimiter">Optional. Used for the case of delimited field splitting policy.</param>
    /// <returns>Iterator with the parsed results.</returns>
    protected IEnumerable<ParseResult<T>> ReadTextAsTemplateMethod(TextReader reader, bool includeHeaders, char? delimiter = null)
    {
      if (reader == null)
      {
        throw new ArgumentNullException($"{nameof(TextReader)} reader");
      }

      // If the file contains headers in the first row, simply skip those...
      if (includeHeaders)
      {
        reader.ReadLine();
      }

      FieldsSplitterBase<T> fieldsSplitter = CreateValuesFieldsSplitter(delimiter);
      var lineParser = new LineParser<T>(fieldsSplitter);

      while (true)
      {
        var line = reader.ReadLine();
        if (String.IsNullOrEmpty(line) == false)
        {
          var parseResult = lineParser.Parse(line);
          yield return parseResult;
        }
        else
        {
          yield break;
        }
      }
    }

    public abstract FieldsSplitterBase<T> CreateValuesFieldsSplitter(char? delimiter = null);
  }
}