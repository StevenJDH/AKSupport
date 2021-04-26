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
    public record K8SBuildInfo
    {
        private readonly string _gitVersion;

        [JsonPropertyName("major")]
        public string Major { get; init; }
        
        [JsonPropertyName("minor")]
        public string Minor { get; init; }
        
        [JsonPropertyName("gitVersion")]
        public string GitVersion
        {
            get => _gitVersion;
            init => _gitVersion = value.Replace("v", "");
        }

        [JsonPropertyName("gitCommit")]
        public string GitCommit { get; init; }
        
        [JsonPropertyName("gitTreeState")]
        public string GitTreeState { get; init; }
        
        [JsonPropertyName("buildDate")]
        public DateTime BuildDate { get; init; }
        
        [JsonPropertyName("goVersion")]
        public string GoVersion { get; init; }
        
        [JsonPropertyName("compiler")]
        public string Compiler { get; init; }
        
        [JsonPropertyName("platform")]
        public string Platform { get; init; }
    }
}
