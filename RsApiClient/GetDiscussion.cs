using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RsApiClient
{
    class GetDiscussion {
        private RsApiClientConfig rsApiClientConfig;
        private AccessTokenProvider accessTokenProvider;
        private string getDiscussionEndpoint;

        public GetDiscussion(RsApiClientConfig rsApiClientConfig, AccessTokenProvider accessTokenProvider) {
            this.rsApiClientConfig = rsApiClientConfig;
            this.accessTokenProvider = accessTokenProvider;
            this.getDiscussionEndpoint = rsApiClientConfig.ApiBaseUrl + "/api/rs/GetDiscussion";
        }

        public object DoGetDiscussion(int applicationId, string orderBy, int topicId) {
            HttpClient client = accessTokenProvider.Client;
            string queryParams = "?";

            if(applicationId != -1) {
                queryParams = queryParams + "application_id=" + applicationId + "&";
            }

            if(!string.IsNullOrEmpty(orderBy)) {
                queryParams = queryParams + "orderby=" + orderBy + "&";
            }

            if (topicId != -1) {
                queryParams = queryParams + "topic_id=" + topicId + "&";
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

            string requestUrl = this.getDiscussionEndpoint + queryParams;
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
