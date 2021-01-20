using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using ScrillaLib;
using ScrillaLib.TradingPlatforms;

namespace FunctionApp1
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task Run([TimerTrigger("*/5 * * * * *")]TimerInfo myTimer, ILogger log)
        {

            Newton n = new Newton();
            var bal = await n.GetBalances();
            
            log.LogInformation(bal);
            
        }
    }
}
