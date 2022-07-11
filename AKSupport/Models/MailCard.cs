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

public record MailCard
{
    [JsonPropertyName("$schema")]
    public string Schema { get; init; } = "https://adaptivecards.io/schemas/adaptive-card.json";

    [JsonPropertyName("type")]
    public string Type { get; init; } = "AdaptiveCard";

    [JsonPropertyName("version")]
    public string Version { get; init; } = "1.0";

    [JsonPropertyName("body")]
    public object[] Groups { get; init; }

    [JsonPropertyName("padding")]
    public string Padding { get; init; } = "None";
}

public record AdaptiveColumnSet
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = "ColumnSet";

    [JsonPropertyName("columns")]
    public AdaptiveColumn[] Columns { get; init; }

    [JsonPropertyName("padding")]
    public AdaptivePadding Padding { get; init; }

    [JsonPropertyName("style")]
    public string Style { get; init; } = "Default";
}

public record AdaptiveContainer
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = "Container";

    [JsonPropertyName("padding")]
    public AdaptivePadding Padding { get; init; }

    [JsonPropertyName("style")]
    public string Style { get; init; } = "Default";

    [JsonPropertyName("spacing")]
    public string Spacing { get; init; } = "None";

    [JsonPropertyName("items")]
    public object[] Items { get; init; }

    [JsonPropertyName("separator")]
    public bool Separator { get; init; }

    [JsonPropertyName("horizontalAlignment")]
    public string HorizontalAlignment { get; init; }
}

public record AdaptiveColumn
{
    [JsonPropertyName("type")] 
    public string Type { get; init; } = "Column";

    [JsonPropertyName("padding")]
    public string Padding { get; init; } = "None";

    [JsonPropertyName("width")]
    public string Width { get; init; } = "auto";

    [JsonPropertyName("items")]
    public object[] Items { get; init; }

    [JsonPropertyName("verticalContentAlignment")]
    public string VerticalContentAlignment { get; init; } = "Top";

    [JsonPropertyName("horizontalAlignment")]
    public string HorizontalAlignment { get; init; } = "Left";
}

public record AdaptivePadding
{
    [JsonPropertyName("top")]
    public string Top { get; init; } = "Default";

    [JsonPropertyName("bottom")]
    public string Bottom { get; init; } = "Default";

    [JsonPropertyName("left")]
    public string Left { get; init; } = "Default";

    [JsonPropertyName("right")]
    public string Right { get; init; } = "Default";
}

public record AdaptiveTextBlock
{
    [JsonPropertyName("type")] 
    public string Type { get; init; } = "TextBlock";

    [JsonPropertyName("text")]
    public string Text { get; init; }

    [JsonPropertyName("wrap")]
    public bool Wrap { get; init; } = true;

    [JsonPropertyName("weight")]
    public string Weight { get; init; } = "Default";

    [JsonPropertyName("size")]
    public string Size { get; init; } = "Default";

    [JsonPropertyName("color")] 
    public string Color { get; init; } = "Default";

    [JsonPropertyName("horizontalAlignment")]
    public string HorizontalAlignment { get; init; } = "Left";
}

public record AdaptiveImage
{
    [JsonPropertyName("type")] 
    public string Type { get; init; } = "Image";

    [JsonPropertyName("url")]
    public string Url { get; init; }

    [JsonPropertyName("size")]
    public string Size { get; init; } = "Small";
}

public record AdaptiveGenericSet
{
    [JsonPropertyName("type")]
    public string Type { get; init; }

    [JsonPropertyName("facts")]
    public AdaptiveFact[] Facts { get; init; }

    [JsonPropertyName("actions")]
    public AdaptiveAction[] Actions { get; init; }
}

public record AdaptiveFact
{
    [JsonPropertyName("title")]
    public string Title { get; init; }

    [JsonPropertyName("value")]
    public string Value { get; init; }
}

public record AdaptiveAction
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = "Action.OpenUrl";

    [JsonPropertyName("title")]
    public string Title { get; init; }

    [JsonPropertyName("url")]
    public string Url { get; init; }

    [JsonPropertyName("style")]
    public string Style { get; init; }

    [JsonPropertyName("isPrimary")]
    public bool IsPrimary { get; init; }
}
