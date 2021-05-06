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
using System.Text;
using System.Threading.Tasks;
using AKSupport.Models;
using AKSupport.Services;

namespace AKSupport
{
    static class Program
    {
        private static readonly EnvironmentConfig Env = LoadConfiguration();
        private static IEnumerable<INotificationService> _services;

        static async Task<int> Main(string[] args)
        {
            Console.WriteLine("{0,-36:o} Checking AKS support status...", DateTimeOffset.UtcNow);
            RegisterNotificationServices();
            
            string kVersion;
            IEnumerable<Orchestrator> aksVersions;

            try
            {
                if (args.Length > 0)
                {
                    kVersion = args[0]; // For testing with custom versions.
                }
                else
                {
                    IKubeApiService kubeApi = new KubeApiService();
                    var kBuild = await kubeApi.GetBuildInfoAsync();
                    kVersion = kBuild.GitVersion;
                }

                IOAuth2Service auth2 = new OAuth2Service(Env.AppTenant, Env.AppId, Env.AppPassword);
                IContainerService aks = new ContainerService(Env.SubscriptionId, auth2);
                aksVersions = await aks.GetSupportedVersionsAsync(Env.AksRegion);

                if (!IsSupported(kVersion, aksVersions))
                {
                    Console.WriteLine("{0,-36:o} Version {1} is no longer supported.", DateTimeOffset.UtcNow, kVersion);
                    await NotifyAsync(kVersion, hasSupportEnded: true);

                    return 2;
                }

                if (IsSupportEnding(kVersion, aksVersions))
                {
                    Console.WriteLine("{0,-36:o} Support is ending for version {1} soon.", DateTimeOffset.UtcNow, kVersion);
                    await NotifyAsync(kVersion, hasSupportEnded: false);

                    return 3;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine("{0,-36:o} {1}", DateTimeOffset.UtcNow, ex.Message);

                return 1;
            }

            Console.WriteLine("{0,-36:o} Support for version {1} is active.", DateTimeOffset.UtcNow, kVersion);
            
            return 0;
        }

        /// <summary>
        /// Determines whether the version of Kubernetes in AKS being used is about to lose Microsoft support.
        /// </summary>
        /// <param name="runningVersion">Kubernetes version of AKS cluster.</param>
        /// <param name="supportedList">List of versions currently supported by Microsoft.</param>
        /// <returns><see langword="true"/> if losing support, <see langword="false"/> if not.</returns>
        private static bool IsSupportEnding(string runningVersion, IEnumerable<Orchestrator> supportedList)
        {
            // Versions in the list are stored in ascending order.
            var midVersion = Version.Parse(supportedList.ElementAt(2).OrchestratorVersion);

            return Version.Parse(runningVersion) < midVersion;
        }

        /// <summary>
        /// Determines whether the version of Kubernetes in AKS being used has lost Microsoft support.
        /// </summary>
        /// <param name="runningVersion">Kubernetes version of AKS cluster.</param>
        /// <param name="supportedList">List of versions currently supported by Microsoft.</param>
        /// <returns><see langword="true"/> if supported, <see langword="false"/> if not.</returns>
        private static bool IsSupported(string runningVersion, IEnumerable<Orchestrator> supportedList)
        {
            return supportedList.Any(aks => runningVersion.Equals(aks.OrchestratorVersion));
        }

        /// <summary>
        /// Issues an AKS support status notification asynchronously for services that implement
        /// <see cref="INotificationService"/>.
        /// </summary>
        /// <param name="kVersion">Kubernetes version of an AKS cluster.</param>
        /// <param name="hasSupportEnded">
        /// <see langword="true"/> if support ended for <paramref name="kVersion"/>, <see langword="false"/> if it
        /// will end soon.
        /// </param>
        /// <returns>A <see cref="Task"/> representing an async operation.</returns>
        /// <exception cref="HttpRequestException">The HTTP response is unsuccessful.</exception>
        private static async Task NotifyAsync(string kVersion, bool hasSupportEnded)
        {
            string description = hasSupportEnded ? 
                "Cluster is no longer covered by Microsoft's Kubernetes Version Support Policy." :
                "Cluster will soon lose support as per Microsoft's Kubernetes Version Support Policy.";
            
            string status = hasSupportEnded ? "Not Supported" : "Support Ending Soon";

            foreach (var service in _services)
            {
                await service.SendNotificationAsync(Env.AksClusterName, kVersion, description, 
                    status, Env.AksClusterUrl);

            }
        }

        /// <summary>
        /// Loads configuration from environment variables in a way that is supported by Linux hosts.
        /// </summary>
        /// <remarks>
        /// Only AZURE_SUBSCRIPTION_ID, AZURE_APP_TENANT, AZURE_APP_ID, AZURE_APP_PASSWORD, and
        /// AZURE_AKS_REGION are required for base functionality. All other environment variables
        /// are service specific, and AZURE_AKS_CLUSTER_URL and AVATAR_IMAGE_URL are optional.
        /// </remarks>
        /// <returns>Application configuration from environment variables.</returns>
        private static EnvironmentConfig LoadConfiguration()
        {
            var target = EnvironmentVariableTarget.Process;

            var env = new EnvironmentConfig
            {
                SubscriptionId = Environment.GetEnvironmentVariable("AZURE_SUBSCRIPTION_ID", target),
                AppTenant = Environment.GetEnvironmentVariable("AZURE_APP_TENANT", target),
                AppId = Environment.GetEnvironmentVariable("AZURE_APP_ID", target),
                AppPassword = Environment.GetEnvironmentVariable("AZURE_APP_PASSWORD", target),
                AksRegion = Environment.GetEnvironmentVariable("AZURE_AKS_REGION", target),
                
                ImageUrl = Environment.GetEnvironmentVariable("AVATAR_IMAGE_URL", target),
                AksClusterName = Environment.GetEnvironmentVariable("AZURE_AKS_CLUSTER_NAME", target),
                AksClusterUrl = Environment.GetEnvironmentVariable("AZURE_AKS_CLUSTER_URL", target),

                ChannelWebhookUrl = Environment.GetEnvironmentVariable("TEAMS_CHANNEL_WEBHOOK_URL", target),

                MailAppTenant = Environment.GetEnvironmentVariable("MAIL_APP_TENANT", target),
                MailAppId = Environment.GetEnvironmentVariable("MAIL_APP_ID", target),
                MailAppPassword = Environment.GetEnvironmentVariable("MAIL_APP_PASSWORD", target),
                SenderId = Environment.GetEnvironmentVariable("MAIL_SENDER_ID", target),
                RecipientAddress = Environment.GetEnvironmentVariable("MAIL_RECIPIENT_ADDRESS", target)
            };

            return env;
        }

        /// <summary>
        /// Registers the different notification services with the service container.
        /// </summary>
        private static void RegisterNotificationServices()
        {
            var services = new List<INotificationService>();

            if (!String.IsNullOrWhiteSpace(Env.ChannelWebhookUrl))
            {
                services.Add(new TeamsService(Env.ChannelWebhookUrl, Env.ImageUrl));
            }

            if (!String.IsNullOrWhiteSpace(Env.MailAppTenant) && !String.IsNullOrWhiteSpace(Env.MailAppId) &&
                !String.IsNullOrWhiteSpace(Env.MailAppPassword) && !String.IsNullOrWhiteSpace(Env.SenderId) &&
                !String.IsNullOrWhiteSpace(Env.RecipientAddress))
            {
                services.Add(new OfficeMailService(Env.SenderId, Env.RecipientAddress, Env.ImageUrl, 
                    new OAuth2Service(Env.MailAppTenant, Env.MailAppId, Env.MailAppPassword)));
            }

            _services = services;
        }
    }
}
