using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scrilla.Lib.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrilla.Lib.Services.Tests
{
    [TestClass()]
    public class WalletServiceTests
    {
        [TestMethod()]
        public void MakeScrillaWalletFromBinanceWalletTest()
        {
            //Binance example wallet
            var wallet = new Scrilla.Lib.Models.Binance.Wallet()
            {
                Coin = "BTC",
                Free = 1234,
                Name = "Bitcoin"
            };

            var walletService = new WalletService();

            try
            {
                var sWallet = walletService.MakeScrillaWalletFromExchange(wallet);
                Assert.IsNotNull(sWallet);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }


        [TestMethod()]
        public void MakeScrillaWalletFromNewtonWalletTest()
        {
            //Binance example wallet
            var wallet = new Scrilla.Lib.Models.Newton.Wallet()
            {
                CoinName = "BTC",
                Value = 1234
            };

            var walletService = new WalletService();

            try
            {
                var sWallet = walletService.MakeScrillaWalletFromExchange(wallet);
                Assert.IsNotNull(sWallet);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}