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
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AKSupport.Models;

namespace AKSupport.Services
{
    class KubeletService : IKubeletService
    {
        private readonly TimeSpan _timeoutSeconds;
        private HttpClient _httpClient;

        /// <summary>
        /// Constructs a new <see cref="KubeletService"/> instance to with the Kubelet API.
        /// </summary>
        /// <param name="timeoutSeconds">
        /// Number of seconds to wait before a request times out. Default is 90 seconds.
        /// </param>
        public KubeletService(int timeoutSeconds = 90)
        {
            _timeoutSeconds = TimeSpan.FromSeconds(timeoutSeconds);
            CreateHttpClient();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="HttpRequestException">The HTTP response is unsuccessful.</exception>
        public async Task<K8SBuildInfo> GetBuildInfoAsync()
        {
            using var response = await _httpClient.GetAsync("version");

            response.EnsureSuccessStatusCode();

            return JsonSerializer.Deserialize<K8SBuildInfo>(await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Creates a new instance of <see cref="HttpClient"/> with configuration needed to interact
        /// with the Kubelet API.
        /// </summary>
        private void CreateHttpClient()
        {
            string workingDir = Path.Join("..", "var", "run", "secrets", "kubernetes.io", "serviceaccount");
            string caPath = Path.Join(workingDir, "ca.crt");
            string authToken = File.ReadAllText(Path.Join(workingDir, "token"));

            var httpClientHandler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                SslProtocols = SslProtocols.Tls12,
                CheckCertificateRevocationList = false,
                // This is required to use the Pod's self-signed certificate.
                ServerCertificateCustomValidationCallback = 
                    (httpRequestMessage, cert, cetChain, policyErrors) => true
            };

            httpClientHandler.ClientCertificates.Add(new X509Certificate2(caPath));

            _httpClient = new HttpClient(httpClientHandler, disposeHandler: true)
            {
                Timeout = _timeoutSeconds,
                BaseAddress = new Uri("https://kubernetes.default.svc")
            };

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", authToken);
        }

        /// <summary>
        /// Releases any unmanaged resources and disposes of the managed resources used
        /// by the <see cref="KubeletService"/>.
        /// </summary>
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
