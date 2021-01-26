using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Web;
using ScrillaLib.Models.Newton;
using ScrillaLib.Models.Common;

namespace ScrillaLib.TradingPlatforms.Newton
{

    //For Reference: 
    //https://newton.stoplight.io/docs/newton-api-docs/docs/authentication/Authentication.md

    public class Newton : ITradingPlatform
    {

        private readonly string ClientId = "";
        private readonly string SecretKey = "";

        private readonly string apiVersion = "/v1";  //Needs the leading slash here to make the auth work correctly

        private string baseUrl = "https://api.newton.co";

        /// <summary>
        /// Creates and sends a API message to the Newton provider
        /// Returns a Json string
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isPublic"></param>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        private async Task<string> SendApiMessageAsync(
            string path, 
            HttpMethod method, 
            bool isPublic, 
            Dictionary<string, string> queryParams,
            string data = null)
        {
            string url = baseUrl + path;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    //Create headers
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
                    if (!isPublic)
                    {
                        //Auth header for private apis
                        string requestEpochTime = GetEpochTime().ToString();
                        string NewtonApiAuth = CreateAuthenticationToken(
                            path,
                            requestEpochTime);

                        client.DefaultRequestHeaders.Add("NewtonAPIAuth", NewtonApiAuth);
                        client.DefaultRequestHeaders.Add("NewtonDate", requestEpochTime);

                    }

                    if (queryParams != null && queryParams.Count > 0)
                    {
                        //Assign query paramter to url
                        url = AddQueryParamsToUrl(queryParams, url);
                    }

                    //Create a get message
                    if(method == HttpMethod.Get)
                    {
                        var res = await client.GetAsync(url);
                        if (res.IsSuccessStatusCode)
                        {
                            return await res.Content.ReadAsStringAsync();
                        }
                        else
                        {
                            //This probably isn't the best way to handle a non-sucessful status
                            //code but it'll do for now
                            throw new Exception($"API returned: {res.StatusCode.ToString()}");
                        }
                    }

                    //Create a post message
                    else if(method == HttpMethod.Post)
                    {
                        HttpContent content = null;
                        if(data != null)
                        {
                            content = new StringContent(data, Encoding.UTF8, "application/json");
                        }
                        var res = await client.PostAsync(url, content);

                        if (res.IsSuccessStatusCode)
                        {
                            return await res.Content.ReadAsStringAsync();
                        }
                        else
                        {
                            //This probably isn't the best way to handle a non-sucessful status
                            //code but it'll do for now
                            throw new Exception($"API returned: {res.StatusCode.ToString()}");
                        }
                    }

                    //Catch all
                    else
                    {
                        throw new Exception($"Method not implemented in this library - method: {method}");
                    }

                }
            }
            catch (Exception err)
            {
                throw new Exception($"Problem creating and sending message to API {url} - {err.Message}");
            }
        }

        #region PublicEndpoints

        public async Task<Fees> GetFeesAsync()
        {
            string path = $"{apiVersion}/fees";
            var fees = await SendApiMessageAsync(path, HttpMethod.Get, true, null);
            return JsonSerializer.Deserialize<Fees>(fees, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<bool> GetHealthCheckAsync()
        {
            //This is hacky -- taking advantage of the upper exception
            try
            {
                string path = "/v1/health-check";
                var isAlive = await SendApiMessageAsync(path, HttpMethod.Get, true, null);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public async Task<Dictionary<string, TradeLimits>> GetMaximumTradeAmountsAsync()
        {
            string path = $"{apiVersion}/order/maximums";
            string max = await SendApiMessageAsync(path, HttpMethod.Get, true, null);
            return JsonSerializer.Deserialize<Dictionary<string, TradeLimits>>(max, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<Dictionary<string, TradeLimits>> GetMinimumTradeAmountsAsync()
        {
            string path = $"{apiVersion}/order/minimums";
            string min = await SendApiMessageAsync(path, HttpMethod.Get, true, null);
            return JsonSerializer.Deserialize<Dictionary<string, TradeLimits>>(min, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<Dictionary<string, Ticks>> GetTickSizesAsync()
        {
            string path = $"{apiVersion}/order/tick-sizes";
            string ticks = await SendApiMessageAsync(path, HttpMethod.Get, true, null);
            return JsonSerializer.Deserialize<Dictionary<string, Ticks>>(ticks, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<List<string>> GetSymbolsAsync()
        {
            string path = $"{apiVersion}/symbols";
            string symbols = await SendApiMessageAsync(path, HttpMethod.Get, true, null);
            return JsonSerializer.Deserialize<List<string>>(symbols);
        }

        #endregion

        #region PrivateEndpoints

        /// <summary>
        /// Get account balances
        /// </summary>
        /// <returns>Dictionary<string,decimal>/returns>
        public async Task<Dictionary<string, decimal>> GetBalancesAsync(string asset = null)
        {
            string path = $"{apiVersion}/balances";

            var qParams = new Dictionary<string, string>();
            if (asset != null)
            {
                qParams.Add("asset", asset);
            }

            var balances = await SendApiMessageAsync(path, HttpMethod.Get, false, qParams);

            return JsonSerializer.Deserialize<Dictionary<string, decimal>>(balances);
        }


        /// <summary>
        /// Cancel an order
        /// What order are we to cancel?
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CancelOrderAsync()
        {
            string path = $"{apiVersion}/order/cancel";
            try
            {
                var canceled = await SendApiMessageAsync(path, HttpMethod.Post, false, null);
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
            string path = $"{apiVersion}/order/history";

            var qParams = new Dictionary<string, string>();

            if (startDate != null)
            {
                var startEpoch = GetEpochTime(startDate);
                qParams.Add("start_date", startEpoch.ToString());
            }
            if (endDate != null)
            {
                var endEpoch = GetEpochTime(endDate);
                qParams.Add("end_date", endEpoch.ToString());
            }
            if (limit != null) qParams.Add("limit", limit.ToString());
            if (offset != null) qParams.Add("offset", offset.ToString());
            if (symbol != null) qParams.Add("symbol", symbol);
            if (timeInForce != TimeInForce.Type.NONE) qParams.Add("time_in_force", timeInForce.ToString());

            var orderHistory = await SendApiMessageAsync(path, HttpMethod.Get, false, qParams);

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
            string path = $"{apiVersion}/order/new";
            var newOrder = await SendApiMessageAsync(path, HttpMethod.Post, false, null);

        }


        public async Task<string> GetOpenOrdersAsync(
            int? limit = null,
            int? offset = null,
            string symbol = null,
            TimeInForce.Type timeInForce = TimeInForce.Type.NONE)
        {
            string path = $"{apiVersion}/order/open";

            var qParams = new Dictionary<string, string>();

            if (limit != null) qParams.Add("limit", limit.ToString());
            if (offset != null) qParams.Add("offset", offset.ToString());
            if (symbol != null) qParams.Add("symbol", symbol);
            if (timeInForce != TimeInForce.Type.NONE) qParams.Add("time_in_force", timeInForce.ToString());

            var openOrders = await SendApiMessageAsync(path, HttpMethod.Get, false, qParams);

            //TODO: This needs to be deserialized when I know what the data looks like
            return openOrders;
        }



        #endregion


        #region Helpers
        private string AddQueryParamsToUrl(Dictionary<string, string> queryParams, string url)
        {
            var builder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(builder.Query);

            foreach (var q in queryParams)
            {
                query[q.Key] = q.Value;
            }
            builder.Query = query.ToString();
            return builder.ToString();
        }


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
        private string CreateAuthenticationToken(string path, string headerDate, string contentType = "", string method = "GET", string body = "")
        {
            string[] signatureParams =
            {
                method,    //Request Type
                contentType,       //Content type Blank for GET, application/json for POST etc
                path,   //URL path
                body,      //Body
                headerDate
            };

            string signatureData = string.Join(':', signatureParams);

            byte[] computedSignature = HashHMAC(StringEncode(SecretKey), StringEncode(signatureData));

            return $"{ClientId}:{Convert.ToBase64String(computedSignature)}";
        }


        private byte[] HashHMAC(byte[] key, byte[] message)
        {
            var hash = new HMACSHA256(key);
            return hash.ComputeHash(message);
        }

        private byte[] StringEncode(string text)
        {
            var encoding = new UTF8Encoding();
            return encoding.GetBytes(text);
        }

        private long GetEpochTime(DateTime? suppliedTime = null)
        {
            TimeSpan t = new TimeSpan();
            if (suppliedTime == null)
            {
                t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            }
            else
            {
                t = (DateTime)suppliedTime - new DateTime(1970, 1, 1);
            }
            return (long)t.TotalSeconds;
        }
        #endregion
    }
}
