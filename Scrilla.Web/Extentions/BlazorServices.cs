using Microsoft.Extensions.DependencyInjection;
using Scrilla.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrilla.Lib.Extentions
{
    public static class BlazorServices
    {
        public static void AddBlazorServices(this IServiceCollection services)
        {
            services.AddSingleton<WalletService>();
        }
    }
}
