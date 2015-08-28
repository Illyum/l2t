namespace IllyumL2T.Core.FieldsSplit.UnitTests.Classes_for_Testing
{
  public class Record
  {
    [IllyumL2T.Core.ParseBehavior(Length = 2)] public ushort Type { get; set; }
    [IllyumL2T.Core.ParseBehavior(Length = 1)] public byte Category { get; set; }
    [IllyumL2T.Core.ParseBehavior(Length = 4)] public uint ID { get; set; }
    [IllyumL2T.Core.ParseBehavior(Length = 3)] public string Label { get; set; }
  }
}