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

namespace AKSupport.Services
{
    interface INotificationService : IDisposable
    {
        /// <summary>
        /// Sends a notification message to the underlining service asynchronously with support details for an AKS
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
        Task<bool> SendNotificationAsync(string clusterName, string version, string description, string status, 
            string clusterUrl);
    }
}
