﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Scrilla.Lib.ExternalApis
{
    public abstract class ExternalApi
    {


        public async Task<string> SendApiMessageAsync(
            Uri uri,
            HttpMethod method,
            bool isPublic = false,
            string data = null,
            bool useQueryParamForAuth = false)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    //Create headers
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    if (!isPublic)
                    {
                        //Auth header for private apis
                        string requestEpochTime = GetEpochTimeSeconds().ToString();
                        Dictionary<string, string> authHeaders = GetAuthHeaders(
                            uri,
                            requestEpochTime,
                            method == HttpMethod.Post ? "application/json" : "",
                            method.ToString().ToUpper(),
                            data != null ? data : "");


                        if (useQueryParamForAuth)
                        {
                            //Some systems require the auth string as a query param
                            UriBuilder builder = new UriBuilder(uri);
                            var query = HttpUtility.ParseQueryString(builder.Query);
                            foreach (var h in authHeaders)
                            {
                                if (h.Key == "X-MBX-APIKEY") continue;
                                query[h.Key] = h.Value;
                                authHeaders.Remove(h.Key);
                            }
                            builder.Query = query.ToString();
                            uri = builder.Uri;
                        }

                        //Add any auth headers
                        foreach (var h in authHeaders)
                        {
                            client.DefaultRequestHeaders.Add(h.Key, h.Value);
                        }
                    }

                    //Create a get message
                    if (method == HttpMethod.Get)
                    {
                        var res = await client.GetAsync(uri);
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
                    else if (method == HttpMethod.Post)
                    {
                        HttpContent content = null;
                        if (data != null)
                        {
                            content = new StringContent(data, Encoding.UTF8, "application/json");
                        }
                        var res = await client.PostAsync(uri, content);

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
                throw new Exception($"Problem creating and sending message to API {uri} - {err.Message}");
            }
        }


        protected long GetEpochTimeSeconds(DateTime? suppliedTime = null)
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

        protected long GetEpochTimeMilliseconds(DateTime? suppliedTime = null)
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
            return (long)t.TotalMilliseconds;
        }

        /// <summary>
        /// Build a Uri
        /// </summary>
        /// <param name="url"></param>
        /// <param name="path"></param>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        protected Uri BuildUri(string url, string path, Dictionary<string, string> queryParams = null, bool deEncodeCommas = false)
        {
            UriBuilder builder = new UriBuilder(url);
            builder.Path = path;

            if (queryParams != null && queryParams.Count > 0)
            {
                var query = HttpUtility.ParseQueryString(builder.Query);
                foreach (var q in queryParams)
                {
                    query[q.Key] = q.Value;
                }
                builder.Query = query.ToString();
                //if (deEncodeCommas)
                //{
                //    builder.Query = builder.Query.Replace("%2c", ",");
                //}
            }
            return builder.Uri;
        }

        /// <summary>
        /// Needs to be overwritten in implementation
        /// </summary>
        /// <param name="path"></param>
        /// <param name="headerDate"></param>
        /// <param name="contentType"></param>
        /// <param name="method"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        protected virtual Dictionary<string, string> GetAuthHeaders(
            Uri uri,
            string headerDate,
            string contentType = "",
            string method = "GET",
            string body = "",
            string contentLength = "")
        {
            throw new NotImplementedException("Override me in concrete class");
        }

    }
}
