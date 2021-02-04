using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ScrillaLib.TradingPlatforms.BitBuy
{
    //For Reference:
    //https://bitbuy.ca/assets/api/docs/#/

    public class BitBuy : TradingPlatform
    {
        private readonly string ClientId = "";
        private readonly string SecretKey = "";

        private readonly string apiVersion = "/v1";  //Needs the leading slash here to make the auth work correctly

        private string baseUrl = "https://partner.bcm.exchange/api";



        public async Task SendApiMessage(Uri uri)
        {
            //Uri i = new Uri("http://www.mercymack")
        }



        #region helpers
        protected new Dictionary<string, string> GetAuthHeaders(
            Uri uri,
            string headerDate,
            string contentType = "",
            string method = "GET",
            string body = "",
            string contentLength = "")
        {
            var signatureData = JsonSerializer.Serialize(new
            {
                path = uri.AbsolutePath,
                query = uri.Query,
                content_length = contentLength
            });
            signatureData = signatureData.Replace("content_length", "content-length");
            //Java version - signatureData
            //JSONObject json = new JSONObject();
            //json.put("path", builder.getPath());
            //json.put("query", builder.getQuery());
            //json.put("content-length", body == null ? -1 : body.length());


            byte[] computedSignature = HashHMAC256(StringEncode(SecretKey), StringEncode(signatureData));

            Dictionary<string, string> authHeaders = new Dictionary<string, string>();

            //var s =  $"{ClientId}:{Convert.ToBase64String(computedSignature)}";

            return authHeaders;
        }

        #endregion
    }
}
