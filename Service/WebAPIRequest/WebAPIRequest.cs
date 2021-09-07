using Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace Service.WebAPIRequest
{
    public class WebAPIRequest : IWebAPIRequest
    {

        private IHttpClientFactory _httpclient;
        public WebAPIRequest(IHttpClientFactory httpClientFactory)
        {
            _httpclient = httpClientFactory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="headers"></param>
        /// <param name="contents">要傳入的body</param>
        /// <returns></returns>
        public async Task<APIResoponse> WebRequest<T>(String url, HttpMethod method, Dictionary<String, String> headers, T contents) where T : class
        {
            var client = _httpclient.CreateClient();

            var request = new HttpRequestMessage(method, url);

            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var result = new APIResoponse();

            switch (url.ToString())
            {
                case "get":
                    Get(ref request, url, contents);
                    break;
                case "post":
                    Post(ref request, url, contents);
                    break;
            }


            if (headers.Count > 0)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            try 
            {
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                result.body = "";
                result.status = "200";

                
            } catch (Exception ex) 
            {
                result.body = ex.Message;
                result.status = "500";
                Console.Out.WriteLine($"WebRequest: url:{url},headers:{JsonSerializer.Serialize(headers)},body:{JsonSerializer.Serialize(contents)}");
            }
            return result;
        }



        private void Get<T>(ref HttpRequestMessage request, string url, T queries) where T : class
        {
            UriBuilder builder = new UriBuilder(url);
            var builderquery = HttpUtility.ParseQueryString(builder.Query);

            if (queries !=null) 
            {
                var dic = JsonSerializer.Deserialize<Dictionary<String, Object>>(JsonSerializer.Serialize(queries));
                foreach (var item in dic)
                {
                    builderquery.Add(item.Key, item.Value.ToString());
                }
            }

            //foreach (var query in queries)
            //{
            //    var dic = JsonSerializer.Deserialize<Dictionary<String, Object>>(JsonSerializer.Serialize(query));
            //    foreach (var item in dic)
            //    {
            //        builderquery.Add(item.Key, item.Value.ToString());
            //    }
            //}

            builder.Query = builderquery.ToString();
            request.RequestUri = builder.Uri;
        }

        private void Post<T>(ref HttpRequestMessage request, string url, T contents) where T : class
        {
            UriBuilder builder = new UriBuilder(url);
            HttpContent httpContent = new StringContent(JsonSerializer.Serialize(contents));
            request.Content = httpContent;
            request.RequestUri = builder.Uri;
        }

    }
}
