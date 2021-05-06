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
            string card = "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"> " +
                       "<script type=\"application/adaptivecard+json\">" +
                       "{\"$schema\":\"http://adaptivecards.io/schemas/adaptive-card.json\"," +
                       "\"type\":\"AdaptiveCard\",\"version\":\"1.0\",\"body\":[{\"type\":\"ColumnSet\"," +
                       "\"id\":\"1ede2aba-61b9-faa0-9895-9ed0c26b2e6f\",\"columns\":[{\"type\":\"Column\"," +
                       "\"id\":\"e5756242-0963-37a2-7cb4-4397886d60bb\",\"padding\":\"None\",\"width\":\"stretch\"," +
                       "\"items\":[{\"type\":\"TextBlock\",\"id\":\"20f3833e-0435-5c87-fad1-b528e0046fb6\"," +
                       $"\"text\":\"Automated Alert - {DateTimeOffset.Now:yyyy/MM/dd, HH:mm \"GMT\"z}\",\"wrap\":true}}]," +
                       "\"verticalContentAlignment\":\"Center\"},{\"type\":\"Column\"," +
                       "\"id\":\"74215a26-fa8b-e549-cced-7f99fd34a661\",\"padding\":\"None\",\"width\":\"auto\"," +
                       "\"items\":[{\"type\":\"Image\",\"id\":\"795047e2-e63e-6e14-07ba-5a3e13323dff\"," +
                       $"\"url\":\"{_imageUrl}\"," +
                       "\"size\":\"Small\"}],\"horizontalAlignment\":\"Right\"}],\"padding\":{\"top\":\"Small\"," +
                       "\"bottom\":\"Small\",\"left\":\"Default\",\"right\":\"Small\"},\"style\":\"emphasis\"}," +
                       "{\"type\":\"Container\",\"id\":\"fbcee869-2754-287d-bb37-145a4ccd750b\"," +
                       "\"padding\":\"Default\",\"spacing\":\"None\",\"items\":[{\"type\":\"TextBlock\"," +
                       "\"id\":\"44906797-222f-9fe2-0b7a-e3ee21c6e380\"," +
                       "\"text\":\"AKSupport Alert: AKS cluster needs attention\",\"wrap\":true,\"weight\":\"Bolder\"," +
                       "\"size\":\"Large\"},{\"type\":\"TextBlock\",\"id\":\"f7abdf1a-3cce-2159-28ef-f2f362ec937e\"," +
                       $"\"text\":\"{description}\"," +
                       "\"wrap\":true},{\"type\":\"FactSet\",\"id\":\"9bfc0c85-3e8e-0d3c-66ba-8d83e02b7e24\"," +
                       $"\"facts\":[{{\"title\":\"Cluster:\",\"value\":\"{clusterName}\"}}," +
                       $"{{\"title\":\"Running:\",\"value\":\"{version}\"}}," +
                       $"{{\"title\":\"Status:\",\"value\":\"{status}\"}}]}},{{\"type\":\"ActionSet\"," +
                       "\"id\":\"c8a5ba7e-7ec6-53ae-79da-0cfb952a527e\",\"actions\":[{\"type\":\"Action.OpenUrl\"," +
                       "\"id\":\"fc2f1ec7-b819-2f3b-e874-1be376092f86\",\"title\":\"View AKS Cluster\"," +
                       $"\"url\":\"{clusterUrl}\",\"style\":\"positive\",\"isPrimary\":true}}," +
                       "{\"type\":\"Action.OpenUrl\",\"id\":\"730628aa-d6ed-edfb-7273-37631a8c577b\"," +
                       "\"title\":\"Version Support Policy\"," +
                       "\"url\":\"https://docs.microsoft.com/en-us/azure/aks/supported-kubernetes-versions#" +
                       "kubernetes-version-support-policy\"}]}]}," +
                       "{\"type\":\"Container\",\"id\":\"77102c5d-fde2-e573-4ea5-66022d646d64\"," +
                       "\"padding\":{\"top\":\"Small\",\"bottom\":\"Small\",\"left\":\"Small\",\"right\":\"Default\"}," +
                       "\"spacing\":\"None\",\"separator\":true,\"items\":[{\"type\":\"TextBlock\"," +
                       "\"id\":\"42654e7e-cece-b419-867a-3e3ef4076870\"," +
                       "\"text\":\"[AKSupport on GitHub](https://github.com/StevenJDH/AKSupport)\"," +
                       "\"wrap\":true,\"color\":\"Accent\",\"horizontalAlignment\":\"Right\"}]," +
                       "\"horizontalAlignment\":\"Right\",\"style\":\"emphasis\"}],\"padding\":\"None\"}" +
                       "</script> </head><body></body></html>";

            return card;
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
