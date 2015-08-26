using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using IllyumL2T.Core.Interfaces;

namespace IllyumL2T.Core
{
  public class FieldParser : IFieldParser
  {
    protected Func<IFieldParser, object> _parseMethod;

    public FieldParser(PropertyInfo propertyInfo)
    {
      FieldName = propertyInfo.Name;
      FieldType = propertyInfo.PropertyType;

      //
      // Look for the ParseBehaviorAttribute if there is one defined for the property.
      // If not, create a new with its defaults...
      //
      var attributes = propertyInfo.GetCustomAttributes(false);
      var attribute = (ParseBehaviorAttribute) attributes.SingleOrDefault(a => a.GetType().IsAssignableFrom(typeof(ParseBehaviorAttribute)));
      ParseBehavior = attribute ?? new ParseBehaviorAttribute();

      //
      // Based on the type, determine the method to parse the value...
      //
      _parseMethod = FieldParserResolver.For(FieldType);

      _errors = new List<string>();
    }

    #region IFieldParser implementation

    public string FieldName { get; private set; }

    public Type FieldType { get; private set; }

    public object FieldValue { get; private set; }

    public byte[] FieldInput { get; private set; }

    public ParseBehaviorAttribute ParseBehavior { get; private set; }

    protected List<string> _errors;
    public IEnumerable<string> Errors
    {
      get { return _errors; }
    }
/*
    public object Parse(string input)
    {
      if(input == null)
      {
        throw new ArgumentNullException("input");
      }

      //
      // First off, we make a copy of the input we are about to parse...
      //
      FieldInput = input;

      //
      // ...then we reset the previous value if we are reusing the same FieldParser...
      //
      FieldValue = null;

      //
      // ...and finally, we reset the errors from the previous parse (if there was one)...
      //
      _errors.Clear();

      //
      // Check if the input matches the pattern if one is defined...
      //
      var pattern = ParseBehavior.Pattern;
      if(String.IsNullOrEmpty(pattern) == false)
      {
        var options = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.ExplicitCapture;
        if(Regex.IsMatch(input, pattern, options) == false)
        {
          _errors.Add(String.Format("{0}: {1} does not match pattern >>> {2}", FieldName, FieldInput, pattern));
          return null;
        }
      }

      FieldValue = _parseMethod(this);

      if(FieldValue != null)
      {
        return FieldValue;
      }

      //
      // Even if the parsed value was null, it is still valid if the field is nullable...
      //
      var isNullable = FieldType.IsGenericType &&
                       FieldType.GetGenericTypeDefinition() == typeof(Nullable<>);
      if(isNullable == false)
      {
        _errors.Add(String.Format("{0}: Unparsable {1} >>> {2}", FieldName, FieldType, FieldInput));
      }

      return null;
    }
*/
    public object Parse(byte[] input)
    {
      if (input == null)
      {
        throw new ArgumentNullException("input");
      }

      //
      // First off, we make a copy of the input we are about to parse...
      //
      FieldInput = input;

      //
      // ...then we reset the previous value if we are reusing the same FieldParser...
      //
      FieldValue = null;

      //
      // ...and finally, we reset the errors from the previous parse (if there was one)...
      //
      _errors.Clear();

      //
      // Check if the input matches the pattern if one is defined...
      //
      var pattern = ParseBehavior.Pattern;
      if (String.IsNullOrEmpty(pattern) == false)
      {
        var options = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.ExplicitCapture;
        if (Regex.IsMatch(input, pattern, options) == false)
        {
          _errors.Add(String.Format("{0}: {1} does not match pattern >>> {2}", FieldName, FieldInput, pattern));
          return null;
        }
      }

      FieldValue = _parseMethod(this);

      if (FieldValue != null)
      {
        return FieldValue;
      }

      //
      // Even if the parsed value was null, it is still valid if the field is nullable...
      //
      var isNullable = FieldType.IsGenericType &&
                       FieldType.GetGenericTypeDefinition() == typeof(Nullable<>);
      if (isNullable == false)
      {
        _errors.Add(String.Format("{0}: Unparsable {1} >>> {2}", FieldName, FieldType, FieldInput));
      }

      return null;
    }
    #endregion
  }
}