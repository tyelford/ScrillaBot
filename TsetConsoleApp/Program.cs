using System;

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
            ScrillaLib.TradingPlatforms.Newton n = new ScrillaLib.TradingPlatforms.Newton();
            var s = n.GetBalances().Result;
            Console.WriteLine(s);
        }
    }
}
