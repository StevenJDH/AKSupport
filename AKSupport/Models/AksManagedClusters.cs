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
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AKSupport.Models;

public record AksManagedClusters
{
    [JsonPropertyName("id")]
    public string Id { get; init; }
    
    [JsonPropertyName("name")]
    public string Name { get; init; }
    
    [JsonPropertyName("type")]
    public string Type { get; init; }
    
    [JsonPropertyName("properties")]
    public OrchestratorVersionProfile VersionProfile { get; init; }

    [JsonPropertyName("orchestrators")]
    public IEnumerable<Orchestrator> Orchestrators { get; init; }
}

public record OrchestratorVersionProfile
{
    [JsonPropertyName("orchestrators")]
    public IEnumerable<Orchestrator> Orchestrators { get; init; }
}

public record Orchestrator
{
    [JsonPropertyName("orchestratorType")]
    public string OrchestratorType { get; init; }
    
    [JsonPropertyName("orchestratorVersion")]
    public string OrchestratorVersion { get; init; }
    
    [JsonPropertyName("upgrades")]
    public Upgrade[] AvailableUpgrades { get; init; }
    
    [JsonPropertyName("default")]
    public bool? IsDefault { get; init; }

    [JsonPropertyName("isPreview")]
    public bool? IsPreview { get; init; }
}

public record Upgrade
{
    [JsonPropertyName("orchestratorType")]
    public string OrchestratorType { get; init; }
    
    [JsonPropertyName("orchestratorVersion")]
    public string OrchestratorVersion { get; init; }

    [JsonPropertyName("isPreview")]
    public bool? IsPreview { get; init; }
}
