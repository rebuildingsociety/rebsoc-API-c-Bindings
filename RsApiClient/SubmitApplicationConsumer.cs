using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace RsApiClient
{
    public class SubmitApplicationConsumer
    {
        private RsApiClientConfig rsApiClientConfig;
        private AccessTokenProvider accessTokenProvider;
        private string submitApplicationConsumerEndpoint;
        private const int SUCCESS_STATE = 1;
        private const int FAIL_STATE = 0;

        public SubmitApplicationConsumer(RsApiClientConfig rsApiClientConfig, AccessTokenProvider accessTokenProvider) {
            this.rsApiClientConfig = rsApiClientConfig;
            this.accessTokenProvider = accessTokenProvider;
            this.submitApplicationConsumerEndpoint = rsApiClientConfig.ApiBaseUrl + "/api/rs/SubmitApplicationConsumer";
        }

        public int DoSubmitApplicationConsumer(int appAmount, int appTerm, int userId, string username, string userEmail, string password, string title, string userFirstName, string userLastName) {
            HttpClient client = accessTokenProvider.Client;

            var requestBody = new Dictionary<string, string>();
            requestBody.Add("app_amount", appAmount.ToString());
            requestBody.Add("app_term", appTerm.ToString());
            requestBody.Add("user_id", userId.ToString());
            requestBody.Add("username", username);
            requestBody.Add("user_email", userEmail);
            requestBody.Add("password", password);
            requestBody.Add("title", title);
            requestBody.Add("user_firstname", userFirstName);
            requestBody.Add("user_lastname", userLastName);

            var content = new FormUrlEncodedContent(requestBody);
            var result = Task.Run(async () => {
                var response = await client.PostAsync(submitApplicationConsumerEndpoint, content);
                if (response.IsSuccessStatusCode)
                {
                    return SUCCESS_STATE;
                }
                return FAIL_STATE;
            }).GetAwaiter().GetResult();

            return result;
        }
    }
}
