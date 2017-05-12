using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RsApiClient
{
    public class Microloans {
        private RsApiClientConfig rsApiClientConfig;
        private AccessTokenProvider accessTokenProvider;
        private string microLoansEndpoint;

        public Microloans(RsApiClientConfig rsApiClientConfig, AccessTokenProvider accessTokenProvider) {
            this.rsApiClientConfig = rsApiClientConfig;
            this.accessTokenProvider = accessTokenProvider;
            this.microLoansEndpoint = rsApiClientConfig.ApiBaseUrl +  "/api/rs/Microloans";
        }

        public object GetMicroloans(int microloanId, string detail, string filter, int applicationId, int forSale) {
            HttpClient client = accessTokenProvider.Client;
            string queryParams = "?";

            if(microloanId != -1) {
                queryParams = queryParams + "microloan_id=" + microloanId + "&";
            }

            if(!string.IsNullOrEmpty(detail)) {
                queryParams = queryParams + "detail=" + detail + "&";
            }

            if(!string.IsNullOrEmpty(filter)) {
                queryParams = queryParams + "filter=" + filter + "&";
            }

            if(applicationId != -1) {
                queryParams = queryParams + "application_id=" + applicationId + "&";
            }

            if(forSale != -1) {
                queryParams = queryParams + "forSale=" + forSale + "&";
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

            string requestUrl = this.microLoansEndpoint + queryParams;
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
