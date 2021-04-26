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
    public record AksManagedClusters
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("type")]
        public string Type { get; set; }
        
        [JsonPropertyName("properties")]
        public OrchestratorVersionProfile VersionProfile { get; set; }

        [JsonPropertyName("orchestrators")]
        public IEnumerable<Orchestrator> Orchestrators { get; set; }
    }

    public record OrchestratorVersionProfile
    {
        [JsonPropertyName("orchestrators")]
        public IEnumerable<Orchestrator> Orchestrators { get; set; }
    }

    public record Orchestrator
    {
        [JsonPropertyName("orchestratorType")]
        public string OrchestratorType { get; set; }
        
        [JsonPropertyName("orchestratorVersion")]
        public string OrchestratorVersion { get; set; }
        
        [JsonPropertyName("upgrades")]
        public Upgrade[] AvailableUpgrades { get; set; }
        
        [JsonPropertyName("default")]
        public bool? IsDefault { get; set; }

        [JsonPropertyName("isPreview")]
        public bool? IsPreview { get; set; }
    }

    public record Upgrade
    {
        [JsonPropertyName("orchestratorType")]
        public string OrchestratorType { get; set; }
        
        [JsonPropertyName("orchestratorVersion")]
        public string OrchestratorVersion { get; set; }

        [JsonPropertyName("isPreview")]
        public bool? IsPreview { get; set; }
    }
}
