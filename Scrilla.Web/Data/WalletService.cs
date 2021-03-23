using Microsoft.Extensions.Configuration;
using Scrilla.Lib.ExternalApis.CryptoCompare;
using Scrilla.Lib.Models.ViewModels;
using Scrilla.Lib.TradingPlatforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scrilla.Web.Data
{
    public class WalletService
    {

        public IConfiguration _config;
        public IEnumerable<ITradingPlatform> _tradingPlatforms;

        public WalletService(
            IConfiguration config,
            IEnumerable<ITradingPlatform> tradingPlatforms)
        {
            _config = config;
            _tradingPlatforms = tradingPlatforms;
        }


        public async Task<List<WalletView>> GetAllWalletViewsAsync()
        {
            List<WalletView> wvs = new List<WalletView>();

            foreach(var tp in _tradingPlatforms)
            {
                wvs.AddRange(await tp.GetWalletViewsBalancesAsync());
            }

            return wvs;
        }
    }
}
