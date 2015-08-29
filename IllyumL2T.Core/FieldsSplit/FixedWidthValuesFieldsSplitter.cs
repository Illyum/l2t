using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IllyumL2T.Core.FieldsSplit.FieldsSplit
{
  public class FixedWidthValuesFieldsSplitter<T> : FieldsSplitterBase<T> where T : class, new()
  {
    private IEnumerable<int> propertyLengths;

    public FixedWidthValuesFieldsSplitter(bool trimInput = true) : base(trimInput)
    {
      propertyLengths = GetPropertyLengths(typeof(T));
    }
    private IEnumerable<int> GetPropertyLengths(Type type) => type.GetProperties().Select(p => (IllyumL2T.Core.ParseBehaviorAttribute)p.GetCustomAttributes(typeof(IllyumL2T.Core.ParseBehaviorAttribute), true).First()).Select(attr => attr.Length);

    public override string[] Split(string line)
    {
      if (line == null)
      {
        throw new ArgumentNullException(nameof(line));
      }

      var result = new List<string>();
      int index = 0;
      foreach (int next_length in propertyLengths)
      {
        if (index >= line.Length)
        {
          break;
        }
        string value = null;
        if (index + next_length - 1 < line.Length)
        {
          value = line.Substring(index, next_length);
          index += next_length;
        }
        result.Add(value);
      }
      return result.ToArray();
    }
  }
}