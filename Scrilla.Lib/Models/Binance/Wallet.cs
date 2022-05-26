using Scrilla.Lib.Attributes;
using Scrilla.Lib.Generics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrilla.Lib.Models.Binance
{
    [WalletSource(typeof(Wallet), "Binance")]
    public class Wallet : BaseWallet
    {
        [WalletProperty("CoinName")]
        public string Coin { get; set; }
        public string Name { get; set; }
        public bool DepositAllEnable { get; set; }
        [WalletProperty("Amount")]
        public double Free { get; set; }
        public double Freeze { get; set; }
        public double Ipoable { get; set; }
        public bool IsLegalMoney { get; set; }
        public double Locked { get; set; }
        public double Storage { get; set; }
        public bool Trading { get; set; }
        public bool WithdrawAllEnable { get; set; }
        public double Withdrawing { get; set; }
        //public List<TradeNetwork> NetworkList { get; set; }
    }
}
