using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DecimalInternetClock
{
    interface IDecimalClock
    {
        DateTime Now { get; set; }
    }
}
