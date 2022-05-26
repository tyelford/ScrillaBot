using Microsoft.Extensions.DependencyInjection;
using Scrilla.Lib.Attributes;
using Scrilla.Lib.Generics;
using Scrilla.Lib.Models.Scrilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Scrilla.Lib.Services
{

    public interface IWalletService
    {

    }

    //public static partial class AddServices
    //{
    //    public static IEnumerable<object> AddWalletServies(this, IServiceCollection services)
    //    {
    //        services.AddSingleton<IWalletService, WalletService>();
    //        return services;
    //    }
    //}

    public class WalletService : IWalletService
    {

        public WalletService()
        {
            //cotr
        }

        /// <summary>
        /// This takes a BaseWallet and uses the attributes on the wallet definition to convert it to a Scrilla Wallet
        /// </summary>
        /// <param name="baseWallet"></param>
        /// <returns></returns>
        public Wallet MakeScrillaWalletFromExchangeWallet(BaseWallet baseWallet)
        {
            try
            {
                var type = baseWallet.GetType();

                var classProp = type.GetCustomAttributes(typeof(WalletSourceAttribute), true).FirstOrDefault() as WalletSourceAttribute;
            
                if(classProp != null)
                {
                    var exchangeName = classProp.sourceName;

                    var attributes = type.GetCustomAttribute<WalletPropertyAttribute>();

                    var scrillaWallet = new Wallet()
                    {
                        ExchangeName = exchangeName
                    };
                    var targetType = typeof(Wallet);

                    var props = type.GetProperties().ToList().Where(x => x.GetCustomAttribute<WalletPropertyAttribute>() != null);

                    foreach(var prop in props)
                    {
                        var walletProp = prop.GetCustomAttribute<WalletPropertyAttribute>();
                        var targetProp = targetType.GetProperty(walletProp.propName);

                        var targetPropType = targetProp.PropertyType;

                        var sourceValue = prop.GetValue(baseWallet);

                        if(sourceValue == null)
                        {
                            continue;
                        }

                        if(sourceValue?.GetType() == targetPropType)
                        {
                            targetProp.SetValue(scrillaWallet, sourceValue);
                        }
                        else
                        {
                            //Need to case to the target type
                            if (targetPropType == typeof(string))
                            {
                                targetProp.SetValue(scrillaWallet, sourceValue?.ToString());
                            }
                            if (targetPropType == typeof(double)){
                                targetProp.SetValue(scrillaWallet, (double)sourceValue);
                            }
                        }
                    }

                    return scrillaWallet;
                }

                return null;
            }
            catch(Exception ex)
            {
                throw new Exception($"Could not map BaseWallet to ScrillaWallet: {ex.Message}");
            }
        }

    }
}
