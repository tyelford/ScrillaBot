using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrilla.Lib.TradingPlatforms
{
    public class BinanceEndpoints
    {

        public static string GetSystemStatus = "/sapi/v1/system/status";

        public static string GetOpenOrders = "/api/v3/openOrders";
        public static string GetWalletCoins = "/sapi/v1/capital/config/getall";

    }
}
