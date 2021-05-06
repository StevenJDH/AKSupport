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
    interface IOAuth2Service
    {
        /// <summary>
        /// Obtains an access token asynchronously using Client Credentials grant flow that permits
        /// machine-to-machine (M2M) applications such as CLIs, daemons, or services running on the back-end
        /// to call other services outside of the context of a user.
        /// </summary>
        /// <param name="client">The <see cref="HttpClient"/> instance to use for obtaining the token.</param>
        /// <param name="scope">
        /// The resource identifier (application ID URI) of the resource to be accessed, affixed with the
        /// .default suffix. For example, Microsoft Graph APIs use the value
        /// https://graph.microsoft.com/.default. This value tells the Microsoft identity platform that of
        /// all the direct application permissions configured for an application, the endpoint should issue a
        /// token for the ones associated with the resource that will be used.
        /// </param>
        /// <returns>An OAuth 2.0 access token for M2M.</returns>
        /// <exception cref="HttpRequestException">The HTTP response is unsuccessful.</exception>
        Task<string> GetAuthorizeTokenAsync(HttpClient client, string scope);
    }
}
