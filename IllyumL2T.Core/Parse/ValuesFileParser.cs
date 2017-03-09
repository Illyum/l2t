using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

using IllyumL2T.Core.Interfaces;
using IllyumL2T.Core.FieldsSplit;

namespace IllyumL2T.Core.Parse
{
  public abstract class ValuesFileParser<T> where T : class, new()
  {
    protected ILineParser<T> _lineParser;
    protected FileParseBehaviorAttribute _fileParseBehavior;

    public ValuesFileParser()
    {
      _lineParser = new LineParser<T>();

      var attributes = typeof(T).GetCustomAttributes(false);
      var attribute = (FileParseBehaviorAttribute)attributes.SingleOrDefault(a => a.GetType().IsAssignableFrom(typeof(FileParseBehaviorAttribute)));
      _fileParseBehavior = attribute ?? new FileParseBehaviorAttribute();
    }

    public IEnumerable<ParseResult<T>> Read(TextReader reader, char delimiter, bool includeHeaders)
    {
      return ReadAsTemplateMethod(reader, includeHeaders: includeHeaders, delimiter: delimiter);
    }

    public IEnumerable<ParseResult<T>> Read(TextReader reader, bool includeHeaders)
    {
      return ReadAsTemplateMethod(reader, includeHeaders: includeHeaders);
    }

    /// <summary>
    /// Defines the abstract (basic) structure of the parsing algorithm. The specific field splitting policy is deferred to subclasses.
    /// By Template Method we mean, quote: Define the skeleton of an algorithm in an operation, deferring some steps to subclasses. Template Method lets subclasses redefine certain steps of an algorithm without changing the algorithm's structure.
    /// http://www.dofactory.com/net/template-method-design-pattern
    /// </summary>
    /// <param name="reader">Input stream of text lines.</param>
    /// <param name="includeHeaders">Are there headers in the first input line?</param>
    /// <param name="delimiter">Optional. Used for the case of delimited field splitting policy.</param>
    /// <returns>Iterator with the parsed results.</returns>
    protected IEnumerable<ParseResult<T>> ReadAsTemplateMethod(TextReader reader, bool includeHeaders, char? delimiter = null)
    {
      if (reader == null)
      {
        throw new ArgumentNullException("StreamReader reader");
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
        if (line == null) //This is the condition to finish the iteration. Based on http://msdn.microsoft.com/en-us/library/system.io.streamreader.readline(v=vs.110).aspx
        {
          yield break;
        }

        bool blankLine = String.IsNullOrWhiteSpace(line);

        if (blankLine && _fileParseBehavior.BlankLineMode == BlankLineMode.Stop)
        {
          yield break;
        }

        if (blankLine && _fileParseBehavior.BlankLineMode == BlankLineMode.Skip)
        {
          continue;
        }

        if (blankLine && _fileParseBehavior.BlankLineMode == BlankLineMode.Nulled)
        {
          yield return new ParseResult<T> { Instance = default(T), Line = line };
        }
        else
        {
          yield return lineParser.Parse(line);
        }
      }
    }

    public abstract FieldsSplitterBase<T> CreateValuesFieldsSplitter(char? delimiter = null);
  }
}