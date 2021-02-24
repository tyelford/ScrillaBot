using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Scrilla.Lib.Models.Binance;

namespace Scrilla.Lib.TradingPlatforms.Binance
{
    //For reference:
    //https://binance-docs.github.io/apidocs/spot/en/#introduction


    public class Binance: TradingPlatform
    {
        private readonly string ApiKey = "";
        private readonly string SecretKey = "";

        private readonly string baseUrl = "https://api.binance.com";


        public Binance()
        {

        }


        #region System Endpoints
        public async Task<SystemStatus> GetSystemStatusAsync()
        {
            string path = BinanceEndpoints.GetSystemStatus;
            var uri = BuildUri(baseUrl, path);
            try
            {
                var status = await SendApiMessageAsync(uri, HttpMethod.Get, false);
                return JsonConvert.DeserializeObject<SystemStatus>(status);
            }
            catch(Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        #endregion

        #region Wallet Endpoints

        public async Task<List<Wallet>> GetWalletCoinsAsync(long? recvWindow = null)
        {
            string path = BinanceEndpoints.GetWalletCoins;

            var qParams = new Dictionary<string, string>();

            if(recvWindow != null) qParams.Add("recvWindow", recvWindow.ToString());

            qParams.Add("timestamp", GetEpochTimeMilliseconds().ToString());

            var uri = BuildUri(baseUrl, path, qParams);

            var coins = await SendApiMessageAsync(uri, HttpMethod.Get, false);
            var deseralized = JsonConvert.DeserializeObject<List<Wallet>>(coins);

            return deseralized.Where(x => x.Free > 0).ToList();
        }

        #endregion


        #region Order Endpoints

        public async Task<string> GetOpenOrdersAsync(string symbol = null, long? recvWindow = null)
        {
            string path = BinanceEndpoints.GetOpenOrders;

            var qParams = new Dictionary<string, string>();

            if (symbol != null) qParams.Add("symbol", symbol);
            if (recvWindow != null) qParams.Add("recvWindow", recvWindow.ToString());

            qParams.Add("timestamp", GetEpochTimeMilliseconds().ToString());
            var uri = BuildUri(baseUrl, path, qParams);
            string res = null;
            try
            {
                res = await SendApiMessageAsync(uri, HttpMethod.Get, false);
                return res;
            }
            catch(Exception err)
            {
                throw new Exception(err.Message);
            }
        }


        #endregion


        #region Helpers

        protected override Dictionary<string, string> GetAuthHeaders(
            Uri uri,
            string headerDate,
            string contentType,
            string method,
            string body = "",
            string contentLength = "")
        {

            //totalParams = query string + body
            string signatureData = uri.Query;

            signatureData = signatureData.StartsWith('?') ? signatureData.Substring(1) : signatureData;

            byte[] computedSignature = HashHMAC256(StringEncode(SecretKey), StringEncode(signatureData));

            Dictionary<string, string> authHeaders = new Dictionary<string, string>();

            authHeaders.Add("signature", HashEncode(computedSignature));
            authHeaders.Add("X-MBX-APIKEY", ApiKey);

            return authHeaders;
        }


        /// <summary>
        /// Wrapper for the super classes SendApiMessage
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="method"></param>
        /// <param name="isPublic"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task<string> SendApiMessageAsync(
            Uri uri,
            HttpMethod method,
            bool isPublic = false,
            string data = null)
        {
            return await base.SendApiMessageAsync(uri, method, isPublic, data, useQueryParamForAuth: true);
        }

        #endregion

    }
}
