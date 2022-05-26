using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scrilla.Lib.Models.Binance;

namespace Scrilla.Lib.TradingPlatforms.BinanceTests.Tests
{
    [TestClass()]
    public class BinanceTests
    {

        string apiKey = "";
        string apiSecret = "";

        [TestInitialize]
        public void Init()
        {
            apiKey = Environment.GetEnvironmentVariable("B_API_KEY");
            apiSecret = Environment.GetEnvironmentVariable("B_API_SECRET");
        }

        [TestMethod()]
        public async Task GetSystemStatusAsyncTest()
        {
            //Arrange
            Type t = typeof(SystemStatus);

            //Act
            var b = new Binance(apiKey, apiSecret);
            var res = await b.GetSystemStatusAsync();

            //Assert
            Assert.AreEqual(res.GetType(), t);
            Assert.IsNotNull(res.Msg);
        }

        [TestMethod()]
        public async Task GetWalletCoinsAsyncTest()
        {
            //Arrange
            var b = new Binance(apiKey, apiSecret);

            //Act
            var res = await b.GetWalletCoinsAsync();

            //Assert
            Assert.IsNotNull(res);
        }
    }
}