using System;

namespace RsApiClient {
    public class RsApiClientConfig {

        private String clientSecret;
        private String clientId;
        private String username;
        private String password;
        private String apiBaseUrl;

        public RsApiClientConfig(String apiBaseUrl, String clientSecret, String clientId) {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.apiBaseUrl = apiBaseUrl;
        }

        public void SetPasswordCredentials(String username, String password) {
            this.username = username;
            this.password = password;
        }

        public string ClientSecret { get => clientSecret; set => clientSecret = value; }
        public string ClientId { get => clientId; set => clientId = value; }
        public string Username { get => username; set => username = value; }
        public string Password { get => password; set => password = value; }
        public string ApiBaseUrl { get => apiBaseUrl; set => apiBaseUrl = value; }
    }
}
