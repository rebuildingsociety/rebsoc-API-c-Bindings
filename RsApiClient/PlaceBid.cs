using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RsApiClient
{
    public class PlaceBid {
        private RsApiClientConfig rsApiClientConfig;
        private AccessTokenProvider accessTokenProvider;
        private string placeBidEndpoint;
        private const int SUCCESS_STATE = 1;
        private const int FAIL_STATE = 0;

        public PlaceBid(RsApiClientConfig rsApiClientConfig, AccessTokenProvider accessTokenProvider) {
            this.rsApiClientConfig = rsApiClientConfig;
            this.accessTokenProvider = accessTokenProvider;
            this.placeBidEndpoint = rsApiClientConfig.ApiBaseUrl + "/api/rs/PlaceBid";
        }

        public int DoPlaceBid(int appId, double bidRate, int bidAmount) {
            HttpClient client = accessTokenProvider.Client;

            var requestBody = new Dictionary<string, string>();
            requestBody.Add("appID", appId.ToString());
            requestBody.Add("bidRate", bidRate.ToString());
            requestBody.Add("bidAmount", bidAmount.ToString());
            var content = new FormUrlEncodedContent(requestBody);

            var result = Task.Run(async () => {
                var response = await client.PostAsync(placeBidEndpoint, content);
                if(response.IsSuccessStatusCode) {
                    return SUCCESS_STATE;
                }
                return FAIL_STATE;
            }).GetAwaiter().GetResult();

            return result;
        }
    }
}
