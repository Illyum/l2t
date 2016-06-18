using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IllyumL2T.Core.FieldsSplit.UnitTests
{
  /// <summary>
  /// Application-level objects serialized as positional/fixed-width text line.
  /// </summary>
  class SimpleOrder
  {
    [IllyumL2T.Core.ParseBehavior(Length = 5)]  public short OrderId { get; set; }
    [IllyumL2T.Core.ParseBehavior(Length = 9)]  public decimal Freight { get; set; }
    [IllyumL2T.Core.ParseBehavior(Length = 50)] public string ShipAddress { get; set; }
    [IllyumL2T.Core.ParseBehavior(Length = 10)] public DateTime DeliveryDate { get; set; }
  }
}