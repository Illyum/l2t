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

      int index = 0;
      foreach (var fieldParser in FieldParsers)
      {
        int field_length = fieldParser.ParseBehavior.Length;
        //result.Add(line.Substring(index, field_length));
        result.Add(line.Substring(index, field_length).Trim()); //There is analysis to do with this Trim
        index += field_length;
      }

      return result.ToArray();
    }
  }
}