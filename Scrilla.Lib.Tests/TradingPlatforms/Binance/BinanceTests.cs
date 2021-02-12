using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scrilla.Lib.Models.Binance.Wallet;
using Scrilla.Lib.TradingPlatforms.Binance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrilla.Lib.TradingPlatforms.Binance.Tests
{
    [TestClass()]
    public class BinanceTests
    {
        [TestMethod()]
        public async Task GetWalletStatusAsyncTest()
        {
            //Arrange
            Type t = typeof(SystemStatus);

            //Act
            var b = new Binance();
            var res = await b.GetWalletStatusAsync();

            //Assert
            Assert.AreEqual(res.GetType(), t);
            Assert.IsNotNull(res.Msg);
        }
    }
}