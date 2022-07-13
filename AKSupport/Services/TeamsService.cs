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
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AKSupport.Models;

namespace AKSupport.Services;

sealed class TeamsService : INotificationService
{
    private readonly string _channelWebhook;
    private readonly string _imageUrl;
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Constructs a new <see cref="TeamsService"/> instance to interact with the Teams service.
    /// </summary>
    /// <param name="channelWebhook">Teams channel incoming webhook URL.</param>
    /// <param name="imageUrl">Optional URL to an avatar image for the notification.</param>
    /// <param name="timeoutSeconds">
    /// Number of seconds to wait before a request times out. Default is 90 seconds.
    /// </param>
    /// <exception cref="ArgumentNullException">The specified argument passed is null.</exception>
    public TeamsService(string? channelWebhook, string? imageUrl, int timeoutSeconds = 90)
    {
        _channelWebhook = channelWebhook ?? throw new ArgumentNullException(nameof(channelWebhook));
        _imageUrl = imageUrl ?? throw new ArgumentNullException(nameof(imageUrl));
        _httpClient = CreateHttpClient(TimeSpan.FromSeconds(timeoutSeconds));
    }

    /// <summary>
    /// Sends a notification message to the Teams service asynchronously with support details for an AKS cluster.
    /// </summary>
    /// <param name="clusterName">Name of AKS cluster.</param>
    /// <param name="version">Version of Kubernetes used by <paramref name="clusterName"/>.</param>
    /// <param name="description">Support description for <paramref name="clusterName"/>.</param>
    /// <param name="status">
    /// 'Not Supported' or 'Support Ending Soon' status for <paramref name="clusterName"/>.
    /// </param>
    /// <param name="clusterUrl">Optional Azure Portal URL for <paramref name="clusterName"/>.</param>
    /// <returns><see langword="true"/> if notification was successful, <see langword="false"/> if not.</returns>
    /// <exception cref="HttpRequestException">The HTTP response is unsuccessful.</exception>
    /// <exception cref="ArgumentNullException">The specified argument passed is null.</exception>
    public async Task<bool> SendNotificationAsync(string? clusterName, string version, string description, 
        string status, string? clusterUrl)
    {
        ArgumentNullException.ThrowIfNull(clusterName);
        ArgumentNullException.ThrowIfNull(clusterUrl);

        var card = GetCardTemplate(clusterName, version, description, status, clusterUrl);
        var jsonCard = new StringContent(JsonSerializer.Serialize(card), Encoding.UTF8, MediaTypeNames.Application.Json);
        using var response = await _httpClient.PostAsync(_channelWebhook, jsonCard);

        response.EnsureSuccessStatusCode();
        
        string returnCode = await response.Content.ReadAsStringAsync();

        return returnCode.Equals("1");
    }

    /// <summary>
    /// Gets a specially formatted message that can be sent as an actionable card to the Teams service.
    /// </summary>
    /// <remarks>
    /// The older Message Card format will be used because it is still not possible to send messages
    /// using the Adaptive Card format to an incoming webhook of a Teams channel.
    /// </remarks>
    /// <param name="clusterName">Name of AKS cluster.</param>
    /// <param name="version">Version of Kubernetes used by <paramref name="clusterName"/>.</param>
    /// <param name="description">Support description for <paramref name="clusterName"/>.</param>
    /// <param name="status">
    /// 'Not Supported' or 'Support Ending Soon' status for <paramref name="clusterName"/>.
    /// </param>
    /// <param name="clusterUrl">Optional Azure Portal URL for <paramref name="clusterName"/>.</param>
    /// <returns>Teams actionable card with support details.</returns>
    private TeamsCard GetCardTemplate(string clusterName, string version, string description,
        string status, string clusterUrl)
    {
        var section = new Section
        {
            ActivityTitle = "Automated Alert",
            ActivitySubtitle = DateTimeOffset.Now.ToString("yyyy/MM/dd, HH:mm \"GMT\"z"),
            ActivityImage = _imageUrl,
            Facts = new[]
            {
                new Fact { Name = "Cluster:",  Value = clusterName },
                new Fact { Name = "Running:",  Value = version },
                new Fact { Name = "Status:", Value = status }
            },
            Text = description
        };

        var clusterButton = new PotentialAction
        {
            Name = "View AKS Cluster",
            Targets = new[] { new Target { Uri = clusterUrl } } // Must be full url like https://google.com.
        };

        var policyButton = new PotentialAction
        {
            Name = "Version Support Policy",
            Targets = new[] { new Target { Uri = "https://docs.microsoft.com/en-us/azure/aks/supported-kubernetes" +
                                                 "-versions#kubernetes-version-support-policy" } }
        };

        var repoButton = new PotentialAction
        {
            Name = "AKSupport on GitHub",
            Targets = new[] { new Target { Uri = "https://github.com/StevenJDH/AKSupport" } }
        };

        var card = new TeamsCard
        {
            Summary = "AKSupport Alert",
            Title = "AKSupport Alert: \"AKS cluster needs attention\"",
            Sections = new[] { section },
            PotentialActions = new[] { clusterButton, policyButton, repoButton }
        };

        return card;
    }

    /// <summary>
    /// Creates a new instance of <see cref="HttpClient"/> with configuration needed to interact
    /// with the Teams service.
    /// </summary>
    /// <param name="timeoutSeconds">Number of seconds to wait before a request times out.</param>
    /// <returns>New HttpClient instance.</returns>
    private static HttpClient CreateHttpClient(TimeSpan timeoutSeconds) => new() { Timeout = timeoutSeconds };

    /// <summary>
    /// Releases any unmanaged resources and disposes of the managed resources used
    /// by the <see cref="TeamsService"/>.
    /// </summary>
    public void Dispose() => _httpClient.Dispose();
}
