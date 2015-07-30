using System;
using System.Globalization;

namespace IllyumL2T.Core.UnitTests
{
  class Order
  {
    public short OrderId { get; set; }

    [ParseBehavior(NumberStyle = NumberStyles.AllowDecimalPoint)]
    public decimal Freight { get; set; }

    public string ShipAddress { get; set; }

    [IllyumL2T.Core.ParseBehavior(DateTimeFormat = "d/M/yyyy")]
    public DateTime DeliveryDate { get; set; }

    public override bool Equals(object other)
    {
      if(other is Order)
      {
        return ((Order) other).GetHashCode() == this.GetHashCode();
      }

      return false;
    }

    public override int GetHashCode()
    {
      return OrderId.GetHashCode() +
             Freight.GetHashCode() +
             ShipAddress.GetHashCode() +
             DeliveryDate.GetHashCode();
    }
  }
}
