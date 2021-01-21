using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Web;

namespace ScrillaLib.TradingPlatforms
{

    //For Reference: 
    //https://newton.stoplight.io/docs/newton-api-docs/docs/authentication/Authentication.md

    public class Newton: ITradingPlatform
    {

        private readonly string ClientId = "";
        private readonly string SecretKey = "";

        private string baseUrl = "https://api.newton.co";

        /// <summary>
        /// Get account balances
        /// </summary>
        /// <returns>Dictionary<string,decimal>/returns>
        public async Task<Dictionary<string, decimal>> GetBalances(string asset = null)
        {
            string path = "/v1/balances";
            string url = baseUrl + path;

            string requestEpochTime = GetEpochTime().ToString();
            string NewtonApiAuth = CreateAuthenticationToken(
                path,
                requestEpochTime);

            //Send the message to the API
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("NewtonAPIAuth", NewtonApiAuth);
                client.DefaultRequestHeaders.Add("NewtonDate", requestEpochTime);
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");

                if(asset != null)
                {
                    var qParams = new Dictionary<string, string>();
                    qParams.Add("asset", asset);
                    url = AddQueryParamsToUrl(qParams, url);
                }

                var balances = await client.GetStringAsync(url);

                return JsonSerializer.Deserialize<Dictionary<string,decimal>>(balances);
            }
        }



        private string AddQueryParamsToUrl(Dictionary<string,string> queryParams, string url)
        {
            var builder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(builder.Query);
            
            foreach(var q in queryParams)
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

            return  $"{ClientId}:{Convert.ToBase64String(computedSignature)}";
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

        private long GetEpochTime()
        {
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return (long)t.TotalSeconds;
        }
    }
}
