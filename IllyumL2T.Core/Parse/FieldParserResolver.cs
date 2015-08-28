using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using IllyumL2T.Core.Interfaces;

namespace IllyumL2T.Core.Parse
{
  public class FieldParserResolver
  {
    static Dictionary<Type, Func<IFieldParser, object>> _parseMethods;

    static FieldParserResolver()
    {
      _parseMethods = new Dictionary<Type, Func<IFieldParser, object>>();

      //
      // The same method for a Type parses Nullables and non-Nullables...
      //

      _parseMethods[typeof(String)] = fieldParser => { return fieldParser.FieldInput; };
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
      _parseMethods[typeof(Nullable<UInt16>)] = ParseUInt16;
      _parseMethods[typeof(Nullable<UInt32>)] = ParseUInt32;
      _parseMethods[typeof(Nullable<UInt64>)] = ParseUInt64;
    }

    public static Func<IFieldParser, object> For(Type type)
    {
      var key = _parseMethods.Keys.First(p => p.IsAssignableFrom(type));
      return _parseMethods[key];
    }

    static object ParseBoolean(IFieldParser fieldParser)
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

    static object ParseDateTime(IFieldParser fieldParser)
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

    static object ParseChar(IFieldParser fieldParser)
    {
      var input = fieldParser.FieldInput;

      if(!String.IsNullOrEmpty(input) && input.Length == 1)
      {
        return input[0];
      }

      return null;
    }

    static object ParseByte(IFieldParser fieldParser)
    {
      return ParseNumber<byte>(fieldParser, byte.TryParse);
    }

    static object ParseInt16(IFieldParser fieldParser)
    {
      return ParseNumber<Int16>(fieldParser, Int16.TryParse);
    }

    static object ParseInt32(IFieldParser fieldParser)
    {
      return ParseNumber<Int32>(fieldParser, Int32.TryParse);
    }

    static object ParseInt64(IFieldParser fieldParser)
    {
      return ParseNumber<Int64>(fieldParser, Int64.TryParse);
    }

    static object ParseUInt16(IFieldParser fieldParser)
    {
      return ParseNumber<UInt16>(fieldParser, UInt16.TryParse);
    }

    static object ParseUInt32(IFieldParser fieldParser)
    {
      return ParseNumber<UInt32>(fieldParser, UInt32.TryParse);
    }

    static object ParseUInt64(IFieldParser fieldParser)
    {
      return ParseNumber<UInt64>(fieldParser, UInt64.TryParse);
    }

    static object ParseDecimal(IFieldParser fieldParser)
    {
      return ParseNumber<decimal>(fieldParser, decimal.TryParse);
    }

    static object ParseSingle(IFieldParser fieldParser)
    {
      return ParseNumber<float>(fieldParser, float.TryParse);
    }

    static object ParseDouble(IFieldParser fieldParser)
    {
      return ParseNumber<double>(fieldParser, double.TryParse);
    }

    #region Support declarations and methods

    delegate bool Parse<T>(string input, NumberStyles numberStyle, CultureInfo cultureInfo, out T result) where T : struct;

    static T? ParseNumber<T>(IFieldParser fieldParser, Parse<T> parse) where T : struct
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
