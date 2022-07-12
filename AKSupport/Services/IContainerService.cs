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
using System.Text;
using System.Threading.Tasks;
using AKSupport.Models;

namespace AKSupport.Services;

interface IContainerService : IDisposable
{
    /// <summary>
    /// Gets a list of Kubernetes versions for AKS asynchronously that are currently supported
    /// by Microsoft. The list also contains the supported upgrade paths for each version
    /// that is returned up to the latest version available.
    /// </summary>
    /// <param name="location">AKS region to use when checking for supported versions.</param>
    /// <returns>A listed of supported versions and their upgrade paths.</returns>
    /// <exception cref="HttpRequestException">The HTTP response is unsuccessful.</exception>
    /// <exception cref="ArgumentNullException">The specified argument passed is null.</exception>
    Task<IEnumerable<Orchestrator>> GetSupportedVersionsAsync(string? location);
}
