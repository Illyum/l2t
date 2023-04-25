using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using IllyumL2T.Core.Interfaces;

namespace IllyumL2T.Core.Parse
{
  public static class FieldParserResolver
  {
    private static readonly Dictionary<Type, Func<IFieldParser, object>> _parseMethods;

    static FieldParserResolver()
    {
      _parseMethods = new Dictionary<Type, Func<IFieldParser, object>>();

      //
      // The same method for a Type parses Nullables and non-Nullables...
      //

      _parseMethods[typeof(String)] = fieldParser => fieldParser.FieldInput;
      _parseMethods[typeof(Nullable<Char>)] = ParseChar;
      _parseMethods[typeof(Nullable<Boolean>)] = ParseBoolean;
      _parseMethods[typeof(Nullable<Byte>)] = ParseByte;
      _parseMethods[typeof(Nullable<Int16>)] = ParseInt16;
      _parseMethods[typeof(Nullable<Int32>)] = ParseInt32;
      _parseMethods[typeof(Nullable<Int64>)] = ParseInt64;
      _parseMethods[typeof(Nullable<Single>)] = ParseSingle;
      _parseMethods[typeof(Nullable<Double>)] = ParseDouble;
      _parseMethods[typeof(Nullable<Decimal>)] = ParseDecimal;
      _parseMethods[typeof(Nullable<DateTime>)] = ParseDateTime;
      _parseMethods[typeof(Nullable<DateTimeOffset>)] = ParseDateTimeOffset;
      _parseMethods[typeof(Nullable<UInt64>)] = ParseUInt64;
    }

    public static Func<IFieldParser, object> For(Type type)
    {
      var key = _parseMethods.Keys.FirstOrDefault(p => p.IsAssignableFrom(type));
      if(key == default) throw new NotSupportedException($"Unsupported property type ({type?.FullName}).");
      return _parseMethods[key];
    }

    private static object ParseBoolean(IFieldParser fieldParser)
    {
      var input = fieldParser.FieldInput;
      if(String.IsNullOrEmpty(input))
      {
        return null;
      }
      
      var result = default(bool);
      if(bool.TryParse(input, out result))
      {
        return result;
      }
      else
      {
        return null;
      }
    }

    private static object ParseDateTime(IFieldParser fieldParser)
    {
      var input = fieldParser.FieldInput;
      if(String.IsNullOrEmpty(input))
      {
        return null;
      }

      var format = fieldParser.ParseBehavior.DateTimeFormat;
      var cultureInfo = new CultureInfo(fieldParser.ParseBehavior.CultureName);
      var dateTimeStyle = fieldParser.ParseBehavior.DateTimeStyle;

      var result = default(DateTime);
      if(DateTime.TryParseExact(input, format, cultureInfo, dateTimeStyle, out result))
      {
        return result;
      }

      return null;
    }

    private static object ParseDateTimeOffset(IFieldParser fieldParser)
    {
      var input = fieldParser.FieldInput;
      if(String.IsNullOrEmpty(input))
      {
        return null;
      }

      var format = fieldParser.ParseBehavior.DateTimeOffsetFormat;
      var cultureInfo = new CultureInfo(fieldParser.ParseBehavior.CultureName);
      var dateTimeStyle = fieldParser.ParseBehavior.DateTimeStyle;

      var result = default(DateTimeOffset);
      if(DateTimeOffset.TryParseExact(input, format, cultureInfo, dateTimeStyle, out result))
      {
        return result;
      }

      return null;
    }

    private static object ParseChar(IFieldParser fieldParser)
    {
      var input = fieldParser.FieldInput;

      if(!String.IsNullOrEmpty(input) && input.Length == 1)
      {
        return input[0];
      }

      return null;
    }

    private static object ParseByte(IFieldParser fieldParser)
    {
      return ParseNumber<byte>(fieldParser, byte.TryParse);
    }

    private static object ParseInt16(IFieldParser fieldParser)
    {
      return ParseNumber<Int16>(fieldParser, Int16.TryParse);
    }

    private static object ParseInt32(IFieldParser fieldParser)
    {
      return ParseNumber<Int32>(fieldParser, Int32.TryParse);
    }

    private static object ParseInt64(IFieldParser fieldParser)
    {
      return ParseNumber<Int64>(fieldParser, Int64.TryParse);
    }

    private static object ParseDecimal(IFieldParser fieldParser)
    {
      return ParseNumber<decimal>(fieldParser, decimal.TryParse);
    }

    private static object ParseSingle(IFieldParser fieldParser)
    {
      return ParseNumber<float>(fieldParser, float.TryParse);
    }

    private static object ParseDouble(IFieldParser fieldParser)
    {
      return ParseNumber<double>(fieldParser, double.TryParse);
    }

    private static object ParseUInt64(IFieldParser fieldParser)
    {
      return ParseNumber<UInt64>(fieldParser, UInt64.TryParse);
    }

    #region Support declarations and methods

    delegate bool Parse<T>(string input, NumberStyles numberStyle, CultureInfo cultureInfo, out T result) where T : struct;

    private static T? ParseNumber<T>(IFieldParser fieldParser, Parse<T> parse) where T : struct
    {
      var input = fieldParser.FieldInput;
      if(String.IsNullOrEmpty(input))
      {
        return null;
      }

      var numberStyle = fieldParser.ParseBehavior.NumberStyle;

      var cultureInfo = default(CultureInfo);
      var cultureName = fieldParser.ParseBehavior.CultureName;
      if(String.IsNullOrEmpty(cultureName) == false)
      {
        cultureInfo = new CultureInfo(cultureName);
      }

      var result = default(T);
      if(parse(input, numberStyle, cultureInfo, out result))
      {
        return result;
      }
      else
      {
        return null;
      }
    }

    #endregion
  }
}