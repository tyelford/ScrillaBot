using Microsoft.Extensions.Configuration;
using Scrilla.Lib.Models.ViewModels;
using Scrilla.Lib.TradingPlatforms.Binance;
using Scrilla.Lib.TradingPlatforms.Newton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scrilla.Web.Data
{
    public class WalletService
    {

        public IConfiguration _config;

        public WalletService(IConfiguration config)
        {
            _config = config;
        }


        public async Task<List<WalletView>> GetAllWalletsAsync()
        {

            //Get data from Binace Wallet
            Binance b = new Binance(_config["Binance:ApiKey"], _config["Binance:SecretKey"]);
            var bWallet = await b.GetWalletCoinsAsync();

            //Get data from Netwon Wallet
            Newton n = new Newton(_config["Newton:ClientId"], _config["Newton:SecretKey"]);
            var nWallet = await n.GetBalancesAsync();

            //Do something to get all wallets from all exchanges here
            var wallets = new List<WalletView>();

            foreach(var w in bWallet)
            {
                wallets.Add(new WalletView 
                { 
                    ExchangeName = "Binance",
                    CoinName = w.Coin,
                    Amount = w.Free,
                    Cad = w.Coin == "BTC" ? (w.Free * GetFakeBtcPrice()) : 0
                });
            }

            foreach(var w in nWallet)
            {
                wallets.Add(new WalletView
                {
                    ExchangeName = "Newton",
                    CoinName = w.Key,
                    Amount = w.Value,
                    Cad = w.Key == "BTC" ? (w.Value * GetFakeBtcPrice()) : 0
                });
            }

            return wallets.OrderBy(x => x.ExchangeName).ThenBy(x => x.CoinName).ToList();

        }

        private int GetFakeBtcPrice()
        {
            Random r = new Random();

            return r.Next(60000, 65000);
        }

    }
}
