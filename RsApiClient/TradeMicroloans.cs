using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RsApiClient
{
    public class TradeMicroloans {
        private RsApiClientConfig rsApiClientConfig;
        private AccessTokenProvider accessTokenProvider;
        private string tradeMicroloansEndpoint;

        public TradeMicroloans(RsApiClientConfig rsApiClientConfig, AccessTokenProvider accessTokenProvider) {
            this.rsApiClientConfig = rsApiClientConfig;
            this.accessTokenProvider = accessTokenProvider;
            this.tradeMicroloansEndpoint = rsApiClientConfig.ApiBaseUrl + "/api/rs/TradeMicroloans";
        }

        public object GetTradeMicroloans(string action, string microloans, double discountPremium) {
            HttpClient client = accessTokenProvider.Client;
            string queryParams = "?";

            if (!string.IsNullOrEmpty(action)) {
                queryParams = queryParams + "action=" + action + "&";
            }

            if (!string.IsNullOrEmpty(microloans)) {
                queryParams = queryParams + "microloans=" + microloans + "&";
            }

            if(discountPremium != -1) {
                queryParams = queryParams + "discountPremium=" + discountPremium.ToString() + "&";
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

            string requestUrl = this.tradeMicroloansEndpoint + queryParams;
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
