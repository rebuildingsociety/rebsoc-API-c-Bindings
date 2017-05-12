using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RsApiClient
{
    public class Statistics {
        private RsApiClientConfig rsApiClientConfig;
        private AccessTokenProvider accessTokenProvider;
        private string statisticsEndpoint;

        public Statistics(RsApiClientConfig rsApiClientConfig, AccessTokenProvider accessTokenProvider) {
            this.rsApiClientConfig = rsApiClientConfig;
            this.accessTokenProvider = accessTokenProvider;
            this.statisticsEndpoint = rsApiClientConfig.ApiBaseUrl +  "/api/rs/Statistics";
        }

        public object getStatistics(string filter, string detail) {
            HttpClient client = accessTokenProvider.Client;
            string queryParams = "?";

            if(!string.IsNullOrEmpty(filter)) {
                queryParams = queryParams + "filter=" + filter + "&";
            }

            if(!string.IsNullOrEmpty(detail)) {
                queryParams = queryParams + "detail=" + detail + "&";
            }

            string accessToken = Task.Run(async () => {
                var result = await accessTokenProvider.GetAccessToken("password").ReadAsStringAsync();
                dynamic tokenObj = JsonConvert.DeserializeObject(result);
                return tokenObj["access_token"];
            }).GetAwaiter().GetResult();

            if (queryParams == "?") {
                queryParams = queryParams + "access_token=" + accessToken;
            }
            else {
                queryParams = queryParams + "&access_token=" + accessToken;
            }

            string requestUrl = this.statisticsEndpoint + queryParams;
            requestUrl = (requestUrl.EndsWith("?")) ? requestUrl.TrimEnd('?') : requestUrl.TrimEnd('&');

            return Task.Run(async () => {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                HttpResponseMessage response = await client.SendAsync(requestMessage);
                var result = await response.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject(result);
                return obj;
            }).GetAwaiter().GetResult();
        }
    }
}
