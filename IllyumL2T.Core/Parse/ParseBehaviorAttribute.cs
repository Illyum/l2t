﻿using System;
using System.Globalization;

using IllyumL2T.Core.Interfaces;

namespace IllyumL2T.Core
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class ParseBehaviorAttribute : Attribute
  {
    public string CultureName { get; set; }

    public string DateTimeFormat { get; set; }

    public DateTimeStyles DateTimeStyle { get; set; }

    public NumberStyles NumberStyle { get; set; }

    public string Pattern { get; set; }

    public ParseBehaviorAttribute()
      : this
        (
          cultureName: CultureInfo.CurrentCulture.Name,
          numberStyle: NumberStyles.None,
          dateTimeStyle: DateTimeStyles.None,
          dateTimeFormat: null,
          pattern: null
        )
    {
    }

    public ParseBehaviorAttribute
    (
      string cultureName,
      NumberStyles numberStyle,
      DateTimeStyles dateTimeStyle,
      string dateTimeFormat,
      string pattern
    )
    {
      CultureName = cultureName;
      NumberStyle = numberStyle;
      DateTimeStyle = dateTimeStyle;
      DateTimeFormat = dateTimeFormat;
      Pattern = pattern;
    }
  }
}