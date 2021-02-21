using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scrilla.Web.Data
{
    public class WalletService
    {

        public async Task<List<Wallet>> GetAllWalletsAsync()
        {
            //Do something to get all wallets from all exchanges here
            var wallets = new List<Wallet>();

            wallets.Add(new Wallet
            {
                ExchangeName = "Newton",
                CoinName = "BTC",
                Amount = 0.00341
            });

            wallets.Add(new Wallet
            {
                ExchangeName = "Binance",
                CoinName = "BTC",
                Amount = 0.00308
            });

            await Task.Delay(0);

            return wallets;

        }

    }
}
