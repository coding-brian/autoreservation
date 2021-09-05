using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace Service.WebAPIRequest
{
    public class WebAPIRequest
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
        /// <param name="contents">要傳入的body，請使用Json</param>
        /// <returns></returns>
        public async  Task WebRequest<T>(String url, HttpMethod method,List<Dictionary<String,String>> headers,List<T> contents) where T : class
        {
            var client = _httpclient.CreateClient();
            
            var request = new HttpRequestMessage(method, url);

            switch (url.ToString()) 
            {
                case "get":
                    Get(ref request, url, contents);
                    break;
                case "post":
                    Post(ref request, url, contents);
                    break;
            }


            if (headers.Count>0) 
            {
                foreach (var header in headers) 
                {
                    
                    foreach (var item in header) 
                    {
                        request.Headers.Add(item.Key, item.Value);
                    }
                }
            }

            

            await client.SendAsync(request);
        }



        private void Get<T>(ref HttpRequestMessage request,string url,List<T> queries)  where T:class
        {
            UriBuilder builder = new UriBuilder(url);
            var builderquery = HttpUtility.ParseQueryString(builder.Query);
            foreach (var query in queries) 
            {
                var dic=JsonSerializer.Deserialize<Dictionary<String, Object>>(JsonSerializer.Serialize(query));
                foreach (var item in dic) 
                {
                    builderquery.Add(item.Key,item.Value.ToString());
                }
            }
            builder.Query = builderquery.ToString();
            request.RequestUri = builder.Uri;
        }

        private void Post<T>(ref HttpRequestMessage request, string url, List<T> contents) where T : class
        {
            UriBuilder builder = new UriBuilder(url);
            HttpContent httpContent=new StringContent(JsonSerializer.Serialize(contents));
            request.Content = httpContent;
            request.RequestUri = builder.Uri;
        }

    }
}
