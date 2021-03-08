using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrilla.Lib.TradingPlatforms.Binance
{
    public class BinanceEndpoints
    {

        public static string GetSystemStatus = "/wapi/v3/systemStatus.html";

        public static string GetOpenOrders = "/api/v3/openOrders";
        public static string GetWalletCoins = "/sapi/v1/capital/config/getall";

    }
}
