using System;
using System.Net.Http;
using System.Collections.Generic;

namespace RsApiClient
{
    public class AccessTokenProvider {
        private RsApiClientConfig clientConfig;
        private String oAuthEndpoint;
        private HttpClient client;

        public String PASSWORD = "password";
        public String CLIENT_CREDENTIALS = "client_credentials";

        public HttpClient Client { get => client; }

        public AccessTokenProvider(RsApiClientConfig clientConfig, HttpClient client) {
            this.clientConfig = clientConfig;
            this.oAuthEndpoint = clientConfig.ApiBaseUrl + "/oauth2/token";
            this.client = client;
        }

        public HttpContent GetAccessToken(String grant_type) {
            var requestBody = new Dictionary<String, String>();
            requestBody.Add("grant_type", grant_type);
            requestBody.Add("client_secret", clientConfig.ClientSecret);
            requestBody.Add("client_id", clientConfig.ClientId);

            if (grant_type == PASSWORD) {
                requestBody.Add("username", clientConfig.Username);
                requestBody.Add("password", clientConfig.Password);
            }

            var content = new FormUrlEncodedContent(requestBody);
            var response = this.client.PostAsync(oAuthEndpoint, content).Result;
            
            return response.Content;
        }
    }
}
