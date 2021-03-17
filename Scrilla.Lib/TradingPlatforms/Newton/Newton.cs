using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using Scrilla.Lib.Models.Newton;
using Scrilla.Lib.Models.Common;
using Scrilla.Lib.Models.ViewModels;
using Microsoft.Extensions.Configuration;

namespace Scrilla.Lib.TradingPlatforms
{

    //For Reference: 
    //https://newton.stoplight.io/docs/newton-api-docs/docs/authentication/Authentication.md

    public class Newton : TradingPlatform, ITradingPlatform
    {

        private readonly string ClientId;
        private readonly string SecretKey;

        private string BaseUrl = "https://api.newton.co";

        private readonly IConfiguration _config;

        public Newton(IConfiguration config) 
        {
            _config = config;
            this.ClientId = _config["Newton:ClientId"];
            this.SecretKey = _config["Newton:SecretKey"];
        }

        public Newton(string ClientId, string SecretKey)
        {
            this.ClientId = ClientId;
            this.SecretKey = SecretKey;
        }

        #region InterfaceMethods

        public async Task<List<WalletView>> GetWalletViewsBalancesAsync()
        {
            var balances = await GetBalancesAsync();

            List<WalletView> wvs = new List<WalletView>();
            foreach(var b in balances)
            {
                wvs.Add(new WalletView
                {
                    ExchangeName = "Newton",
                    CoinName = b.Key,
                    Amount = b.Value
                });
            }

            return wvs;
        }

        #endregion

        #region PublicEndpoints

