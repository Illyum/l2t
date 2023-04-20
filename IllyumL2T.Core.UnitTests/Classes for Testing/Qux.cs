namespace IllyumL2T.Core.FieldsSplit.UnitTests;

class Qux
{
    public ushort UShortProperty { get; set; }
    public ushort? NullableUShortProperty { get; set; }

    public override bool Equals(object other) => (other as Qux)?.GetHashCode() == GetHashCode();

    public override int GetHashCode() =>
        UShortProperty.GetHashCode() +
        NullableUShortProperty?.GetHashCode() ?? 0;
}