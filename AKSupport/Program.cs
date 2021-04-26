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
using System.Text;
using System.Threading.Tasks;
using AKSupport.Models;
using AKSupport.Services;

namespace AKSupport
{
    static class Program
    {
        static async Task<int> Main(string[] args)
        {
            Console.WriteLine("{0,-36:o} Checking AKS support status...", DateTimeOffset.UtcNow);

            var target = EnvironmentVariableTarget.Process;
            string subscriptionId = Environment.GetEnvironmentVariable("AZURE_SUBSCRIPTION_ID", target);
            string appTenant = Environment.GetEnvironmentVariable("AZURE_APP_TENANT",target);
            string appId = Environment.GetEnvironmentVariable("AZURE_APP_ID", target);
            string appPassword = Environment.GetEnvironmentVariable("AZURE_APP_PASSWORD", target);
            string aksRegion = Environment.GetEnvironmentVariable("AZURE_AKS_REGION", target);
            
            string kVersion;

            if (args.Length > 0)
            {
                kVersion = args[0]; // For testing with custom versions.
            }
            else
            {
                IKubeletService kubelet = new KubeletService();
                var kBuild = await kubelet.GetBuildInfoAsync();
                kVersion = kBuild.GitVersion;
            }

            IContainerService aks = new ContainerService(subscriptionId, appTenant, appId, appPassword);
            var aksVersions = await aks.GetSupportedVersionsAsync(aksRegion);

            if (!IsSupported(kVersion, aksVersions))
            {
                Console.Error.WriteLine("{0,-36:o} Version {1} is no longer supported.", DateTimeOffset.UtcNow, kVersion);
                return 1;
            }

            if (IsSupportEnding(kVersion, aksVersions))
            {
                Console.Error.WriteLine("{0,-36:o} Support is ending for version {1} soon.", DateTimeOffset.UtcNow, kVersion);
                return 2;
            }

            Console.WriteLine("{0,-36:o} Support for version {1} is active.", DateTimeOffset.UtcNow, kVersion);
            return 0;
        }

        private static bool IsSupportEnding(string runningVersion, IEnumerable<Orchestrator> supportedList)
        {
            var midVersion = Version.Parse(supportedList.ElementAt(2).OrchestratorVersion);

            return Version.Parse(runningVersion) < midVersion;
        }

        private static bool IsSupported(string runningVersion, IEnumerable<Orchestrator> supportedList)
        {
            return supportedList.Any(aks => runningVersion.Equals(aks.OrchestratorVersion));
        }
    }
}
