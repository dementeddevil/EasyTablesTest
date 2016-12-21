using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Im.Basket.Client.Services
{
    public class DirectlineClient
    {
        // TODO: Write API client functions for talking to DL v3

        // TODO: We will need a conversation token for all methods used by this class

        // TODO: The conversation token will be retrieved from an endpoint on our server

        public async Task<DirectlineToken> RefreshToken(DirectlineToken currentToken)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", currentToken.RawToken);
                var response = await client
                    .PostAsync("/v3/directline/tokens/refresh", null)
                    .ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                var contentBody = await response.Content
                    .ReadAsStringAsync()
                    .ConfigureAwait(false);
                var rawTokenWrapper = JsonConvert.DeserializeObject<RawDirectlineTokenObject>(contentBody);
                return new DirectlineToken(rawTokenWrapper.Token, rawTokenWrapper.ExpiresIn);
            }
        }

        public async Task

        private bool ShouldRefreshToken(DirectlineToken token)
        {
            return ((token.ExpiryUtc - DateTime.UtcNow) < TimeSpan.FromMinutes(4));
        }
    }

    public class RawDirectlineTokenObject
    {
        [JsonProperty("conversationId")]
        public string ConversationId { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }

    public class DirectlineToken
    {
        public DirectlineToken(string token, int expiresInSeconds)
        {
            RawToken = token;
            ExpiryUtc = DateTime.UtcNow.AddSeconds(expiresInSeconds);
        }

        public string RawToken { get; }

        public DateTime ExpiryUtc { get; }
    }
}
