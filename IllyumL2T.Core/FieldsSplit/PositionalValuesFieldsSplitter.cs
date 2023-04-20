using System;
using System.Collections.Generic;

namespace IllyumL2T.Core.FieldsSplit
{
  public class PositionalValuesFieldsSplitter<T> : IllyumL2T.Core.FieldsSplit.FieldsSplitterBase<T> where T : class, new()
  {
    /// <summary>
    /// Break an input text line into individual parseable unit values.
    /// </summary>
    /// <param name="input_line">Input text line.</param>
    /// <returns>Parseable unit values.</returns>
    public override string[] Split(string input_line)
    {
      if (input_line == null)
      {
        throw new ArgumentNullException(nameof(input_line));
      }

      var result = new List<string>();

      int index_start_of_property = 0; //The start position of each property based on its declared IllyumL2T.Core.ParseBehaviorAttribute.Length.
      foreach (var fieldParser in FieldParsers) //FieldParsers are populated at base's constructor.
      {
        int property_declared_length = fieldParser.ParseBehavior.Length;
        if (property_declared_length <= 0)
        {
          throw new ArgumentOutOfRangeException("ParseBehaviorAttribute.Length");
        }

        //Splitting stops if the end limit of input line has passed.
        int input_segment_available_length = property_declared_length;
        if (index_start_of_property >= input_line.Length)
        {
          break;
        }

        //If actual input length is less of declared length then the actual input fragment is taken.
        if (index_start_of_property + property_declared_length >= input_line.Length)
        {
          input_segment_available_length = input_line.Length - index_start_of_property;
        }

        var input_line_segment = input_line.Substring(index_start_of_property, input_segment_available_length);

        /*
        The input line segment should be trimmed based on the following current facts:
        1. For consistency, as the input line segment is trimmed by the current policy at IllyumL2T.Core\FieldsSplit\DelimiterSeparatedValuesFieldsSplitter.cs
        2. For the actual .NET related behavior; that is, the difference in returned value of the TryParse methods for numeric data types; for instance:
            Int32.TryParse("123", System.Globalization.NumberStyles.AllowDecimalPoint, default(System.Globalization.CultureInfo), out n) -> True
            Int32.TryParse("  123", System.Globalization.NumberStyles.AllowDecimalPoint, default(System.Globalization.CultureInfo), out n) -> False
        */
        var input_line_segment_trimmed = input_line_segment.Trim();

        result.Add(input_line_segment_trimmed);
        index_start_of_property += property_declared_length;//Move forward to the start position of the next property.
      }

      return result.ToArray();
    }
  }
}