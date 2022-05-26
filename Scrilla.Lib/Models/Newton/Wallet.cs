using Scrilla.Lib.Attributes;
using Scrilla.Lib.Generics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrilla.Lib.Models.Newton
{
    /// <summary>
    /// This wallet is FAKE, their API doc doesn't have one
    /// </summary>
    [WalletSource(typeof(Wallet), "Newton")]
    public class Wallet : BaseWallet
    {
        [WalletProperty("CoinName")]
        public string CoinName { get; set; }
        [WalletProperty("Amount")]
        public double Value { get; set; }
    }
}
