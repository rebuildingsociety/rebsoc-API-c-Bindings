using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RsApiClient
{
    public class News {
        private RsApiClientConfig rsApiClientConfig;
        private AccessTokenProvider accessTokenProvider;
        private string newsEndpoint;

        public News(RsApiClientConfig rsApiClientConfig, AccessTokenProvider accessTokenProvider) {
            this.rsApiClientConfig = rsApiClientConfig;
            this.accessTokenProvider = accessTokenProvider;
            this.newsEndpoint = rsApiClientConfig.ApiBaseUrl +  "/api/rs/News";
        }

        public object getNews(int newsId, int maxResults) {
            HttpClient client = accessTokenProvider.Client;
            string queryParams = "?";

            if(newsId != -1) {
                queryParams = queryParams + "news_id=" + newsId + "&";
            }

            if(maxResults != -1) {
                queryParams = queryParams + "max_results=" + maxResults + "&";
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

            string requestUrl = this.newsEndpoint + queryParams;
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
