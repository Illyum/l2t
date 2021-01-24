using System;
using System.Globalization;

using IllyumL2T.Core.Interfaces;

namespace IllyumL2T.Core
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class ParseBehaviorAttribute : Attribute
  {
    public string CultureName { get; set; }

    public string DateTimeFormat { get; set; }

    public string DateTimeOffsetFormat { get; set; }

    public DateTimeStyles DateTimeStyle { get; set; }

    public NumberStyles NumberStyle { get; set; }

    public string Pattern { get; set; }

    public int Length { get; set; } //Apparently, System.Nullable<uint> cannot be used within CLR attributes.

    public ParseBehaviorAttribute() : this
        (
          cultureName: CultureInfo.CurrentCulture.Name,
          numberStyle: NumberStyles.None,
          dateTimeStyle: DateTimeStyles.None,
          dateTimeFormat: null,
          pattern: null,
          length: int.MinValue
        )
    {
    }

    public ParseBehaviorAttribute
    (
      string cultureName,
      NumberStyles numberStyle,
      DateTimeStyles dateTimeStyle,
      string dateTimeFormat,
      string pattern,
      int length
    )
    {
      CultureName = cultureName;
      NumberStyle = numberStyle;
      DateTimeStyle = dateTimeStyle;
      DateTimeFormat = dateTimeFormat;
      Pattern = pattern;
      Length = length;
    }
  }
}