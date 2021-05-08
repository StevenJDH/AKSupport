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
    sealed class OfficeMailService : INotificationService
    {
        private readonly string _senderId;
        private readonly string _recipient;
        private readonly string _imageUrl;
        private readonly IOAuth2Service _oAuth2;
        private readonly TimeSpan _timeoutSeconds;
        private HttpClient _httpClient;

        /// <summary>
        /// Constructs a new <see cref="OfficeMailService"/> instance to interact with the Office Mail service.
        /// </summary>
        /// <param name="senderId">Email address or user's Object Id of sender.</param>
        /// <param name="recipient">Email address of recipient.</param>
        /// <param name="imageUrl">Optional URL to an avatar image for the notification.</param>
        /// <param name="oAuth2">An <see cref="OAuth2Service"/> instance to authenticate the request.</param>
        /// <param name="timeoutSeconds">
        /// Number of seconds to wait before a request times out. Default is 90 seconds.
        /// </param>
        public OfficeMailService(string senderId, string recipient, string imageUrl, IOAuth2Service oAuth2, 
            int timeoutSeconds = 90)
        {
            _senderId = senderId;
            _recipient = recipient;
            _imageUrl = imageUrl;
            _oAuth2 = oAuth2;
            _timeoutSeconds = TimeSpan.FromSeconds(timeoutSeconds);
            CreateHttpClient();
        }

        /// <summary>
        /// Sends a notification message to the Office Mail service asynchronously with support details for an AKS
        /// cluster.
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
        public async Task<bool> SendNotificationAsync(string clusterName, string version, string description, 
            string status, string clusterUrl)
        {
            var url = $"https://graph.microsoft.com/v1.0/users/{_senderId}/sendMail";
            var emailAddress = new EmailAddress { Address = _recipient };
            
            var email = new Email
            {
                NewMessage = new Message
                {
                    ToRecipients = new[] { new ToRecipient { EmailAddress = emailAddress } },
                    Subject = "AKSupport Alert: \"AKS cluster needs attention\"",
                    Body = new Body
                    {
                        ContentType = "HTML", // Text or HTML.
                        Content = GetHtmlCardTemplate(clusterName, version, description, status, clusterUrl)
                    }
                }
            };
            
            var jsonMail = new StringContent(JsonSerializer.Serialize(email), Encoding.UTF8, "application/json");
            string token = await _oAuth2.GetAuthorizeTokenAsync(_httpClient, "https://graph.microsoft.com/.default")
                .ConfigureAwait(false);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await _httpClient.PostAsync(url, jsonMail);

            response.EnsureSuccessStatusCode();

            string returnCode = await response.Content.ReadAsStringAsync();

            return returnCode.Equals("1");
        }

        /// <summary>
        /// Gets a specially formatted message that can be sent as an actionable adaptive card to the Office Mail
        /// service.
        /// </summary>
        /// <param name="clusterName">Name of AKS cluster.</param>
        /// <param name="version">Version of Kubernetes used by <paramref name="clusterName"/>.</param>
        /// <param name="description">Support description for <paramref name="clusterName"/>.</param>
        /// <param name="status">
        /// 'Not Supported' or 'Support Ending Soon' status for <paramref name="clusterName"/>.
        /// </param>
        /// <param name="clusterUrl">Optional Azure Portal URL for <paramref name="clusterName"/>.</param>
        /// <returns>Office Mail actionable adaptive card with support details.</returns>
        private string GetHtmlCardTemplate(string clusterName, string version, string description,
            string status, string clusterUrl)
        {
            var header = new AdaptiveColumn
            {
                Width = "stretch",
                Items = new object[] { new AdaptiveTextBlock
                {
                    Text = $"Automated Alert - {DateTimeOffset.Now:yyyy/MM/dd, HH:mm \"GMT\"z}"
                }},
                VerticalContentAlignment = "Center"
            };

            var headerImage = new AdaptiveColumn
            {
                Items = new object[] { new AdaptiveImage
                {
                    Url = _imageUrl
                }},
                HorizontalAlignment = "Right"
            };

            var body = new AdaptiveContainer
            {
                Padding = new AdaptivePadding(), // Default padding.
                Items = new object[]
                {
                    new AdaptiveTextBlock
                    {
                        Text = "AKSupport Alert: \"AKS cluster needs attention\"",
                        Weight = "Bolder",
                        Size = "Large"
                    },
                    new AdaptiveTextBlock { Text = description },
                    new AdaptiveGenericSet
                    {
                        Type = "FactSet",
                        Facts = new[]
                        {
                            new AdaptiveFact { Title = "Cluster:", Value = clusterName },
                            new AdaptiveFact { Title = "Running:", Value = version },
                            new AdaptiveFact { Title = "Status:", Value = status }
                        }
                    },
                    new AdaptiveGenericSet
                    {
                        Type = "ActionSet",
                        Actions = new[]
                        {
                            new AdaptiveAction
                            {
                                Title = "View AKS Cluster",
                                Url = clusterUrl,
                                Style = "positive",
                                IsPrimary = true
                            },
                            new AdaptiveAction
                            {
                                Title = "Version Support Policy",
                                Url = "https://docs.microsoft.com/en-us/azure/aks/supported-kubernetes" +
                                      "-versions#kubernetes-version-support-policy"
                            }
                        }
                    }
                }
            };

            var footer = new AdaptiveContainer
            {
                Padding = new AdaptivePadding { Top = "Small", Bottom = "Small", Left = "Small" },
                Separator = true,
                Items = new object[]
                {
                    new AdaptiveTextBlock
                    {
                        Text = "[AKSupport on GitHub](https://github.com/StevenJDH/AKSupport)",
                        Color = "Accent",
                        HorizontalAlignment = "Right"
                    }
                },
                HorizontalAlignment = "Right",
                Style = "emphasis"
            };

            var card = new MailCard
            {
                Groups = new object[]
                {
                    new AdaptiveColumnSet
                    {
                        Columns = new[] { header, headerImage },
                        Padding = new AdaptivePadding { Top = "Small", Bottom = "Small", Right = "Small" },
                        Style = "emphasis"
                    },
                    body,
                    footer
                }
            };

            var sb = new StringBuilder();

            sb.Append("<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
            sb.Append("<script type=\"application/adaptivecard+json\">");
            sb.Append(JsonSerializer.Serialize(card, new JsonSerializerOptions { IgnoreNullValues = true }));
            sb.Append("</script> </head><body></body></html>");

            return sb.ToString();
        }

        /// <summary>
        /// Creates a new instance of <see cref="HttpClient"/> with configuration needed to interact
        /// with the Office Mail service.
        /// </summary>
        private void CreateHttpClient() => _httpClient = new HttpClient { Timeout = _timeoutSeconds };

        /// <summary>
        /// Releases any unmanaged resources and disposes of the managed resources used
        /// by the <see cref="OfficeMailService"/>.
        /// </summary>
        public void Dispose() => _httpClient?.Dispose();
    }
}
