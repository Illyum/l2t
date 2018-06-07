namespace IllyumL2T.Core.FieldsSplit.UnitTests
{
  class AssociativeArrayEntry
  {
    public short Id { get; set; }
    public string Name { get; set; }

    public override bool Equals(object other)
    {
      if (other is AssociativeArrayEntry)
      {
        return ((AssociativeArrayEntry)other).GetHashCode() == this.GetHashCode();
      }

      return false;
    }

    public override int GetHashCode()
    {
      int result = Id.GetHashCode();
      if (Name != null)
      {
        result += Name.GetHashCode();
      }
      return result;
    }
  }
}