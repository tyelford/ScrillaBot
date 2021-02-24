using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scrilla.Lib.Models.Binance;
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
        public async Task GetSystemStatusAsyncTest()
        {
            //Arrange
            Type t = typeof(SystemStatus);

            //Act
            var b = new Binance();
            var res = await b.GetSystemStatusAsync();

            //Assert
            Assert.AreEqual(res.GetType(), t);
            Assert.IsNotNull(res.Msg);
        }

        [TestMethod()]
        public async Task GetWalletCoinsAsyncTest()
        {
            //Arrange
            var b = new Binance();

            //Act
            var res = await b.GetWalletCoinsAsync();

            //Assert
            Assert.IsNotNull(res);
        }
    }
}