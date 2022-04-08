using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WildberriesAPI.Helpers
{
    public static class RequestHelper
    {
        public static async Task<IRestResponse> MakeRequest(string url)
        {
            RestClient client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.Timeout = -1;

            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");

            return await client.ExecuteAsync(request);
        }
    }
}
