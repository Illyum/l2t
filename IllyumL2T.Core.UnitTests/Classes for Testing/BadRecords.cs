using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IllyumL2T.Core.FieldsSplit.UnitTests
{
  class BadRecord
  {
    [IllyumL2T.Core.ParseBehavior(Length = 5)] public short OrderId { get; set; }
    [IllyumL2T.Core.ParseBehavior(Length = -1)] public decimal Whole { get; set; }
    [IllyumL2T.Core.ParseBehavior(Length = 5)] public string Code { get; set; }
  }

  class BadRecord2
  {
    [IllyumL2T.Core.ParseBehavior(Length = 5)] public short OrderId { get; set; }
    [IllyumL2T.Core.ParseBehavior(Length = 0)] public decimal Whole { get; set; }
    [IllyumL2T.Core.ParseBehavior(Length = 5)] public string Code { get; set; }
  }
}