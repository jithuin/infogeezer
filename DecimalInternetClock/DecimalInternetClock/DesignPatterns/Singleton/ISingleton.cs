using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DecimalInternetClock.DesignPatterns.Singleton
{
    public interface ISingleton
    {
        ISingleton Instance { get; }
    }
}