using System;
using System.Globalization;

namespace IllyumL2T.Core.FieldsSplit.UnitTests
{
  class Foo
  {
    public string StringProperty { get; set; }

    [IllyumL2T.Core.ParseBehavior(Pattern = @"\b[A-Z0-9a-z._%+-]+@(?:[A-Z0-9a-z-]+\.)+[A-Za-z]{2,4}\b")]
    public string StringPropertyWithPattern { get; set; }

    public char CharProperty { get; set; }

    public char? NullableCharProperty { get; set; }

    public byte ByteProperty { get; set; }

    public byte? NullableByteProperty { get; set; }

    public short Int16Property { get; set; }

    public short? NullableInt16Property { get; set; }

    public int Int32Property { get; set; }

    public int? NullableInt32Property { get; set; }

    public long Int64Property { get; set; }

    public long? NullableInt64Property { get; set; }

    [ParseBehavior(NumberStyle = NumberStyles.AllowDecimalPoint)]
    public float SingleProperty { get; set; }
    
    public float? NullableSingleProperty { get; set; }

    [ParseBehavior(NumberStyle = NumberStyles.AllowDecimalPoint)]
    public double DoubleProperty { get; set; }

    public double? NullableDoubleProperty { get; set; }

    [ParseBehavior(NumberStyle = NumberStyles.AllowDecimalPoint)]
    public decimal DecimalProperty { get; set; }

    public decimal? NullableDecimalProperty { get; set; }

    [IllyumL2T.Core.ParseBehavior(DateTimeFormat = "dd/MM/yyyy")]
    public DateTime DateTimeProperty { get; set; }

    [IllyumL2T.Core.ParseBehavior(DateTimeFormat = "dd/MM/yyyy")]
    public DateTime? NullableDateTimeProperty { get; set; }

    public override bool Equals(object other)
    {
      if(other is Foo)
      {
        return ((Foo) other).GetHashCode() == this.GetHashCode(); 
      }

      return false;
    }

    public override int GetHashCode()
    {
      // This could be the worst GetHashCode() implementation in history of mankind but it is enough for our purposes...

      return StringProperty.GetHashCode() +
             StringPropertyWithPattern.GetHashCode() +
             CharProperty.GetHashCode() +
             NullableCharProperty.GetHashCode() +
             ByteProperty.GetHashCode() +
             NullableByteProperty.GetHashCode() +
             Int16Property.GetHashCode() +
             NullableInt16Property.GetHashCode() +
             Int32Property.GetHashCode() +
             NullableInt32Property.GetHashCode() +
             Int64Property.GetHashCode() +
             NullableInt64Property.GetHashCode() +
             SingleProperty.GetHashCode() +
             NullableSingleProperty.GetHashCode() +
             DoubleProperty.GetHashCode() +
             NullableDoubleProperty.GetHashCode() +
             DecimalProperty.GetHashCode() +
             NullableDecimalProperty.GetHashCode() +
             DateTimeProperty.GetHashCode() +
             NullableDateTimeProperty.GetHashCode();
    }
  }
}