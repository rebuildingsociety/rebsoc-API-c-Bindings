using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RsApiClient
{
    public class Loans {
        private RsApiClientConfig rsApiClientConfig;
        private AccessTokenProvider accessTokenProvider;

        private string loansEndpoint;

        public Loans(RsApiClientConfig rsApiClientConfig, AccessTokenProvider accessTokenProvider) {
            this.rsApiClientConfig = rsApiClientConfig;
            this.accessTokenProvider = accessTokenProvider;
            this.loansEndpoint = rsApiClientConfig.ApiBaseUrl + "/api/rs/Loans";
        }

        public object GetLoans(int loanId, string filter, string detail) {
            HttpClient client = accessTokenProvider.Client;
            string queryParams = "?";

            if(!loanId.Equals(-1)) {
                queryParams += "loan_id=" + loanId.ToString() + "&";
            }

            if(!string.IsNullOrEmpty(filter) && ValidateFilter(filter)) {
                queryParams += "filter=" + filter + "&";
            }

            if(!string.IsNullOrEmpty(detail) && ValidateDetail(detail)) {
                queryParams += "detail=" + detail + "&";
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

            string requestUrl = this.loansEndpoint + queryParams;
            requestUrl = (requestUrl.EndsWith("?")) ? requestUrl.TrimEnd('?') : requestUrl.TrimEnd('&');

            return Task.Run(async () => {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                HttpResponseMessage response = await client.SendAsync(requestMessage);
                var result = await response.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject(result);
                return obj;
            }).GetAwaiter().GetResult();
        }

        private bool ValidateDetail(string detail) {
            switch(detail) {
                case "FULL":
                    return true;
                case "SUMMARY":
                    return true;
            }
            return false;
        }

        private bool ValidateFilter(string filter) {
            switch(filter) {
                case "APPROVED":
                    return true;
                case "MYLOANS":
                    return true;
                case "CLOSED":
                    return true;
                case "ALL":
                    return true;
            }
            return false;
        }
    }
}
