using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RsApiClient
{
    public class UserStatement
    {
        private RsApiClientConfig rsApiClientConfig;
        private AccessTokenProvider accessTokenProvider;
        private string userStatementEndpoint;

        public UserStatement(RsApiClientConfig rsApiClientConfig, AccessTokenProvider accessTokenProvider) {
            this.rsApiClientConfig = rsApiClientConfig;
            this.accessTokenProvider = accessTokenProvider;
            this.userStatementEndpoint = rsApiClientConfig.ApiBaseUrl + "/api/rs/UserStatement";
        }

        public object GetUserStatement(string detail, string filter) {
            HttpClient client = accessTokenProvider.Client;
            string queryParams = "?";

            if (!string.IsNullOrEmpty(detail)) {
                queryParams = queryParams + "detail=" + detail + "&";
            }

            if (!string.IsNullOrEmpty(filter)) {
                queryParams = queryParams + "filter=" + filter + "&";
            }

            string accessToken = Task.Run(async () => {
                var result = await accessTokenProvider.GetAccessToken("password").ReadAsStringAsync();
                dynamic tokenObj = JsonConvert.DeserializeObject(result);
                return tokenObj["access_token"];
            }).GetAwaiter().GetResult();

            if (queryParams == "?") {
                queryParams = queryParams + "access_token=" + accessToken;
            } else {
                queryParams = queryParams + "&access_token=" + accessToken;
            }

            string requestUrl = this.userStatementEndpoint + queryParams;
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
