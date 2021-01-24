using System;

namespace IllyumL2T.Core.FieldsSplit.UnitTests
{
  class Bar
  {
    [IllyumL2T.Core.ParseBehavior(DateTimeOffsetFormat = "dd/MM/yyyy zzz")]
    public DateTimeOffset DateTimeOffsetProperty { get; set; }

    public override bool Equals(object other) => (other as Bar)?.GetHashCode() == GetHashCode();
    public override int GetHashCode() => DateTimeOffsetProperty.GetHashCode();
  }
}