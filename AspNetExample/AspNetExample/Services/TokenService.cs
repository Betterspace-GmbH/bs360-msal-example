using AspNetExample.Clients.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Headers;
using System.Text.Json;

namespace AspNetExample.Clients
{
    public class TokenService(IHttpClientFactory _httpClientFactory, IMemoryCache _inMemoryCache, string _clientId, string _clientSecret, string _scope, string _authority)
    {
        private const string _cacheKey = "betterspace_token";
        private static readonly JsonSerializerOptions _serializerOptions;

        static TokenService()
        {
            _serializerOptions = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                WriteIndented = true
            };
        }
 
        public async Task<string> GetAccessTokenAsync()
        {
            if (!IsTokenValid())
            {
                await AcquireNewTokenAsync();
            }

            var token = _inMemoryCache.Get<TokenResponse>(_cacheKey);
            return token!.AccessToken;
        }

        private bool IsTokenValid()
        {
            if (_inMemoryCache.TryGetValue(_cacheKey, out TokenResponse? token))
            {
                var expirationDate = DateTimeOffset.FromUnixTimeSeconds(token!.ExpiresOn);
                if (expirationDate.AddMinutes(1) > DateTimeOffset.UtcNow)
                {
                    return true;
                }
            }
            return false;
        }

        private async Task AcquireNewTokenAsync()
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, _authority)
            {
                Content = B2CContent(),
            };

            var httpClient = _httpClientFactory.CreateClient();

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            using var contentStream = await httpResponseMessage.EnsureSuccessStatusCode()
                                                               .Content
                                                               .ReadAsStreamAsync();

            var tokenResponse = await JsonSerializer.DeserializeAsync<TokenResponse>(contentStream, _serializerOptions)
                             ?? throw new Exception("Cannot deserialize response.");

            _inMemoryCache.Set(_cacheKey, tokenResponse);
        }

        private MultipartFormDataContent B2CContent()
        {
            var content = new MultipartFormDataContent
            {
                { new StringContent("client_credentials"), "grant_type" },
                { new StringContent(_clientId), "client_id" },
                { new StringContent(_clientSecret), "client_secret" },
                { new StringContent(_scope), "scope" }
            };
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");

            return content;
        }
    }
}
