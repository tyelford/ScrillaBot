using System;
using System.Collections.Generic;
using System.Text;

namespace Scrilla.Lib.Models.Newton
{
    /// <summary>
    /// Defines a static trade limit (max or min)
    /// </summary>
    public class TradeLimits
    {
        public float Buy { get; set; }
        public float Sell { get; set; }
    }
}
