using Microsoft.Extensions.DependencyInjection;
using Scrilla.Lib.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrilla.Lib.TradingPlatforms
{
    public interface ITradingPlatform
    {
        Task<List<WalletView>> GetWalletViewsBalancesAsync();
    }

    public static class TradingPlatformServices
    {
        public static void AddTradingPlatformServices(this IServiceCollection services, string[] enabledTradingPlatforms)
        {
            foreach(var t in enabledTradingPlatforms)
            {
                var type = Type.GetType($"Scrilla.Lib.TradingPlatforms.{t}, Scrilla.Lib");
                services.Add(new ServiceDescriptor(typeof(ITradingPlatform), type, ServiceLifetime.Transient));
            }
        }
    }
}
