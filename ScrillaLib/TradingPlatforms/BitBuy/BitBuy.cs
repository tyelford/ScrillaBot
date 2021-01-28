using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ScrillaLib.TradingPlatforms.BitBuy
{
    //For Reference:
    //https://bitbuy.ca/assets/api/docs/#/

    public class BitBuy : ITradingPlatform
    {
        private readonly string ClientId = "";
        private readonly string SecretKey = "";

        private readonly string apiVersion = "/v1";  //Needs the leading slash here to make the auth work correctly

        private string baseUrl = "https://partner.bcm.exchange/api";


        #region helpers
        private string CreateAuthenticationToken(string path, string queryString, int contentLength)
        {
            var signatureData = JsonSerializer.Serialize(new
            {
                path = path,
                query = queryString,
                content_length = contentLength
            });
            signatureData = signatureData.Replace("content_length", "content-length");
            //Java version - signatureData
            //JSONObject json = new JSONObject();
            //json.put("path", builder.getPath());
            //json.put("query", builder.getQuery());
            //json.put("content-length", body == null ? -1 : body.length());


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
