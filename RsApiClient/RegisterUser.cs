using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RsApiClient
{
    public class RegisterUser
    {
        private RsApiClientConfig rsApiClientConfig;
        private AccessTokenProvider accessTokenProvider;
        private string borrowerCheckEndpoint;
        private const int SUCCESS_STATE = 1;
        private const int FAIL_STATE = 0;

        public RegisterUser(RsApiClientConfig rsApiClientConfig, AccessTokenProvider accessTokenProvider) {
            this.rsApiClientConfig = rsApiClientConfig;
            this.accessTokenProvider = accessTokenProvider;
            this.borrowerCheckEndpoint = rsApiClientConfig.ApiBaseUrl + "/api/rs/RegisterUser";
        }

        public int DoRegisterUser(string username, string email, string password, string firstName, string lastName, string userType, string date) {
            HttpClient client = accessTokenProvider.Client;
            string queryParams = "?";

            if(!string.IsNullOrEmpty(username)) {
                queryParams = queryParams + "username=" + username + "&";
            }

            if(!string.IsNullOrEmpty(email)) {
                queryParams = queryParams + "email=" + email + "&";
            }

            if (!string.IsNullOrEmpty(password)) {
                queryParams = queryParams + "password=" + password + "&";
            }

            if (!string.IsNullOrEmpty(firstName)) {
                queryParams = queryParams + "first_name=" + firstName + "&";
            }

            if (!string.IsNullOrEmpty(lastName)) {
                queryParams = queryParams + "last_name=" + lastName + "&";
            }

            if (!string.IsNullOrEmpty(userType)) {
                queryParams = queryParams + "user_type=" + userType + "&";
            }

            if (!string.IsNullOrEmpty(date)) {
                queryParams = queryParams + "date_of_birth=" + date + "&";
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

            string requestUrl = this.borrowerCheckEndpoint + queryParams;
            requestUrl = (requestUrl.EndsWith("?")) ? requestUrl.TrimEnd('?') : requestUrl.TrimEnd('&');

            return Task.Run(async () => {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                HttpResponseMessage response = await client.SendAsync(requestMessage);
                var result = await response.Content.ReadAsStringAsync();
                return SUCCESS_STATE;
            }).GetAwaiter().GetResult();
        }
    }
}
