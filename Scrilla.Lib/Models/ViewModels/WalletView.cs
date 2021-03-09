using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrilla.Lib.Models.ViewModels
{
    public class WalletView
    {
        public string ExchangeName { get; set; }
        public string CoinName { get; set; }
        public double Amount { get; set; }
        public double Cad
        {
            get
            {
                if (_cad != 0)
                {
                    return Math.Round(_cad, 2);
                }
                return _cad;
            }
            set
            {
                _cad = value;
            }
        }
        private double _cad;
    }
}
