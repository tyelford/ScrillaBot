using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrilla.Lib.Models.Binance
{
    public class TradeNetwork
    {
        public string AddressRegex { get; set; }
        public string Coin { get; set; }
        public string DepositDesc { get; set; }
        public bool DespositEnable { get; set; }
        public bool IsDefault { get; set; }
        public string MemoRegex { get; set; }
        public int MinConfirm { get; set; }
        public string Name { get; set; }
        public string Network { get; set; }
        public bool ResetAddressStatus { get; set; }
        public string SpecialTips { get; set; }
        public int UnlockConfirm { get; set; }
        public string WithdrawDesc { get; set; }
        public bool WithdrawEnable { get; set; }
        public double WithdrawFee { get; set; }
        public double WithdrawMin { get; set; }
    }
}
