using Scrilla.Lib.ExternalApis.CryptoCompare;
using Scrilla.Lib.TradingPlatforms;
using System;
using System.Threading.Tasks;

namespace TsetConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            new Tester();
        }
    }

    public class Tester
    {
        public Tester()
        {
            Task.Run(() => this.TestApiCalls()).Wait();
        }

        /// <summary>
        /// Test some api calls here asyncly
        /// </summary>
        /// <returns></returns>
        private async Task TestApiCalls()
        {
            //Binance b = new Binance();
            //var status = await b.GetWalletCoinsAsync();

            //Newton n = new Newton();
            //var s = await n.GetBalancesAsync();

            CryptoCompare c = new CryptoCompare("");
            var b = new string[] { "BTC", "XMR", "SFP", "ADA" };
            var f = new string[] { "CAD", "USD" };
            var s = await c.MultiSymbolPrice(b, f);
            return;
        }
    }
}
