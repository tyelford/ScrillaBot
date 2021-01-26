﻿using ScrillaLib.TradingPlatforms.Newton;
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
            Newton n = new Newton();
            var f = await n.GetTickSizesAsync();

            return;
        }
    }
}