        public async Task<Fees> GetFeesAsync()
        {
            string path = NewtonEndpoints.Fees;
            var uri = BuildUri(BaseUrl, path);
            

            var fees = await SendApiMessageAsync(uri, HttpMethod.Get, true);
            return JsonSerializer.Deserialize<Fees>(fees, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<bool> GetHealthCheckAsync()
        {
            //This is hacky -- taking advantage of the upper exception
            try
            {
                string path = NewtonEndpoints.HealthCheck;
                var uri = BuildUri(BaseUrl, path);
                var isAlive = await SendApiMessageAsync(uri, HttpMethod.Get, true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public async Task<Dictionary<string, TradeLimits>> GetMaximumTradeAmountsAsync()
        {
            string path = NewtonEndpoints.MaximumTradeAmounts;
            var uri = BuildUri(BaseUrl, path);
            string max = await SendApiMessageAsync(uri, HttpMethod.Get, true);
            return JsonSerializer.Deserialize<Dictionary<string, TradeLimits>>(max, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<Dictionary<string, TradeLimits>> GetMinimumTradeAmountsAsync()
        {
            string path = NewtonEndpoints.MinimumTradeAmounts;
            var uri = BuildUri(BaseUrl, path);
            string min = await SendApiMessageAsync(uri, HttpMethod.Get, true);
            return JsonSerializer.Deserialize<Dictionary<string, TradeLimits>>(min, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<Dictionary<string, Ticks>> GetTickSizesAsync()
        {
            string path = NewtonEndpoints.TickSizes;
            var uri = BuildUri(BaseUrl, path);
            string ticks = await SendApiMessageAsync(uri, HttpMethod.Get, true);
            return JsonSerializer.Deserialize<Dictionary<string, Ticks>>(ticks, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<List<string>> GetSymbolsAsync()
        {
            string path = NewtonEndpoints.Symbols;
            var uri = BuildUri(BaseUrl, path);
            string symbols = await SendApiMessageAsync(uri, HttpMethod.Get, true);
            return JsonSerializer.Deserialize<List<string>>(symbols);
        }

        #endregion

        #region PrivateEndpoints

        /// <summary>
        /// Get account balances
        /// </summary>
        /// <returns>Dictionary<string,decimal>/returns>
        public async Task<Dictionary<string, double>> GetBalancesAsync(string asset = null)
        {
            string path = NewtonEndpoints.Balances;
            
            var qParams = new Dictionary<string, string>();
            if (asset != null)
            {
                qParams.Add("asset", asset);
            }

            var uri = BuildUri(BaseUrl, path, qParams);

            var balances = await SendApiMessageAsync(uri, HttpMethod.Get, false);

            return JsonSerializer.Deserialize<Dictionary<string, double>>(balances);
        }


        /// <summary>
        /// Cancel an order
        /// What order are we to cancel?
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CancelOrderAsync()
        {
            string path = NewtonEndpoints.CancelOrder;
            var uri = BuildUri(BaseUrl, path);
            try
            {
                var canceled = await SendApiMessageAsync(uri, HttpMethod.Post, false);
                return true;
            }

            catch (Exception)
            {
                // Problem cancelling order
                return false;
            }
        }

        /// <summary>
        /// Get order history
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="symbol"></param>
        /// <param name="timeInForce"></param>
        /// <returns></returns>
        public async Task<string> GetOrderHistoryAsync(
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? limit = null,
            int? offset = null,
            string symbol = null,
            TimeInForce.Type timeInForce = TimeInForce.Type.NONE)
        {
            string path = NewtonEndpoints.OrderHistory;

            var qParams = new Dictionary<string, string>();

            if (startDate != null)
            {
                var startEpoch = GetEpochTimeSeconds(startDate);
                qParams.Add("start_date", startEpoch.ToString());
            }
            if (endDate != null)
            {
                var endEpoch = GetEpochTimeSeconds(endDate);
                qParams.Add("end_date", endEpoch.ToString());
            }
            if (limit != null) qParams.Add("limit", limit.ToString());
            if (offset != null) qParams.Add("offset", offset.ToString());
            if (symbol != null) qParams.Add("symbol", symbol);
            if (timeInForce != TimeInForce.Type.NONE) qParams.Add("time_in_force", timeInForce.ToString());

            var uri = BuildUri(BaseUrl, path, qParams);

            var orderHistory = await SendApiMessageAsync(uri, HttpMethod.Get, false);

            //TODO: This needs to be deserialized when I know what the data looks like
            return orderHistory;
        }


        /// <summary>
        /// Create a new order
        /// </summary>
        /// <returns></returns>
        public async Task CreateNewOrderAsync()
        {
            //TODO: The documentation here is not good, needs review
            string path = NewtonEndpoints.NewOrder;
            var uri = BuildUri(BaseUrl, path);
            var newOrder = await SendApiMessageAsync(uri, HttpMethod.Post, false);

        }


        public async Task<string> GetOpenOrdersAsync(
            int? limit = null,
            int? offset = null,
            string symbol = null,
            TimeInForce.Type timeInForce = TimeInForce.Type.NONE)
        {
            string path = NewtonEndpoints.OpenOrders;

            var qParams = new Dictionary<string, string>();

            if (limit != null) qParams.Add("limit", limit.ToString());
            if (offset != null) qParams.Add("offset", offset.ToString());
            if (symbol != null) qParams.Add("symbol", symbol);
            if (timeInForce != TimeInForce.Type.NONE) qParams.Add("time_in_force", timeInForce.ToString());

            var uri = BuildUri(BaseUrl, path, qParams);

            var openOrders = await SendApiMessageAsync(uri, HttpMethod.Get, false);

            //TODO: This needs to be deserialized when I know what the data looks like
            return openOrders;
        }



        #endregion


        #region Helpers

        /// <summary>
        /// Create the authorization header
        /// Deafults set for simple get request but can be included for Posts
        /// </summary>
        /// <param name="path"></param>
        /// <param name="headerDate"></param>
        /// <param name="contentType"></param>
        /// <param name="method"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        protected override Dictionary<string, string> GetAuthHeaders(
            Uri uri, 
            string headerDate, 
            string contentType = "", 
            string method = "GET", 
            string body = "",
            string contentLength = "")
        {
            string[] signatureParams =
            {
                method,    //Request Type
                contentType,       //Content type Blank for GET, application/json for POST etc
                uri.AbsolutePath,   //URL path
                body,      //Body
                headerDate
            };

            string signatureData = string.Join(':', signatureParams);

            byte[] computedSignature = HashHMAC256(StringEncode(SecretKey), StringEncode(signatureData));

            Dictionary<string, string> authHeaders = new Dictionary<string, string>();

            authHeaders.Add("NewtonAPIAuth", $"{ClientId}:{Convert.ToBase64String(computedSignature)}");
            authHeaders.Add("NewtonDate", headerDate);

            return authHeaders;
        }

        
        #endregion
    }
}
