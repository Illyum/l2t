using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IllyumL2T.Core.FieldsSplit.UnitTests
{
  class SimpleOrder
  {
    [IllyumL2T.Core.ParseBehavior(Length = 5)]
    public short OrderId { get; set; }

    [IllyumL2T.Core.ParseBehavior(Length = 9, NumberStyle = NumberStyles.AllowDecimalPoint)]
    public decimal Freight { get; set; }

    [IllyumL2T.Core.ParseBehavior(Length = 50)]
    public string ShipAddress { get; set; }

    [IllyumL2T.Core.ParseBehavior(Length = 10, DateTimeFormat = "d/M/yyyy")]
    public DateTime DeliveryDate { get; set; }

    public override bool Equals(object other)
    {
      if (other is SimpleOrder)
      {
        return ((SimpleOrder)other).GetHashCode() == this.GetHashCode();
      }

      return false;
    }

    public override int GetHashCode()
    {
      int result =
        OrderId.GetHashCode() +
        Freight.GetHashCode() +
        DeliveryDate.GetHashCode();
      if (ShipAddress != null)
      {
        result += ShipAddress.GetHashCode();
      }
      return result;
    }
  }
}