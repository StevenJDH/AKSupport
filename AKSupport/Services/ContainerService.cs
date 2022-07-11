/*
 * This file is part of AKSupport <https://github.com/StevenJDH/AKSupport>.
 * Copyright (C) 2021-2022 Steven Jenkins De Haro.
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

namespace AKSupport.Services;

sealed class ContainerService : IContainerService
{
    private readonly string _subscriptionId;
    private readonly IOAuth2Service _oAuth2;
    private readonly TimeSpan _timeoutSeconds;
    private HttpClient _httpClient;

    /// <summary>
    /// Constructs a new <see cref="ContainerService"/> instance to interact with the AKS API.
    /// </summary>
    /// <param name="subscriptionId">Subscription Id of the Service Principal.</param>
    /// <param name="oAuth2">An <see cref="OAuth2Service"/> instance to authenticate the request.</param>
    /// <param name="timeoutSeconds">
    /// Number of seconds to wait before a request times out. Default is 90 seconds.
    /// </param>
    public ContainerService(string subscriptionId, IOAuth2Service oAuth2, int timeoutSeconds = 90)
    {
        _subscriptionId = subscriptionId;
        _oAuth2 = oAuth2;
        _timeoutSeconds = TimeSpan.FromSeconds(timeoutSeconds);
        CreateHttpClient();
    }

    /// <summary>
    /// Gets a list of Kubernetes versions for AKS asynchronously that are currently supported
    /// by Microsoft. The list also contains the supported upgrade paths for each version
    /// that is returned up to the latest version available.
    /// </summary>
    /// <param name="location">AKS region to use when checking for supported versions.</param>
    /// <returns>A listed of supported versions and their upgrade paths.</returns>
    /// <exception cref="HttpRequestException">The HTTP response is unsuccessful.</exception>
    public async Task<IEnumerable<Orchestrator>> GetSupportedVersionsAsync(string location)
    {
        string token = await _oAuth2.GetAuthorizeTokenAsync(_httpClient, "https://management.azure.com/.default")
            .ConfigureAwait(false);

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await _httpClient.GetAsync($"https://management.azure.com/subscriptions/" +
            $"{_subscriptionId}/providers/Microsoft.ContainerService/locations/{location}/orchestrators?" +
            "api-version=2019-08-01&resource-type=managedClusters");

        response.EnsureSuccessStatusCode();

        var aksVersions = JsonSerializer.Deserialize<AksManagedClusters>(await response.Content.ReadAsStringAsync());

        // Some regions like northeurope and eastus return slightly different responses.
        return aksVersions?.VersionProfile?.Orchestrators ?? aksVersions?.Orchestrators ?? 
            Enumerable.Empty<Orchestrator>();
    }

    /// <summary>
    /// Creates a new instance of <see cref="HttpClient"/> with configuration needed to interact
    /// with the AKS API.
    /// </summary>
    private void CreateHttpClient() => _httpClient = new HttpClient { Timeout = _timeoutSeconds };

    /// <summary>
    /// Releases any unmanaged resources and disposes of the managed resources used
    /// by the <see cref="ContainerService"/>.
    /// </summary>
    public void Dispose() => _httpClient?.Dispose();
}
