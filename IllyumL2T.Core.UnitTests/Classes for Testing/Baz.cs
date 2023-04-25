using System;
using System.Globalization;

namespace IllyumL2T.Core.FieldsSplit.UnitTests
{
  class Baz
  {
    public bool BooleanProperty { get; set; }
    public bool? NullableBooleanProperty { get; set; }

    public override bool Equals(object other)
    {
      if(other is Baz)
      {
        return ((Baz) other).GetHashCode() == this.GetHashCode(); 
      }

      return false;
    }

    public override int GetHashCode()
    {
      return BooleanProperty.GetHashCode() +
             NullableBooleanProperty.GetHashCode();
    }
  }
}