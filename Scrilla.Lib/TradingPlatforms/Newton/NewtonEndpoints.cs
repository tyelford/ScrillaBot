using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrilla.Lib.TradingPlatforms
{
    public class NewtonEndpoints
    {

        public static string ApiVersion => "/v1";

        public static string Fees =>  ApiVersion + "/fees";
        public static string HealthCheck => ApiVersion + "/health-check";
        public static string MaximumTradeAmounts => ApiVersion + "/order/maximums";
        public static string MinimumTradeAmounts => ApiVersion + "/order/minimums";
        public static string TickSizes => ApiVersion + "/order/tick-sizes";
        public static string Symbols => ApiVersion + "/symbols";

        public static string Balances => ApiVersion + "/balances";
        public static string CancelOrder => ApiVersion + "/order/cancel";
        public static string OrderHistory => ApiVersion + "/order/history";
        public static string NewOrder => ApiVersion + "/order/new";
        public static string OpenOrders => ApiVersion + "/order/open";

    }
}
