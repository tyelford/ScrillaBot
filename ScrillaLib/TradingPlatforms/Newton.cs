using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Net.Http;
using System.Threading.Tasks;

namespace ScrillaLib.TradingPlatforms
{

    //For Reference: 
    //https://newton.stoplight.io/docs/newton-api-docs/docs/authentication/Authentication.md

    public class Newton: ITradingPlatform
    {

        private readonly string ClientId = "";
        private readonly string SecretKey = "";

        private string baseUrl = "https://api.newton.co/v1";

        public async Task<string> GetBalances()
        {
            string url = $"{baseUrl}/balances";

            string requestEpochTime = GetEpochTime().ToString();

            string NewtonApiAuth = CreateAuthenticationToken(requestEpochTime);

            //Tset this function
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("NewtonApiAuth", NewtonApiAuth);
            client.DefaultRequestHeaders.Add("NewtonDate", requestEpochTime);

            var stringTask = await client.GetStringAsync(url);

            return stringTask;
        }



        private string CreateAuthenticationToken(string headerDate)
        {

            string[] signatureParams =
            {
                "GET",    //Request Type
                "",       //Content type Blank for GET, application/json for POST etc
                "/api/PATH",   //URL path
                "",      //Body
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
