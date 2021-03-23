using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Scrilla.Lib.Models.CryptoCompare;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Scrilla.Lib.ExternalApis.CryptoCompare
{
    public class CryptoCompare : ExternalApi
    {

        private readonly string ApiKey;

        private string BaseUrl = "https://min-api.cryptocompare.com";

        public CryptoCompare(string ApiKey)
        {
            this.ApiKey = ApiKey;
        }


        public async Task<string> SingleSymbolPrice(
            string cryptoSymbol,
            string[] fiatSymbols,
            bool tryConvert = true,
            bool relaxedValidation = true
            )
        {

            var fiatSymbolsCsv = string.Join(",", fiatSymbols);

            string path = CryptoCompareEndpoints.SingleSymbolPrice;

            var qParams = new Dictionary<string, string>();
            if (cryptoSymbol != null) qParams.Add("fsym", cryptoSymbol);
            if (!string.IsNullOrEmpty(fiatSymbolsCsv)) qParams.Add("tsyms", fiatSymbolsCsv);

            qParams.Add("tryConvert", tryConvert.ToString().ToLower());
            qParams.Add("relaxedValidation", relaxedValidation.ToString().ToLower());

            var uri = BuildUri(BaseUrl, path, qParams);

            var res = await SendApiMessageAsync(uri, HttpMethod.Get, false);
            return res;
        }


        public async Task<CryptoCompareConversions> MultiSymbolPrice(
            string[] cryptoSymbol,
            string[] fiatSymbols,
            bool tryConvert = true,
            bool relaxedValidation = true
            )
        {
            string path = CryptoCompareEndpoints.MultiSymbolPrice;

            var cryptoSumbolsCsv = string.Join(",", cryptoSymbol);
            var fiatSymbolsCsv = string.Join(",", fiatSymbols);

            var qParams = new Dictionary<string, string>();
            if (cryptoSymbol != null) qParams.Add("fsyms", cryptoSumbolsCsv);
            if (!string.IsNullOrEmpty(fiatSymbolsCsv)) qParams.Add("tsyms", fiatSymbolsCsv);

            qParams.Add("tryConvert", tryConvert.ToString().ToLower());
            qParams.Add("relaxedValidation", relaxedValidation.ToString().ToLower());

            var uri = BuildUri(BaseUrl, path, qParams);

            var res = await SendApiMessageAsync(uri, HttpMethod.Get, false);

            var converts = new CryptoCompareConversions();

            converts.ConversionResults = JsonConvert.DeserializeObject<Dictionary<string,Dictionary<string,double>>>(res);

            return converts;
        }


        protected override Dictionary<string, string> GetAuthHeaders(
            Uri uri,
            string headerDate,
            string contentType = "",
            string method = "GET",
            string body = "",
            string contentLength = "")
        {
            var authHeaders = new Dictionary<string, string>();

            authHeaders.Add("authorization", ApiKey);

            return authHeaders;
        }
    }
}
