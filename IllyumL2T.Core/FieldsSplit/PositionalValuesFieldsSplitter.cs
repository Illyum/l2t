using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IllyumL2T.Core.FieldsSplit
{
  public class PositionalValuesFieldsSplitter<T> : IllyumL2T.Core.FieldsSplit.FieldsSplitterBase<T> where T : class, new()
  {
    public override string[] Split(string line)
    {
      var result = new List<string>();

      int index_start_of_property = 0; //The start position of each property based on its declared IllyumL2T.Core.ParseBehaviorAttribute.Length.
      foreach (var fieldParser in FieldParsers) //FieldParsers are populated at base's constructor.
      {
        int property_declared_length = fieldParser.ParseBehavior.Length;

        //Splitting stops if the end limit of input line has passed.
        int input_segment_available_length = property_declared_length;
        if (index_start_of_property >= line.Length)
        {
          break;
        }

        //If actual input length is less of declared length then the actual input fragment is taken.
        if (index_start_of_property + property_declared_length >= line.Length)
        {
          input_segment_available_length = line.Length - index_start_of_property;
        }

        var input_line_segment = line.Substring(index_start_of_property, input_segment_available_length);

        /*
        The input line segment should be trimmed based on the following current facts:
        1. For consistency, as the input line segment is trimmed by the current policy at IllyumL2T.Core\FieldsSplit\DelimiterSeparatedValuesFieldsSplitter.cs
        2. The actual .NET behavior, that is, the difference in returned value of the TryParse methods for numeric data types; for instance:
          Int32.TryParse("123", System.Globalization.NumberStyles.AllowDecimalPoint, default(System.Globalization.CultureInfo), out n) -> True
          Int32.TryParse("  123", System.Globalization.NumberStyles.AllowDecimalPoint, default(System.Globalization.CultureInfo), out n) -> False
        */
        var input_line_segment_trimmed = input_line_segment.Trim();

        result.Add(input_line_segment_trimmed);
        index_start_of_property += property_declared_length;//Move forward to the next property's start position.
      }

      return result.ToArray();
    }
  }
}