using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
//using WebSocket4Net;

namespace Zen.Tracker.Client.Services
{
    public class DirectlineClient
    {
        // TODO: Write API client functions for talking to DL v3

        // TODO: We will need a conversation token for all methods used by this class

        // TODO: The conversation token will be retrieved from an endpoint on our server

        public async Task<DirectlineToken> RefreshTokenAsync(DirectlineToken token)
        {
            using (var client = CreateDirectlineHttpClient(token))
            {
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

        public async Task<DirectlineConversation> StartConversationAsync(DirectlineToken token)
        {
            using (var client = CreateDirectlineHttpClient(token))
            {
                var response = await client
                    .PostAsync("/v3/directline/conversations", null)
                    .ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                var contentBody = await response.Content
                    .ReadAsStringAsync()
                    .ConfigureAwait(false);
                var rawTokenWrapper = JsonConvert.DeserializeObject<RawDirectlineTokenObject>(contentBody);
                return new DirectlineConversation(new Uri(rawTokenWrapper.StreamUrl));
            }
        }

        public async Task<DirectlineConversation> ReconnectConversationAsync(DirectlineToken token, string conversationId, string watermark = null)
        {
            using (var client = CreateDirectlineHttpClient(token))
            {
                string url;
                if (string.IsNullOrEmpty(watermark))
                {
                    url = $"/v3/directline/conversations/{conversationId}";
                }
                else
                {
                    url = $"/v3/directline/conversations/{conversationId}?watermark={watermark}";
                }
                var response = await client
                    .GetAsync(url)
                    .ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                var contentBody = await response.Content
                    .ReadAsStringAsync()
                    .ConfigureAwait(false);
                var rawTokenWrapper = JsonConvert.DeserializeObject<RawDirectlineTokenObject>(contentBody);
                return new DirectlineConversation(new Uri(rawTokenWrapper.StreamUrl));
            }
        }

        private bool ShouldRefreshToken(DirectlineToken token)
        {
            return ((token.ExpiryUtc - DateTime.UtcNow) < TimeSpan.FromMinutes(4));
        }

        private HttpClient CreateDirectlineHttpClient(DirectlineToken token = null)
        {
            var client =
                new HttpClient
                {
                    BaseAddress = new Uri("https://directline.botframework.com")
                };
            if (token != null)
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token.RawToken);
            }
            return client;
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

        [JsonProperty("streamUrl")]
        public string StreamUrl { get; set; }
    }

    public class DirectlineConversation
    {
        //private readonly JsonWebSocket _webSocket;

        public DirectlineConversation(Uri remoteUri)
        {

        }

        public string Watermark { get; private set; }
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
