using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text.Json;
using Scrilla.Lib.ExternalApis;
using Microsoft.Extensions.Configuration;

namespace Scrilla.Lib.TradingPlatforms
{
    public abstract class TradingPlatform : ExternalApi
    {

        protected JsonSerializerOptions JsonOps = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly IConfiguration _config;


        public TradingPlatform() { }

        public TradingPlatform(IConfiguration config)
        {
            _config = config;
        }

        protected string GetFiatConversion(string fiatToConvertTo)
        {
            return null;
        }

        protected byte[] HashHMAC256(byte[] key, byte[] message)
        {
            var hash = new HMACSHA256(key);
            return hash.ComputeHash(message);
        }

        protected byte[] HashHMAC512(byte[] key, byte[] message)
        {
            var hash = new HMACSHA512(key);
            return hash.ComputeHash(message);
        }

        protected byte[] StringEncode(string text)
        {
            var encoding = new UTF8Encoding();
            return encoding.GetBytes(text);
        }

        /// <summary>
        /// Convert byte array to Hash string
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        protected string HashEncode(byte[] hash)
        {
            return BitConverter.ToString(hash).Replace("-", "");
        }

       
    }
}
