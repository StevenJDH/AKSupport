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

public record TeamsCard
{
    [JsonPropertyName("@type")] 
    public string Type { get; init; } = "MessageCard";

    [JsonPropertyName("@context")] 
    public string Context { get; init; } = "https://schema.org/extensions";

    [JsonPropertyName("summary")] 
    public string? Summary { get; init; }

    [JsonPropertyName("themeColor")] 
    public string ThemeColor { get; init; } = "0078D7";

    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("sections")]
    public Section[]? Sections { get; init; }

    [JsonPropertyName("potentialAction")]
    public PotentialAction[]? PotentialActions { get; init; }
}

public record Section
{
    [JsonPropertyName("activityTitle")]
    public string? ActivityTitle { get; init; }

    [JsonPropertyName("activitySubtitle")]
    public string? ActivitySubtitle { get; init; }

    [JsonPropertyName("activityImage")]
    public string? ActivityImage { get; init; }

    [JsonPropertyName("facts")]
    public Fact[]? Facts { get; init; }

    [JsonPropertyName("text")]
    public string? Text { get; init; }
}

public record Fact
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("value")]
    public string? Value { get; init; }
}

public record PotentialAction
{
    [JsonPropertyName("@type")] 
    public string Type { get; init; } = "OpenUri";

    [JsonPropertyName("name")] 
    public string? Name { get; init; }

    [JsonPropertyName("targets")]
    public Target[]? Targets { get; init; }
}

public record Target
{
    [JsonPropertyName("os")] 
    public string Os { get; init; } = "default";

    [JsonPropertyName("uri")]
    public string? Uri { get; init; }
}
