using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ScrillaLib.TradingPlatforms.Binance
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

        public async Task GetWalletStatusAsync()
        {
            string path = "/wapi/v3/systemStatus.html";
            var uri = BuildUri(baseUrl, path);
            try
            {
                var status = await SendApiMessageAsync(uri, HttpMethod.Get, false, useQueryParamForAuth:true);
                return;
            }
            catch(Exception)
            {
                return;
            }
        }


        public async Task<string> GetWalletCoinsAsync()
        {
            string path = "/sapi/v1/capital/config/getall";

            var qParams = new Dictionary<string, string>();
            //qParams.Add("symbol", "LTCBTC");
            //qParams.Add("side", "BUY");
            //qParams.Add("type", "LIMIT");
            //qParams.Add("timeInForce", "GTC");
            //qParams.Add("quantity", "1");
            //qParams.Add("price", "0.1");
            //qParams.Add("timestamp", "1499827319559");
            qParams.Add("recvWindow", "5000");
            qParams.Add("timestamp", GetEpochTime().ToString());

            var uri = BuildUri(baseUrl, path, qParams);

            var coins = await SendApiMessageAsync(uri, HttpMethod.Get, false, useQueryParamForAuth: true);

            return coins;
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
    }
}
