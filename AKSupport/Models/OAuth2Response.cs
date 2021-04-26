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
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AKSupport.Models
{
    public record OAuth2Response
    {
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
        
        [JsonPropertyName("expires_in")]
        public string ExpiresIn { get; set; }
        
        [JsonPropertyName("ext_expires_in")]
        public string ExtendedExpiresIn { get; set; }
        
        [JsonPropertyName("expires_on")]
        public string ExpiresOn { get; set; }
        
        [JsonPropertyName("not_before")]
        public string NotBefore { get; set; }
        
        [JsonPropertyName("resource")]
        public string Resource { get; set; }
        
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }
}
