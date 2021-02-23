using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Scrilla.Lib.Models.Binance.Wallet;

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

        #region Wallet Endpoints

        public async Task<SystemStatus> GetWalletStatusAsync()
        {
            string path = "/wapi/v3/systemStatus.html";
            var uri = BuildUri(baseUrl, path);
            try
            {
                var status = await SendApiMessageAsync(uri, HttpMethod.Get, false);
                return JsonSerializer.Deserialize<SystemStatus>(status, JsonOps);
            }
            catch(Exception err)
            {
                throw new Exception(err.Message);
            }
        }


        public async Task<string> GetWalletCoinsAsync(long? recvWindow = null)
        {
            string path = "/sapi/v1/capital/config/getall";

            var qParams = new Dictionary<string, string>();

            if(recvWindow != null) qParams.Add("recvWindow", recvWindow.ToString());

            qParams.Add("timestamp", GetEpochTimeMilliseconds().ToString());

            var uri = BuildUri(baseUrl, path, qParams);

            var coins = await SendApiMessageAsync(uri, HttpMethod.Get, false);

            return coins;
        }

        #endregion


        #region Order Endpoints

        public async Task<string> GetOpenOrdersAsync()
        {
            string path = "/api/v3/openOrders";

            var qParams = new Dictionary<string, string>();
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


        private string GenTimeStamp(DateTime baseDateTime)
        {
            var dtOffset = new DateTimeOffset(baseDateTime);
            return dtOffset.ToUnixTimeMilliseconds().ToString();
        }

        #endregion


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



    }
}
