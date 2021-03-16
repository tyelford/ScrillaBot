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
        public static void AddTradingPlatformServices(this IServiceCollection services)
        {
            var types = new List<Type> { typeof(Newton.Newton), typeof(Binance.Binance) };

            foreach(var t in types)
            {
                services.Add(new ServiceDescriptor(typeof(ITradingPlatform), t, ServiceLifetime.Transient));
            }
        }

        //public static void RegisterAllTypes<T>(this IServiceCollection services, Assembly[] assemblies,
        //ServiceLifetime lifetime = ServiceLifetime.Transient)
        //{
        //    var typesFromAssemblies = assemblies.SelectMany(a => a.DefinedTypes.Where(x => x.GetInterfaces().Contains(typeof(T))));
        //    foreach (var type in typesFromAssemblies)
        //        services.Add(new ServiceDescriptor(typeof(T), type, lifetime));
        //}
    }
}
