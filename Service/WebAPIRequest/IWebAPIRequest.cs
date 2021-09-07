using Model;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Service.WebAPIRequest
{
    public interface IWebAPIRequest
    {
        Task<APIResoponse> WebRequest<T>(string url, HttpMethod method, Dictionary<string, string> headers, T contents) where T : class;
    }
}