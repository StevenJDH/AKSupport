/**
 * This file is part of AKSupport <https://github.com/StevenJDH/AKSupport>.
 * Copyright (C) 2021 Steven Jenkins De Haro.
 *
 * AKSupport is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * AKSupport is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with AKSupport.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AKSupport.Models;

namespace AKSupport.Services
{
    class ContainerService : IContainerService
    {
        private readonly string _subscriptionId;
        private readonly string _tenant;
        private readonly string _appId;
        private readonly string _password;
        private readonly TimeSpan _timeoutSeconds;
        private HttpClient _httpClient;
        private OAuth2Response _cachedAuthResponse;

        /// <summary>
        /// Constructs a new <see cref="ContainerService"/> instance to interact with the AKS API.
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="tenant"></param>
        /// <param name="appId"></param>
        /// <param name="password"></param>
        /// <param name="timeoutSeconds">
        /// Number of seconds to wait before a request times out. Default is 90 seconds.
        /// </param>
        public ContainerService(string subscriptionId, string tenant, string appId, string password, 
            int timeoutSeconds = 90)
        {
            _subscriptionId = subscriptionId;
            _tenant = tenant;
            _appId = appId;
            _password = password;
            _timeoutSeconds = TimeSpan.FromSeconds(timeoutSeconds);
            CreateHttpClient();
        }

        public async Task<IEnumerable<Orchestrator>> GetSupportedVersionsAsync(string location)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", await GetAuthorizeTokenAsync()
                    .ConfigureAwait(false));

            using var response = await _httpClient
                .GetAsync($"https://management.azure.com/subscriptions/{_subscriptionId}/providers/Microsoft.ContainerService/locations/{location}/orchestrators?api-version=2019-08-01&resource-type=managedClusters");

            response.EnsureSuccessStatusCode();

            var aksVersions = JsonSerializer.Deserialize<AksManagedClusters>(await response.Content.ReadAsStringAsync());

            // Some regions like northeurope and eastus return slightly different responses.
            return aksVersions?.VersionProfile?.Orchestrators ?? aksVersions?.Orchestrators ?? 
                Enumerable.Empty<Orchestrator>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="HttpRequestException">The HTTP response is unsuccessful.</exception>
        private async Task<string> GetAuthorizeTokenAsync()
        {
            if (_cachedAuthResponse != null && IsTokenLifeTimeValid(_cachedAuthResponse))
            {
                return _cachedAuthResponse.AccessToken;
            }

            var url = $"https://login.microsoftonline.com/{_tenant}/oauth2/token";

            var keyValues = new List<KeyValuePair<string, string>>
            {
                new ("grant_type", "client_credentials"),
                new ("client_id", _appId),
                new ("client_secret", _password),
                new ("resource", "https://management.azure.com")
            };

            var req = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(keyValues)
            };

            using var response = await _httpClient.SendAsync(req);

            response.EnsureSuccessStatusCode();
            _cachedAuthResponse = JsonSerializer.Deserialize<OAuth2Response>(await response.Content.ReadAsStringAsync());
            
            return _cachedAuthResponse?.AccessToken ?? "";
        }
        
        private static bool IsTokenLifeTimeValid(OAuth2Response auth2Response)
        {
            return auth2Response.ExpiresOn.CompareTo(DateTime.UtcNow.TimeOfDay) > 0;
        }

        /// <summary>
        /// Creates a new instance of <see cref="HttpClient"/> with configuration needed to interact
        /// with the AKS API.
        /// </summary>
        private void CreateHttpClient()
        {
            _httpClient = new HttpClient
            {
                Timeout = _timeoutSeconds
            };
        }

        /// <summary>
        /// Releases any unmanaged resources and disposes of the managed resources used
        /// by the <see cref="ContainerService"/>.
        /// </summary>
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
