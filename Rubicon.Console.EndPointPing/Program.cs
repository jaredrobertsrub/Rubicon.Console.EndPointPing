using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace Rubicon.Scheduler.EndPointPing
{
    public class Program
    {
        public static void Main(string[] args)
        {
            PingAsync().Wait();
        }

        public static async Task PingAsync()
        {
            var client = new ApiClient();
            var service = new Service(client);

            var returnedMessage = await service.PingEndPoint();
            Console.WriteLine(returnedMessage);
        }
    }

    public class Service
    {
        private readonly ApiClient _client;

        public Service(ApiClient client)
        {
            _client = client;
        }

        public async Task<string> PingEndPoint()
        {
            return await _client.PingEndPoint();
        }
    }

    public class ApiClient
    {
        private const string EndpointApiUrlConfigValue = "EndPointURL";
        private const string EndpointPathConfigValue = "EndpointPath";

        public async Task<string> PingEndPoint()
        {
            using (var client = new HttpClient())
            {
                var apiUrl = WebConfigurationManager.AppSettings[EndpointApiUrlConfigValue];

                var uriBuilder = new UriBuilder(apiUrl)
                {
                    Path = WebConfigurationManager.AppSettings[EndpointPathConfigValue]
                };

                var queryString = HttpUtility.ParseQueryString(uriBuilder.Query);
                uriBuilder.Query = queryString.ToString();

                var response = await client.PostAsync(uriBuilder.Uri, new StringContent(string.Empty));

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return $"Call returned with error status code: {response.StatusCode}";
                }

                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
